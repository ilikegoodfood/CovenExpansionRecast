using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Toad : Trait
    {
        public int Timer;

        public Person Person;

        public T_Toad(Person person, int duration = -1)
        {
            Timer = duration;
            Person = person;
        }

        public override string getName()
        {
            return $"Toad Form ({Timer} turns)";
        }

        public override string getDesc()
        {
            if (!(Person.unit is UAEN_Toad toad) || toad.SubsumedUnit == null)
            {
                return $"This is a Toad. It is absolutely ordinary in every way. It's barely even worthy of notice.";
            }

            return $"This is a Toad. It is absolutely ordinary in every way. It's barely even worthy of notice. Now, where did {toad.SubsumedUnit.getName()} go..? They were supposed to be right here!";
        }

        public override int getMaxLevel()
        {
            return 1;
        }

        public override int getAttackChange()
        {
            return -3;
        }

        public override int getMightChange()
        {
            return -3;
        }

        public override int getCommandChange()
        {
            return -3;
        }

        public override int getLoreChange()
        {
            return -3;
        }

        public override int getIntrigueChange()
        {
            return -3;
        }

        public override void onAcquire(Person person)
        {
            person.customGraphics = "CovenExpansionRecast.Icon_FrogCurse.png";

            if (person.unit is UA ua)
            {
                for (int i = 0; i < ua.minions.Length; i++)
                {
                    if (ua.minions[i] != null)
                    {
                        ua.minions[i].disband("Toads can't have minions");
                        ua.minions[i] = null;
                    }
                }
            }

            if (person.rulerOf != -1)
            {
                Location location = person.map.locations[person.rulerOf];
                if (location.settlement is SettlementHuman humanSettlement)
                {
                    humanSettlement.actionUnderway = new Act_Gridlock(location);
                }
            }
        }

        public override void turnTick(Person p)
        {
            for (int i = 0; i < p.traits.Count; i++)
            {
                if (p.traits[i] == this)
                {
                    break;
                }

                if (p.traits[i] is T_Toad)
                {
                    p.traits.RemoveAt(i);
                    break;
                }
            }

            if (Timer == -1)
            {
                if (!(p.unit is UAEN_Toad))
                {
                    p.traits.Remove(this);
                    p.customGraphics = null;
                }
            }
            else if (Timer > 0)
            {
                Timer--;
                if (Timer <= 0)
                {
                    p.traits.Remove(this);
                    p.customGraphics = null;
                }
            }
        }
    }
}
