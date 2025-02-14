using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_OutcastShelters : HolyTenet
    {
        public H_OutcastShelters(HolyOrder us)
            : base(us)
        {

        }

        public override string getName()
        {
            return "Outcast Shelters";
        }

        public override string getDesc()
        {
            return "All nonhuman characters and non-commandable agents staying in locations following this holy order lose profile and menace each turn.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 0;
        }

        public override int getMaxNegativeInfluence()
        {
            return -2;
        }

        public override bool structuralTenet()
        {
            return false;
        }

        public override void turnTickSettlement(SettlementHuman settlementHuman)
        {
            foreach (Unit unit in settlementHuman.location.units)
            {
                if (unit.person == null || unit.isCommandable())
                {
                    continue;
                }

                Species species = unit.person.species;
                if (species != settlementHuman.map.species_human && species != settlementHuman.map.species_elf)
                {
                    unit.addProfile(0.8 * status);
                    unit.addMenace(0.3 * status);
                }
                else if (unit is UAEN)
                {
                    unit.addProfile(0.8 * status);
                    unit.addMenace(0.3 * status);
                }
            }
        }
    }
}
