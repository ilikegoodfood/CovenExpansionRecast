using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Generosity : Trait
    {
        public HolyOrder Order;

        public int GoldCost = 5;

        public T_Generosity(HolyOrder order)
        {
            Order = order;
        }

        public override string getName()
        {
            return "Curse of Generosity";
        }

        public override string getDesc()
        {
            if (Order == null || Order.isGone())
            {
                return "This person's curse has been lifted as the holy order they were compelled to donate to to has been scoured from the world.";
            }

            return $"This person is compelled to donate {GoldCost} gold each turn to {Order.getName()}. If they do not have enough money on them it will be taken from their holy order if possible.";
        }

        public override int getMaxLevel()
        {
            return 1;
        }

        public override void turnTick(Person p)
        {
            if ( p == p.map.awarenessManager.chosenOne?.person || p.unit == null || p.unit.isCommandable() || !(p.unit is UAA uaa) || uaa.order == Order || p.unit.society is HolyOrder_Ophanim)
            {
                p.traits.Remove(this);
                return;
            }

            if (CovensCore.Instance.TryGetModIntegrationData("OrcsPlus", out ModIntegrationData intDataOP) && intDataOP.TypeDict.TryGetValue("OrcCulture", out Type orcCultureType))
            {
                if (p.unit.society.GetType() == orcCultureType || p.unit.society.GetType().IsSubclassOf(orcCultureType))
                {
                    p.traits.Remove(this);
                    return;
                }
            }

            if (p.gold >= GoldCost)
            {
                p.gold -= GoldCost;
                Order.reserves += GoldCost;
                return;
            }

            int gold = p.gold;
            p.gold = GoldCost - p.gold;

            int remainder = GoldCost - gold;

            if (uaa.order.reserves > 0)
            {
                if (uaa.order.reserves < remainder)
                {
                    gold += uaa.order.reserves;
                    remainder = GoldCost - gold;
                }
                else
                {
                    uaa.order.reserves -= remainder;
                    Order.reserves += GoldCost;
                    return;
                }
            }
            if (remainder > 0 && uaa.order.cashForPreaching > 0)
            {
                if (uaa.order.cashForPreaching < remainder)
                {
                    gold += uaa.order.cashForPreaching;
                    remainder = GoldCost - gold;
                }
                else
                {
                    uaa.order.cashForPreaching -= remainder;
                    Order.reserves += GoldCost;
                    return;
                }
            }

            if (remainder > 0 && uaa.order.cashForTemples > 0)
            {
                if (uaa.order.cashForTemples < remainder)
                {
                    gold += uaa.order.cashForTemples;
                    uaa.order.cashForPreaching = 0;
                    remainder = GoldCost - gold;
                }
                else
                {
                    uaa.order.cashForTemples -= remainder;
                    Order.reserves += GoldCost;
                    return;
                }
            }

            if (remainder > 0 && uaa.order.cashForAcolytes > 0)
            {
                if (uaa.order.cashForAcolytes < remainder)
                {
                    gold += uaa.order.cashForAcolytes;
                    remainder = GoldCost - gold;
                }
                else
                {
                    uaa.order.cashForPreaching -= remainder;
                    Order.reserves += GoldCost;
                    return;
                }
            }

            Order.reserves += gold;
        }

        public override double getUtilityChanges(Challenge c, UA uA, List<ReasonMsg> reasons)
        {
            if (c is Ch_FundHolyOrder fundOrder && fundOrder.order == Order)
            {
                reasons?.Add(new ReasonMsg("Compelled by the Curse of Generosity", 20.0));
                return 20.0;
            }

            return 0.0;
        }

        public override int[] getTags()
        {
            return new int[]
            {
                Tags.RELIGION,
                Tags.GOLD
            };
        }
    }
}
