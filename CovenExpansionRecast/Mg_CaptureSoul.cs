using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_CaptureSoul : Ritual
    {
        public Pr_FallenHuman Target;

        public Mg_CaptureSoul(Location location, Pr_FallenHuman target)
            : base(location)
        {
            Target = target;
        }

        public override string getName()
        {
            return $"Capture {SoulTypeUtils.GetTitle(map.persons[Target.personIndex])} Soul";
        }

        public override string getDesc()
        {
            string result = $"Captures the soul of {map.persons[Target.personIndex].getFullName()} ({SoulTypeUtils.GetTitle(map.persons[Target.personIndex])}) in an inactive soulstone, saving it for later use.";

            if (map.persons[Target.personIndex].society == map.soc_dark)
            {
                result += $"They are an Agent of The Dark, and cannot be cursed.";
            }
            else if (map.persons[Target.personIndex].society is SG_AgentWanderers)
            {
                result += $"They have a Mounstrous Soul, and cannot be cursed.";
            }

            return result;
        }

        public override string getCastFlavour()
        {
            return "The afterlife can wait. We still have use for this one.";
        }

        public override string getRestriction()
        {
            return $"Requires an inactive soulstone and to be at the location of {Target.getName()}'s soul.";
        }

        public override Sprite getSprite()
        {
            if (Target == null)
            {
                return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Inactive");
            }

            Person person = location.map.persons[Target.personIndex];
            if (person.shadow > 0.5)
            {
                return person.getPortraitAlt();
            }

            return person.getPortrait();
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Target != null && Target.charge > 0.0;
        }

        public override bool validFor(UA ua)
        {
            return ua.location.properties.Contains(Target) && ua.person.traits.Any(t => t is T_MasteryCurseweaving mastery && mastery.level > 0) && ua.person.items.Any(i => i is I_Soulstone soulstone && soulstone.CapturedSoul == null);
        }

        public override Challenge.challengeStat getChallengeType()
        {
            return Challenge.challengeStat.OTHER;
        }

        public override double getUtility(UA ua, List<ReasonMsg> msgs)
        {
            double utility = base.getUtility(ua, msgs);

            Person target = map.persons[Target.personIndex];

            double val = -35.0 * (1.0 * ua.person.shadow);
            msgs?.Add(new ReasonMsg("Base Reluctance", val));
            utility += val;

            val = -30.0 * ua.person.getTagRanking(target.index + 10000);
            if (val > 0.0)
            {
                msgs?.Add(new ReasonMsg("Personal Grudge", val));
            }
            else if (val < 0.0)
            {
                msgs?.Add(new ReasonMsg("Desire for friend to rest in peace", val));
            }
            utility += val;

            val = -30.0 * ua.person.getTagRanking(target.house.index + 30000);
            if (val > 0.0)
            {
                msgs?.Add(new ReasonMsg("Vendetta against house", val));
                utility += val;
            }

            return utility;
        }

        public override double getComplexity()
        {
            return 2.0;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override void complete(UA u)
        {
            I_Soulstone soulstone = (I_Soulstone)u.person.items.FirstOrDefault(i => i is I_Soulstone stone && stone.CapturedSoul == null);
            if (soulstone == null || Target == null)
            {
                return;
            }

            soulstone.CapturedSoul = map.persons[Target.personIndex];
            Target.charge = 0.0;
            u.location.properties.Remove(Target);
            u.rituals.Remove(this);
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.CRUEL
            };
        }
    }
}
