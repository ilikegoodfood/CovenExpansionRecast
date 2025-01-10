using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_DoomedProphetsCurse : Trait
    {
        public T_DoomedProphetsCurse()
        {

        }

        public override string getName()
        {
            return "Doomed Prophet's Curse";
        }

        public override string getDesc()
        {
            return "This persons efforts to warn the world are futile and will only spread shadow.";
        }

        public override void onAcquire(Person person)
        {
            person.awareness = 1.0;
        }

        public override void turnTick(Person p)
        {
            if (!p.items.Any(i  => i is I_DoomedProphetRing))
            {
                p.traits.Remove(this);
                return;
            }

            if (p.unit != null && (p.unit.isCommandable() || p.unit == p.map.awarenessManager.getChosenOne()))
            {
                p.traits.Remove(this);
                return;
            }
        }

        public override double getUtilityChanges(Challenge c, UA uA, List<ReasonMsg> reasons)
        {
            double val = 0.0;
            if (c is Ch_WarnTheWorld)
            {
                if (c.location.settlement is SettlementHuman humanSettlement && humanSettlement.ruler != null)
                {
                    val = 0.0;

                    // get and neagte the original base utility.
                    double originalBaseUtiltiy = c.getUtility(uA, null);

                    // caluclate a new base utility excluding unit shadow effect.
                    double baseUtility = (1.0 - humanSettlement.ruler.awareness) * (1.0 - humanSettlement.ruler.shadow) * c.map.awarenessManager.data_awarenessDelta * 1000.0;
                    val += baseUtility;

                    // Original penalty calculation
                    double change = 0.0;
                    double modifier = (humanSettlement.ruler.shadow - 1.0) * c.map.awarenessManager.data_awarenessDelta * 1000.0;
                    change -= modifier;
                    val += change;

                    // Scaling balancing lever.
                    val *= 1.0;

                    // Apply the original base utility as a negative, as we are completely recalculating it.
                    val -= originalBaseUtiltiy;

                    if (humanSettlement.ruler.shadow < 0.99)
                    {
                        val += 10.0;
                    }

                    // Report the change in utility from the original base utility to the new combined utility.
                    reasons?.Add(new ReasonMsg("Doomed Prophet", val - originalBaseUtiltiy));
                }
            }

            return val;
        }

        public override void completeChallenge(Challenge challenge)
        {
            if (!(challenge is Ch_WarnTheWorld))
            {
                return;
            }

            Location location = challenge.location;

            location.settlement.shadow += 0.25;
            if (location.settlement.shadow > 1.0)
            {
                location.settlement.shadow = 1.0;
            }

            Person ruler = location.person();
            if (ruler != null)
            {
                if (ruler.awareness > 1.0)
                {
                    ruler.awareness = 1.0;
                }

                ruler.awareness -= challenge.map.param.ch_warntheworld_parameterValue3;

                if (ruler.awareness < 0.0)
                {
                    ruler.awareness = 0.0;
                }

                foreach (Location neighbour in location.getNeighbours())
                {
                    if (location.settlement == null)
                    {
                        continue;
                    }

                    neighbour.settlement.shadow += 0.25;
                    if (neighbour.settlement.shadow > 1.0)
                    {
                        neighbour.settlement.shadow = 1.0;
                    }

                    ruler = neighbour.person();
                    if (ruler.awareness > 1.0)
                    {
                        ruler.awareness = 1.0;
                    }

                    ruler.awareness -= challenge.map.param.ch_warntheworld_parameterValue3;

                    if (ruler.awareness < 0.0)
                    {
                        ruler.awareness = 0.0;
                    }
                }
            }
        }
    }
}
