using Assets.Code;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                if (NationalActions.Count > 0)
                {
                    value++;
                }
                if (LocalActions.Count > 0)
                {
                    value++;
                }
                if (Challenges.Count > 0)
                {
                    value++;
                }

                return value;
            }
        }

        public void PopulateTypeSelectorOptions()
        {
            TypeSelectorOptions.Clear();

            if (NationalActions.Count > 0)
            {
                TypeSelectorOptions.Add("National Actions");
            }
            if (LocalActions.Count > 0)
            {
                TypeSelectorOptions.Add("Local Actions");
            }
            if (Challenges.Count > 0)
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
