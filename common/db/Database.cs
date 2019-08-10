using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.db
{
    public class Database
    {
        public readonly int DatabaseIndex;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IServer _server;

        public IDatabase Conn => _db;
        public readonly ISubscriber Sub;

        public Database(string host, int port, string password, int dbIndex)
        {
            DatabaseIndex = dbIndex;
            var conString = host + ":" + port + ",syncTimeout=60000";
            if (!string.IsNullOrWhiteSpace(password))
                conString += ",password=" + password;

            _redis = ConnectionMultiplexer.Connect(conString);
            _server = _redis.GetServer(_redis.GetEndPoints(true)[0]);
            _db = _redis.GetDatabase(DatabaseIndex);
            Sub = _redis.GetSubscriber();
        }
    }
}
