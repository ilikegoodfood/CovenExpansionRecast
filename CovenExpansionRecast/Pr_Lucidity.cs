using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Pr_Lucidity : Property
    {
        public Pr_Lucidity(Location loc)
            : base(loc)
        {
            stackStyle = stackStyleEnum.ADD_CHARGE;
        }
    }
}
