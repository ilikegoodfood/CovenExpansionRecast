using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_PigeonTargetSelector : SelectClickReceiver
    {
        public Map Map;

        public UA Caster;

        public List<UA> Targets;

        public Sel2_PigeonTargetSelector(Map map, UA caster, List<UA> targets)
        {
            Map = map;
            Caster = caster;
            Targets = targets;
        }

        public void cancelled()
        {

        }

        public void selectableClicked(string text, int index)
        {
            M_Pigeon pigeonMinion = null;
            int pigeonIndex = -1;
            for (int i = 0; i < Caster.minions.Length; i++)
            {
                if (Caster.minions[i] is M_Pigeon pigeon)
                {
                    pigeonMinion = pigeon;
                    pigeonIndex = i;
                    break;
                }
            }

            if (pigeonIndex == -1)
            {
                Console.WriteLine("CovenExpansionRecast: ERROR: Carrier Pigeon Not found.");
                return;
            }
            Caster.minions[pigeonIndex].disband("Carrier Pigeon sent on courier mission");
            Caster.minions[pigeonIndex] = null;

            UAEN_Pigeon pigeonUnit = new UAEN_Pigeon(Caster.location, Map.soc_neutral, Caster, Targets[index], pigeonMinion);
            pigeonUnit.location.units.Add(pigeonUnit);
            Map.units.Add(pigeonUnit);

            Map.world.prefabStore.popItemTrade(pigeonUnit.person, Caster.person, "Swap Items", -1, -1);
        }
    }
}
