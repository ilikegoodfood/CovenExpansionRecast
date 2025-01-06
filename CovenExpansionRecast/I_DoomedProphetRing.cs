using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_DoomedProphetRing : Item
    {
        public I_DoomedProphetRing(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Doomed Prophet's Ring";
        }

        public override string getShortDesc()
        {
            return "A ring from a cursed prophet, shows the wearer the truth but curses them to be unable from sharing it with others. When the person holding the ring attempts to warn the world they spread shadow instead of awareness. No effect on the chosen one.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_ProphetRing.png");
        }

        public override void turnTick(Person owner)
        {
            if (owner.unit != null || owner.unit.isCommandable() || owner.unit == map.awarenessManager.getChosenOne())
            {
                return;
            }

            T_DoomedProphetsCurse curse = (T_DoomedProphetsCurse)owner.traits.FirstOrDefault(t => t is T_DoomedProphetsCurse);
            if (curse == null)
            {
                curse = new T_DoomedProphetsCurse();
                owner.receiveTrait(curse);
            }
        }

        public override int getLevel()
        {
            return LEVEL_NODROP;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
