using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rtm_SendCarrierPigeon : Ritual
    {
        public Rtm_SendCarrierPigeon(Location location)
            : base(location)
        {
            
        }

        public override string getName()
        {
            return "Send Carrier Pigeon";
        }

        public override string getDesc()
        {
            return "Transfer items and/or gold to your pigeon before sending them to trade with another of your agents. After making the delivery the Pigeon will return to this agent.";
        }

        public override string getRestriction()
        {
            return "Must have at least 2 agents.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Pigeon.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool valid()
        {
            int count = 0;
            foreach (Unit unit in map.overmind.agents)
            {
                if (unit is UA)
                {
                    count++;

                    if (count >= 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool validFor(UA ua)
        {
            if (!ua.minions.Any(m => m is M_Pigeon))
            {
                ua.rituals.Remove(this);
                return false;
            }

            return true;
        }

        public override double getComplexity()
        {
            return 2.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            if (msgs != null)
            {
                msgs.Add(new ReasonMsg("Base", 1.0));
            }
            return 1.0;
        }

        public override void complete(UA u)
        {
            List<UA> targets = new List<UA>();
            List<string> targetLabels = new List<string>();

            foreach (Unit unit in map.overmind.agents)
            {
                if (unit != u && unit is UA ua)
                {
                    targets.Add(ua);
                    targetLabels.Add(ua.getName());
                }
            }

            Sel2_PigeonTargetSelector selector = new Sel2_PigeonTargetSelector(map, u, targets);
            selector.Targets = targets;
            map.world.prefabStore.getScrollSetText(targetLabels, false, selector, "Choose Traget", "Select and agent for the carrier pigeon to fly to.");
        }

        public override int[] buildPositiveTags()
        {
            return new int[] { Tags.COOPERATION };
        }
    }
}
