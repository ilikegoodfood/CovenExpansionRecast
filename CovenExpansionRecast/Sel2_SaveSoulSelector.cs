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
        List<I_Soulstone> Soulstones;

        public Sel2_SaveSoulSelector(List<I_Soulstone> soulstones)
        {
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
