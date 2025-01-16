using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Wanderlust : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Wanderlust(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            return "Curseweaving: Curse of Wanderlust";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return "Curses the House of the soul contained in this soulstone with an insatiable Wonderlust. Their heroes will seek out progressively stranger quests never, satisfied with doing the same thing twice.";
            }

            return $"Curses House {Soulstone.CapturedSoul.house} with an insatiable Wonderlust. Their heroes will seek out progressively stranger quests never, satisfied with doing the same thing twice.";
        }

        public override string getCastFlavour()
        {
            return "They seldom return, but each time they do they breach the doors of their homes, stinking of strange concoctions and pockmarked with scars and tattoos, bringing with them ever more incomprhensible tales of their journeys. Their family listens to these stories with growing mortification, secretly praying that one day they will never return.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 1 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Wanderer.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_Wanderer);
        }

        public override bool validFor(UA ua)
        {
            return ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 0);
        }

        public override double getComplexity()
        {
            return 30.0;
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

        public override int getCompletionProfile()
        {
            return 2;
        }

        public override int getCompletionMenace()
        {
            return 10;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_Wanderer());
            Soulstone.CapturedSoul = null;
        }
    }
}
