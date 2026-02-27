using Assets.Code;
using CommunityLib;

namespace CovenExpansionRecast
{
    public class Task_PigeonCarryToUnit : Task_GoToUnit
    {
        public UAEN_Pigeon Pigeon;

        public Task_PigeonCarryToUnit(UAEN_Pigeon self, Unit c)
            : base(self, c, 1, 0, false)
        {
            Pigeon = self;
        }

        public override string getShort()
        {
            if (target == null)
            {
                return $"Travelling to trade with no-one";
            }

            if (Pigeon.returning)
            {
                return $"Returning to {target.getName()}";
            }

            return $"Travelling to trade with {target.getName()}";
        }

        public override string getLong()
        {
            if (target == null)
            {
                return $"This pigeon is travelling to trade with no-one.";
            }

            if (Pigeon.returning)
            {
                return $"This pigeon is returning to its ower {target.getName()} at {target.location.getName()}.";
            }

            return $"This pigeon is travelling to trade with {target.getName()} at {target.location.getName()}.";
        }

        public override Location getLocation()
        {
            return target.location;
        }

        public override void turnTick(Unit unit)
        {
            if (hasArrived)
            {
                unit.task = null;
            }

            if (target == null || !(target is UA) || (!ModCore.Get().checkIsUnitSubsumed(target) && target.isDead) || targetLocation == null)
            {
                unit.task = null;
                return;
            }

            targetLocation = target.location;
            if (ModCore.Get().checkIsUnitSubsumed(target) && target.person != null)
            {
                targetLocation = target.person.unit.location;
            }

            if (unit.location == targetLocation)
            {
                hasArrived = true;
                if (Pigeon.returning)
                {
                    Pigeon.map.world.prefabStore.popItemTrade(Pigeon.person, target.person, "Take Items from Returning Pigeon", -1, -1);
                    Pigeon.GainPigeon(target as UA);
                }
                else
                {
                    Pigeon.map.world.prefabStore.popItemTrade(Pigeon.person, target.person, "Swap Items", -1, -1);
                    Pigeon.returning = true;
                }
                unit.task = null;
            }

            while (unit.movesTaken < unit.getMaxMoves())
            {
                targetLocation = target.location;
                if (ModCore.Get().checkIsUnitSubsumed(target) && target.person != null)
                {
                    targetLocation = target.person.unit.location;
                }

                bool moved = unit.map.moveTowards(unit, targetLocation);
                if (!moved)
                {
                    World.log("Move unsuccessful. Cancelling go to challenge");
                    unit.task = null;
                    return;
                }
                unit.movesTaken++;

                if (unit.location == targetLocation)
                {
                    hasArrived = true;
                    if (Pigeon.returning)
                    {
                        Pigeon.map.world.prefabStore.popItemTrade(Pigeon.person, target.person, "Take Items from Returning Pigeon", -1, -1);
                        Pigeon.GainPigeon(target as UA);
                    }
                    else
                    {
                        Pigeon.map.world.prefabStore.popItemTrade(Pigeon.person, target.person, "Swap Items", -1, -1);
                        Pigeon.returning = true;
                    }
                    unit.task = null;
                    return;
                }
            }
        }
    }
}
