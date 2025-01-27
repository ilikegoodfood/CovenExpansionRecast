using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Pr_SoulSnare : Property
    {
        public Pr_SoulSnare(Location loc)
            : base(loc)
        {
            stackStyle = stackStyleEnum.NONE;
        }

        public override string getName()
        {
            return "Soul Snare";
        }

        public override string getDesc()
        {
            return "An invisible snare lies in wait here. The next hero that attempts to perform a quest with no associated stat (with agrey colouring) at this location will be ensnared, costing them their soul, which will drop at this location. Has no effect on the chosen one.";
        }

        public override Sprite getSprite(World world)
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_SoulSnare.png");
        }

        public override void turnTick()
        {
            if (charge > 300.0)
            {
                charge = 300.0;
            }

            influences.Add(new ReasonMsg("Disappearing", -1.0));

            foreach (Unit u in location.units)
            {
                if (!(u is UA ua) || u.person == null || !u.person.hasSoul)
                {
                    continue;
                }

                if (u.isCommandable() || map.awarenessManager.getChosenOne() == u)
                {
                    continue;
                }

                if (!(ua.task is Task_PerformChallenge performChallenge) || performChallenge.progress > 1.0)
                {
                    continue;
                }

                u.person.hasSoul = false;
                location.properties.Add(new Pr_FallenHuman(location, u.person));
                u.person.receiveTrait(new T_Soulless());
                location.properties.Remove(this);
                map.addUnifiedMessage(u, null, "Soul Snared", $"{u.getName()} has ventured into {location.getName()}, not realizing the insidious trap lying in wait for them. An implacable feeling of emptiness washes over them as their soul is torn from their body and made ready for your machinations.", "Heroe Loses Soul", true);
                break;
            }
        }

        public override bool removedOnRuin()
        {
            return false;
        }

        public static void addToProperty(string cause, double amount, Location loc)
        {
            Pr_SoulSnare snare = (Pr_SoulSnare)loc.properties.FirstOrDefault(pr => pr is Pr_SoulSnare);
            if (snare == null)
            {
                snare = new Pr_SoulSnare(loc);
                loc.properties.Add(snare);
            }

            snare.influences.Add(new ReasonMsg(cause, amount));
        }
    }
}
