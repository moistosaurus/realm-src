using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common.resources
{
    public class XmlData
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public Dictionary<ushort, XElement> ObjectTypeToElement = new Dictionary<ushort, XElement>();
        public Dictionary<ushort, string> ObjectTypeToId = new Dictionary<ushort, string>();
        public Dictionary<string, ushort> IdToObjectType = new Dictionary<string, ushort>();
        public Dictionary<string, ushort> DisplayIdToObjectType = new Dictionary<string, ushort>();

        public Dictionary<ushort, XElement> TileTypeToElement = new Dictionary<ushort, XElement>();
        public Dictionary<ushort, string> TileTypeToId = new Dictionary<ushort, string>();
        public Dictionary<string, ushort> IdToTileType = new Dictionary<string, ushort>();

        public Dictionary<ushort, TileDesc> Tiles = new Dictionary<ushort, TileDesc>();
        public Dictionary<ushort, Item> Items = new Dictionary<ushort, Item>();
        public Dictionary<ushort, ObjectDesc> Objects = new Dictionary<ushort, ObjectDesc>();
        public Dictionary<ushort, PlayerDesc> Players = new Dictionary<ushort, PlayerDesc>();
        public Dictionary<ushort, PortalDesc> Portals = new Dictionary<ushort, PortalDesc>();
        public Dictionary<ushort, SkinDesc> Skins = new Dictionary<ushort, SkinDesc>();

        public XmlData()
        {
            Log.Info("Loading XmlData...");
            LoadXmls();

            Log.Info("Loaded {0} Tiles...", Tiles.Count);
            Log.Info("Loaded {0} Items...", Items.Count);
            Log.Info("Loaded {0} Objects...", Objects.Count);
            Log.Info("Loaded {0} Players...", Players.Count);
            Log.Info("Loaded {0} Portals...", Portals.Count);
            Log.Info("Loaded {0} Skins...", Skins.Count);
        }

        private void LoadXmls()
        {
            var xmls = Directory.EnumerateFiles(Resources.SourcePath + "xml/", "*xml", SearchOption.AllDirectories).ToArray();
            foreach (string k in xmls)
            {
                var xml = Utils.Read(k);
                ProcessXml(XElement.Parse(xml));
            }
        }

        private void AddObjects(XElement root)
        {
            foreach (var e in root.Elements("Object"))
            {
                var cls = e.GetValue<string>("Class");
                if (string.IsNullOrWhiteSpace(cls))
                    continue;

                var id = e.GetAttribute<string>("id");
                var type = e.GetAttribute<ushort>("type");
                var displayId = e.GetValue<string>("DisplayId");
                var displayName = string.IsNullOrWhiteSpace(displayId) ? id : displayId;

                if (ObjectTypeToId.ContainsKey(type))
                    Log.Warn("'{0}' and '{1}' have the same type of '0x{2:x4}'", id, ObjectTypeToId[type], type);

                if (IdToObjectType.ContainsKey(id))
                    Log.Warn("'0x{0:x4}' and '0x{1:x4}' have the same id of '{2}'", type, IdToObjectType[id], id);

                ObjectTypeToId[type] = id;
                ObjectTypeToElement[type] = e;
                IdToObjectType[id] = type;
                DisplayIdToObjectType[displayName] = type;

                switch (cls)
                {
                    case "Equipment":
                    case "Dye":
                        Items[type] = new Item(type, e);
                        break;
                    case "Player":
                        Players[type] = new PlayerDesc(type, e);
                        Objects[type] = Players[type];
                        break;
                    case "GuildHallPortal":
                    case "Portal":
                        Portals[type] = new PortalDesc(type, e);
                        Objects[type] = Portals[type];
                        break;
                    case "Skin":
                        Skins[type] = new SkinDesc(type, e);
                        break;
                    default:
                        Objects[type] = new ObjectDesc(type, e);
                        break;
                }
            }
        }

        private void AddGrounds(XElement root)
        {
            foreach (var e in root.Elements("Ground"))
            {
                var id = e.GetAttribute<string>("id");
                var type = e.GetAttribute<ushort>("type");

                if (TileTypeToId.ContainsKey(type))
                    Log.Warn("'{0}' and '{1}' have the same type of '0x{2:x4}'", id, TileTypeToId[type], type);

                if (IdToTileType.ContainsKey(id))
                    Log.Warn("'0x{0:x4}' and '0x{1:x4}' have the same id of '{2}'", type, IdToTileType[id], id);

                TileTypeToId[type] = id;
                TileTypeToElement[type] = e;
                IdToTileType[id] = type;

                Tiles[type] = new TileDesc(type, e);
            }
        }

        private void ProcessXml(XElement root)
        {
            AddObjects(root);
            AddGrounds(root);
        }
    }
}
