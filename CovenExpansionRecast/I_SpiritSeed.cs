using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_SpiritSeed : Item
    {
        public I_SpiritSeed(Map map)
            : base(map)
        {
            challenges.Add(new Rti_SpiritTree(map.locations[0]));
        }

        public override string getName()
        {
            return "Spirit Tree Seed";
        }

        public override string getShortDesc()
        {
            return "A seed from an otherworldly arbor. Can be used to create a spirit tree massively increasing a locations habitability and prosperity.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_Seed.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            return challenges;
        }

        public override int getLevel()
        {
            return LEVEL_RARE;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
