using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_Aviaries : HolyTenet
    {
        public H_Aviaries(HolyOrder us)
            : base(us)
        {

        }

        public override string getName()
        {
            return "Aviaries";
        }

        public override string getDesc()
        {
            return $"Allows additional types of bird minions to be purchased at covens. Acolytes of this religion will fill an empty minion slot with a Raven every {World.staticMap.param.trait_commandOfVerminPeriod} turns.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 0;
        }

        public override int getMaxNegativeInfluence()
        {
            return -1;
        }

        public override void turnTickTemple(Sub_Temple temple)
        {
            if (status == 0)
            {
                return;
            }
            bool pigeon = false;
            bool owl = false;

            foreach (Challenge challenge in temple.settlement.customChallenges)
            {
                if (!(challenge is Ch_RecruitMinion recruit))
                {
                    continue;
                }

                if (recruit.exemplar is M_Pigeon)
                {
                    pigeon = true;

                    if (owl)
                    {
                        break;
                    }
                }
                else if (recruit.exemplar is M_Owl)
                {
                    owl = true;

                    if (pigeon)
                    {
                        break;
                    }
                }
            }

            if (!pigeon)
            {
                temple.settlement.customChallenges.Add(new Ch_RecruitMinion(temple.settlement.location, new M_Pigeon(temple.settlement.map), -1, temple));
            }

            if (!owl)
            {
                temple.settlement.customChallenges.Add(new Ch_RecruitMinion(temple.settlement.location, new M_Owl(temple.settlement.map), -1, temple));
            }
        }

        public override void turnTick(UAA ua)
        {
            T_MurderOfCrows ravenTrait = (T_MurderOfCrows)ua.person.traits.FirstOrDefault(t => t is T_MurderOfCrows);
            if (status < 0)
            {
                if (ravenTrait == null)
                {
                    ravenTrait = new T_MurderOfCrows();
                    ua.person.receiveTrait(ravenTrait);
                }
            }
            else
            {
                if (ravenTrait != null)
                {
                    ua.person.traits.Remove(ravenTrait);
                }
            }
        }
    }
}
