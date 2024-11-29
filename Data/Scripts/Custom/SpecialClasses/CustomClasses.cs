using Server.Mobiles;

namespace Server.Custom
{
    public static class CustomClasses
    {
        public static TroubadourSpecialization Troubadour = new TroubadourSpecialization();
        public static SorcererSpecialization Sorcerer = new SorcererSpecialization();
        public static BarbarianSpecialization Barbarian = new BarbarianSpecialization();

        public static bool Activate(PlayerMobile player)
        {
            return CustomClasses.Troubadour.Activate(player)
                || CustomClasses.Sorcerer.Activate(player)
                || CustomClasses.Barbarian.Activate(player);
        }
    }
}
