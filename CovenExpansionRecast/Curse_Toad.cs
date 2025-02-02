using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Curse_Toad : Curse
    {
        public int Timer = 10;

        public int TurnLastChecked = 0;

        public override string getName()
        {
            return "Curse of Toad";
        }

        public override string getDesc()
        {
            return "This family has been transformed into toads.";
        }

        public override void turnTick(Person p)
        {
            if (p.map.turn > TurnLastChecked)
            {
                TurnLastChecked = p.map.turn;
                Timer--;

                if (Timer <= 0)
                {
                    p.house.curses.Remove(this);
                    return;
                }
            }

            if (Timer > 0)
            {
                if (p == p.map.awarenessManager.chosenOne?.person)
                {
                    return;
                }

                if (p.unit != null && (p.unit is UAG || (p.unit is UAA uaa && uaa.society.GetType() == typeof(HolyOrder))) && !p.unit.isDead && !p.unit.isCommandable())
                {
                    UAEN_Toad toad = new UAEN_Toad(p.unit.location, p.map.soc_dark, p, Timer);
                    p.unit.isDead = true;
                    p.unit.location.units.Remove(p.unit);
                    p.map.units.Remove(p.unit);

                    toad.SubsumedUnit = p.unit;
                    p.unit = toad;
                    p.map.units.Add(toad);
                    p.unit.location.units.Add(toad);

                    T_Toad toadTrait = (T_Toad)p.traits.FirstOrDefault(t => t is T_Toad);
                    if (toadTrait == null)
                    {
                        p.receiveTrait(new T_Toad(p));
                    }
                    else
                    {
                        toadTrait.Timer = -1;
                    }
                }

                if (p.rulerOf != -1)
                {
                    Location location = p.map.locations[p.rulerOf];
                    if (location.settlement is SettlementHuman humanSettlement)
                    {
                        if (!humanSettlement.customChallenges.Any(ch => ch is Ch_SquashToad squash && squash.Person == p))
                        {
                            humanSettlement.customChallenges.Add(new Ch_SquashToad(location, p));
                        }
                    }

                    if (!(p.unit is UAEN_Toad))
                    {
                        T_Toad toadTrait = (T_Toad)p.traits.FirstOrDefault(t => t is T_Toad);
                        if (toadTrait == null)
                        {
                            p.receiveTrait(new T_Toad(p, Timer));
                        }
                        else
                        {
                            toadTrait.Timer = Timer;
                        }
                    }
                }
            }
        }
    }
}
