using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Mirror : Trait
    {
        public Person Target;

        public T_Mirror(Person target)
        {
            Target = target;
        }

        public override string getName()
        {
            return $"Mirrored";
        }

        public override string getDesc()
        {
            return "This person is a perfect mirror of another. Their original identity is lost forever.";
        }

        public override void turnTick(Person p)
        {
            if (Target == null || p == Target || p == p.map.awarenessManager.chosenOne?.person || (p.unit != null && p.unit.isCommandable()))
            {
                p.traits.Remove(this);
                return;
            }

            validateTagOpinions(p);
            validateTagOpinions(Target);

            foreach(int tag in getAllTags(Target).Union(getAllTags(p)))
            {
                int targetOpinion = getTagOpinion(Target, tag);
                int personOpinion = getTagOpinion(p, tag);

                if (personOpinion == targetOpinion)
                {
                    continue;
                }

                while (personOpinion > targetOpinion)
                {
                    p.decreasePreference(tag);
                    personOpinion--;
                }

                while (personOpinion < targetOpinion)
                {
                    p.increasePreference(tag);
                    personOpinion++;
                }
            }
        }

        public int[] getAllTags(Person person)
        {
            List<int> result = new List<int>();
            result.AddRange(person.extremeLikes);
            result.AddRange(person.likes);
            result.AddRange(person.hates);
            result.AddRange(person.extremeHates);
            return result.Distinct().ToArray();
        }

        public int getTagOpinion(Person person, int tag)
        {
            if (person.extremeLikes.Contains(tag))
            {
                return 2;
            }

            if (person.likes.Contains(tag))
            {
                return 1;
            }

            if (person.extremeHates.Contains(tag))
            {
                return -2;
            }

            if (person.hates.Contains(tag))
            {
                return -1;
            }

            return 0;
        }

        public void validateTagOpinions(Person person)
        {
            foreach (int tag in person.extremeLikes.Intersect(person.likes))
            {
                person.likes.Remove(tag);
            }

            foreach (int tag in person.extremeHates.Intersect(person.hates))
            {
                person.hates.Remove(tag);
            }

            foreach (int tag in person.likes.Intersect(person.hates))
            {
                person.likes.Remove(tag);
                person.hates.Remove(tag);
            }

            foreach (int tag in person.extremeLikes.Intersect(person.extremeHates))
            {
                person.extremeLikes.Remove(tag);
                person.extremeHates.Remove(tag);
            }
        }
    }
}
