using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_CraftList : Item
    {
        public I_CraftList(Map map)
            : base(map)
        {
            challenges.Add(new Rti_ReadTransposingScroll(map.locations[0]));
        }

        public override string getName()
        {
            return "Transposing Scroll";
        }

        public override string getShortDesc()
        {
            return "A scroll containing recipes for soul transposition. Can be read to view all transposition recipes. Also increases the rate of transposition rituals by 1 per turn.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_List.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            return challenges;
        }

        public override double getChallengeProgressChange(Challenge challenge, UA unit, List<ReasonMsg> msgs)
        {
            if (challenge is Mg_Rti_TransposeSoul)
            {
                msgs?.Add(new ReasonMsg("Transposing Scroll", 1.0));
                return 1.0;
            }
            return 0.0;
        }

        public override int getLevel()
        {
            return LEVEL_COMMON;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
