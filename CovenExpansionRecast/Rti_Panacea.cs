using Assets.Code;
using DuloGames.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_Panacea : Ritual
    {
        public Rti_Panacea(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Employ Panacea";
        }

        public override string getDesc()
        {
            return "Uses a single drop of the Panacea to cure all Unrest, Political Instability, Hunger, Plague, Madness and Devastation in a location. Reduces the agents menace by 1 per 10% charge cured and increases their profile by the same amount.";
        }

        public override string getCastFlavour()
        {
            return "Even diluted a thousandfold, the Panacea is undeniably potent as the problems faced by the local populace wash away like spring rain.";
        }

        public override string getRestriction()
        {
            return "Must be used at settlement with a ruler which has one of the following cureable modifiers; Devastation, Famine, Madness, Plague, Political Instability, or Unrest.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_Panacea.png");
        }

        public override bool validFor(UA ua)
        {
            if (ua.location.person() == null)
            {
                return false;
            }

            foreach (Property pr in ua.location.properties)
            {
                if (pr is Pr_Unrest || pr is Pr_Plague || pr is Pr_Devastation || pr is Pr_PoliticalInstability || pr is Pr_Famine || pr is Pr_Madness)
                {
                    return true;
                }
            }

            return false;
        }

        public override double getComplexity()
        {
            return 2.0;
        }

        public override Challenge.challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override double getUtility(UA ua, List<ReasonMsg> msgs)
        {
            double utility = base.getUtility(ua, msgs);
            double val = 0.0;
            foreach (Property pr in ua.location.properties)
            {
                if (pr is Pr_Unrest || pr is Pr_Plague || pr is Pr_Devastation || pr is Pr_PoliticalInstability || pr is Pr_Famine || pr is Pr_Madness)
                {
                    val = pr.charge / 2.0;
                    msgs?.Add(new ReasonMsg($"Existing {pr.getName()}", val));
                    utility += val;
                }
            }

            return utility;
        }

        public override void complete(UA u)
        {
            double attention = 0.0;
            foreach (Property pr in u.location.properties)
            {
                if (pr is Pr_Unrest || pr is Pr_Plague || pr is Pr_Devastation || pr is Pr_PoliticalInstability || pr is Pr_Famine || pr is Pr_Madness)
                {
                    attention += pr.charge / 10.0;
                    pr.charge = 0.0;
                }
            }

            u.addMenace(-attention);
            u.addProfile(attention);
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.COOPERATION
            };
        }

        public override int[] buildNegativeTags()
        {
            return new int[]
            {
                Tags.DISCORD,
                Tags.DISEASE,
                Tags.MADNESS
            };
        }
    }
}
