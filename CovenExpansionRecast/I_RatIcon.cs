using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_RatIcon : Item
    {
        public I_RatIcon(Map map)
            : base(map)
        {

        }

        public override string getName()
        {
            return "Razor Icon";
        }

        public override string getShortDesc()
        {
            return "The bearer gains +2 <b>Might</b>. If held by an agent of a dark power, all razor rat minions any agent of that power have increase danger every 3 turns, instead of 10, and increase it by 6, instead of their normal amount. Allows the bearer to steal gold and a personal item from damaged heroes.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_RazorTrophy.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            List<Ritual> challenges = new List<Ritual>();
            foreach (Unit u in ua.location.units)
            {
                if (!u.isCommandable() && u is UA other && other.person != null && other.hp < other.maxHp)
                {
                    challenges.Add(new Rti_RatIconSteal(ua.location, other));
                }
            }
            challenges.AddRange(this.challenges);

            return challenges;
        }

        public override void turnTick(Person owner)
        {
            if (owner.unit == null || !owner.unit.isCommandable())
            {
                return;
            }

            foreach (Unit playerUnit in map.overmind.agents)
            {
                if (!(playerUnit is UA ua) || ua.isDead || !ua.isCommandable())
                {
                    continue;
                }

                foreach (Minion minion in ua.minions)
                {
                    if (!(minion is M_Razorrat) || minion.traits == null || minion.traits.Count == 0)
                    {
                        continue;
                    }

                    foreach (MinionTrait trait in minion.traits)
                    {
                        if (!(trait is Mt_UrbanProwler prowler) || prowler.charge < 3)
                        {
                            continue;
                        }

                        if (ua.location.settlement is Set_City || ua.location.settlement is Set_DwarvenCity)
                        {
                            prowler.charge = 0;

                            foreach (Challenge challenge in ua.location.GetChallenges())
                            {
                                if (challenge.isGoodTernary() > -1 &&  challenge.addedDanger < map.param.minion_hazardousDangerIncrease * 3)
                                {
                                    challenge.addedDanger += 6;
                                    map.addMessage($"{minion.getName()} added danger", 0.25, true, ua.location.hex);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override int getMightBonus()
        {
            return 2;
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
