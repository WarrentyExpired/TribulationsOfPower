using Server.Mobiles;

namespace Server.Custom
{
    public enum SpecializationType
    {
        None = 0,
        Barbarian,
        Sorcerer,
        Troubadour
    }

    public interface IClassSpecialization
    {
        bool ValidateEquipment(PlayerMobile player);
        bool ValidateSkills(PlayerMobile player);
        bool ValidateStats(PlayerMobile player);
        bool Activate(PlayerMobile player);
    }
}
