using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Custom.Jerbal.Jako;

namespace Server.SkillHandlers
{
	public class Druidism
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Druidism].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500328 ); // What animal should I look at?

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( !from.Alive )
				{
					from.SendLocalizedMessage( 500331 ); // The spirits of the dead are not the province of animal lore.
				}
				else if ( targeted is BaseCreature )
				{
					BaseCreature c = (BaseCreature)targeted;

					if ( !c.IsDeadPet )
					{
						if ( c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea )
						{
							if ( (!c.Controlled || !c.Tamable) && from.Skills[SkillName.Druidism].Base < 100.0 )
							{
								from.SendLocalizedMessage( 1049674 ); // At your skill level, you can only lore tamed creatures.
							}
							else if ( !c.Tamable && from.Skills[SkillName.Druidism].Base < 110.0 )
							{
								from.SendLocalizedMessage( 1049675 ); // At your skill level, you can only lore tamed or tameable creatures.
							}
							else if ( !from.CheckTargetSkill( SkillName.Druidism, c, 0.0, 120.0 ) )
							{
								from.SendLocalizedMessage( 500334 ); // You can't think of anything you know offhand.
							}
							else
							{
								from.CloseGump( typeof( DruidismGump ) );
								from.SendGump( new DruidismGump( c, from ) );
							}
						}
						else
						{
							from.SendLocalizedMessage( 500329 ); // That's not an animal!
						}
					}
					else
					{
						from.SendLocalizedMessage( 500331 ); // The spirits of the dead are not the province of animal lore.
					}
				}
				else
				{
					from.SendLocalizedMessage( 500329 ); // That's not an animal!
				}
			}
		}
	}

	public class DruidismGump : Gump
	{
		private BaseCreature m_bc;
		private static string FormatSkill( BaseCreature c, SkillName name )
		{
			Skill skill = c.Skills[name];

			if ( skill.Base < 10.0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0:F1}</div>", skill.Base );
        }

        #region Jako Taming
        private static string FormatAttributes(int cur, uint max)
        {
            return FormatAttributes(cur, (int)max);
        }
        #endregion

		private static string FormatAttributes( int cur, int max )
		{
			if ( max == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}/{1}</div>", cur, max );
		}

		private static string FormatStat( int val )
		{
			if ( val == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}</div>", val );
		}

		private static string FormatDouble( double val )
		{
			if ( val == 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0:F1}</div>", val );
		}

		private static string FormatElement( int val )
		{
			if ( val <= 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}%</div>", val );
		}

		#region Mondain's Legacy
		private static string FormatDamage( int min, int max )
		{
			if ( min <= 0 || max <= 0 )
				return "<div align=right>---</div>";

			return String.Format( "<div align=right>{0}-{1}</div>", min, max );
		}
		#endregion
        private static string FormatString(string str)
        {
            if (str.Length == 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0}</div>",str);

        }
		private const int LabelColor = 0x24E5;

		public DruidismGump( BaseCreature c, Mobile m ) : base( 250, 50 )
		{
            #region Jako Taming Added
            m_bc = c;
            #endregion
			AddPage( 0 );

			AddImage( 100, 100, 2080 );
			AddImage( 118, 137, 2081 );
			AddImage( 118, 207, 2081 );
			AddImage( 118, 277, 2081 );
			AddImage( 118, 347, 2083 );

			AddHtml( 147, 108, 210, 18, String.Format( "<center><i>{0}</i></center>", c.Name ), false, false );

			AddButton( 240, 77, 2093, 2093, 2, GumpButtonType.Reply, 0 );

			AddImage( 140, 138, 2091 );
			AddImage( 140, 335, 2091 );

            #region Jako Taming Edited
            int pages = ( Core.AOS ? 5 : 3 ) + ( c.JakoIsEnabled ? 1 : 0 ) + (c.Controlled && c.ControlMaster != null ? 1 : 0);
            #endregion

            int page = 0;


			#region Attributes
			AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 1049593, 200, false, false ); // Attributes

			AddHtmlLocalized( 153, 168, 160, 18, 1049578, LabelColor, false, false ); // Hits
			AddHtml( 280, 168, 75, 18, FormatAttributes( c.Hits, c.HitsMax ), false, false );

			AddHtmlLocalized( 153, 186, 160, 18, 1049579, LabelColor, false, false ); // Stamina
			AddHtml( 280, 186, 75, 18, FormatAttributes( c.Stam, c.StamMax ), false, false );

			AddHtmlLocalized( 153, 204, 160, 18, 1049580, LabelColor, false, false ); // Mana
			AddHtml( 280, 204, 75, 18, FormatAttributes( c.Mana, c.ManaMax ), false, false );

			AddHtmlLocalized( 153, 222, 160, 18, 1028335, LabelColor, false, false ); // Strength
			AddHtml( 320, 222, 35, 18, FormatStat( c.Str ), false, false );

			AddHtmlLocalized( 153, 240, 160, 18, 3000113, LabelColor, false, false ); // Dexterity
			AddHtml( 320, 240, 35, 18, FormatStat( c.Dex ), false, false );

			AddHtmlLocalized( 153, 258, 160, 18, 3000112, LabelColor, false, false ); // Intelligence
			AddHtml( 320, 258, 35, 18, FormatStat( c.Int ), false, false );

			if ( Core.AOS )
			{
				int y = 276;

				if ( Core.SE )
				{
					double bd = Items.BaseInstrument.GetBaseDifficulty( c );
					if ( c.Uncalmable )
						bd = 0;

					AddHtmlLocalized( 153, 276, 160, 18, 1070793, LabelColor, false, false ); // Barding Difficulty
					AddHtml( 320, y, 35, 18, FormatDouble( bd ), false, false );

					y += 18;
				}

				AddImage( 128, y + 2, 2086 );
				AddHtmlLocalized( 147, y, 160, 18, 1049594, 200, false, false ); // Loyalty Rating
				y += 18;

				AddHtmlLocalized( 153, y, 160, 18, (!c.Controlled || c.Loyalty == 0) ? 1061643 : 1049595 + (c.Loyalty / 10), LabelColor, false, false );
			}
			else
			{
				AddImage( 128, 278, 2086 );
				AddHtmlLocalized( 147, 276, 160, 18, 3001016, 200, false, false ); // Miscellaneous

				AddHtmlLocalized( 153, 294, 160, 18, 1049581, LabelColor, false, false ); // Armor Rating
				AddHtml( 320, 294, 35, 18, FormatStat( c.VirtualArmor ), false, false );
			}

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, pages );
			#endregion

			#region Resistances
			if ( Core.AOS )
			{
				AddPage( ++page );

				AddImage( 128, 152, 2086 );
				AddHtmlLocalized( 147, 150, 160, 18, 1061645, 200, false, false ); // Resistances

				AddHtmlLocalized( 153, 168, 160, 18, 1061646, LabelColor, false, false ); // Physical
				AddHtml( 320, 168, 35, 18, FormatElement( c.PhysicalResistance ), false, false );

				AddHtmlLocalized( 153, 186, 160, 18, 1061647, LabelColor, false, false ); // Fire
				AddHtml( 320, 186, 35, 18, FormatElement( c.FireResistance ), false, false );

				AddHtmlLocalized( 153, 204, 160, 18, 1061648, LabelColor, false, false ); // Cold
				AddHtml( 320, 204, 35, 18, FormatElement( c.ColdResistance ), false, false );

				AddHtmlLocalized( 153, 222, 160, 18, 1061649, LabelColor, false, false ); // Poison
				AddHtml( 320, 222, 35, 18, FormatElement( c.PoisonResistance ), false, false );

				AddHtmlLocalized( 153, 240, 160, 18, 1061650, LabelColor, false, false ); // Energy
				AddHtml( 320, 240, 35, 18, FormatElement( c.EnergyResistance ), false, false );

				AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
				AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			}
			#endregion

			#region Damage
			if ( Core.AOS )
			{
				AddPage( ++page );

				AddImage( 128, 152, 2086 );
				AddHtmlLocalized( 147, 150, 160, 18, 1017319, 200, false, false ); // Damage

				AddHtmlLocalized( 153, 168, 160, 18, 1061646, LabelColor, false, false ); // Physical
				AddHtml( 320, 168, 35, 18, FormatElement( c.PhysicalDamage ), false, false );

				AddHtmlLocalized( 153, 186, 160, 18, 1061647, LabelColor, false, false ); // Fire
				AddHtml( 320, 186, 35, 18, FormatElement( c.FireDamage ), false, false );

				AddHtmlLocalized( 153, 204, 160, 18, 1061648, LabelColor, false, false ); // Cold
				AddHtml( 320, 204, 35, 18, FormatElement( c.ColdDamage ), false, false );

				AddHtmlLocalized( 153, 222, 160, 18, 1061649, LabelColor, false, false ); // Poison
				AddHtml( 320, 222, 35, 18, FormatElement( c.PoisonDamage ), false, false );

				AddHtmlLocalized( 153, 240, 160, 18, 1061650, LabelColor, false, false ); // Energy
				AddHtml( 320, 240, 35, 18, FormatElement( c.EnergyDamage ), false, false );

				#region Mondain's Legacy
				if ( Core.ML )
				{
					AddHtmlLocalized( 153, 258, 160, 18, 1076750, LabelColor, false, false ); // Base Damage
					AddHtml( 300, 258, 55, 18, FormatDamage( c.DamageMin, c.DamageMax ), false, false );
				}
				#endregion

				AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
				AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			}
			#endregion

			#region Skills
			AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 3001030, 200, false, false ); // Combat Ratings

			AddHtmlLocalized( 153, 168, 160, 18, 1044103, LabelColor, false, false ); // Wrestling
			AddHtml( 320, 168, 35, 18, FormatSkill( c, SkillName.FistFighting ), false, false );

			AddHtmlLocalized( 153, 186, 160, 18, 1044087, LabelColor, false, false ); // Tactics
			AddHtml( 320, 186, 35, 18, FormatSkill( c, SkillName.Tactics ), false, false );

			AddHtmlLocalized( 153, 204, 160, 18, 1044086, LabelColor, false, false ); // Magic Resistance
			AddHtml( 320, 204, 35, 18, FormatSkill( c, SkillName.MagicResist ), false, false );

			AddHtmlLocalized( 153, 222, 160, 18, 1044061, LabelColor, false, false ); // Anatomy
			AddHtml( 320, 222, 35, 18, FormatSkill( c, SkillName.Anatomy ), false, false );

			#region Mondain's Legacy
			if ( c is CuSidhe )
			{
				AddHtmlLocalized( 153, 240, 160, 18, 1044077, LabelColor, false, false ); // Healing
				AddHtml( 320, 240, 35, 18, FormatSkill( c, SkillName.Healing ), false, false );
			}
			else
			{
				AddHtmlLocalized( 153, 240, 160, 18, 1044090, LabelColor, false, false ); // Poisoning
				AddHtml( 320, 240, 35, 18, FormatSkill( c, SkillName.Poisoning ), false, false );
			}
			#endregion

			AddImage( 128, 260, 2086 );
			AddHtmlLocalized( 147, 258, 160, 18, 3001032, 200, false, false ); // Lore & Knowledge

			AddHtmlLocalized( 153, 276, 160, 18, 1044085, LabelColor, false, false ); // Magery
			AddHtml( 320, 276, 35, 18, FormatSkill( c, SkillName.Magery ), false, false );

			AddHtmlLocalized( 153, 294, 160, 18, 1044076, LabelColor, false, false ); // Evaluating Intelligence
			AddHtml( 320, 294, 35, 18,FormatSkill( c, SkillName.Psychology ), false, false );

			AddHtmlLocalized( 153, 312, 160, 18, 1044106, LabelColor, false, false ); // Meditation
			AddHtml( 320, 312, 35, 18, FormatSkill( c, SkillName.Meditation ), false, false );

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			#endregion

            #region Jako Taming | Skills
            if (c.Tamable)
            {
                #region Jako Taming When Tamed
                if (c.Controlled && c.ControlMaster != null)
                {
                    AddPage(++page);

                    AddImage(128, 152, 2086);
                    AddHtml(147, 150, 160, 18, "<basefont color=#003142>Characteristics</basefont>", false, false);

                    AddHtml(153, 168, 160, 18, "<basefont color=#4A3929>Level</basefont>", false, false);
                    AddHtml(280, 168, 75, 18, FormatAttributes((int)c.Level, (int)c.MaxLevel), false, false);

                    AddHtml(153, 186, 160, 18, "<basefont color=#4A3929>Traits Remaining</basefont>", false, false);
                    AddHtml(280, 186, 75, 18, FormatStat((int)c.Traits), false, false);

                    AddHtml(153, 204, 160, 18, "<basefont color=#4A3929>Mating Level</basefont>", false, false);
                    AddHtml(280, 204, 75, 18, FormatStat((int)c.MatingLevel), false, false);

                    AddHtml(153, 222, 160, 18, "<basefont color=#4A3929>Sex</basefont>", false, false);
                    AddHtml(320, 222, 35, 18, FormatString(c.SexString), false, false);

                    AddHtml(153, 240, 160, 18, "<basefont color=#4A3929>Experience Earned</basefont>", false, false);
                    AddHtml(320, 240, 35, 18, FormatStat((int)c.Experience), false, false);

                    AddHtml(153, 258, 160, 18, "<basefont color=#4A3929>Experience Needed</basefont>", false, false);
                    AddHtml(320, 258, 35, 18, FormatStat((int)c.ExpToNextLevel), false, false);

                    AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
                    AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
                }
                #endregion


                #region Jako Taming Max Stats/Attributes
                AddPage(++page);
                
                AddImage(128, 152, 2086);
                AddHtml(147, 150, 160, 18, "<basefont color=#003142>Max Resistances</basefont>", false, false); // Resistances

                AddHtmlLocalized(153, 168, 160, 18, 1061646, LabelColor, false, false); // Physical
                AddHtml(280, 168, 75, 18, FormatAttributes(c.PhysicalResistance, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPhysResist).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 186, 160, 18, 1061647, LabelColor, false, false); // Fire
                AddHtml(280, 186, 75, 18, FormatAttributes(c.FireResistance, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusFireResist).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 204, 160, 18, 1061648, LabelColor, false, false); // Cold
                AddHtml(280, 204, 75, 18, FormatAttributes(c.ColdResistance, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusColdResist).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 222, 160, 18, 1061649, LabelColor, false, false); // Poison
                AddHtml(280, 222, 75, 18, FormatAttributes(c.PoisonResistance, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPoisResist).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 240, 160, 18, 1061650, LabelColor, false, false); // Energy
                AddHtml(280, 240, 75, 18, FormatAttributes(c.EnergyResistance, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusEnerResist).MaxBonus(c)), false, false);

                AddImage(128, 260, 2086);
                AddHtml(147, 258, 160, 18, "<basefont color=#003142>Max Attributes</basefont>", false, false); // Lore & Knowledge


                AddHtmlLocalized(153, 276, 160, 18, 1049578, LabelColor, false, false); // Hits
                AddHtml(280, 276, 75, 18, FormatAttributes(c.HitsMax, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Hits).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 294, 160, 18, 1049579, LabelColor, false, false); // Stamina
                AddHtml(280, 294, 75, 18, FormatAttributes(c.StamMax, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Stam).MaxBonus(c)), false, false);

                AddHtmlLocalized(153, 312, 160, 18, 1049580, LabelColor, false, false); // Mana
                AddHtml(280, 312, 75, 18, FormatAttributes(c.ManaMax, c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Mana).MaxBonus(c)), false, false);

                if (c.ControlMaster == m)
                {
                    Int32 locked = 0x82C;
                    Int32 up = 0x983;
                    Int32 b1004 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPhysResist).MaxBonus(c) <= c.PhysicalResistance ? locked : up);
                    Int32 b1005 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusFireResist).MaxBonus(c) <= c.FireResistance ? locked : up);
                    Int32 b1006 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusColdResist).MaxBonus(c) <= c.ColdResistance ? locked : up);
                    Int32 b1007 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPoisResist).MaxBonus(c) <= c.PoisonResistance ? locked : up);
                    Int32 b1008 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusEnerResist).MaxBonus(c) <= c.EnergyResistance ? locked : up);
                    Int32 b1001 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Hits).MaxBonus(c) <= c.HitsMax ? locked : up);
                    Int32 b1002 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Stam).MaxBonus(c) <= c.StamMax ? locked : up);
                    Int32 b1003 = (c.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Mana).MaxBonus(c) <= c.ManaMax ? locked : up);

                    AddButton(130, 168, b1004, b1004, 1004, GumpButtonType.Reply, 0);
                    AddButton(130, 186, b1005, b1005, 1005, GumpButtonType.Reply, 0);
                    AddButton(130, 204, b1006, b1006, 1006, GumpButtonType.Reply, 0);
                    AddButton(130, 222, b1007, b1007, 1007, GumpButtonType.Reply, 0);
                    AddButton(130, 240, b1008, b1008, 1008, GumpButtonType.Reply, 0);
                    AddButton(130, 276, b1001, b1001, 1001, GumpButtonType.Reply, 0);
                    AddButton(130, 294, b1002, b1002, 1002, GumpButtonType.Reply, 0);
                    AddButton(130, 312, b1003, b1003, 1003, GumpButtonType.Reply, 0);

                }


                AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
                AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
                #endregion
            }
            #endregion

            #region Misc
            AddPage( ++page );

			AddImage( 128, 152, 2086 );
			AddHtmlLocalized( 147, 150, 160, 18, 1049563, 200, false, false ); // Preferred Foods

			int foodPref = 3000340;

			if ( (c.FavoriteFood & FoodType.FruitsAndVegies) != 0 )
				foodPref = 1049565; // Fruits and Vegetables
			else if ( (c.FavoriteFood & FoodType.GrainsAndHay) != 0 )
				foodPref = 1049566; // Grains and Hay
			else if ( (c.FavoriteFood & FoodType.Fish) != 0 )
				foodPref = 1049568; // Fish
			else if ( (c.FavoriteFood & FoodType.Meat) != 0 )
				foodPref = 1049564; // Meat

			AddHtmlLocalized( 153, 168, 160, 18, foodPref, LabelColor, false, false );

			AddImage( 128, 188, 2086 );
			AddHtmlLocalized( 147, 186, 160, 18, 1049569, 200, false, false ); // Pack Instincts

			int packInstinct = 3000340;

			if ( (c.PackInstinct & PackInstinct.Canine) != 0 )
				packInstinct = 1049570; // Canine
			else if ( (c.PackInstinct & PackInstinct.Ostard) != 0 )
				packInstinct = 1049571; // Ostard
			else if ( (c.PackInstinct & PackInstinct.Feline) != 0 )
				packInstinct = 1049572; // Feline
			else if ( (c.PackInstinct & PackInstinct.Arachnid) != 0 )
				packInstinct = 1049573; // Arachnid
			else if ( (c.PackInstinct & PackInstinct.Daemon) != 0 )
				packInstinct = 1049574; // Daemon
			else if ( (c.PackInstinct & PackInstinct.Bear) != 0 )
				packInstinct = 1049575; // Bear
			else if ( (c.PackInstinct & PackInstinct.Equine) != 0 )
				packInstinct = 1049576; // Equine
			else if ( (c.PackInstinct & PackInstinct.Bull) != 0 )
				packInstinct = 1049577; // Bull

			AddHtmlLocalized( 153, 204, 160, 18, packInstinct, LabelColor, false, false );

			if ( !Core.AOS )
			{
				AddImage( 128, 224, 2086 );
				AddHtmlLocalized( 147, 222, 160, 18, 1049594, 200, false, false ); // Loyalty Rating

				AddHtmlLocalized( 153, 240, 160, 18, (!c.Controlled || c.Loyalty == 0) ? 1061643 : 1049595 + (c.Loyalty / 10), LabelColor, false, false );
			}

			AddButton( 340, 358, 5601, 5605, 0, GumpButtonType.Page, 1 );
			AddButton( 317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1 );
			#endregion
		}
        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            String reply = "" ;
            switch (info.ButtonID)
            {
                case 1001: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Hits).DoOnClick(m_bc); break;
                case 1002: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Stam).DoOnClick(m_bc); break;
                case 1003: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.Mana).DoOnClick(m_bc); break;
                case 1004: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPhysResist).DoOnClick(m_bc); break;
                case 1005: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusFireResist).DoOnClick(m_bc); break;
                case 1006: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusColdResist).DoOnClick(m_bc); break;
                case 1007: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusPoisResist).DoOnClick(m_bc); break;
                case 1008: reply = m_bc.m_jakoAttributes.GetAttribute(JakoAttributesEnum.BonusEnerResist).DoOnClick(m_bc); break;

            }

            if (reply != null)
                from.SendMessage(reply);

            base.OnResponse(sender, info);
        }
    }

   
}
