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

        public Ch_SquashFrog Ch_SquashToad;

        public Rt_Croak Rt_Croak;

        public int Timer = 5;

        public UAEN_Toad(Location loc, Society sg, Person p)
            : base(loc, sg, p)
        {
            Ch_SquashToad = new Ch_SquashFrog(loc, p);
            Rt_Croak = new Rt_Croak(loc, this);
            rituals.Add(Rt_Croak);
            corrupted = false;
        }

        public override Sprite getPortraitForeground()
        {
            return EventManager.getImg("CovenExpansion.Icon_FrogCurse.png");
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
                person = null;

                disband(map, "Toad returned to human form.");
            }
        }

        public override void addChallenges(Location location, List<Challenge> standardChallenges)
        {
            standardChallenges.Add(Ch_SquashToad);
        }

        public override bool isCommandable()
        {
            return corrupted;
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
