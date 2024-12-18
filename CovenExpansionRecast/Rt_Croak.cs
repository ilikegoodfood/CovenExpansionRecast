using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rt_Croak : Ritual
    {
        public UA Owner;

        public Rt_Croak(Location location, UA owner)
            : base(location)
        {
           Owner = owner;
        }

        public override string getName()
        {
            return "Croak!";
        }

        public override string getDesc()
        {
            return "Does nothing.";
        }

        public override string getCastFlavour()
        {
            return "In some extremely unusual and generally undesiarable situations, all you can do is hop around and make croaking noises. This is one such situation.";
        }

        public override Sprite getSprite()
        {
            return map.world.iconStore.enshadow;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override double getProfile()
        {
            return 10;
        }

        public override bool validFor(UA ua)
        {
            return ua is UAEN_Toad;
        }

        public override double getComplexity()
        {
            return 10;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override int getCompletionProfile()
        {
            return 3;
        }
        public override int getCompletionMenace()
        {
            return 3;
        }

        public override bool isIndefinite()
        {
            return true;
        }
    }
}
