using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_CultivateMagicDisease : Challenge
    {
        public Pr_MagicPlague Plague;

        public Ch_CultivateMagicDisease(Location loc, Pr_MagicPlague plague)
            : base(loc)
        {
            Plague = plague;
        }

        public override string getName()
        {
            return "Cultivate Psychogenic Illness";
        }

        public override string getDesc()
        {
            double madnessCharge = 0.0;
            foreach (Property property in location.properties)
            {
                if (property is Pr_Madness)
                {
                    madnessCharge += property.charge / 100;
                }
            }

            return $"increases the <b>Psychogenic Illness</b> in this location by {map.param.ch_cultivatedisease_parameterValue7 * 0.8 * madnessCharge}. The increase scales with the <b>madness</b> at this location.";
        }

        public override string getCastFlavour()
        {
            return "What is real? What is true? The line between reality and fiction draws thin as the illness spreads.";
        }

        public override string getRestriction()
        {
            return $"Requires <b>Psychogenic Illness</b> at this location to be less than {map.param.ch_cultivatedisease_parameterValue8}";
        }

        public override double getProfile()
        {
            return map.param.ch_h_cultivatehergifts_aiProfile;
        }

        public override double getMenace()
        {
            if (location.getShadow() > map.param.ch_cultivatedisease_aiMenace)
            {
                return map.param.ch_cultivatedisease_aiMenace1;
            }

            return (150.0 - Plague.charge) * map.param.ch_cultivatedisease_parameterValue3;
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansion.Icon_PsychIllness.png");
        }

        public override bool valid()
        {
            return Plague != null && Plague.charge < map.param.ch_cultivatedisease_parameterValue8;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatLore();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));

            return val;
        }

        public override double getComplexity()
        {
            return map.param.ch_cultivatedisease_complexity;
        }

        public override int getCompletionProfile()
        {
            return map.param.ch_h_cultivatehergifts_completionProfile;
        }

        public override int getCompletionMenace()
        {
            return map.param.ch_h_cultivatehergifts_completionMenace;
        }

        public override void complete(UA u)
        {
            double madnessCharge = 0.0;
            foreach (Property property in location.properties)
            {
                if (property is Pr_Madness)
                {
                    madnessCharge += property.charge / 100;
                }
            }

            Plague.charge += map.param.ch_cultivatedisease_parameterValue7 * 0.8 * madnessCharge;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.CRUEL,
                Tags.DISEASE,
                Tags.MADNESS
            };
        }
    }
}
