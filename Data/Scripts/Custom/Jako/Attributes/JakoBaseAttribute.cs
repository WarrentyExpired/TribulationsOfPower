﻿using System;
using Server;
using Server.Mobiles;

namespace Custom.Jerbal.Jako
{
    public abstract class JakoBaseAttribute : IComparable
    {
        protected uint m_traitsGiven = 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual uint TraitsGiven
        {
            get { return m_traitsGiven; }
            set { m_traitsGiven = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual uint IncreasedBy
        {
            get { return (m_traitsGiven * AttributesGiven/PointsTaken); }          
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public abstract uint Cap
        {
            get;   
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public abstract double CapScale
        {
            get;   
        }

        public abstract uint GetStat(BaseCreature bc);

        protected abstract void SetStat(BaseCreature bc, uint toThis);

        [CommandProperty(AccessLevel.GameMaster)]
        public abstract uint AttributesGiven
        {
            get;   
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public abstract uint PointsTaken
        {
            get;
        }

        public virtual uint MaxBonus(BaseCreature bc)
        {
            if (GetStat(bc) >= Cap)
                return (GetStat(bc));
            if ((int)((GetStat(bc) - IncreasedBy) * CapScale) > Cap)
                return Cap;
            return (uint)((GetStat(bc) - IncreasedBy) * CapScale);
        }

        public virtual string DoOnClick(BaseCreature bc)
        {
            if (MaxBonus(bc) <= (GetStat(bc) + AttributesGiven))
                return "That would place this creature's stat above maximum.";
            if ((int)bc.Traits < PointsTaken)
                return "You do not have enough traits to do that.";

            if (IncBonus(bc, AttributesGiven))
                return "Attribute adjusted.";

            return "Error in IncBonus."; //Hopefully no one ever sees this, if they do, we know where it is.
            

        }

        public virtual bool IncBonus(BaseCreature bc, uint byThis)
        {
            return SetBonus(bc, GetStat(bc) + byThis);
        }

        new public abstract string ToString();

        public virtual bool SetBonus(BaseCreature bc, uint toThis)
        {
            if (toThis > MaxBonus(bc))
                return false;
            uint oldTraits = TraitsGiven;
            TraitsGiven += ((toThis - GetStat(bc)) / AttributesGiven) * PointsTaken;            
            bc.Traits -= TraitsGiven - oldTraits;
            SetStat(bc, toThis);
            return true;
        }
        public virtual void Serialize(GenericWriter writer)
        {

            writer.Write(1);//Version
            writer.Write(m_traitsGiven);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        m_traitsGiven = reader.ReadUInt();
                        break;
                    }
                    
            }
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            return (int)(TraitsGiven - ((JakoBaseAttribute)obj).TraitsGiven);
        }

        #endregion
    }
}
