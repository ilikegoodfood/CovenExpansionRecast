using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_CurseTargetChooser : SelectClickReceiver
    {
        public List<UAA> Targets;

        public Ch_H_CurseIntrudingAcolyte Challenge;

        public Sel2_CurseTargetChooser(Map map, Ch_H_CurseIntrudingAcolyte challenge, List<UAA> opts)
        {
            Challenge = challenge;
            Targets = opts;
        }

        public void cancelled()
        {
            Challenge.Target = Targets[0];
        }

        public void selectableClicked(string text, int index)
        {
            Challenge.Target = Targets[index];
        }
    }
}
