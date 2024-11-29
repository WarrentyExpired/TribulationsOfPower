using Server.Items;
using Server.Mobiles;

namespace Server.Custom
{
    public class SorcererSpecialization : ClassSpecializationBase, IClassSpecialization
    {
        public SorcererSpecialization() : base(SpecializationType.Sorcerer)
        {
        }

        public override bool ValidateEquipment(PlayerMobile player)
        {
            bool hasTrinket = false;
            foreach (var item in player.Items)
            {
                switch (item.Layer)
                {
                    case Layer.Trinket:
                        if (item == null) return false;

                        hasTrinket = item is Spellbook || item is NecromancerSpellbook;
                        break;

                    case Layer.OneHanded:
                    case Layer.TwoHanded:
                        if (item != null && (item is BaseWeapon || item is BaseShield)) return false;
                        break;

                    case Layer.Bracelet:
                    case Layer.Earrings:
                    case Layer.Neck:
                    case Layer.Ring:
                    case Layer.Arms:
                    case Layer.Cloak:
                    case Layer.Gloves:
                    case Layer.Helm:
                    case Layer.InnerLegs:
                    case Layer.InnerTorso:
                    case Layer.MiddleTorso:
                    case Layer.OuterLegs:
                    case Layer.OuterTorso:
                    case Layer.Pants:
                    case Layer.Shirt:
                    case Layer.Shoes:
                    case Layer.Waist:
                        if (item != null && !(item is BaseClothing || item is IClothingStub || item is BaseTrinket)) return false;
                        break;
                }
            }

            return hasTrinket;
        }

        public override bool ValidateSkills(PlayerMobile player)
        {
            if (player.Skills[SkillName.Magery].Value < 90) return false;
            if (player.Skills[SkillName.Psychology].Value < 90) return false;

            return true;
        }

        public override bool ValidateStats(PlayerMobile player)
        {
            if (player.Int < 50) return false;

            return true;
        }
    }
}
