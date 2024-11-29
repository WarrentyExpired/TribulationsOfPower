using System;
using Server;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Custom;
using Server.Engines.Craft;
using Server.Mobiles;
using System.Text;
using Server.Misc;

namespace Server.Items
{
	public class BarbarianHelm : BaseGiftArmor, ISpecializationObserver, IDisableableItem
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 80; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 40; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		private PlayerMobile _boundTo;

		public bool IsDisabled
		{
			get
			{
				return _boundTo == null || _boundTo.CustomClass != SpecializationType.Barbarian || _boundTo.NpcGuild != NpcGuild.WarriorsGuild;
			}
		}

		[Constructable]
		public BarbarianHelm() : base( 0x2645 )
		{
			Name = "Barbarians Helm";
			Weight = 5.0;
			Server.Misc.Arty.ArtySetup( this, m_Points, "" );
		}

		public BarbarianHelm( Serial serial ) : base( serial )
		{
		}
		
		public override bool CanEquip(Mobile from)
		{
			var canEquip = base.CanEquip(from);
			if (!canEquip) return false;
			if (_boundTo == null || _boundTo == from) return true;
			from.SendAsciiMessage("These helm did not accept you.");
			return false;
		}

		public override bool OnEquip(Mobile from)
		{
			var equipped = base.OnEquip(from);
			if (!equipped) return false;
			var player = from as PlayerMobile;
			if (player == null) return false;
			if (_boundTo == null)
			{
				_boundTo = player;
                int t_Points = 0;
				int a_Points = 0;
				t_Points = 2 * (int)(player.Skills[SkillName.Tactics].Base);
				a_Points = (int)(player.Skills[SkillName.Anatomy].Base) / 2;
				int total_Points = t_Points + a_Points;
				if (total_Points > 300)
				{
					m_Points = 300;
				}
				else
				{
					m_Points = total_Points;
				}
				InvalidateProperties();
				player.SendAsciiMessage("The Helm accepts your lust for battle!");
			}
			return true;
		}
		public override void GetProperties(ObjectPropertyList list)
		{
			if (IsDisabled)
			{
				base.AddNameProperty(list);
				list.Add(1049644, "Required Class: Barbarian");
				list.Add(1049644, "Required Guild: Warrior");
				if (_boundTo == null) list.Add(1070722, "Will Cromm accepts you.");
				else if (m_Points > 5) list.Add(1070722, "Single Click to Enchant");
			}
			else
			{
				base.GetProperties(list);
			}
			if (_boundTo != null) list.Add(1070722, "The Helm accepted " + _boundTo.Name);
		}

		public void SpecializationUpdated(PlayerMobile player, SpecializationType specialization)
		{
			if (IsDisabled) RemoveStatMods(player);
			else AddStatMods(player);
			InvalidateProperties();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write( (int) 1 );
			writer.WriteMobile(_boundTo);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			_boundTo = reader.ReadMobile() as PlayerMobile;
		}
	}
}
