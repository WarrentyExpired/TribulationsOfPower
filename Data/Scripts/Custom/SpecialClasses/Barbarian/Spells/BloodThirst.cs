using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Barbarian
{
	public class BloodThirstSpell : BarbarianSpell
	{
		private static SpellInfo m_Info = new SpellInfo("Blood Lust", "Blóðþrá", 203, 9031);
	
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.75 ); } }
		public override double RequiredSkill{ get{ return 20.0; } }
		public override int RequiredMana{ get{ return 7; } }
		public override int RequiredTithing{ get{ return 49; } }	
                public override bool BlocksMovement{ get{ return false; } }
		private PlayerMobile barbarian;

		public BloodThirstSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			BaseWeapon weapon = Caster.Weapon as BaseWeapon;
			if ( weapon == null || weapon is Fists )
			{
				Caster.SendAsciiMessage( "You must have a 2 handed weapon equipped." );
			}
			else if ( CheckSequence() )
			{
				Caster.SendMessage( "Your enemies wounds will healing you.");
				DrainStainOnCloth( Caster, RequiredTithing );
      	                	Caster.PlaySound( 0x387 );
      	                        Caster.FixedParticles( 0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head );
                                Caster.FixedParticles( 0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255 );
                                new SoundEffectTimer( Caster ).Start();

				int nBenefit = 0;
				if ( Caster is PlayerMobile )
					nBenefit = (int)(Caster.Skills[SkillName.Anatomy].Value / 4);

				TimeSpan duration = TimeSpan.FromSeconds( (Spell.ItemSkillValue( Caster, SkillName.Tactics, false ) / 3.4) + 1.0 + nBenefit );

				Timer t = (Timer)m_Table[weapon];
				if ( t != null )
					t.Stop();

				weapon.Cursed = true;
				
				m_Table[weapon] = t = new ExpireTimer( weapon, duration );
				t.Start();
			}
			FinishSequence();
		}

		private static Hashtable m_Table = new Hashtable();
		private class ExpireTimer : Timer
		{
			private BaseWeapon m_Weapon;
			public ExpireTimer( BaseWeapon weapon, TimeSpan delay ) : base( delay )
			{
				m_Weapon = weapon;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Weapon.Cursed = false;
                                m_Table.Remove( this );
				Effects.PlaySound( m_Weapon.GetWorldLocation(), m_Weapon.Map, 0xFA );
      	                        m_Table.Remove( this );
			}			

		}
	
		private class SoundEffectTimer : Timer
		{
			private Mobile m_Mobile;

			public SoundEffectTimer( Mobile m ) : base( TimeSpan.FromSeconds( 0.75 ) )
			{
				m_Mobile = m;
				Priority = TimerPriority.FiftyMS;
			}
		
			protected override void OnTick()
			{
				m_Mobile.PlaySound( 0xFA );
			}
		}
	}
}
