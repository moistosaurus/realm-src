using common;
using common.resources;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wServer.networking;
using wServer.realm.worlds;

namespace wServer.realm
{
    public struct RealmTime
    {
        public long TickCount;
        public long TotalElapsedMs;
        public int TickDelta;
        public int ElaspedMsDelta;
    }

    public enum PendingPriority
    {
        Emergent,
        Destruction,
        Normal,
        Creation,
    }

    public enum PacketPriority
    {
        High,
        Normal,
        Low // no guarantees that packets of low priority will be sent
    }

    public class RealmManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly bool _initialized;
        public string InstanceId { get; private set; }
        public bool Terminating { get; private set; }

        public Resources Resources { get; private set; }
        public Database Database { get; private set; }
        public ServerConfig Config { get; private set; }
        public int TPS { get; private set; }

        public ConnectManager ConMan { get; private set; }
        public BehaviorDb Behaviors { get; private set; }
        public ISManager InterServer { get; private set; }
        public ISControl ISControl { get; private set; }
        public ChatManager Chat { get; private set; }
        public DbServerManager DbServerController { get; private set; }
        public CommandManager Commands { get; private set; }
        public PortalMonitor Monitor { get; private set; }
        public DbEvents DbEvents { get; private set; }

        private Thread _network;
        private Thread _logic;
        public NetworkTicker Network { get; private set; }
        public FLLogicTicker Logic { get; private set; }

        public readonly ConcurrentDictionary<int, World> Worlds = new ConcurrentDictionary<int, World>();
        public readonly ConcurrentDictionary<Client, PlayerInfo> Clients = new ConcurrentDictionary<Client, PlayerInfo>();

        private int _nextWorldId = 0;
        private int _nextClientId = 0;

        public RealmManager(Resources resources, Database db, ServerConfig config)
        {
            Log.Info("Initalizing Realm Manager...");

            Resources = resources;
            Database = db;
            Config = config;
            TPS = config.serverInfo.tps;
            InstanceId = config.serverInfo.instanceId;

            InterServer = new ISManager(Database, config);
            ISControl = new ISControl(this);
            //Chat = new ChatManager(this);
            //DbServerController = new DbServerManager(this);
            DbEvents = new DbEvents(this);
        }

        public void Run()
        {
            Log.Info("Starting Realm Manager...");

            // start server logic management
            Logic = new FLLogicTicker(this);
            var logic = new Task(() => Logic.TickLoop(), TaskCreationOptions.LongRunning);
            logic.ContinueWith(Program.Stop, TaskContinuationOptions.OnlyOnFaulted);
            logic.Start();

            // start received packet processor
            Network = new NetworkTicker(this);
            var network = new Task(() => Network.TickLoop(), TaskCreationOptions.LongRunning);
            network.ContinueWith(Program.Stop, TaskContinuationOptions.OnlyOnFaulted);
            network.Start();

            Log.Info("Realm Manager started.");
        }

        public void Stop()
        {
            Log.Info("Stopping Realm Manager...");

            Terminating = true;
            InterServer.Dispose();
            Resources.Dispose();
            Network.Shutdown();

            Log.Info("Realm Manager stopped.");
        }

        public bool TryConnect(Client client)
        {
            if (client?.Account == null)
                return false;

            client.Id = Interlocked.Increment(ref _nextClientId);
            var plrInfo = new PlayerInfo()
            {
                AccountId = client.Account.AccountId,
                GuildId = client.Account.GuildId,
                Name = client.Account.Name,
                WorldInstance = -1
            };
            Clients[client] = plrInfo;

            // recalculate usage statistics
            Config.serverInfo.players = ConMan.GetPlayerCount();
            Config.serverInfo.maxPlayers = Config.serverSettings.maxPlayers;
            Config.serverInfo.queueLength = ConMan.QueueLength();
            Config.serverInfo.playerList.Add(plrInfo);
            return true;
        }

        public void Disconnect(Client client)
        {
            var player = client.Player;
            player?.Owner?.LeaveWorld(player);

            PlayerInfo plrInfo;
            Clients.TryRemove(client, out plrInfo);

            // recalculate usage statistics
            Config.serverInfo.players = ConMan.GetPlayerCount();
            Config.serverInfo.queueLength = ConMan.QueueLength();
            Config.serverInfo.playerList.Remove(plrInfo);
        }

        private void AddWorld(string name, bool actAsNexus = false)
        {
            AddWorld(Resources.Worlds.Data[name], actAsNexus);
        }

        private void AddWorld(ProtoWorld proto, bool actAsNexus = false)
        {
            int id;
            if (actAsNexus)
            {
                id = World.Nexus;
            }
            else
            {
                id = (proto.id < 0)
                    ? proto.id
                    : Interlocked.Increment(ref _nextWorldId);
            }

            World world;
            DynamicWorld.TryGetWorld(proto, null, out world);
            if (world != null)
            {
                if (world is Marketplace && !Config.serverSettings.enableMarket)
                    return;

                AddWorld(id, world);
                return;
            }

            AddWorld(id, new World(proto));
        }

        private void AddWorld(int id, World world)
        {
            if (world.Manager != null)
                throw new InvalidOperationException("World already added.");
            world.Id = id;
            Worlds[id] = world;
            if (_initialized)
                OnWorldAdded(world);
        }

        public World AddWorld(World world)
        {
            if (world.Manager != null)
                throw new InvalidOperationException("World already added.");
            world.Id = Interlocked.Increment(ref _nextWorldId);
            Worlds[world.Id] = world;
            if (_initialized)
                OnWorldAdded(world);
            return world;
        }

        public World GetWorld(int id)
        {
            World ret;
            if (!Worlds.TryGetValue(id, out ret)) return null;
            if (ret.Id == 0) return null;
            return ret;
        }

        public bool RemoveWorld(World world)
        {
            if (world.Manager == null)
                throw new InvalidOperationException("World is not added.");
            if (Worlds.TryRemove(world.Id, out world))
            {
                OnWorldRemoved(world);
                return true;
            }
            else
                return false;
        }

        void OnWorldAdded(World world)
        {
            world.Manager = this;
            Log.Info("World {0}({1}) added. {2} Worlds existing.", world.Id, world.Name, Worlds.Count);
        }

        void OnWorldRemoved(World world)
        {
            //world.Manager = null;
            Monitor.RemovePortal(world.Id);
            Log.Info("World {0}({1}) removed.", world.Id, world.Name);
        }

        public World GetRandomGameWorld()
        {
            var realms = Worlds.Values
                .OfType<Realm>()
                .Where(w => !w.Closed)
                .ToArray();

            return realms.Length == 0 ?
                Worlds[World.Nexus] :
                realms[Environment.TickCount % realms.Length];
        }
    }
}
