using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_Curse_Lycanthropy : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Rti_Curse_Lycanthropy(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Curse of Lycanthropy";
            }

            return $"Curseweaving: Curse of Lycanthropy (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curses the House of the soul contained in this soulstone, causing them to transform into Werewolves that will hunt down other heroes and ravage populations.";
            }

            return $"Curses House {Soulstone.CapturedSoul.house.name}, causing them to transform into Werewolves that will hunt down other heroes and ravage populations.";
        }

        public override string getCastFlavour()
        {
            return "They watch the cycles of the moon nervously, unsure of what new horrors the full moon will bring. As it approaches, they scratch at their skin as if it were nt their own, and their bodyhair grows longer and thicker.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 2 and a soulstone containing a werewolf soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Werewolf.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.SoulType == SoulType.Werewolf && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_Lycanthropy);
        }

        public override bool validFor(UA ua)
        {
            return ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 1);
        }

        public override double getComplexity()
        {
            return 45.0;
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
            return 30;
        }

        public override int getCompletionMenace()
        {
            return 20;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_Lycanthropy());
            Soulstone.CapturedSoul = null;
        }
    }
}
