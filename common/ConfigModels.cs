using NLog;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace common
{
    public class ServerConfig
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly DbInfo dbInfo;
        public readonly ServerInfo serverInfo;
        public readonly InitInfo initInfo;

        public ServerConfig()
        {
            Log.Info("Loading config files...");
            dbInfo = new DbInfo(XElement.Parse(Utils.Read("db.config")));
            serverInfo = new ServerInfo(XElement.Parse(Utils.Read("server.config")));
            initInfo = new InitInfo(XElement.Parse(Utils.Read("init.config")));
        }
    }

    public class InitInfo
    {
        public readonly XElement Xml;

        public readonly int UseExternalPayments;
        public readonly int MaxStackablePotions;
        public readonly int PotionPurchaseCooldown;
        public readonly int PotionPurchaseCostCooldown;
        public readonly int[] PotionPurchaseCosts;
        public readonly NewAccounts NewAccounts;
        public readonly NewCharacters NewCharacters;

        public InitInfo(XElement e)
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

    public class DbInfo
    {
        public readonly string host;
        public readonly int port;
        public readonly int index;
        public readonly string password;

        public DbInfo(XElement e)
        {
            host = e.GetValue<string>("host", "localhost");
            port = e.GetValue<int>("port", 6379);
            index = e.GetValue<int>("index", 0);
            password = e.GetValue<string>("password", null);
        }
    }

    public class ServerInfo
    {
        public readonly string bindAddress;
        public readonly int bindPort;
        public readonly string resourcePath;
        public readonly bool debugMode;
        public readonly string instanceId;

        public readonly string version;

        public readonly string name;
        public readonly string address;
        public readonly int port;
        public readonly bool adminOnly;
        public readonly int maxPlayers;
        public readonly int tps;

        public readonly float latitude;
        public readonly float longitude;

        public int players { get; set; } = 0;

        public ServerInfo(XElement e)
        {
            instanceId = Guid.NewGuid().ToString();
            bindAddress = e.GetValue<string>("bindAddress", "127.0.0.1");
            bindPort = e.GetValue<int>("bindPort", 2050);
            resourcePath = e.GetValue<string>("resourcePath", "resources");
            address = e.GetValue<string>("address", "127.0.0.1");
            port = e.GetValue<int>("port", 2050);
            debugMode = e.GetValue<int>("debugMode", 1) == 1;
            name = e.GetValue<string>("name", "Localhost");
            adminOnly = e.GetValue<int>("adminOnly", 1) == 1;
            maxPlayers = e.GetValue<int>("maxPlayers", 256);
            latitude = e.GetValue<float>("latitude", 0);
            longitude = e.GetValue<float>("longitude", 0);
            tps = e.GetValue<int>("tps", 20);
            version = e.GetValue<string>("version", "0.0");
        }
    }
}
