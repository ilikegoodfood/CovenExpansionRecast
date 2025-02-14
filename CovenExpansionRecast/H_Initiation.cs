using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class H_Initiation : HolyTenet
    {
        public int LastTurnChecked = 0;

        public H_Initiation(HolyOrder us)
            : base(us)
        {

        }

        public override string getName()
        {
            return "Initiation Rites";
        }

        public override string getDesc()
        {
            return "Increases the maximum number of Acolytes this holy order can have by 1 per point in this tenet and adds 4 gold per turn towards the recruitment of new acolytes.";
        }

        public override int getMaxPositiveInfluence()
        {
            return 6;
        }

        public override int getMaxNegativeInfluence()
        {
            return 0;
        }

        public override bool structuralTenet()
        {
            return true;
        }

        public override void turnTick(UAA ua)
        {
            int turn = ua.map.turn;
            if (LastTurnChecked >= turn)
            {
                return;
            }
            LastTurnChecked = turn;

            if (order.cashForAcolytes < order.costAcolyte)
            {
                order.cashForAcolytes = Math.Min(order.cashForAcolytes + 4, order.cashForAcolytes);
            }

            if (order.nAcolytes >= order.map.param.holy_maxAcolytes && order.nAcolytes - status < order.map.param.holy_maxAcolytes)
            {
                if (order.cashForAcolytes >= order.costAcolyte)
                {
                    order.cashForAcolytes -= order.costAcolyte;
                    order.createAcolyte();
                }
            }
        }
    }
}
