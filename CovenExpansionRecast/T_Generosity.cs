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

        public T_Generosity(HolyOrder order)
        {
            Order = order;
        }
    }
}
