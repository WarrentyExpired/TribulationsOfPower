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
	public class BarbarianLoinCloth : BaseGiftWaist, ISpecializationObserver, IDisableableItem
	{
		public int Stains;
		private PlayerMobile _boundTo;
		
		public bool IsDisabled
		{
			get
			{
				return _boundTo == null || _boundTo.CustomClass != SpecializationType.Barbarian || _boundTo.NpcGuild != NpcGuild.WarriorsGuild;
			}
		}

		[Constructable]
		public BarbarianLoinCloth() : base( 0x2B68 )
		{
			Name = "Barbarians Loincloth";
			Weight = 1.0;
			Server.Misc.Arty.ArtySetup( this, m_Points, "" );
		}

		public BarbarianLoinCloth( Serial serial ) : base( serial )
		{
		}

		public override bool CanEquip(Mobile from)
		{
			var canEquip = base.CanEquip(from);
			if (!canEquip) return false;
			if ((int)from.Skills[SkillName.Tactics].Base > 90)
			{
				return true;
			}
			else
			{
				from.SendAsciiMessage("You need 90 or more in Tactics Skill");
				return false;
			}
			if (_boundTo == null || _boundTo == from) return true;
			from.SendAsciiMessage("This is not your loincloth!");
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
				m_Points = (int)(player.Skills[SkillName.Tactics].Base + player.Skills[SkillName.Anatomy].Base) / 2;
				InvalidateProperties();
				player.SendAsciiMessage("The Loincloth is now yours.");
			}
			return true;
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			if (IsDisabled)
			{
				base.AddNameProperty(list);
				list.Add(1049644, "Required Guild: Warrior");
				list.Add(1049644, "Required Class: Barbarian Activated");
				if (_boundTo == null) list.Add(1070722, "Will this Loincloth fit you?");
			}
			else
			{
				base.GetProperties(list);
			}
			if (_boundTo != null) list.Add(1070722, "The Loincloth only fits " + _boundTo.Name);
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
