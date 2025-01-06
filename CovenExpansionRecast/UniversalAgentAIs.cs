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
    public class UniversalAgentAIs
    {
        public Map Map;

        public UniversalAgentAIs(Map map)
        {
            Map = map;

            PopulatePigeon();
            PopulateToad();
        }

        public void PopulatePigeon()
        {
            AgentAI.ControlParameters controlParams = new AgentAI.ControlParameters(true);
            controlParams.respectDanger = false;
            controlParams.respectArmyIntercept = false;
            controlParams.includeDangerousFoe = false;

            List<AITask> tasks = new List<AITask> {
                    new AITask(typeof(Task_GoToUnit), "Deliver Item", Map, Delegate_Instantiate_DeliverItems, AITask.TargetCategory.Unit, null, new Color(0.5f, 0.5f, 0.5f, 1.0f)),
                    new AITask(typeof(Task_GoToUnit), "Fly Home", Map, Delegate_Instantiate_PigeonFlyHome, AITask.TargetCategory.Unit, null, new Color(0.5f, 0.5f, 0.5f, 1.0f))
                };

            tasks[0].delegates_Valid.Add(Delegate_Validity_DeliverItems);
            tasks[0].delegates_Utility.Add(Delegate_Utility_DeliverItems);
            tasks[1].delegates_Valid.Add(Delegate_Validity_PigeonFlyHome);
            tasks[1].delegates_Utility.Add(Delegate_Utility_PigeonFlyHome);

            CovensCore.ComLib.GetAgentAI().RegisterAgentType(typeof(UAEN_Pigeon), controlParams);
            CovensCore.ComLib.GetAgentAI().AddTasksToAgentType(typeof(UAEN_Pigeon), tasks);
        }

        private Assets.Code.Task Delegate_Instantiate_DeliverItems(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData)
        {
            if (!(ua is UAEN_Pigeon pigeon))
            {
                return null;
            }

            UA target = taskData.targetUnit as UA;
            if (target != null && (!target.isDead || CovensCore.ComLib.checkIsUnitSubsumed(target)))
            {
                Task_GoToUnit follow = new Task_GoToUnit(ua, target.person.unit, -1, 1);
                follow.reasonsMessages.Add(new ReasonMsg("Delivering Items", 100.0));
                return follow;
            }

            return null;
        }

        private bool Delegate_Validity_DeliverItems(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData)
        {
            if (!(ua is UAEN_Pigeon pigeon) || pigeon.returning)
            {
                return false;
            }

            if (pigeon.Target == null || (pigeon.Target.isDead && !CovensCore.ComLib.checkIsUnitSubsumed(pigeon.Target)))
            {
                pigeon.returning = true;
                return false;
            }

            if (taskData.targetCategory == AITask.TargetCategory.Unit && taskData.targetUnit != pigeon.Target)
            {
                return false;
            }

            return true;
        }

        private double Delegate_Utility_DeliverItems(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData, List<ReasonMsg> reasonMsgs)
        {
            double utility = 100;
            reasonMsgs?.Add(new ReasonMsg("Base", utility));

            return utility;
        }

        private Assets.Code.Task Delegate_Instantiate_PigeonFlyHome(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData)
        {
            if (!(ua is UAEN_Pigeon pigeon))
            {
                return null;
            }

            UA target = taskData.targetUnit as UA;
            if (target != null && (!target.isDead || CovensCore.ComLib.checkIsUnitSubsumed(target)))
            {
                Task_GoToUnit follow = new Task_GoToUnit(ua, target.person.unit, -1, 1);
                follow.reasonsMessages.Add(new ReasonMsg("Delivering Items", 100.0));
                return follow;
            }

            return null;
        }

        private bool Delegate_Validity_PigeonFlyHome(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData)
        {
            if (!(ua is UAEN_Pigeon pigeon) || !pigeon.returning)
            {
                return false;
            }

            if (pigeon.Owner == null || (pigeon.Owner.isDead && !CovensCore.ComLib.checkIsUnitSubsumed(pigeon.Owner)))
            {
                return false;
            }

            if (taskData.targetCategory == AITask.TargetCategory.Unit && taskData.targetUnit != pigeon.Owner)
            {
                return false;
            }

            return true;
        }

        private double Delegate_Utility_PigeonFlyHome(UA ua, AITask.TargetCategory targetCategory, AgentAI.TaskData taskData, List<ReasonMsg> reasonMsgs)
        {
            double utility = 100;
            reasonMsgs?.Add(new ReasonMsg("Base", utility));

            return utility;
        }

        private void PopulateToad()
        {
            AgentAI.ControlParameters controlParams = new AgentAI.ControlParameters(true);
            controlParams.respectDanger = false;
            controlParams.respectArmyIntercept = false;
            controlParams.includeDangerousFoe = false;

            AIChallenge challenge = new AIChallenge(typeof(Rt_Croak), 0.0, new List<AIChallenge.ChallengeTags> { AIChallenge.ChallengeTags.BaseValid, AIChallenge.ChallengeTags.BaseValidFor, AIChallenge.ChallengeTags.RequireLocal });
            challenge.delegates_Utility.Add(Delegate_Utility_Ribbit);

            ModCore.Get().GetAgentAI().RegisterAgentType(typeof(UAEN_Toad), controlParams);
            ModCore.Get().GetAgentAI().AddChallengeToAgentType(typeof(UAEN_Toad), challenge);
        }

        private double Delegate_Utility_Ribbit(AgentAI.ChallengeData challengeData, UA ua, double utility, List<ReasonMsg> reasonMsgs)
        {
            utility += 100.0;
            reasonMsgs?.Add(new ReasonMsg("Base", utility));
            return utility;
        }
    }
}
