using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class UniversalAgentAIs
    {
        public Map Map;

        public UniversalAgentAIs(Map map)
        {
            Map = map;

            populateToad();
        }

        private void populateToad()
        {
            AgentAI.ControlParameters controlParams = new AgentAI.ControlParameters(true);
            controlParams.respectDanger = false;
            controlParams.respectArmyIntercept = false;
            controlParams.includeDangerousFoe = false;

            AIChallenge challenge = new AIChallenge(typeof(Rt_Croak), 0.0, new List<AIChallenge.ChallengeTags> { AIChallenge.ChallengeTags.BaseValid, AIChallenge.ChallengeTags.BaseValidFor, AIChallenge.ChallengeTags.RequireLocal });
            challenge.delegates_Utility.Add(delegate_Utility_Ribbit);

            ModCore.Get().GetAgentAI().RegisterAgentType(typeof(UAEN_Toad), controlParams);
            ModCore.Get().GetAgentAI().AddChallengeToAgentType(typeof(UAEN_Toad), challenge);
        }

        private double delegate_Utility_Ribbit(AgentAI.ChallengeData challengeData, UA ua, double utility, List<ReasonMsg> reasonMsgs)
        {
            utility += 100.0;
            reasonMsgs?.Add(new ReasonMsg("Base", utility));
            return utility;
        }
    }
}
