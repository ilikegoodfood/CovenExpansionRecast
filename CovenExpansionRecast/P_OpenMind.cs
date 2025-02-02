using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class P_OpenMind : Power
    {
        public int Cost = 0;

        public int TurnLastChecked = 0;

        public List<Type> PropertyTypeBlacklist = new List<Type> { typeof(Pr_Vinverva_Food), typeof(Pr_Vinverva_Gold), typeof(Pr_Vinverva_Peace) };

        public P_OpenMind(Map map)
            : base(map)
        {
            
        }

        public override string getName()
        {
            return "Eldritch Command";
        }

        public override string getDesc()
        {
            return "Choose the challenge being performed by the target agent, the local action being performed by the ruler of the target settlement, or the national action being performed by the target capital city. If the target has mutliple options available, such as a capital city with both local and national actions, you will be offered a choice of which to change. Youcan only select National actions with utility of -40.0, or greater. The first cast of this power each turn is free.";
        }

        public override string getFlavour()
        {
            return "There is no safety for them now. Their minds are laid bare before you, plain as day; all that's left is to give an order.";
        }

        public override string getRestrictionText()
        {
            return "Must be cast on an agent or ruler with the Collected Mind curse. Cannot target the chosen one, or one of your own agents.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Mind.png");
        }

        public override bool validTarget(Unit unit)
        {
            return !unit.isCommandable() && unit != map.awarenessManager.chosenOne && unit.person != null && unit.person.traits.Any(t => t is T_CollectedMind);
        }

        public override bool validTarget(Location loc)
        {
            return loc.person() != null && loc.person() != map.awarenessManager.chosenOne?.person && loc.person().traits.Any(t => t is T_CollectedMind);
        }

        public override int getCost()
        {
            if (map.turn > TurnLastChecked)
            {
                TurnLastChecked = map.turn;
                Cost = 0;
            }

            return Cost;
        }

        public override void cast(Location loc)
        {
            base.cast(loc);
            Person p = loc.person();
            if (p != null)
            {
                Cast(p);
            }
        }

        public override void cast(Unit unit)
        {
            base.cast(unit);
            if (unit.person != null)
            {
                Cast(unit.person);
            }
        }

        public virtual void Cast(Person p)
        {
            Cost++;
            OpenMindSelectorData selectorData = new OpenMindSelectorData(p);

            if (p.rulerOf != -1 && map.locations[p.rulerOf].settlement is SettlementHuman humanSettlement && humanSettlement.ruler == p && map.locations[p.rulerOf].soc is Society society)
            {
                selectorData.HumanSettlement = humanSettlement;
                if (society.getCapitalHex() == map.locations[p.rulerOf].hex)
                {
                    selectorData.Society = society;
                    PopulateNationalActionData(selectorData, p, society);
                }

                PopulateLocalActionData(selectorData, p);
            }

            if (p.unit is UA ua)
            {
                selectorData.Ua = ua;
                PopulateChallengeData(selectorData, ua);
            }

            if (selectorData.ActionTypeCount == 0)
            {
                RefundCast(p, "Target has no valid actions or challenges that they can perform which they are not already performing");
                return;
            }

            selectorData.PopulateTypeSelectorOptions();
            Sel2_OpenMind_TypeSelector.PopInstance(selectorData);
        }

        public virtual void RefundCast(Person p, string reason = null)
        {
            if (Cost <= 0)
            {
                Cost = 0;
                return;
            }

            if (!string.IsNullOrEmpty(reason))
            {
                map.addUnifiedMessage(p, null, "Cast Failed", reason, "Cast Failed", true);
            }
            
            Cost--;
            map.overmind.power += Cost;

            if (map.overmind.power > map.overmind.getMaxPower())
            {
                map.overmind.power = map.overmind.getMaxPower();
            }
        }

        public virtual void PopulateNationalActionData(OpenMindSelectorData data, Person p, Society society)
        {
            foreach (AN action in society.actions)
            {
                if (action != society.actionUnderway && action.valid(society, p))
                {
                    if (action.getUtility(society, p, null) < -40.0)
                    {
                        continue;
                    }

                    data.NationalActions.Add(action);
                    data.NationalActionNames.Add(action.getName());
                }
            }
        }

        public virtual void PopulateLocalActionData(OpenMindSelectorData data, Person p)
        {
            Location actionLocation = map.locations[p.rulerOf];
            SettlementHuman targetSettlement = actionLocation.settlement as SettlementHuman;
            if (targetSettlement == null)
            {
                return;
            }

            HashSet<Assets.Code.Action> blacklistedActions = new HashSet<Assets.Code.Action>();
            foreach (Property property in actionLocation.properties)
            {
                bool blacklisted = false;
                foreach (Type blacklistedType in PropertyTypeBlacklist)
                {
                    if (property.GetType() == blacklistedType || property.GetType().IsSubclassOf(blacklistedType))
                    {
                        blacklisted = true;
                        foreach (Assets.Code.Action action in property.getActions())
                        {
                            blacklistedActions.Add(action);
                        }
                    }
                }

                if (blacklisted)
                {
                    break;
                }
            }

            foreach (Assets.Code.Action action in targetSettlement.getLocalActions())
            {
                if (action != targetSettlement.actionUnderway && !blacklistedActions.Contains(action) && action.valid(p, targetSettlement))
                {
                    data.LocalActions.Add(action);
                    data.LocalActionNames.Add(action.getName());
                }
            }
        }

        public virtual void PopulateChallengeData(OpenMindSelectorData data, UA ua)
        {
            Challenge currentChallenge = null;
            if (ua.task is Task_PerformChallenge performChallenge)
            {
                currentChallenge = performChallenge.challenge;
            }
            else if (ua.task is Task_GoToPerformChallenge goToPerformChallenge)
            {
                currentChallenge = goToPerformChallenge.challenge;
            }

            if  (CommunityLib.ModCore.Get().GetAgentAI().TryGetAgentType(ua.GetType(), out _))
            {
                List<CommunityLib.AgentAI.ChallengeData> challengeData = CommunityLib.ModCore.Get().GetAgentAI().getAllValidChallengesAndRituals(ua);

                foreach (CommunityLib.AgentAI.ChallengeData cd in challengeData)
                {
                    if (cd.challenge is Ritual || cd.challenge == currentChallenge)
                    {
                        continue;
                    }

                    data.Challenges.TryAddChallenge(cd.challenge);
                }
            }
            else
            {
                foreach (Challenge challenge in ua.getAllValidChallenges())
                {
                    if (challenge is Ritual || challenge == currentChallenge)
                    {
                        continue;
                    }

                    data.Challenges.TryAddChallenge(challenge);
                }
            }
        }
    }
}
