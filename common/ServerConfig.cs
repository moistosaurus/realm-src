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

        public ServerConfig(string configPath)
        {
            Log.Info("Loading config files...");
            DbSettings = new DbSettings(XElement.Parse(Utils.Read(configPath + "db.config")));
            ServerSettings = new ServerSettings(XElement.Parse(Utils.Read(configPath + "server.config")));
            WorldSettings = new WorldSettings(XElement.Parse(Utils.Read(configPath + "world.config")));
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
        public ServerSettings(XElement e)
        {

        }
    }

    public class WorldSettings
    {
        public WorldSettings(XElement e)
        {

        }
    }
}
