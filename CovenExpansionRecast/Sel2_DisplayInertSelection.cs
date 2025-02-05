using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_DisplayInertSelection : SelectClickReceiver
    {
        public World World;

        public Sel2_DisplayInertSelection(Map map)
        {
            World = map.world;
        }

        public void cancelled()
        {
            
        }

        public void selectableClicked(string text, int index)
        {
            
        }
    }
}
