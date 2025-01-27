using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_Curse_Flourishing : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Rti_Curse_Flourishing(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Curse of Flourishing";
            }

            return $"Curseweaving: Curse of Flourishing (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curses the House of the soul contained in this soulstone, causing life to spring up wherever they step. Rapidly increases the population of their locations, eventually leading to overpopulation.";
            }

            return $"Curses House {Soulstone.CapturedSoul.house.name}, causing life to spring up wherever they step. Rapidly increases the population of their locations, eventually leading to overpopulation.";
        }

        public override string getCastFlavour()
        {
            return "They celebrate each new life, praising the sudden prosperity of their grand house. Behind forced smiles lies a growing dread, as tables fill to bursting and fresh mouths squawk out for their daily rations.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 2 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Flourish.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_Flourish);
        }

        public override bool validFor(UA ua)
        {
            return Soulstone.CapturedSoul.house != ua.person.house && ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 1);
        }

        public override double getComplexity()
        {
            return 35.0;
        }

        public override Challenge.challengeStat getChallengeType()
        {
            return Challenge.challengeStat.LORE;
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

        public override int getCompletionProfile()
        {
            return 0;
        }

        public override int getCompletionMenace()
        {
            return 20;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_Flourish());
            Soulstone.CapturedSoul = null;
        }
    }
}
