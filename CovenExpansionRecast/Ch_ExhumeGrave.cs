using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_ExhumeGrave : Challenge
    {
        public double deltaRobbed = 35.0;

        public double robbedCutoff = 100.0;

        public Ch_ExhumeGrave(Location loc)
            : base(loc)
        {

        }

        public override string getName()
        {
            return "Exhume Grave";
        }

        public override string getDesc()
        {
            return $"Exhumes a long buried grave, increasing the Robbed Graves property by {(int)deltaRobbed}%, and creating a soul which carries at least 25 gold in burial goods.";
        }

        public override string getCastFlavour()
        {
            return "There is a mine of gold and jewels here just below the surface. A sturdy shovel and a lack of conscience are all that's required to exhume them.";
        }

        public override string getRestriction()
        {
            return $"Requires this Catacomb to be infiltrated, and for the robbed graves property to be less than {(int) robbedCutoff}";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansion.Icon_Graveyard.png");
        }

        public override double getProfile()
        {
            return 50.0;
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            if (!(location.settlement is SettlementHuman humanSettlement))
            {
                return false;
            }

            Sub_Catacombs catacombs = (Sub_Catacombs)humanSettlement.subs.FirstOrDefault(sub => sub is Sub_Catacombs && sub.infiltrated);
            if (catacombs != null)
            {
                Pr_RobbedGraves robbed = (Pr_RobbedGraves)location.properties.FirstOrDefault(pr => pr is Pr_RobbedGraves);
                if (robbed != null && robbed.charge > 99.0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public override double getComplexity()
        {
            return 20.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.INTRIGUE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatIntrigue();
            if (val > 0.0)
            {
                msgs?.Add(new ReasonMsg("Stat: Intrigue", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));

            return val;
        }

        public override int getCompletionProfile()
        {
            return 4;
        }

        public override int getCompletionMenace()
        {
            return 10;
        }

        public override void complete(UA u)
        {
            Pr_RobbedGraves robbed = (Pr_RobbedGraves)location.properties.FirstOrDefault(pr => pr is Pr_RobbedGraves);
            if (robbed != null)
            {
                robbed.charge += deltaRobbed;
            }
            else
            {
                robbed = new Pr_RobbedGraves(location);
                robbed.charge = 35.0;
                location.properties.Add(robbed);
            }

            Person person;
            if (!(location.settlement is SettlementHuman humanSettlement) || humanSettlement.ruler == null)
            {
                person = new Person(map.soc_dark, null);
            }
            else
            {
                person = new Person(humanSettlement.ruler.society, humanSettlement.ruler.house);
                person.embedIntoSociety();
            }

            person.isDead = true;
            T_ChallengeBooster booster = robbed.GenerateChallengeBooster();
            person.receiveTrait(booster);
            person.addGold((int)(25.0 + Math.Sqrt(Eleven.random.Next(2500))));

            int roll = Eleven.random.Next(300);

            if (roll > 294)
            {
                T_CallOfTheAbyss deepOneCurse = new T_CallOfTheAbyss();
                deepOneCurse.strength = Eleven.random.Next(70);
                person.receiveTrait(deepOneCurse);
            }
            else if (roll > 289)
            {
                person.goInsane();
            }
            else if (roll > 287)
            {
                T_TheHunger hunger = new T_TheHunger();
                hunger.strength = Eleven.random.Next(70);
                person.receiveTrait(hunger);
            }

            roll = Eleven.random.Next(300);
            switch (roll)
            {
                case 299:
                    person.firstName = "Midas";
                    person.gainItem(new I_BagOfBoundlessWealth(map), true);
                    person.gainItem(new I_BagOfBoundlessWealth(map), true);
                    person.gainItem(new I_BagOfBoundlessWealth(map), true);
                    break;
                case 298:
                    person.gainItem(new I_PageFromTome(map), true);
                    person.gainItem(new I_EvilBook(map), true);
                    person.gainItem(new I_PoisonedDagger(map), true);
                    break;
                case 297:
                    person.gainItem(new I_Poison(map), true);
                    person.gainItem(new I_ToxicVial(map), true);
                    person.gainItem(new I_StudentsManual(map), true);
                    break;
                case 296:
                    person.gainItem(new I_WarAxe(map), true);
                    person.gainItem(new I_ReliableShield(map), true);
                    person.gainItem(new I_ManticoreTrophy(map), true);
                    break;
                case 295:
                    person.gainItem(new I_RuinedPotionOfHealing(map), true);
                    person.gainItem(new I_RuinedPotionOfHealing(map), true);
                    person.gainItem(new I_RuinedPotionOfHealing(map), true);
                    break;
                case 294:
                    person.gainItem(new I_Deathstone(map), true);
                    person.gainItem(new I_SacrificialDagger(map), true);
                    person.gainItem(new I_UnassumingHood(map), true);
                    break;
                case 293:
                    person.gainItem(new I_BagOfPoverty(map), true);
                    person.gainItem(new I_DoomedProphetRing(map), true);
                    person.gainItem(new I_PageFromTome(map), true);
                    break;
                case 292:
                    person.gainItem(new I_SettlersWreath(map), true);
                    person.gainItem(new I_SettlersWreath(map), true);
                    person.gainItem(new I_SettlersWreath(map), true);
                    break;
                default:
                    break;
            }

            // Rolls of 98 and 150 give nothing.
            if (roll < 98)
            {
                person.gainItem(Item.getItemFromPool1(map, -1), true);
            }

            if (roll > 98 && roll < 150)
            {
                person.gainItem(Item.getItemFromPool2(map, -1), true);
            }

            if (roll > 150 && roll < 181)
            {
                person.gainItem(Item.getItemFromPool3(map, -1), true);
            }

            Pr_FallenHuman soul = new Pr_FallenHuman(location, person);
            soul.nameOnDeath = $"{booster.getName()} {person.getName()}";
            location.properties.Add(soul);

            if (u.isCommandable())
            {
                map.world.prefabStore.popItemTrade(u.person, person, "Swap Items", -1, -1);
                return;
            }

            u.person.gold += person.gold;
            person.gold = 0;
            for (int i = 0; i < person.items.Length; i++)
            {
                if (person.items[i] != null)
                {
                    u.person.gainItem(person.items[i]);
                    person.items[i] = null;
                }
            }
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.GOLD,
                Tags.UNDEAD,
                Tags.DISEASE
            };
        }
    }
}
