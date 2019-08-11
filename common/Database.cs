using common.resources;
using NLog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    public class Database : IDisposable
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly int DatabaseIndex;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IServer _server;

        private readonly Resources _resources;
        private readonly ServerConfig _config;

        public IDatabase Conn => _db;
        public readonly ISubscriber Sub;

        public Database(Resources resources, ServerConfig config)
        {
            Log.Info("Initializing Database...");
            _resources = resources;
            _config = config;

            DatabaseIndex = config.DbSettings.Index;
            var conString = config.DbSettings.Host + ":" + config.DbSettings.Port + ",syncTimeout=60000";
            if (!string.IsNullOrWhiteSpace(config.DbSettings.Password))
                conString += ",password=" + config.DbSettings.Password;

            _redis = ConnectionMultiplexer.Connect(conString);
            _server = _redis.GetServer(_redis.GetEndPoints(true)[0]);
            _db = _redis.GetDatabase(DatabaseIndex);
            Sub = _redis.GetSubscriber();
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
