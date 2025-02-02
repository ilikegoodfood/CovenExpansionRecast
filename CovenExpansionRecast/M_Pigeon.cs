using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class M_Pigeon : Minion
    {
        public Rtm_SendCarrierPigeon SendRitual;

        public M_Pigeon(Map map)
            : base(map)
        {
            traits.Add(new Mt_CarrierPigeon());
            SendRitual = new Rtm_SendCarrierPigeon(map.locations[0]);
        }

        public override string getName()
        {
            return "Carrier Pigeon";
        }

        public override Sprite getIcon()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Pigeon.png");
        }

        public override int getCommandCost()
        {
            return 1;
        }

        public override int getMaxHP()
        {
            return 1;
        }
        public override int getMaxDefence()
        {
            return 0;
        }

        public override int getAttack()
        {
            return 1;
        }

        public override void turnTick(UA ua)
        {
            base.turnTick(ua);

            if (!ua.rituals.Any(rt => rt is Rtm_SendCarrierPigeon))
            {
                ua.rituals.Add(SendRitual);
            }
        }

        public override int getGoldCost()
        {
            return 15;
        }

        public override Minion getClone()
        {
            return new M_Pigeon(map);
        }
    }
}
