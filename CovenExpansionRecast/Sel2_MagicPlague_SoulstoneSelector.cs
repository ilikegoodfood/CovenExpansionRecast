using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_MagicPlague_SoulstoneSelector : SelectClickReceiver
    {
        public Map map;

        public UA Caster;

        public List<I_Soulstone> TargetList;

        public void cancelled()
        {
            Caster.inner_profile -= 14;
            Caster.inner_menace -= 30;
        }

        public void selectableClicked(string text, int index)
        {
            Mg_MagicPlague.PopPlaguePropertySelect(Caster, TargetList[index]);
        }
    }
}
