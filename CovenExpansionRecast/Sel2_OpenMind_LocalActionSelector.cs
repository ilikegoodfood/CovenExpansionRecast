using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_OpenMind_LocalActionSelector : SelectClickReceiver
    {
        public static void PopInstance(OpenMindSelectorData selectorData)
        {
            if (selectorData.Map == null || selectorData.HumanSettlement == null)
            {
                return;
            }

            List<string> labels = new List<string>();
            if (selectorData.ActionTypeCount > 1)
            {
                labels.Add("BACK");
            }

            bool hasData = false;
            foreach (Assets.Code.Action action in selectorData.LocalActions)
            {
                labels.Add(action.getName());
                hasData = true;
            }

            if (!hasData)
            {
                Console.WriteLine($"CovensExpansionRecast: OpenMindSelector did not have required data.");
                return;
            }

            Sel2_OpenMind_LocalActionSelector selector = new Sel2_OpenMind_LocalActionSelector();
            selector.SelectorData = selectorData;
            selectorData.Map.world.prefabStore.getScrollSetText(labels, false, selector, "Select Local Action", $"Select the local ruler action that you wish {selectorData.Ua.getName()} to perform at {selectorData.Map.locations[selectorData.Person.rulerOf].getName()}, or go back.");
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
            if (SelectorData.Map == null || SelectorData.HumanSettlement == null)
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

            Assets.Code.Action action = SelectorData.LocalActions[index];
            if (action == null)
            {
                cancelled();
                return;
            }

            SelectorData.HumanSettlement.actionUnderway = action;
            SelectorData.HumanSettlement.actionProgress = 0;
        }
    }
}
