using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common.resources
{
    public class AppSettings
    {
        public readonly XElement Xml;

        public readonly int UseExternalPayments;
        public readonly int MaxStackablePotions;
        public readonly int PotionPurchaseCooldown;
        public readonly int PotionPurchaseCostCooldown;
        public readonly int[] PotionPurchaseCosts;
        public readonly NewAccounts NewAccounts;
        public readonly NewCharacters NewCharacters;

        public AppSettings(string dir)
        {
            XElement e = XElement.Parse(Utils.Read(dir));
            Xml = e;
            UseExternalPayments = e.GetValue<int>("UseExternalPayments");
            MaxStackablePotions = e.GetValue<int>("MaxStackablePotions");
            PotionPurchaseCooldown = e.GetValue<int>("PotionPurchaseCooldown");
            PotionPurchaseCostCooldown = e.GetValue<int>("PotionPurchaseCostCooldown");

            var newAccounts = e.Element("NewAccounts");
            NewAccounts = new NewAccounts(e.Element("NewAccounts"));
            newAccounts.Remove(); // don't export with /app/init

            var newCharacters = e.Element("NewCharacters");
            NewCharacters = new NewCharacters(e.Element("NewCharacters"));
            newCharacters.Remove();

            List<int> costs = new List<int>();
            foreach (var i in e.Element("PotionPurchaseCosts").Elements("cost"))
                costs.Add(Utils.GetInt(i.Value));
            PotionPurchaseCosts = costs.ToArray();
        }
    }

    public class NewAccounts
    {
        public readonly int MaxCharSlot;
        public readonly int VaultCount;
        public readonly int Fame;
        public readonly int Credits;
        public readonly int[] Slots;
        public readonly bool ClassesUnlocked;
        public readonly bool SkinsUnlocked;

        public NewAccounts(XElement e)
        {
            MaxCharSlot = e.GetValue<int>("MaxCharSlot", 1);
            VaultCount = e.GetValue<int>("VaultCount", 1);
            Fame = e.GetValue<int>("Fame", 0);
            Credits = e.GetValue<int>("Credits", 0);

            ClassesUnlocked = e.HasElement("ClassesUnlocked");
            SkinsUnlocked = e.HasElement("SkinsUnlocked");

            if (e.HasElement("Slots"))
            {
                List<int> slots = new List<int>();
                foreach (var i in e.Element("Slots").Elements("cost"))
                    slots.Add(Utils.GetInt(i.Value));
                Slots = slots.ToArray();
            }
            else
                Slots = new int[1] { 1000 };
        }

        public int GetPrice(int slot)
        {
            return Slots[Math.Max(Math.Min(slot - MaxCharSlot, Slots.Length - 1), 0)];
        }
    }

    public class NewCharacters
    {
        public readonly bool Maxed;
        public readonly int Level;

        public NewCharacters(XElement e)
        {
            Maxed = e.HasElement("Maxed");
            Level = e.GetValue<int>("Level", 1);
        }
    }
}
