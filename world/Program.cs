using common;
using common.resources;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace world
{
    class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static readonly ManualResetEvent Shutdown = new ManualResetEvent(false);

        internal static ServerConfig Config;
        internal static Resources Resources;
        internal static Database Database;

        static void Main(string[] args)
        {
            Config = new ServerConfig();
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            using (Resources = new Resources(Config.serverInfo.resourcePath, false))
            using (Database = new Database(Resources, Config))
            {
                Console.CancelKeyPress += delegate
                {
                    Shutdown.Set();
                };

                Shutdown.WaitOne();
                Log.Info("Terminating...");
            }
        }

        public static void Stop()
        {
            Shutdown.Set();
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal((Exception)args.ExceptionObject);
        }
    }
}
