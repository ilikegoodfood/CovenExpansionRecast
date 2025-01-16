using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class UAEN_Toad : UAEN
    {
        public Unit SubsumedUnit;

        public Ch_SquashToad Ch_SquashToad;

        public Rt_Croak Rt_Croak;

        public int Timer;

        public UAEN_Toad(Location loc, Society sg, Person p, int duration = 5)
            : base(loc, sg, p)
        {
            Ch_SquashToad = new Ch_SquashToad(loc, p);
            Rt_Croak = new Rt_Croak(loc, this);
            rituals.Add(Rt_Croak);
            corrupted = false;
            Timer = duration;
        }

        public override Sprite getPortraitForeground()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_FrogCurse.png");
        }

        public override void turnTickAI()
        {
            Console.WriteLine("CovenExpansionRecast: ERROR: This unit's AI should be managed by the Community Library. This message will only appear if that has not happened.");
        }

        public override void turnTick(Map map)
        {
            Timer--;
            if (Timer < 1)
            {
                if (SubsumedUnit == null)
                {
                    Console.WriteLine("CovenExpansionRecast: ERROR: All Toads should contain a subsumed unit. This one, for whatever reason, does not.");
                    disband(map, "Toad failed to return to human form.");
                    return;
                }

                SubsumedUnit.isDead = false;
                location.units.Add(SubsumedUnit);
                map.units.Add(SubsumedUnit);
                person.unit = SubsumedUnit;

                T_Toad toadTrait = (T_Toad)person.traits.FirstOrDefault(t => t is T_Toad);
                if (toadTrait != null)
                {
                    person.traits.Remove(toadTrait);
                }

                person = null;

                disband(map, "Toad returned to human form.");
                return;
            }
        }

        public override void addChallenges(Location location, List<Challenge> standardChallenges)
        {
            standardChallenges.Add(Ch_SquashToad);
        }

        public override bool isCommandable()
        {
            bool result = corrupted;

            if (!result)
            {
                foreach (Trait trait in person.traits)
                {
                    if (trait.grantsCommand())
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        public override void die(Map map, string v, Person killer = null)
        {
            base.die(map, v, killer);

            if (SubsumedUnit != null)
            {
                location.properties.Add(new Pr_FallenHuman(location, person));
            }
        }
    }
}
