using Assets.Code;
using System.Linq;

namespace CovenExpansionRecast
{
    public class Curse_CollectedMind : Curse
    {
        public override string getName()
        {
            return "Collected Mind";
        }

        public override string getDesc()
        {
            return "The minds of this family have been cracked open and collected for later use. With but a whisper; they will dance to the elder gods wishes.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                return;
            }

            T_CollectedMind collected = (T_CollectedMind)p.traits.FirstOrDefault(t => t is T_CollectedMind);
            if (collected == null)
            {
                collected = new T_CollectedMind();
                p.receiveTrait(collected);
            }

            if (!p.map.overmind.god.powers.Contains(CovensCore.Instance.OpenMindPower))
            {
                p.map.overmind.god.powers.Add(CovensCore.Instance.OpenMindPower);
                p.map.overmind.god.powerLevelReqs.Add(0);
            }
        }
    }
}
