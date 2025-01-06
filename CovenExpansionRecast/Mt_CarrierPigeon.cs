using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Mt_CarrierPigeon : MinionTrait
    {
        public override string getName()
        {
            return "Carrier Pigeon";
        }

        public override string getDesc()
        {
            return "Can be sent off to deliver or retrieve items and gold from other agents.";
        }
    }
}
