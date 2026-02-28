using Assets.Code;
using System.Collections.Generic;

namespace CovenExpansionRecast
{
    public class OpenMindSelectorData
    {
        public readonly List<AN> NationalActions = new List<AN>();

        public readonly List<string> NationalActionNames = new List<string>();

        public readonly List<Assets.Code.Action> LocalActions = new List<Assets.Code.Action>();

        public readonly List<string> LocalActionNames = new List<string>();

        public readonly OpenMindChallengeCollection Challenges;

        public readonly List<string> TypeSelectorOptions = new List<string>();

        public Map Map;

        public Person Person;

        public UA Ua;

        public SettlementHuman HumanSettlement;

        public Society Society;

        public int ActionTypeCount
        {
            get
            {
                int value = 0;

                if (NationalActions != null && NationalActions.Count > 0)
                {
                    value++;
                }
                if (LocalActions != null && LocalActions.Count > 0)
                {
                    value++;
                }
                if (Challenges != null && Challenges.Count > 0)
                {
                    value++;
                }

                return value;
            }
        }

        public void PopulateTypeSelectorOptions()
        {
            TypeSelectorOptions.Clear();

            if (NationalActions != null && NationalActions.Count > 0)
            {
                TypeSelectorOptions.Add("National Actions");
            }
            if (LocalActions != null && LocalActions.Count > 0)
            {
                TypeSelectorOptions.Add("Local Actions");
            }
            if (Challenges != null && Challenges.Count > 0)
            {
                TypeSelectorOptions.Add("Challenges");
            }
        }

        public OpenMindSelectorData(Person person)
        {
            Person = person;
            if (person.unit is UA ua)
            {
                Ua = ua;
                Challenges = new OpenMindChallengeCollection(ua.location);
            }
            Map = person.map;
        }
    }
}
