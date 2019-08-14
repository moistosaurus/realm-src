using Newtonsoft.Json;
using NLog;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace common
{
    public class ServerConfig
    {
        public DbInfo dbInfo { get; set; } = new DbInfo();
        public ServerInfo serverInfo { get; set; } = new ServerInfo();
        public ServerSettings serverSettings { get; set; } = new ServerSettings();

        [JsonIgnore]
        public AppSettings appSettings { get; set; }

        public static ServerConfig ReadFile(string fileName)
        {
            using (var r = new StreamReader(fileName))
            {
                return ReadJson(r.ReadToEnd());
            }
        }

        public static ServerConfig ReadJson(string json)
        {
            var sConfig = JsonConvert.DeserializeObject<ServerConfig>(json);
            sConfig.appSettings = new AppSettings(XElement.Parse(Utils.Read("init.config")));
            return sConfig;
        }
    }

    public class DbInfo
    {
        public string host { get; set; } = "127.0.0.1";
        public int port { get; set; } = 6379;
        public string auth { get; set; } = "";
        public int index { get; set; } = 0;
    }

    public class ServerInfo
    {
        public ServerType type { get; set; } = ServerType.World;
        public string name { get; set; } = "Localhost";
        public string address { get; set; } = "127.0.0.1";
        public string bindAddress { get; set; } = "127.0.0.1";
        public int port { get; set; } = 2051;
        public Coordinates coordinates { get; set; } = new Coordinates();
        public int players { get; set; } = 0;
        public int maxPlayers { get; set; } = 100;
        public int queueLength { get; set; } = 0;
        public bool adminOnly { get; set; } = false;
        public int minRank { get; set; } = 0;
        public string instanceId { get; set; } = "";
        public PlayerList playerList { get; set; } = new PlayerList();
    }

    public class ServerSettings
    {
        public string resourceFolder { get; set; } = "./resources";
        public string version { get; set; } = "1.0.0";
        public int tps { get; set; } = 20;
        public string key { get; set; } = "B1A5ED";
        public int maxConnections { get; set; } = 256;
        public int maxPlayers { get; set; } = 100;
        public int maxPlayersWithPriority { get; set; } = 120;
        public string sendGridApiKey { get; set; } = "";
    }

    public enum ServerType
    {
        Account,
        World
    }

    public class Coordinates
    {
        public float latitude { get; set; } = 0;
        public float longitude { get; set; } = 0;
    }
    public class PlayerInfo
    {
        public int AccountId;
        public int GuildId;
        public string Name;
        public string WorldName;
        public int WorldInstance;
    }

    public class PlayerList : IEnumerable<PlayerInfo>
    {
        private readonly ConcurrentDictionary<PlayerInfo, int> PlayerInfo;

        public PlayerList(IEnumerable<PlayerInfo> playerList = null)
        {
            PlayerInfo = new ConcurrentDictionary<PlayerInfo, int>();

            if (playerList == null)
                return;

            foreach (var plr in playerList)
            {
                Add(plr);
            }
        }

        public void Add(PlayerInfo playerInfo)
        {
            PlayerInfo.TryAdd(playerInfo, 0);
        }

        public void Remove(PlayerInfo playerInfo)
        {
            if (playerInfo == null)
                return;

            int ignored;
            PlayerInfo.TryRemove(playerInfo, out ignored);
        }

        IEnumerator<PlayerInfo> IEnumerable<PlayerInfo>.GetEnumerator()
        {
            return PlayerInfo.Keys.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return PlayerInfo.Keys.GetEnumerator();
        }
    }

    public class AppSettings
    {
        public readonly XElement Xml;

        public readonly int UseExternalPayments;
        public readonly int MaxStackablePotions;
        public readonly int PotionPurchaseCooldown;
        public readonly int PotionPurchaseCostCooldown;
        public readonly int[] PotionPurchaseCosts;
        public readonly NewAccounts NewAccounts;
        public readonly NewCharacters NewCharacters;

        public AppSettings(XElement e)
        {
            Xml = e;
            UseExternalPayments = e.GetValue<int>("UseExternalPayments");
            MaxStackablePotions = e.GetValue<int>("MaxStackablePotions");
            PotionPurchaseCooldown = e.GetValue<int>("PotionPurchaseCooldown");
            PotionPurchaseCostCooldown = e.GetValue<int>("PotionPurchaseCostCooldown");

            var newAccounts = e.Element("NewAccounts");
            NewAccounts = new NewAccounts(e.Element("NewAccounts"));
            newAccounts.Remove(); // don't export with /app/init

            var newCharacters = e.Element("NewCharacters");
            NewCharacters = new NewCharacters(e.Element("NewCharacters"));
            newCharacters.Remove();

            List<int> costs = new List<int>();
            foreach (var i in e.Element("PotionPurchaseCosts").Elements("cost"))
                costs.Add(Utils.GetInt(i.Value));
            PotionPurchaseCosts = costs.ToArray();
        }
    }

    public class NewAccounts
    {
        public readonly int MaxCharSlot;
        public readonly int VaultCount;
        public readonly int Fame;
        public readonly int Credits;
        public readonly int[] Slots;
        public readonly bool ClassesUnlocked;
        public readonly bool SkinsUnlocked;

        public NewAccounts(XElement e)
        {
            MaxCharSlot = e.GetValue<int>("MaxCharSlot", 1);
            VaultCount = e.GetValue<int>("VaultCount", 1);
            Fame = e.GetValue<int>("Fame", 0);
            Credits = e.GetValue<int>("Credits", 0);

            ClassesUnlocked = e.HasElement("ClassesUnlocked");
            SkinsUnlocked = e.HasElement("SkinsUnlocked");

            if (e.HasElement("Slots"))
            {
                List<int> slots = new List<int>();
                foreach (var i in e.Element("Slots").Elements("cost"))
                    slots.Add(Utils.GetInt(i.Value));
                Slots = slots.ToArray();
            }
            else
                Slots = new int[1] { 1000 };
        }

        public int GetPrice(int slot)
        {
            return Slots[Math.Max(Math.Min(slot - MaxCharSlot, Slots.Length - 1), 0)];
        }
    }

    public class NewCharacters
    {
        public readonly bool Maxed;
        public readonly int Level;

        public NewCharacters(XElement e)
        {
            Maxed = e.HasElement("Maxed");
            Level = e.GetValue<int>("Level", 1);
        }
    }
}
