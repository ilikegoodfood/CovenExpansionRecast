using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Sel2_MagicPlague_PropertySelector : SelectClickReceiver
    {
        public UA Caster;

        public I_Soulstone Soulstone;

        public List<Property> TargetList;

        public void cancelled()
        {
            Caster.inner_profile -= 14;
            Caster.inner_menace -= 30;
        }

        public void selectableClicked(string text, int index)
        {
            Soulstone.CapturedSoul = null;
            Property property = TargetList[index];
            Pr_MagicPlague magicPlague = new Pr_MagicPlague(Caster.location, property);
            magicPlague.charge = 50.0 + (property.charge / 3.0);
            Caster.location.properties.Add(magicPlague);
        }
    }
}
