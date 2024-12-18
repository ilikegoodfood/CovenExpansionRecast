using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_CollectedMind : Trait
    {
        public override string getName()
        {
            return "Collected Mind";
        }

        public override string getDesc()
        {
            return "The mind of this person has been cracked open and collected. With but a whisper, they will dance to the elder gods wishes.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                p.traits.Remove(this);
            }
        }
    }
}
