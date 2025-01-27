using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_SpiritCage : Item
    {
        public UAE_Spirit Spirit;

        public Person Holder;

        public bool HasSpirit
        {
            get
            {
                if (Spirit == null)
                {
                    return false;
                }

                if (Spirit.isDead)
                {
                    Spirit = null;
                    return false;
                }

                return true;
            }
            
        }

        public I_SpiritCage(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Spirit Cage";
        }

        public override string getShortDesc()
        {
            if (HasSpirit)
            {
                return "Creates a spirit agent linked to the cage, the spirit cannot move on its own but will follow the person holding the cage each time they move.";
            }

            return "Once this artifact was host to a powerful spectre, now it is an empty box devoid of purpose.";
        }

        public override Sprite getIconFore()
        {
            if (HasSpirit)
            {
                return EventManager.getImg("CovenExpansionRecast.Fore_SpiritCage_Full.png");
            }

            return EventManager.getImg("CovenExpansionRecast.Fore_SpiritCage_Empty.png");
        }

        public override void turnTick(Person owner)
        {
            Holder = owner;

            if (!HasSpirit)
            {
                return;
            }

            if (Spirit.location != owner.getLocation())
            {
                Spirit.location.units.Remove(Spirit);
                owner.getLocation().units.Add(Spirit);
                Spirit.location = owner.getLocation();
            }

            if (Spirit.task is Task_PerformChallenge performChallenge && performChallenge.challenge is Rt_Haunt)
            {
                if (owner != null && owner.unit != null && !owner.unit.isCommandable() && owner.unit != owner.map.awarenessManager.getChosenOne())
                {
                    owner.sanity -= 1.0;
                    if (owner.sanity <= 0.0)
                    {
                        owner.goInsane();
                        Spirit.addMenace(0.4);
                    }
                }
            }
        }

        public override void onTravel(Person p, Location current, Location destination)
        {
            if (!HasSpirit)
            {
                return;
            }

            Spirit.location.units.Remove(Spirit);
            p.getLocation().units.Add(Spirit);
            Spirit.location = p.getLocation();
        }

        public override int getLevel()
        {
            if (HasSpirit)
            {
                return LEVEL_NODROP;
            }

            return LEVEL_COMMON;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
