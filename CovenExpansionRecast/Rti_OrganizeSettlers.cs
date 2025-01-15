using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_OrganizeSettlers : Ritual
    {
        public I_SettlersWreath Wreath;

        public Rti_OrganizeSettlers(Location location, I_SettlersWreath wreath)
            : base(location)
        {
            Wreath = wreath;
        }

        public override string getName()
        {
            return "Organize Mass Settlement";
        }

        public override string getDesc()
        {
            return "Discusses plans for mass colinization with a ruler to prepare for the claiming of land.";
        }

        public override string getRestriction()
        {
            return "Must be performed at the capital of a nation";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_SettlersWreath.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool validFor(UA ua)
        {
            if (ua.location.soc is Society society && ua.location.index == society.capital && !(society is HolyOrder) && ua.location.settlement is SettlementHuman humanSettlement && humanSettlement.ruler != null)
            {
                return true;
            }

            return false;
        }

        public override double getComplexity()
        {
            return 30.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.COMMAND;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatCommand();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Command", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override int getCompletionProfile()
        {
            return 4;
        }

        public override void complete(UA u)
        {
            Wreath.Society = u.location.soc as Society;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.COOPERATION
            };
        }
    }
}
