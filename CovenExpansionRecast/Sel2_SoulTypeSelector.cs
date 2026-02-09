using Assets.Code;
using System.Collections.Generic;

namespace CovenExpansionRecast
{
    public class Sel2_SoulTypeSelector : SelectClickReceiver
    {
        private List<SoulType> _soulTypes;

        private I_Soulstone _soulstone;

        public Sel2_SoulTypeSelector(I_Soulstone soulstone, List<SoulType> soulTypes)
        {
            _soulstone = soulstone;
            _soulTypes = soulTypes;
        }

        public void selectableClicked(string text, int index)
        {
            _soulstone.SoulType = _soulTypes[index];
        }

        public void cancelled()
        {
            return;
        }
    }
}
