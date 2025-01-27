using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_RiteOfMasks : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Rti_RiteOfMasks(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Rite of Masks";
            }

            return $"Curseweaving: Rite of Masks (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Changes the casters house and society to the house and society of a soul trapped within a soulstone. While in a location with a ruler of the new house that location loses 1 security.";
            }

            return $"Changes the casters house and society to mathc that of {Soulstone.CapturedSoul.getFullName()}. While in a location with a ruler of house {Soulstone.CapturedSoul.house.name}, the location loses 1 security.";
        }

        public override string getCastFlavour()
        {
            return "Their history and memories are unwoven and remade into an elaborate tapestry of lies. They welcome their false family with open arms, as if they'd known them all their lives.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 1 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_Mask.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark;
        }

        public override bool validFor(UA ua)
        {
            return Soulstone.CapturedSoul.house != ua.person.house && ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level  > 0);
        }

        public override double getComplexity()
        {
            return 16.0;
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

        public override int getCompletionMenace()
        {
            return 2;
        }

        public override void complete(UA u)
        {
            T_FictitiousBonds bonds = (T_FictitiousBonds)u.person.traits.FirstOrDefault(t => t is T_FictitiousBonds);
            if (bonds != null)
            {
                bonds.House = Soulstone.CapturedSoul.house;
            }
            else
            {
                bonds = new T_FictitiousBonds(Soulstone.CapturedSoul.house);
                u.person.receiveTrait(bonds);
            }

            u.person.society = Soulstone.CapturedSoul.society;
            u.person.house = Soulstone.CapturedSoul.house;
            u.person?.embedIntoSociety();
            Soulstone.CapturedSoul = null;
        }

        public override int[] getPositiveTags()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null)
            {
                return new int[0];
            }

            return new int[]
            {
                Soulstone.CapturedSoul.house.index + 30000
            };
        }
    }
}
