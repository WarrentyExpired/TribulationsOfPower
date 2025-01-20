using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public class BondCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Bond", AccessLevel.Player, new CommandEventHandler(Bond_OnCommand));
        }

        [Usage("Bond")]
        [Description("Sets a targeted tamed animal to bonded.")]
        public static void Bond_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Select the tamed animal you wish to bond.");
            e.Mobile.Target = new BondTarget();
        }
    }

    public class BondTarget : Target
    {
        public BondTarget() : base(10, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            BaseCreature creature = targeted as BaseCreature;
            if (creature != null)
            {
                if (!creature.Tamable)
                {
                    from.SendMessage("That creature cannot be bonded.");
                }
                else if (!creature.Controlled || creature.ControlMaster != from)
                {
                    from.SendMessage("You do not control this creature.");
                }
                else if (creature.IsBonded)
                {
                    from.SendMessage("This creature is already bonded.");
                }
                else
                {
                    creature.IsBonded = true;
                    from.SendMessage("The creature is now bonded to you.");
                }
            }
            else
            {
                from.SendMessage("That is not a valid target.");
            }
        }
    }
}

