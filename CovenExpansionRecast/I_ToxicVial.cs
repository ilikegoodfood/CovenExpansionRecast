using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_ToxicVial : Item
    {
        public I_ToxicVial(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Phthisical Vial";
        }

        public override string getShortDesc()
        {
            return "A vial of liquid misery. +2 <b>Intrigue</b>, When completing a poison hero challenge the disruption lasts twice as long and the stat penalty becomes permanent.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansion.Icon_ToxicVial.png");
        }

        public override int getIntrigueBonus()
        {
            return 2;
        }

        public override void turnTick(Person owner)
        {
            T_Poisoner poisoner = (T_Poisoner)owner.traits.FirstOrDefault(t => t is T_Poisoner);
            if (poisoner == null)
            {
                poisoner = new T_Poisoner();
                owner.receiveTrait(poisoner);
            }
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
