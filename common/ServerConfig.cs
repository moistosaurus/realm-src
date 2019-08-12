using common.resources;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common
{
    public class ServerConfig
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly DbSettings DbSettings;
        public readonly ServerSettings ServerSettings;
        public readonly WorldSettings WorldSettings;
        public readonly InitConfig InitSettings;

        public ServerConfig()
        {
            Log.Info("Loading config files...");
            DbSettings = new DbSettings(XElement.Parse(Utils.Read("db.config")));
            ServerSettings = new ServerSettings(XElement.Parse(Utils.Read("server.config")));
            WorldSettings = new WorldSettings(XElement.Parse(Utils.Read("world.config")));
            InitSettings = new InitConfig(XElement.Parse(Utils.Read("init.config")));
        }
    }

    public class InitConfig
    {
        public readonly XElement Xml;

        public readonly int UseExternalPayments;
        public readonly int MaxStackablePotions;
        public readonly int PotionPurchaseCooldown;
        public readonly int PotionPurchaseCostCooldown;
        public readonly int[] PotionPurchaseCosts;

        public InitConfig(XElement e)
        {
            Xml = e;
            UseExternalPayments = e.GetValue<int>("UseExternalPayments");
            MaxStackablePotions = e.GetValue<int>("MaxStackablePotions");
            PotionPurchaseCooldown = e.GetValue<int>("PotionPurchaseCooldown");
            PotionPurchaseCostCooldown = e.GetValue<int>("PotionPurchaseCostCooldown");

            List<int> costs = new List<int>();
            foreach (var i in e.Element("PotionPurchaseCosts").Elements("cost"))
                costs.Add(Utils.GetInt(i.Value));
            PotionPurchaseCosts = costs.ToArray();
        }
    }

    public class DbSettings
    {
        public readonly string Host;
        public readonly int Port;
        public readonly int Index;
        public readonly string Password;

        public DbSettings(XElement e)
        {
            Host = e.GetValue<string>("host", "localhost");
            Port = e.GetValue<int>("port", 6379);
            Index = e.GetValue<int>("index", 0);
            Password = e.GetValue<string>("password", null);
        }
    }

    public class ServerSettings
    {
        public readonly string BindAddress;
        public readonly int Port;
        public readonly string ResourcePath;
        public readonly bool DebugRequests;

        public ServerSettings(XElement e)
        {
            BindAddress = e.GetValue<string>("bindAddress", "127.0.0.1");
            Port = e.GetValue<int>("port", 8080);
            ResourcePath = e.GetValue<string>("resourcePath", "resources");
            DebugRequests = e.GetValue<int>("debugRequests", 1) == 1;
        }
    }

    public class WorldSettings
    {
        public WorldSettings(XElement e)
        {

        }
    }
}
