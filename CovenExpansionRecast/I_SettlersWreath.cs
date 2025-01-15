using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_SettlersWreath : Item
    {
        public Society Society;

        public I_SettlersWreath(Map map)
            : base(map)
        {
            challenges.Add(new Rti_OrganizeSettlers(map.locations[0], this));
            challenges.Add(new Rti_MassSettlement(map.locations[0], this));
        }

        public override string getName()
        {
            if (Society != null)
            {
                return $"Wreath of Manifest {Society.getName()})";
            }

            return "Wreath of Manifest";
        }

        public override string getShortDesc()
        {
            if (Society == null)
            {
                return $"A kingdom contained in a laurel wreath. Currently can be used to settle a large territory all at once for the kingdom of {Society.getName()}.";
            }

            return "A kingdom contained in a laurel wreath. After discussing settlement plans at a capital can be used to settle a large territory all at once.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_SettlersWreath.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            return challenges;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }
    }
}
