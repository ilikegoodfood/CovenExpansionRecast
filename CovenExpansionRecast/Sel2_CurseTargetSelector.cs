using Assets.Code;
using System.Collections.Generic;

namespace CovenExpansionRecast
{
    public class Sel2_CurseTargetSelector : SelectClickReceiver
    {
        public List<UAA> Targets;

        public Ch_H_CurseIntrudingAcolyte Challenge;

        public Sel2_CurseTargetSelector(Map map, Ch_H_CurseIntrudingAcolyte challenge, List<UAA> opts)
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
