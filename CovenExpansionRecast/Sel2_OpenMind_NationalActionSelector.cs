using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_OpenMind_NationalActionSelector : SelectClickReceiver
    {
        public static void PopInstance(OpenMindSelectorData selectorData)
        {
            if (selectorData.Map == null || selectorData.Society == null)
            {
                return;
            }

            List<string> labels = new List<string>();
            if (selectorData.ActionTypeCount > 1)
            {
                labels.Add("BACK");
            }

            bool hasData = false;
            foreach (AN action in selectorData.NationalActions)
            {
                labels.Add(action.getName());
                hasData = true;
            }

            if (!hasData)
            {
                Console.WriteLine($"CovensExpansionRecast: OpenMindSelector did not have required data.");
                return;
            }

            Sel2_OpenMind_NationalActionSelector selector = new Sel2_OpenMind_NationalActionSelector();
            selector.SelectorData = selectorData;
            selectorData.Map.world.prefabStore.getScrollSetText(labels, false, selector, "Select National Action", $"Select the national action that you wish {selectorData.Ua.getName()} to perform as leader of the {selectorData.Society.getName()}, or go back.");
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
            if (SelectorData.Map == null || SelectorData.Society == null)
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

            AN action = SelectorData.NationalActions[index];
            if (action == null)
            {
                cancelled();
                return;
            }

            SelectorData.Society.actionUnderway = action;
            SelectorData.Society.actionProgress = 0;
        }
    }
}
