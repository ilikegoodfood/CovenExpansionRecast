using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_RatIconSteal : Ritual
    {
        UA Target;

        public Rti_RatIconSteal(Location location, UA target)
            : base(location)
        {
            Target = target;
        }

        public override string getName()
        {
            return "Rats Theft";
        }

        public override string getDesc()
        {
            return $"Steals all gold and a personal item from {Target.getName()}.";
        }

        public override string getCastFlavour()
        {
            return "An open vein, an open pocket. To the Razor Icon's bearer they are the same.";
        }

        public override string getRestriction()
        {
            return "Must be cast at a location with a damaged hero.";
        }

        public override Sprite getSprite()
        {
            if (Target != null && Target.person != null)
            {
                return Target.person.getPortrait();
            }

            return EventManager.getImg("CovenExpansionRecast.Fore_RazorTrophy.png");
        }

        public override Challenge.challengeStat getChallengeType()
        {
            return Challenge.challengeStat.OTHER;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Target != null &&  Target.person != null && !Target.isDead;
        }

        public override bool validFor(UA ua)
        {
            return ua.person.items.Any(i => i is I_RatIcon);
        }

        public override double getUtility(UA ua, List<ReasonMsg> msgs)
        {
            double utility = base.getUtility(ua, msgs);
            if (Target != null && Target.person != null)
            {
                double val = Target.person.gold / 2.0;
                if (val > 0.0)
                {
                    msgs?.Add(new ReasonMsg("Potential Gold Gained", val));
                }
                utility += val;
            }

            return utility;
        }

        public override double getComplexity()
        {
            return 1.0;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override int getCompletionMenace()
        {
            return 5;
        }

        public override int getCompletionProfile()
        {
            return 3;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.GOLD,
                Tags.DANGER
            };
        }

        public override int[] getNegativeTags()
        {
            if (Target != null && Target.person != null)
            {
                return new int[] {
                    Target.person.index + 10000
                };
            }

            return new int[0];
        }
    }
}
