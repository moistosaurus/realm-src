using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.resources
{
    public class Resources
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly string ResourcePath;
        public readonly XmlData XmlData;
        public readonly Dictionary<string, byte[]> WebFiles = new Dictionary<string, byte[]>();

        public Resources(string resourcePath, bool loadServerFiles)
        {
            Log.Info("Loading resources...");
            ResourcePath = resourcePath;
            XmlData = new XmlData(resourcePath + "/xml");

            if (loadServerFiles)
            {
                webFiles(resourcePath + "/web");
            }
        }

        void webFiles(string dir)
        {
            Log.Info("Loading web data...");

            var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var webPath = file.Substring(dir.Length, file.Length - dir.Length)
                    .Replace("\\", "/");

                WebFiles[webPath] = File.ReadAllBytes(file);
            }
        }
    }
}
