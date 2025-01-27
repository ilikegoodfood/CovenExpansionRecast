using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rt_StudyCurseweaving : Ritual
    {
        public UA Caster;

        public Rt_StudyCurseweaving(Location location, UA caster)
            : base(location)
        {
            Caster = caster;
        }

        public override string getName()
        {
            return "Study Curseweaving";
        }

        public override string getDesc()
        {
            return "Increases the mastery of this character for the School of Curseweaving. Requires fewer secrets to learn than other schools of magic but needs soulstones filled with various types of souls for spells to be cast.";
        }

        public override string getCastFlavour()
        {
            return "Curseweaving is the art of sewing human souls into hexes and enchantments. Curseweaving allows the caster to bring entire families to ruin by using the souls of humans as thread and cloth to create intricate curses.";
        }

        public override string getRestriction()
        {
            T_MasteryCurseweaving curseweaving = (T_MasteryCurseweaving)Caster.person.traits.FirstOrDefault(t => t is T_MasteryCurseweaving);
            if (curseweaving.level < 3)
            {
                return $"Requires {GetArcaneSecretsRequired()} Arcane Knowledge";
            }
            else
            {
                return "Maximum level reached";
            }
        }

        public int GetArcaneSecretsRequired()
        {
            int[] array = new int[]
            {
                1,
                2,
                3
            };
            T_MasteryCurseweaving curseweaving = (T_MasteryCurseweaving)Caster.person.traits.FirstOrDefault(t => t is T_MasteryCurseweaving);
            if (curseweaving == null)
            {
                return array[0];
            }
            return array[curseweaving.level];
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Curseweave.png");
        }

        public override bool validFor(UA ua)
        {
            if (!ua.isCommandable())
            {
                return false;
            }

            T_MasteryCurseweaving curseweaving = (T_MasteryCurseweaving)Caster.person.traits.FirstOrDefault(t => t is T_MasteryCurseweaving);
            if (curseweaving.level >= curseweaving.getMaxLevel())
            {
                return false;
            }

            T_ArcaneKnowledge arcaneKnowledge = (T_ArcaneKnowledge)ua.person.traits.FirstOrDefault(t => t is T_ArcaneKnowledge);
            if (arcaneKnowledge == null || arcaneKnowledge.level < GetArcaneSecretsRequired())
            {
                return false;
            }

            return true;
        }

        public override double getComplexity()
        {
            return 40.0;
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
            if (val >= 1.0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override void complete(UA u)
        {
            T_ArcaneKnowledge arcaneKnowledge = (T_ArcaneKnowledge)u.person.traits.FirstOrDefault(t => t is T_ArcaneKnowledge);
            T_MasteryCurseweaving curseweaving = (T_MasteryCurseweaving)Caster.person.traits.FirstOrDefault(t => t is T_MasteryCurseweaving);
            if (arcaneKnowledge == null || arcaneKnowledge.level < GetArcaneSecretsRequired() || (curseweaving != null && curseweaving.level >= curseweaving.getMaxLevel()))
            {
                return;
            }

            arcaneKnowledge.level -= GetArcaneSecretsRequired();

            if (curseweaving == null)
            {
                curseweaving = new T_MasteryCurseweaving();
                u.person.receiveTrait(curseweaving);
            }
            curseweaving.level++;

            if (map.opt_allowMagicalArmsRace && u.isCommandable() && map.overmind.magicalArmsRace < curseweaving.level)
            {
                map.overmind.magicalArmsRace = curseweaving.level;
            }
        }
    }
}
