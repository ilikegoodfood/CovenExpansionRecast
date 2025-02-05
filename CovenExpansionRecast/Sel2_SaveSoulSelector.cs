using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_SaveSoulSelector : SelectClickReceiver
    {
        Map Map;

        List<I_Soulstone> Soulstones;

        public Sel2_SaveSoulSelector(Map map, List<I_Soulstone> soulstones)
        {
            Map = map;
            Soulstones = soulstones;
        }

        public void cancelled()
        {
            int index = Eleven.random.Next(Soulstones.Count);
            for (int i = 0; i < Soulstones.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }

                Soulstones[i].CapturedSoul = null;
            }
        }

        public void selectableClicked(string text, int index)
        {
            for (int i = 0; i < Soulstones.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }

                Soulstones[i].CapturedSoul = null;
            }
        }
    }
}
