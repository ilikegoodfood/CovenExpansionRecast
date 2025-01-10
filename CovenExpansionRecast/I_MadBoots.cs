using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_MadBoots : Item
    {
        public I_MadBoots(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Madcap Boots";
        }

        public override string getShortDesc()
        {
            return "The cursed and twisted leather of these boots grants uncanny speed and a penchant for recklessness. Grants +2 movement each turn. However, the boots will not allow the user to flee from a fight sending the wearer charging at their assailant when one tries to attack them.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_MadBoots.png");
        }

        public override void turnTick(Person owner)
        {
            if (owner.unit == null)
            {
                return;
            }

            owner.unit.movesTaken -= 2;

            if (owner.unit.engagedBy != null || owner.unit.engaging != null)
            {
                return;
            }

            Dictionary<UA, Location[]> attackingAgents = new Dictionary<UA, Location[]>();
            int distance = 0;
            foreach (Unit unit in map.units)
            {
                if (!(unit is UA ua))
                {
                    continue;
                }

                if (unit.task == null)
                {
                    continue;
                }

                bool isAttacking = false;
                if (unit.task is Task_AttackUnit attack)
                {
                    if (attack.target == owner.unit)
                    {
                        isAttacking = true;
                    }
                }
                else if (unit.task is Task_AttackUnitWithEscort attackWithEscort)
                {
                    if (attackWithEscort.target == owner.unit)
                    {
                        isAttacking = true;
                    }
                }

                if (!isAttacking)
                {
                    continue;
                }

                if (owner.getLocation() == ua.location)
                {
                    if (distance > 0)
                    {
                        attackingAgents.Clear();
                        distance = 0;
                    }

                    attackingAgents.Add(ua, new Location[0]);
                    continue;
                }

                Location[] pathTo = Pathfinding.getPathTo(owner.getLocation(), ua.location, owner.unit);
                if (pathTo == null || pathTo.Length < 2)
                {
                    continue;
                }

                int dist = pathTo.Count() - 1;
                if (attackingAgents.Count == 0 || dist <= distance)
                {
                    if (dist < distance)
                    {
                        attackingAgents.Clear();
                        distance = dist;
                    }

                    attackingAgents.Add(ua, pathTo);
                    continue;
                }
            }

            UA attackingAgent = null;
            Location[] path = null;
            if (attackingAgents.Count > 0)
            {
                if (attackingAgents.Count > 1)
                {
                    List<UA> keys = attackingAgents.Keys.ToList();
                    attackingAgent = keys[Eleven.random.Next(keys.Count())];
                    path = attackingAgents[attackingAgent];
                }
                else
                {
                    attackingAgent = attackingAgents.Keys.First();
                    path = attackingAgents[attackingAgent];
                }
            }

            if (attackingAgent != null && path.Length > 0)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (owner.unit.movesTaken >= owner.unit.getMaxMoves())
                    {
                        break;
                    }

                    map.adjacentMoveTo(owner.unit, path[i]);
                    owner.unit.movesTaken++;
                }

                owner.unit.movesTaken = owner.unit.getMaxMoves();
                map.addUnifiedMessage(owner, attackingAgent, "Forced Movement", $"{owner.getName()}'s {getName()} have rocketed them into danger, sending them chasing after their assailent {attackingAgent.getName()}.", "BOOTS FORCE MOVEMENT", true);
            }
        }

        public override int getLevel()
        {
            return LEVEL_RARE;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
