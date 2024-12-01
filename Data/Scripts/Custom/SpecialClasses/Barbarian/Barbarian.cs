using Server.Items;
using Server.Mobiles;

namespace Server.Custom
{
    public class BarbarianSpecialization : ClassSpecializationBase, IClassSpecialization
    {
        public BarbarianSpecialization() : base(SpecializationType.Barbarian)
        {
        }

        public override bool ValidateEquipment(PlayerMobile player)
        {
	    var hasValidWeapon = false;
	    var hasLoinCloth = false;
            foreach (var item in player.Items)
            {
                switch (item.Layer)
                {
                    case Layer.OneHanded:
			if (item != null) return false;
			break;
                    case Layer.TwoHanded:
                        if (item != null && (item is BaseShield || item is Fists || item is PugilistGloves || item is ThrowingGloves || item is BaseRanged))
			{
				hasValidWeapon = false;
			}
			else
			{
				hasValidWeapon = true;
			}
                        break;

		    case Layer.Waist:
			if (item != null)
			{
				hasLoinCloth = hasLoinCloth || item is BarbarianLoinCloth;
			}
			break;

                    case Layer.Helm:
			if (item != null && !(item is BaseArmor)) return false;
			break;

                    case Layer.Neck:
			if (item != null && !(item is BaseArmor)) return false;
			break;

                    case Layer.Gloves:
			if (item != null && !(item is BaseArmor)) return false;
			break;			

                    case Layer.Pants: // Leg Armor
			if (item != null && !(item is BaseArmor)) return false;
			break;

                    case Layer.Trinket:
                    case Layer.Bracelet:
                    case Layer.Earrings:
                    case Layer.Ring:
                    case Layer.Cloak:
                    case Layer.InnerLegs:
                    case Layer.OuterLegs:
                    case Layer.Shoes:
                        if (item != null && !(item is BaseClothing || item is IClothingStub || item is BaseTrinket)) return false;
                        break;

                    case Layer.InnerTorso: // Chest Armor
			if ( item != null ) return false;
			break;

                    case Layer.Arms:
			if ( item != null ) return false;
			break;

                    case Layer.Shirt:
			if ( item != null ) return false;
			break;

                    case Layer.OuterTorso:
			if (item != null ) return false;
			break;

                    case Layer.MiddleTorso:
			if ( item != null ) return false;
                        break;

                }
            }
	    return hasValidWeapon && hasLoinCloth;
        }

        public override bool ValidateSkills(PlayerMobile player)
        {
            if (player.Skills[SkillName.Tactics].Value < 90) return false;
	    if (player.Skills[SkillName.Anatomy].Value < 90) return false;
	    if (player.Skills[SkillName.Focus].Value < 50) return false;
            return true;
        }

        public override bool ValidateStats(PlayerMobile player)
        {
            if (player.Dex < 100) return false;
	    if (player.Str < 100) return false;

            return true;
        }
    }
}
