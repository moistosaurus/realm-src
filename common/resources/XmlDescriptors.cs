using System.Xml.Linq;

namespace common.resources
{
    public class ObjectDesc
    {
        public ObjectDesc(ushort type, XElement e)
        {

        }
    }

    public class PortalDesc : ObjectDesc
    {
        public PortalDesc(ushort type, XElement e) : base(type, e)
        {

        }
    }

    public class PlayerDesc : ObjectDesc
    {
        public readonly int[] SlotTypes;
        public readonly ushort[] Equipment;
        public readonly Stat[] Stats;
        public readonly UnlockClass Unlock;

        public PlayerDesc(ushort type, XElement e) : base(type, e)
        {
            SlotTypes = e.GetValue<string>("SlotTypes").CommaToArray<int>();
            Equipment = e.GetValue<string>("Equipment").CommaToArray<ushort>();
            Stats = new Stat[8];
            for (var i = 0; i < Stats.Length; i++)
                Stats[i] = new Stat(i, e);
            if (e.HasElement("UnlockLevel") || e.HasElement("UnlockCost"))
                Unlock = new UnlockClass(e);
        }
    }

    public class Stat
    {
        public readonly string Type;
        public readonly int MaxValue;
        public readonly int StartingValue;
        public readonly int MinIncrease;
        public readonly int MaxIncrease;

        public Stat(int index, XElement e)
        {
            Type = StatIndexToName(index);
            var x = e.Element(Type);
            if (x != null)
            {
                StartingValue = int.Parse(x.Value);
                MaxValue = e.GetAttribute<int>("max");
            }

            var y = e.Elements("LevelIncrease");
            foreach (var k in y)
                if (k.Value == Type)
                {
                    MinIncrease = k.GetAttribute<int>("min");
                    MaxIncrease = k.GetAttribute<int>("max");
                    break;
                }
        }

        private static string StatIndexToName(int index)
        {
            switch (index)
            {
                case 0: return "MaxHitPoints";
                case 1: return "MaxMagicPoints";
                case 2: return "Attack";
                case 3: return "Defense";
                case 4: return "Speed";
                case 5: return "Dexterity";
                case 6: return "HpRegen";
                case 7: return "MpRegen";
            }
            return null;
        }
    }

    public class UnlockClass
    {
        public readonly ushort? Type;
        public readonly ushort? Level;
        public readonly uint? Cost;

        public UnlockClass(XElement e)
        {
            var n = e.Element("UnlockLevel");
            if(n != null && n.HasAttribute("type") && n.HasAttribute("level"))
            {
                Type = n.GetAttribute<ushort>("type");
                Level = n.GetAttribute<ushort>("level");
            }

            n = e.Element("UnlockCost");
            if (n != null)
            {
                Cost = (uint)int.Parse(n.Value);
            }
        }
    }

    public class SkinDesc
    {
        public readonly ushort ClassType;
        public readonly int UnlockLevel;

        public SkinDesc(XElement e)
        {
            ClassType = e.GetValue<ushort>("PlayerClassType");
            UnlockLevel = e.GetValue<int>("UnlockLevel");
        }
    }

    public class Item
    {
        public Item(ushort type, XElement e)
        {

        }
    }

    public class ProjectileDesc
    {
        public ProjectileDesc(XElement e)
        {

        }
    }

    public class ConditionEffect
    {
        public ConditionEffect(XElement e)
        {

        }
    }

    public class ActivateEffect
    {
        public ActivateEffect(XElement e)
        {

        }
    }

    public class TileDesc
    {
        public TileDesc(XElement e)
        {

        }
    }
}
