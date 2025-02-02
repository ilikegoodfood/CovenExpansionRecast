using Assets.Code;
using System.Collections.Generic;
using System;

namespace CovenExpansionRecast
{
    public class Sel2_OpenMind_ChallengeSelectorInner : SelectClickReceiver
    {
        public static void PopInstance(OpenMindSelectorData selectorData, int index)
        {
            if (selectorData?.Map == null || selectorData.Ua == null)
            {
                return;
            }

            List<string> labels = new List<string>();
            if (selectorData.ActionTypeCount > 1 || selectorData.Challenges.Count > 1)
            {
                labels.Add("BACK");
            }

            bool hasData = false;
            foreach (KeyValuePair<string, Challenge> Kvp in selectorData.Challenges[index])
            {
                labels.Add(Kvp.Key);
                hasData = true;
            }

            if (!hasData)
            {
                Console.WriteLine($"CovensExpansionRecast: OpenMindSelector did not have required data.");
                return;
            }

            Sel2_OpenMind_ChallengeSelectorInner selector = new Sel2_OpenMind_ChallengeSelectorInner();
            selector.SelectorData = selectorData;
            selector.Index = index;
            selectorData.Map.world.prefabStore.getScrollSetText(labels, false, selector, "Select Challenge", $"Select the specific challenge that you wish {selectorData.Ua.getName()} to perform, or go back.");
        }

        public OpenMindSelectorData SelectorData;

        public int Index;

        public void cancelled()
        {
            if (SelectorData.Challenges.Count > 1)
            {
                Sel2_OpenMind_ChallengeSelector.PopInstance(SelectorData);
                return;
            }
            if (SelectorData.ActionTypeCount > 1)
            {
                Sel2_OpenMind_TypeSelector.PopInstance(SelectorData);
                return;
            }

            CovensCore.Instance.OpenMindPower.RefundCast(SelectorData.Person);
        }

        public void selectableClicked(string text, int index)
        {
            if (SelectorData.Ua == null)
            {
                cancelled();
                return;
            }

            if (SelectorData.ActionTypeCount > 1 || SelectorData.Challenges.Count > 1)
            {
                if (index == 0)
                {
                    if (SelectorData.Challenges.Count > 1)
                    {
                        Sel2_OpenMind_ChallengeSelector.PopInstance(SelectorData);
                        return;
                    }

                    Sel2_OpenMind_TypeSelector.PopInstance(SelectorData);
                }
                index--;
            }

            Challenge challenge = SelectorData.Challenges[Index]?[index];
            if (challenge == null)
            {
                cancelled();
                return;
            }

            if (SelectorData.Ua.location == challenge.location)
            {
                SelectorData.Ua.task = new Task_PerformChallenge(challenge);
            }
            else
            {
                SelectorData.Ua.task = new Task_GoToPerformChallenge(challenge);
            }
        }
    }
}