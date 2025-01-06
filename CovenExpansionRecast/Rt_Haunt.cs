using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rt_Haunt : Ritual
    {
        public UAE_Spirit Owner;

        public Rt_Haunt(Location location, UAE_Spirit owner)
            : base(location)
        {
            Owner = owner;
        }

        public override string getName()
        {
            if (Owner?.Cage?.Holder != null)
            {
                return $"Haunt {Owner.Cage.Holder.getName()}";
            }

            return "Haunt";
        }

        public override string getDesc()
        {
            return "Torments the cages bearer causing them to lose 1 sanity each turn. This unit gains 0.4 menace each turn spent haunting.";
        }

        public override string getCastFlavour()
        {
            return "Subtle whisperings, disturbances on the edge of sleep. Subtle enough that those who complain of it would be dismissed as mad, even as they become so.";
        }

        public override string getRestriction()
        {
            return "Does not effect the chosen one, or player agents.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Spirit.png");
        }

        public override double getProfile()
        {
            return 10.0;
        }

        public override bool validFor(UA ua)
        {
            return Owner?.Cage?.Holder != null && (Owner.Cage.Holder.unit == null || (!Owner.Cage.Holder.unit.isCommandable() && Owner.Cage.Holder.unit != ua.map.awarenessManager.getChosenOne()));
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = Owner.getStatLore();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override void turnTick(UA ua)
        {
            ua.midchallengeTimer = 0;
        }

        public override bool isIndefinite()
        {
            return true;
        }

        public override int[] buildPositiveTags()
        {
            return new int[] {
                Tags.MADNESS
            };
        }

        public override int[] getNegativeTags()
        {
            if (Owner?.Cage?.Holder != null)
            {
                return new int[]
                {
                    Owner.Cage.Holder.index + 10000
                };
            }

            return new int[0];
        }
    }
}
