using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_OpenMind_ChallengeSelector : SelectClickReceiver
    {
        public static void PopInstance(OpenMindSelectorData selectorData)
        {
            if (selectorData?.Map == null || selectorData.Ua == null)
            {
                return;
            }

            List<string> labels = new List<string>();
            if (selectorData.ActionTypeCount > 1)
            {
                labels.Add("BACK");
            }

            bool hasData = false;
            foreach (KeyValuePair<Tuple<Type, string>, CommunityLib.OrderedDictionary<string, Challenge>> outerKvp in selectorData.Challenges)
            {
                if (outerKvp.Value.Count == 1)
                {
                    labels.Add(outerKvp.Value.GetKeyAtIndex(0));
                    hasData = true;
                    continue;
                }

                labels.Add($"{outerKvp.Key.Item2} ({outerKvp.Value.Count})");
                hasData = true;
            }

            if (!hasData)
            {
                Console.WriteLine($"CovensExpansionRecast: OpenMindSelector did not have required data.");
                return;
            }

            Sel2_OpenMind_ChallengeSelector selector = new Sel2_OpenMind_ChallengeSelector();
            selector.SelectorData = selectorData;
            selectorData.Map.world.prefabStore.getScrollSetText(labels, false, selector, "Select Challenge", $"Select the challenge or challnge type that you wish {selectorData.Ua.getName()} to perform, or go back.");
        }

        public OpenMindSelectorData SelectorData;

        public void cancelled()
        {
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

            if (SelectorData.ActionTypeCount > 1)
            {
                if (index == 0)
                {
                    Sel2_OpenMind_TypeSelector.PopInstance(SelectorData);
                    return;
                }
                index--;
            }

            if (SelectorData.Challenges[index].Count > 1)
            {
                Sel2_OpenMind_ChallengeSelectorInner.PopInstance(SelectorData, index);
                return;
            }

            Challenge challenge = SelectorData.Challenges[index]?[0];
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
