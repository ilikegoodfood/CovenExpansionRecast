using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_SoulFinder : SelectClickReceiver
    {
        public List<Pr_FallenHuman> TargetList;

        public void cancelled()
        {
            return;
        }

        public void selectableClicked(string text, int index)
        {
            Pr_FallenHuman soul = TargetList[index];
            GraphicalMap.selectedUnit = null;
            GraphicalMap.selectedHex = soul.location.hex;
            GraphicalMap.panTo(soul.location.hex);
            GraphicalMap.checkData();
        }
    }
}
