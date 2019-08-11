using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.resources
{
    public class Resources
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public const string SourcePath = "resources/";

        public readonly XmlData XmlData;

        public Resources()
        {
            Log.Info("Loading resources...");
            XmlData = new XmlData();
        }
    }
}
