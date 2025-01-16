using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_SquashToad : Challenge
    {
        public Person Person;

        public Ch_SquashToad(Location loc, Person p)
            : base(loc)
        {
            Person = p;
        }

        public override string getName()
        {
            if (Person != null)
            {
                return $"Squash {Person.getName()}";
            }

            return "Squash a Toad";
        }

        public override string getDesc()
        {
            if (Person != null)
            {
                return $"Squashes {Person.getName()}, killing them.";
            }

            return "Squashes someone, killing them.";
        }

        public override string getCastFlavour()
        {
            return "All the carefully thought out layers of defense only apply when the target is human. As a frog all that is required is proper timing and a well laced boot.";
        }

        public override string getRestriction()
        {
            return "Requires a toad to be present";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_FrogCurse.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            bool valid = Person != null && Person.traits.Any(t => t is T_Toad toad && (toad.Timer == -1 || toad.Timer > 0)) && ((Person.unit is UAEN_Toad && Person.unit.location == location) || location.person() == Person);
            if (!valid && location.settlement != null)
            {
                location.settlement.customChallenges.Remove(this);
            }
            return valid;
        }

        public override double getComplexity()
        {
            return 10;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.INTRIGUE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatIntrigue();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Intrigue", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));

            return val;
        }

        public override int getCompletionProfile()
        {
            return 3;
        }

        public override int getCompletionMenace()
        {
            return 4;
        }

        public override void complete(UA u)
        {
            if (Person == null)
            {
                return;
            }

            // Person.die handles the proper removal of the unit, and ruler clearance.
            u.person.statistic_kills++;
            Person.die($"Squashed by {u.getName()}", true, u.person);

            if (u.location.settlement != null)
            {
                u.location.settlement.customChallenges.Remove(this);
            }
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.COMBAT,
                Tags.CRUEL
            };
        }

        public override int[] getNegativeTags()
        {
            if (Person != null)
            {
                return Person.getTags();
            }

            return new int[0];
        }
    }
}
