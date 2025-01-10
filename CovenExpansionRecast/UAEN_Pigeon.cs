using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

namespace CovenExpansionRecast
{
    public class UAEN_Pigeon : UAEN
    {
        public UA Owner;

        public UA Target;

        public bool returning;

        public M_Pigeon Pigeon;

        public UAEN_Pigeon(Location loc, Society sg, UA owner, UA target, M_Pigeon minion = null)
            : base(loc, sg)
        {
            Owner = owner;
            Target = target;
            returning = false;

            hp = 1;
            maxHp = 1;
            dontDisplayBorder = true;
            corrupted = false;

            person = new Person(map.soc_neutral);
            person.alert_aware = true;
            person.alert_halfShadow = true;
            person.alert_maxShadow = true;
            person.stat_might = 1;
            person.stat_intrigue = 0;
            person.stat_lore = 1;
            person.stat_command = 0;

            if (minion == null)
            {
                Pigeon = new M_Pigeon(map);
            }
            else
            {
                Pigeon = minion;
            }
        }

        public override bool definesName()
        {
            return true;
        }

        public override string getName()
        {
            return "Carrier Pigeon";
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

        public void GainPigeon(UA target)
        {
            bool gainedPigeon = false;
            if (target.getCurrentlyUsedCommand() < target.getStatCommandLimit())
            {
                for (int i = 0; i < target.minions.Length; i++)
                {
                    if (target.minions[i] == null)
                    {
                        gainedPigeon = true;
                        target.minions[i] = Pigeon;
                        break;
                    }
                }
            }

            if (!gainedPigeon && target.isCommandable() && target.getStatCommandLimit() > 0 && target.map.burnInComplete)
            {
                target.map.world.prefabStore.popMinionDismiss(target, Pigeon);
                return;
            }

            if (gainedPigeon)
            {
                disband(map, $"Carrier pigeon returned to {target.getName()}");
            }
            else
            {
                disband(map, $"Carrier pigeon was released into the wild by {target.getName()}");
            }
        }

        public override void turnTickAI()
        {
            disband(map, $"ERROR: Pigeon AI was not handled by the Community Library. Ensure dependency is loaded.");
        }

        new public List<Unit> getVisibleUnits()
        {
            return new List<Unit>();
        }
    }
}
