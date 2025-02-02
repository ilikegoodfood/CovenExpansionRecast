using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_TransmutationMaster : Trait
    {
        public override string getName()
        {
            return "Weaver of Souls";
        }

        public override string getDesc()
        {
            return "When transposing two souls you may choose which of the souls to keep. When aquired gain two soulstones filled with random souls.";
        }

        public override void onAcquire(Person person)
        {
            I_Soulstone soulstoneA = new I_Soulstone(person.map);
            I_Soulstone soulstoneB = new I_Soulstone(person.map);

            Person victim = new Person(person.society);
            T_ChallengeBooster booster = new T_ChallengeBooster(Eleven.random.Next(6) + 100);
            victim.isDead = true;
            victim.receiveTrait(booster);
            soulstoneA.CapturedSoul = victim;

            victim = new Person(person.society);
            booster = new T_ChallengeBooster(Eleven.random.Next(6) + 100);
            victim.isDead = true;
            victim.receiveTrait(booster);
            soulstoneB.CapturedSoul = victim;

            if (person.unit?.isCommandable() ?? false && person.map.burnInComplete && !person.map.automatic)
            {
                ItemToWorldExchange trade = new ItemToWorldExchange(person.map);
                trade.addItemToSet(soulstoneA);
                trade.addItemToSet(soulstoneB);
                person.map.world.prefabStore.popItemTrade(person, trade, "Swap Items", -1, -1);
                return;
            }

            person.gainItem(soulstoneA);
            person.gainItem(soulstoneB);
        }

        public override int getMaxLevel()
        {
            return 1;
        }

        public override int[] getTags()
        {
            return new int[] {
                Tags.CRUEL
            };
        }
    }
}
