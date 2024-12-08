using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Barbarian
{
    public class RageSpell : BarbarianSpell
    {
            private static SpellInfo m_Info = new SpellInfo("Rage", "Þjóstr", 203, 9031);
            public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0); } }
            public override double RequiredSkill{ get { return 40; } }
            public override int RequiredMana{ get{ return 50; } } // Uses Stamina Instead of Mana
            public override int RequiredStains{ get{ return 60; } }
            public override bool BlocksMovement{ get{ return false; } }
            private PlayerMobile barbarian;
             
            public RageSpell(Mobile caster, Item scroll ) : base( caster, scroll, m_Info)
            {
            }

            public override void OnCast()
            {
                BaseWeapon weapon = Caster.Weapon as BaseWeapon;
		int hasHI = weapon.Attributes.AttackChance;
		int hasSSI = weapon.Attributes.WeaponSpeed;
                if (Caster is PlayerMobile && !((PlayerMobile)Caster).Barbarian())
                {
                    Caster.SendMessage("You are not a Barbarian.");
                }
                else if ( CheckSequence())
                {
                    Caster.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "You fly in to a furious RAGE!.", Caster.NetState);
                    //Caster.SendMessage("You fly in to a furious RAGE!");
                    DrainStainOnCloth( Caster, RequiredStains );
      	                	Caster.PlaySound( 0x387 );
      	                        Caster.FixedParticles( 0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head );
                                Caster.FixedParticles( 0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255 );
                                new SoundEffectTimer( Caster ).Start();

				//int nBenefit = 0;
				//if ( Caster is PlayerMobile )
				//	nBenefit = (int)(Caster.Skills[SkillName.Anatomy].Value / 4);

				//TimeSpan duration = TimeSpan.FromSeconds( (Spell.ItemSkillValue( Caster, SkillName.Tactics, false ) / 3.4) + 1.0 + nBenefit );
				TimeSpan duration = TimeSpan.FromSeconds( 30 );
				Timer t = (Timer)m_Table[weapon];
				if ( t != null )
					t.Stop();

				weapon.Attributes.AttackChance = hasHI + 20;
				weapon.Attributes.WeaponSpeed = hasSSI + 30;
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
				
				m_Weapon.Attributes.AttackChance = m_Weapon.Attributes.AttackChance - 20 ;
				m_Weapon.Attributes.WeaponSpeed = m_Weapon.Attributes.WeaponSpeed - 30;
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
