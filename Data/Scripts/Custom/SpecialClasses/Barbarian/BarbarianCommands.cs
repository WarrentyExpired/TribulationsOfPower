using System;
using Server;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Barbarian;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class BarbarianCommands
	{
		public static void Initialize()
		{
			Properties.Initialize();
				Register( "BBloodThirst", AccessLevel.Player, new CommandEventHandler( BBloodThirst_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
			CommandSystem.Register(command, access, handler);
		}

                [Usage( "BBloodThirst" )]
                [Description( "Activates Blood Lust" )]
                public static void BBloodThirst_OnCommand( CommandEventArgs e )
                {
                	Mobile from = e.Mobile;
                	if ( !Multis.DesignContext.Check( e.Mobile ) ){ return; }
               		 new BloodThirstSpell( e.Mobile, null ).Cast();
                }
	}
}

