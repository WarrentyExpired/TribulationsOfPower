using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Collections;

namespace Server.Spells.Barbarian
{
	public abstract class BarbarianSpell : Spell
	{
		public abstract int RequiredStains { get; }
		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana { get; }
		public override bool ClearHandsOnCast { get { return false; } }
		public override SkillName CastSkill { get { return SkillName.Tactics; } }
		public override SkillName DamageSkill { get { return SkillName.Tactics; } }
		public override int CastRecoveryBase { get { return 7; } }
		
		public BarbarianSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

	
		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			return false;
			if ( Caster.Stam < (int)(10 * MySettings.S_PlayerLevelMod ) )
			{
				Caster.SendMessage( "You are to fatigued to do that now." );
				return false;
			}
			else if ( Caster.Fame < 0 )
			{
				Caster.SendMessage("You are not Famous enough to use this ability.");
				return false;
			}
			else if ( Caster.Skills[CastSkill].Value < RequiredSkill )
			{
				Caster.SendMessage("You must have at least " + RequiredSkill + " Tactics to use this ability.");
				return false;
			}
			else if ( GetStainsOnCloth( Caster ) < RequiredStains )
			{
				Caster.SendMessage("You must have at least " + RequiredStains + " Stains to uses this ability.");
				return false;
			}
			else if ( Caster.Mana < GetMana() )
			{
				Caster.SendMessage( "You must have at least " + GetMana() + " Mana to use this ability.");
				return false;
			}
			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredStains = GetStainsOnCloth( Caster, this );
			int mana = GetMana();
		
			if ( Caster.Stam < (int)( 10 * MySettings.S_PlayerLevelMod ) )
			{
				Caster.SendMessage( "You are too fatigued to do that now." );
                                return false;
                        	}
                        	else if ( Caster.Fame < 0 )
                        	{
                                	Caster.SendMessage( "You are not Famous enough to do that now." );
                                	return false;
                        	}
                        	else if ( Caster.Skills[CastSkill].Value < RequiredSkill )
                        	{
                                	Caster.SendMessage( "You must have at least " + RequiredSkill + " Tactics to use this ability." );
                                	return false;
                        	}
                        	else if ( GetStainsOnCloth( Caster ) < requiredStains )
                        	{
                                	Caster.SendMessage( "You must have at least " + requiredStains + " Stains to use this ability." );
                        	        return false;
                        	}
                        	else if ( Caster.Mana < mana )
                        	{
                                	Caster.SendMessage( "You must have at least " + mana + " mana to use this ability." );
                                	return false;
	                        }

        	                if ( !base.CheckFizzle() )
                        	        return false;

                	        return true;
                	}

		public override void DoFizzle()
		{
        		Caster.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "You fail to invoke the power.", Caster.NetState);
                	Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
                	Caster.PlaySound( 0x19D );
                	Caster.NextSpellTime = DateTime.Now;
        	}

		public override int ComputeFameAward()
		{
			int circle = (int)(RequiredSkill / 10);
                	if ( circle < 1 ){ circle = 1; }
	                return ( 40 + ( 10 * circle ) );
		}

	        public override int GetMana()
        	{
        		return ScaleMana( RequiredMana );
        	}

		public static int GetStainsOnCloth( Mobile Caster, BarbarianSpell spell )
        	{
        		if ( AosAttributes.GetValue( Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
                	return 0;

        	        return spell.RequiredStains;
        	}

       	 	public override void SayMantra()
        	{
        		Caster.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, Info.Mantra );
                	Caster.PlaySound( 0x19E );
        	}

        	public override void DoHurtFizzle()
        	{
        		Caster.PlaySound( 0x19D );
        	}

        	public override void OnDisturb( DisturbType type, bool message )
        	{
        		base.OnDisturb( type, message );

                	if ( message )
                	Caster.PlaySound( 0x19D );
        	}

        	public override void OnBeginCast()
        	{
        		base.OnBeginCast();
			Caster.FixedEffect( 0x37C4, 10, 42, 4, 3 );
        	}

        	public override void GetCastSkills( out double min, out double max )
        	{
        		min = RequiredSkill;
                	max = RequiredSkill + 40.0;
        	}

                public int ComputePowerValue( int div )
                {
                        return ComputePowerValue( Caster, div );
                }

                public static int ComputePowerValue( Mobile from, int div )
                {
                        if ( from == null )
                                return 0;

                        int v = (int) Math.Sqrt( from.Fame + 20000 + ( from.Skills.Tactics.Fixed * 10 ) );

                        return v / div;
                }

		public static void DrainStainOnCloth( Mobile from, int stains )
		{
			if ( AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
				stains = 0;
			if ( stains > 0 )
			{
				ArrayList targets = new ArrayList();
				foreach ( Item item in World.Items.Values )
				{
					if ( item is BarbarianLoinCloth )
					{
						BarbarianLoinCloth loincloth = (BarbarianLoinCloth)item;
						if ( loincloth._boundTo == from )
						{
							loincloth.Stains = loincloth.Stains - stains;
							if ( loincloth.Stains < 1 ){ loincloth.Stains = 0; }
							loincloth.InvalidateProperties();
						}
					}
				}
			}
		}

		public static int GetStainsOnCloth( Mobile from )
		{
			int stains = 0;
			ArrayList targets = new ArrayList();
			foreach ( Item item in World.Items.Values )
			{
				if ( item is BarbarianLoinCloth )
				{
					BarbarianLoinCloth loincloth = (BarbarianLoinCloth)item;
					if ( loincloth._boundTo == from )
					{
						stains = loincloth.Stains;
					}
				}
			}
			return stains;
		}

		public static double GetFamePower( Mobile from )
		{
			int fame = ( from.Fame );
				if ( fame < 1 ){ fame = 0; }
				if ( fame > 15000 ){ fame = 15000; }

			double smash = fame / 125;
			return smash;
		}
	}
}





