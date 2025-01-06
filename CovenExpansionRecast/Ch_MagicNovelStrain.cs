using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_MagicNovelStrain : Challenge
    {
        public Pr_MagicPlague Plague;

        public Ch_MagicNovelStrain(Location loc, Pr_MagicPlague plague)
            : base(loc)
        {
            Plague = plague;
        }

        public override string getName()
        {
            return "Shatter Lucidity";
        }

        public override string getDesc()
        {
            return "Halves the <b>Lucidity</b> in this and surrounding locations, allowing <b>Psychogenic Illnesses</b> to continue to grow.";
        }

        public override string getRestriction()
        {
            return "Requires <b>Psychogenic Illnesses</b>";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_PsychIllness.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Plague.charge > 0.0;
        }

        public override double getProfile()
        {
            return map.param.ch_novelstrain_aiProfile;
        }

        public override double getMenace()
        {
            return map.param.ch_novelstrain_aiMenace1;
        }

        public override double getComplexity()
        {
            return map.param.ch_novelstrain_complexity;
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

        public override int getCompletionProfile()
        {
            return map.param.ch_novelstrain_completionProfile;
        }

        public override int getCompletionMenace()
        {
            return map.param.ch_novelstrain_completionMenace;
        }

        public override void complete(UA u)
        {
            foreach (Property property in location.properties)
            {
                if (property is Pr_Lucidity)
                {
                    property.charge *= 0.5;
                }
            }

            foreach (Location neighbour in location.getNeighbours())
            {
                foreach (Property property in neighbour.properties)
                {
                    if (property is Pr_Lucidity)
                    {
                        property.charge *= 0.5;
                    }
                }
            }
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.CRUEL,
                Tags.DISEASE
            };
        }
    }
}
