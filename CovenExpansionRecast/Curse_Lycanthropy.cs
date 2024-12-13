using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Lycanthropy : Curse
    {
        public override string getName()
        {
            return "Lycanthropy";
        }

        public override string getDesc()
        {
            return "This family is cursed to transform into ruthless predators under a full moon.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person)
            {
                return;
            }

            if (CovensCore.Instance.TryGetModIntegrationData("LivingWilds", out ModIntegrationData intDataLW))
            {
                if (intDataLW.MethodInfoDict.TryGetValue("T_Lycanthropy.IsWerewolf", out MethodInfo MI_IsWerewolf) && MI_IsWerewolf != null)
                {
                    if (intDataLW.MethodInfoDict.TryGetValue("T_Lycanthropy.InfectPerson", out MethodInfo MI_InfectPerson) && MI_InfectPerson != null)
                    {
                        if (!(bool)MI_IsWerewolf.Invoke(null, new object[] { p }))
                        {
                            object[] args = new object[] { p, true, false };

                            MI_InfectPerson.Invoke(null, args);
                        }
                    }
                }
            }
        }
    }
}
