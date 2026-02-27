using Assets.Code;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_Curse_CollectMind : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Rti_Curse_CollectMind(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Curse of Collected Minds";
            }

            return $"Curseweaving: Curse of Collected Minds (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curses the House of the soul contained in this soulstone, granting you a god power that allows you to choose their next action. Cannot choose ruler actions from Vinnervas gifts or any action gained by a trait such as The Hunger.";
            }

            return $"Curses House {Soulstone.CapturedSoul.house.name}, granting you a god power that allows you to choose their next action. Cannot choose ruler actions from Vinnervas gifts or any action gained by a trait such as The Hunger.";
        }

        public override string getCastFlavour()
        {
            return "They awaken, hands coated in dirt and blood, their body weathered by years they do not remember passing. They bolt upright, perhaps to run, perhaps to scream. Before they can act a single whisper enters their mind, and then it is their mind no longer.";
        }

        public override string getRestriction()
        {
            return $"Requires Mastery of Curseweaving at least 3 and a soulstone containing a {SoulTypeUtils.GetTitle(SoulType.Physician)} or {SoulTypeUtils.GetTitle(SoulType.Mediator)} soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Mind.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && (Soulstone.SoulType == SoulType.Physician || Soulstone.SoulType == SoulType.Mediator) && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_CollectedMind);
        }

        public override bool validFor(UA ua)
        {
            return ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 2);
        }

        public override double getComplexity()
        {
            return 70.0;
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
            return 7;
        }

        public override int getCompletionMenace()
        {
            return 32;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_CollectedMind());
            Soulstone.CapturedSoul = null;

            if (!map.overmind.god.powers.Contains(CovensCore.Instance.OpenMindPower))
            {
                map.overmind.god.powers.Add(CovensCore.Instance.OpenMindPower);
                map.overmind.god.powerLevelReqs.Add(0);
            }
        }
    }
}
