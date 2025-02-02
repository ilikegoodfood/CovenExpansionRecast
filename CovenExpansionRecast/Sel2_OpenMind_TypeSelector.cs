using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_OpenMind_TypeSelector : SelectClickReceiver
    {
        public static void PopInstance(OpenMindSelectorData selectorData)
        {
            if (selectorData == null || selectorData.ActionTypeCount == 0)
            {
                return;
            }

            if (selectorData.ActionTypeCount == 1)
            {
                if (selectorData.NationalActions.Count > 0)
                {
                    Sel2_OpenMind_NationalActionSelector.PopInstance(selectorData);
                    return;
                }

                if (selectorData.LocalActions.Count > 0)
                {
                    Sel2_OpenMind_LocalActionSelector.PopInstance(selectorData);
                    return;
                }

                if (selectorData.Challenges.Count > 0)
                {
                    Sel2_OpenMind_ChallengeSelector.PopInstance(selectorData);
                    return;
                }
            }

            List<string> labels = new List<string>();
            if (selectorData.NationalActions.Count > 0)
            {
                labels.Add("National Actions");
            }

            if (selectorData.LocalActions.Count > 0)
            {
                labels.Add("Local Actions");
            }

            if (selectorData.Challenges.Count > 0)
            {
                labels.Add("Challenges");
            }
        }

        public OpenMindSelectorData SelectorData;

        public void cancelled()
        {
            CovensCore.Instance.OpenMindPower.RefundCast(SelectorData.Person);
        }

        public void selectableClicked(string text, int index)
        {
            switch(text)
            {
                case "National Actions":
                    Sel2_OpenMind_NationalActionSelector.PopInstance(SelectorData);
                    return;
                case "Local Actions":
                    Sel2_OpenMind_LocalActionSelector.PopInstance(SelectorData);
                    return;
                case "Challenges":
                    Sel2_OpenMind_ChallengeSelector.PopInstance(SelectorData);
                    return;
            }

            CovensCore.Instance.OpenMindPower.RefundCast(SelectorData.Person, "No valid actions");
        }
    }
}
