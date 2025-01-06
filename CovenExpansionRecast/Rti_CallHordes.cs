using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_CallHordes : Ritual
    {
        public Rti_CallHordes(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Call Orc Hordes";
        }

        public override string getDesc()
        {
            return "Calls all orc hordes not in battle to this location. While casting the hordes move faster based on the casters command stat. Has no effect on the first turn.";
        }

        public override string getCastFlavour()
        {
            return "Move along, scum!";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_DominionBanner.png");
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.COMMAND;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override double getComplexity()
        {
            return 1.0;
        }

        public override bool isIndefinite()
        {
            return true;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override void turnTick(UA ua)
        {
            bool allGathered = true;
            foreach (Unit unit in map.units)
            {
                if (!(unit is UM_OrcArmy) || unit.task is Task_InBattle)
                {
                    allGathered = false;
                    continue;
                }

                if (unit.location == ua.location || (unit.task is Task_GoToLocation goToLocation && goToLocation.target == ua.location))
                {
                    continue;
                }

                Location[] path = Pathfinding.getPathTo(unit.location, ua.location, unit);
                if (path == null || path.Length < 2)
                {
                    continue;
                }

                unit.task = new Task_GoToLocation(ua.location);

                if (ua.getStatCommand() >= 5 && ua.task is Task_PerformChallenge performChallenge2 && performChallenge2.progress > 1.9)
                {
                    int i = 0;
                    while (i < ua.getStatCommand() / 5)
                    {
                        if (path == null || path.Length < 2)
                        {
                            break;
                        }

                        ua.map.adjacentMoveTo(unit, path[1]);
                        path = Pathfinding.getPathTo(unit.location, ua.location, unit);

                        i++;
                    }
                }
            }

            if (allGathered)
            {
                ua.task = null;
                if (ua.isCommandable())
                {
                    ua.map.addUnifiedMessage(ua, null, "Orc Hordes Gathered", $"All orc hordes that can reach {ua.location.getName()} have either arrived, or are on their way.", "Call Orc Hordes Complete", true);
                }
            }
        }

        public override int[] buildPositiveTags()
        {
            return new int[] {
                Tags.ORC
            };
        }
    }
}
