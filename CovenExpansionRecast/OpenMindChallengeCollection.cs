using Assets.Code;
using CommunityLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CovenExpansionRecast
{
    public class OpenMindChallengeCollection : IEnumerable, IEnumerable<KeyValuePair<Tuple<Type, string>, OrderedDictionary<string, Challenge>>>, IEnumerable<Challenge>
    {
        private readonly OrderedDictionary<Tuple<Type, string>, OrderedDictionary<string, Challenge>> outerDict = new OrderedDictionary<Tuple<Type, string>, OrderedDictionary<string, Challenge>>();

        private readonly Location location;

        public OpenMindChallengeCollection(Location location)
        {
            this.location = location;
        }

        public OrderedDictionary<string, Challenge> this[int index]
        {
            get
            {
                return outerDict[index];
            }
        }

        public OrderedDictionary<string, Challenge> this[Tuple<Type, string> key]
        {
            get
            {
                return outerDict[key];
            }
        }

        public OrderedDictionary<string, Challenge> this[Type type, string name]
        {
            get
            {
                return outerDict[Tuple.Create(type, name)];
            }
        }

        public OrderedDictionary<string, Challenge> this[Challenge challenge]
        {
            get
            {
                return outerDict[Tuple.Create(challenge.GetType(), challenge.getName())];
            }
        }

        // Retrieve or create an inner collection for a challenge type and name
        private OrderedDictionary<string, Challenge> GetOrCreateInnerCollection(Challenge challenge)
        {
            var key = Tuple.Create(challenge.GetType(), challenge.getName());
            return GetOrCreateInnerCollection(key);
        }

        private OrderedDictionary<string, Challenge> GetOrCreateInnerCollection(Type challengeType, string challengeName)
        {
            var key = Tuple.Create(challengeType, challengeName);
            return GetOrCreateInnerCollection(key);
        }

        private OrderedDictionary<string, Challenge> GetOrCreateInnerCollection(Tuple<Type, string> key)
        {
            if (!outerDict.TryGetValue(key, out var innerCollection))
            {
                innerCollection = new OrderedDictionary<string, Challenge>();
                outerDict[key] = innerCollection;
            }
            return innerCollection;
        }

        private bool TryGetInnerCollection(Challenge challenge, out OrderedDictionary<string, Challenge> innerCollection)
        {
            var key = Tuple.Create(challenge.GetType(), challenge.getName());
            return TryGetInnerCollection(key, out innerCollection);
        }

        private bool TryGetInnerCollection(Type challengeType, string challengeName, out OrderedDictionary<string, Challenge> innerCollection)
        {
            var key = Tuple.Create(challengeType, challengeName);
            return TryGetInnerCollection(key, out innerCollection);
        }

        private bool TryGetInnerCollection(Tuple<Type, string> key, out OrderedDictionary<string, Challenge> innerCollection)
        {
            return outerDict.TryGetValue(key, out innerCollection);
        }

        // Generate a composite key based on challenge name, location, and distance
        private string GenerateCompositeKey(Challenge challenge)
        {
            string locationName = challenge.location.getName();
            int distance = challenge.map.getStepDist(location, challenge.location);
            return $"{challenge.getName()} at {locationName} (distance: {distance})";
        }

        // Try to add a new challenge; returns false if the composite key already exists
        public bool TryAddChallenge(Challenge challenge)
        {
            OrderedDictionary<string, Challenge> innerCollection = GetOrCreateInnerCollection(challenge);
            string compositeKey = GenerateCompositeKey(challenge);

            if (innerCollection.ContainsKey(compositeKey))
            {
                return false;
            }

            innerCollection.Add(compositeKey, challenge);
            return true;
        }

        // Check if a challenge exists in the collection
        public bool ContainsChallenge(Challenge challenge)
        {
            if (TryGetInnerCollection(challenge, out OrderedDictionary<string, Challenge> innerCollection))
            {
                string compositeKey = GenerateCompositeKey(challenge);
                return innerCollection.ContainsKey(compositeKey);
            }

            return false;
        }

        // Remove a challenge using its composite key
        public bool RemoveChallenge(Challenge challenge)
        {
            if (TryGetInnerCollection(challenge, out OrderedDictionary<string, Challenge> innerCollection))
            {
                string compositeKey = GenerateCompositeKey(challenge);
                return innerCollection.Remove(compositeKey);
            }

            return false;
        }

        // Retrieve all challenges as a flattened list (combining all inner collections)
        public List<Challenge> GetAllChallenges()
        {
            List<Challenge> allChallenges = new List<Challenge>();

            foreach (OrderedDictionary<string, Challenge> innerCollection in outerDict.Values)
            {
                foreach (Challenge challenge in innerCollection.Values)
                {
                    allChallenges.Add(challenge);
                }
            }

            return allChallenges;
        }

        // Enumerate through all key-value pairs (outer and inner)
        public IEnumerator<Challenge> EnumerateChallenges()
        {
            foreach (var outerKvp in outerDict)
            {
                foreach (var innerKvp in outerKvp.Value)
                {
                    yield return innerKvp.Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<Tuple<Type, string>, OrderedDictionary<string, Challenge>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<Tuple<Type, string>, OrderedDictionary<string, Challenge>>>)outerDict).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Challenge> IEnumerable<Challenge>.GetEnumerator()
        {
            return EnumerateChallenges();
        }

        // Count of all challenges in the collection
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var innerCollection in outerDict.Values)
                {
                    count += innerCollection.Count;
                }
                return count;
            }
        }
    }
}
