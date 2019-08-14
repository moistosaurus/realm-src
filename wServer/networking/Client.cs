using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;

namespace wServer.networking
{
    public partial class Client
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public RealmManager Manager { get; private set; }

        static readonly byte[] ServerKey = new byte[] { 0x61, 0x2a, 0x80, 0x6c, 0xac, 0x78, 0x11, 0x4b, 0xa5, 0x01, 0x3c, 0xb5, 0x31 };
        static byte[] _clientKey = new byte[] { 0x81, 0x1f, 0x50, 0x39, 0x1f, 0xb4, 0x55, 0x89, 0x9c, 0xa9, 0xd7, 0x4b, 0x72 };

        public RC4 ReceiveKey { get; private set; }
        public RC4 SendKey { get; private set; }
    }
}
