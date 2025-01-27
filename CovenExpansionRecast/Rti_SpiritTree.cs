using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_SpiritTree : Ritual
    {
        public Rti_SpiritTree(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Plant Spirit Tree";
        }

        public override string getDesc()
        {
            return "Creates an enormous spirit tree at this location, boosting food production and prosperity. The tree will create forest guardian armies over time.";
        }

        public override string getCastFlavour()
        {
            return "The tree unfurls; its trunk taller than any building. Miles of roots weave through every part of city, strengthening its infrastructure and nurturing its citizans.";
        }

        public override string getRestriction()
        {
            return "Must be cast at a location with a human settlement.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_SpiritTree.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool validFor(UA ua)
        {
            return ua.location.settlement is SettlementHuman && !ua.location.properties.Any(pr => pr is Pr_SpiritTree);
        }

        public override double getComplexity()
        {
            return 60.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatLore();
            if (val >= 1.0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override double getUtility(UA ua, List<ReasonMsg> msgs)
        {
            double utility = 0.0;
            if (ua.society != null && ua.location.soc != null && (ua.society.isDark() == ua.location.soc.isDark()) || ua.society.getRel(ua.location.soc).state == DipRel.dipState.war || ua.society.getRel(ua.location.soc).state == DipRel.dipState.hostile)
            {
                msgs?.Add(new ReasonMsg("Benefit to my Enemies", -60.0));
                utility -= 60.0;
            }

            if (ua.society != null && ua.location.soc == ua.society)
            {
                msgs?.Add(new ReasonMsg("Benefit to my nation", 20.0));
                utility += 20.0;
            }

            if (ua.location.settlement is Set_City || ua.location.settlement is Set_DwarvenCity)
            {
                msgs?.Add(new ReasonMsg("Benefit to City", 40.0));
                utility += 40.0;
            }

            if (ua.location.index == ua.homeLocation)
            {
                msgs.Add(new ReasonMsg("Benefit to my home location", 30.0));
                utility += 30.0;
            }

            return utility;
        }

        public override void complete(UA u)
        {
            for (int i = 0; i < u.person.items.Length; i++)
            {
                if (u.person.items[i] is I_SpiritSeed)
                {
                    u.person.items[i] = null;
                    break;
                }
            }

            Pr_SpiritTree spiritTree = new Pr_SpiritTree(u.location);
            spiritTree.charge = 300.0;
            u.location.properties.Add(spiritTree);
        }

        public override int getCompletionProfile()
        {
            return 25;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.AMBITION,
                Tags.COOPERATION
            };
        }
    }
}
