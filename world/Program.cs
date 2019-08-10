using common.db;
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
        internal static readonly Logger Log = LogManager.GetCurrentClassLogger();
        internal static readonly ManualResetEvent Shutdown = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            Console.CancelKeyPress += delegate
            {
                Shutdown.Set();
            };

            Shutdown.WaitOne();
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal((Exception)args.ExceptionObject);
        }
    }
}
