using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common.resources
{
    public class ObjectDesc
    {
        public ObjectDesc(XElement e)
        {

        }
    }

    public class PortalDesc : ObjectDesc
    {
        public PortalDesc(XElement e) : base(e)
        {

        }
    }

    public class PlayerDesc : ObjectDesc
    {
        public PlayerDesc(XElement e) : base(e)
        {

        }
    }

    public class Stat
    {
        public Stat(XElement e)
        {

        }
    }

    public class UnlockClass
    {
        public UnlockClass(XElement e)
        {

        }
    }

    public class SkinDesc
    {
        public SkinDesc(XElement e)
        {

        }
    }

    public class Item
    {
        public Item(XElement e)
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
