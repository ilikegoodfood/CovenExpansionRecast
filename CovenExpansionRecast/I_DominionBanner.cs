using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_DominionBanner : Item
    {
        public I_DominionBanner(Map map)
            : base(map)
        {
            challenges.Add(new Rti_CallHordes(map.locations[0]));
        }

        public override string getName()
        {
            return "Banner of Barberous Dominion";
        }

        public override string getShortDesc()
        {
            return "A portent of a brutal future. The Orcish hordes follow its bearer without question. Grants + 3 <b>Command</b> as well as access to a ritual that draws all orc hordes not in combat to the bearers location.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_DominionBanner.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            return challenges;
        }

        public override int getCommandBonus()
        {
            return 3;
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
