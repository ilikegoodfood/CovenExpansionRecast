using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Wanderer : Trait
    {
        public Dictionary<Type, double> ChallengeTypeDict = new Dictionary<Type, double>();

        public double decayRate = 1.0;

        public bool Guarding = false;

        public int GuardCount = 0;

        public bool Attacking = false;

        public int AttackCount = 0;

        public bool Disrupting = false;

        public int DisruptCount = 0;

        public override string getName()
        {
            return "Insatiable Wanderlust";
        }

        public override string getDesc()
        {
            return "This person is afflicted by a gnawing restlessness that permeates their life. They are constantly compelled to experience new sensations, never satisfied with their lot in life.";
        }

        public override void turnTick(Person p)
        {
            if (p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                p.traits.Remove(this);
                return;
            }

            switch (p.unit.task)
            {
                case Task_AttackUnit _:
                    if (!Attacking)
                    {
                        AttackCount++;
                        Attacking = true;
                        Guarding = false;
                        Disrupting = false;
                    }
                    break;
                case Task_Bodyguard _:
                    if (!Guarding)
                    {
                        GuardCount++;
                        Guarding = true;
                        Attacking = false;
                        Disrupting = false;
                    }
                    break;
                case Task_DisruptUA _:
                    if (!Disrupting)
                    {
                        DisruptCount++;
                        Disrupting = true;
                        Attacking = false;
                        Guarding = false;
                    }
                    break;
                default:
                    Attacking = false;
                    Guarding = false;
                    Disrupting = false;
                    break;
            }

            List<Type> typesToRemove = new List<Type>();
            foreach (Type ChallengeType in ChallengeTypeDict.Keys)
            {
                ChallengeTypeDict[ChallengeType] -= decayRate;

                if (ChallengeTypeDict[ChallengeType] < 1.0)
                {
                    typesToRemove.Add(ChallengeType);
                }
            }

            foreach (Type ChallengeType in typesToRemove)
            {
                ChallengeTypeDict.Remove(ChallengeType);
            }

            if (p.map.turn % 50 == 0)
            {
                AttackCount--;
                GuardCount--;
                DisruptCount--;
            }
        }

        public override double getUtilityChanges(Challenge c, UA uA, List<ReasonMsg> reasons)
        {
            Type challengeType = c.GetType();
            if (ChallengeTypeDict.TryGetValue(challengeType, out double utilityModifier))
            {
                reasons?.Add(new ReasonMsg("Wonderlust", -utilityModifier));
                return -utilityModifier;
            }

            reasons?.Add(new ReasonMsg("Wonderlust", utilityModifier));
            return utilityModifier;
        }

        public override void completeChallenge(Challenge challenge)
        {
            Type challengeType = challenge.GetType();
            if (ChallengeTypeDict.ContainsKey(challengeType))
            {
                ChallengeTypeDict[challengeType] += 50.0;
            }
            else
            {
                ChallengeTypeDict.Add(challengeType, 50.0);
            }
        }
    }
}
