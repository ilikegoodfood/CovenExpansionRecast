using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_ToadCurse : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_ToadCurse(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Curse of Frog";
            }

            return $"Curseweaving: Curse of Frog (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curses the House of the soul contained in this soulstone, turning all their members into frogs for 10 turns. During this time they will be extremely vulnerable to attack.";
            }

            return $"Curses House {Soulstone.CapturedSoul.house.name}, turning all their members into frogs for 10 turns. During this time they will be extremely vulnerable to attack.";
        }

        public override string getCastFlavour()
        {
            return "At first it is hilarious, but the laughter will soon be cut short when the first noble is found crushed to a bloody pulp.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 2 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_FrogCurse.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_Toad);
        }

        public override bool validFor(UA ua)
        {
            return Soulstone.CapturedSoul.house != ua.person.house && ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 1);
        }

        public override double getComplexity()
        {
            return 40.0;
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
            return 3;
        }

        public override int getCompletionMenace()
        {
            return 20;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_Toad());
            Soulstone.CapturedSoul = null;
        }
    }
}
