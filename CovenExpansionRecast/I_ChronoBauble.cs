using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_ChronoBauble : Item
    {
        public I_ChronoBauble(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Chronobauble";
        }

        public override string getShortDesc()
        {
            return "Time flows quickly around the wearer of this charm. They will age twice as fast and their traits will trigger their start of turn effects an additional time each turn.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_ChronoBauble.png");
        }

        public override void turnTick(Person owner)
        {
            foreach (Trait trait in owner.traits)
            {
                trait.turnTick(owner);
            }

            if (map.turn % 52 == owner.birthday)
            {
                owner.age++;
            }
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
