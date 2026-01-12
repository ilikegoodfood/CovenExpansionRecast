using Assets.Code;
using System.Collections.Generic;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_TreatMagicDisease : Challenge
    {
        public Pr_MagicPlague Plague;

        public Ch_TreatMagicDisease(Location loc, Pr_MagicPlague plague)
            : base(loc)
        {
            Plague = plague;
        }

        public override string getName()
        {
            return "Treat Psychogenic Illness";
        }

        public override string getDesc()
        {
            return "Reduces the level of psychogenic illness at this location by a large degree";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Lucid.png");
        }

        public override double getProfile()
        {
            if (Plague != null)
            {
                return Plague.charge * map.param.ch_treatdisease_aiProfile;
            }

            return 0.0;
        }

        public override double getMenace()
        {
            if (Plague != null)
            {
                return Plague.charge * map.param.ch_treatdisease_aiMenace;
            }

            return 0.0;
        }

        public override bool valid()
        {
            return Plague != null && Plague.charge > 0.0;
        }

        public override int isGoodTernary()
        {
            return 1;
        }

        public override double getComplexity()
        {
            return map.param.ch_treatdisease_complexity;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatLore();
            if (val > 0.0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));

            return val;
        }

        public override int getCompletionProfile()
        {
            return map.param.ch_treatdisease_completionProfile;
        }

        public override void complete(UA u)
        {
            if (Plague == null)
            {
                return;
            }

            Plague.charge += map.param.ch_treatdisease_parameterValue4;
            if (Plague.charge < 0.0)
            {
                Plague.charge = 0.0;
            }
        }

        public override int[] buildNegativeTags()
        {
            return new int[]
            {
                Tags.CRUEL,
                Tags.DISEASE
            };
        }
    }
}
