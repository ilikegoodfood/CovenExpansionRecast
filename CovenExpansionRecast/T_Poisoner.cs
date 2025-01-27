using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_Poisoner : Trait
    {
        public override string getName()
        {
            return "Esoteric Poisons";
        }

        public override string getDesc()
        {
            return "Through the application of the toxins within the Phthisical Vial, the effectiveness of the Poison Hero challenge will be greatly enhanced. The target will be disrupted for twice as long, and will suffer a permanent stat penalty.";
        }

        public override void turnTick(Person p)
        {
            if (!p.items.Any(i => i is I_ToxicVial))
            {
                p.traits.Remove(this);
            }
        }

        public override void completeChallenge(Challenge challenge)
        {
            if (!(challenge is Ch_PoisonHero poisonHero))
            {
                return;
            }

            UA target = poisonHero.target;
            if (target == null || target.person == null)
            {
                return;
            }

            bool superPoisoned = false;
            for (int i = 0; i < target.person.traits.Count; i++)
            {
                if (target.person.traits[i] is T_Poisoned)
                {
                    target.person.traits.Remove(target.person.traits[i]);
                }
                else if (target.person.traits[i] is T_PhthisicalPoisoned esotericPoison)
                {
                    esotericPoison.level++;
                    superPoisoned = true;
                }
            }

            if (!superPoisoned)
            {
                T_PhthisicalPoisoned esotericPoison = new T_PhthisicalPoisoned();
                target.person.receiveTrait(esotericPoison);
            }
        }

        public override int getMaxLevel()
        {
            return 1;
        }
}
