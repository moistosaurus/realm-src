using common;
using common.resources;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.realm
{
    public class RealmManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly Resources Resources;
        public readonly Database Database;
        public readonly ServerConfig Config;
        public readonly int TPS;
        public readonly string InstanceId;

        public readonly ISManager InterServer;
        public readonly ISControl ISControl;
        public readonly DbEvents DbEvents;

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
        }
    }
}
