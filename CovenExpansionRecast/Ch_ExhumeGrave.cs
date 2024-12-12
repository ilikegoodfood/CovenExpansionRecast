﻿using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            if (!(location.settlement is SettlementHuman humanSettlement) || !(location.soc is Society))
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

            AssignRandomSpecialEffects(person, out bool endFunctionEarly, out _);
            if (endFunctionEarly)
            {
                return;
            }

            AssignRandomMadness(person, out _);
            AssignRandomCurseTrait(person, out _);
            AssignRandomItems(person, true, out _);

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

        public void AssignRandomSpecialEffects(Person person, out bool endFunctionEarly, out int roll)
        {
            roll = Eleven.random.Next(300);
            endFunctionEarly = false;
            switch (roll)
            {
                case 299:
                    // Ixthus mmortal Case goes here.
                    // Will asign maddness, curses, and items manually.
                    // Will NOT create Soul Property.
                    // Will pop trade window manually.
                    // Will endFunctionEarly.
                    break;
                default:
                    break;
            }
        }

        public void AssignRandomMadness(Person person, out int roll)
        {
            int madness = RollRandomMadness(out roll);
            for (int i = 0; i < madness; i++)
            {
                person.goInsane();
            }
        }

        public int RollRandomMadness(out int roll)
        {
            int result = 0;
            roll = Eleven.random.Next(300);

            if (roll < 200)
            {
                // 200 / 300 CHance (66.6%)
                result = 0;
            }
            else if (roll < 250)
            {
                // 50 / 300 Chance (16.6%)
                result = 1;
            }
            else if (roll < 280)
            {
                // 30 / 300 Chance (10%)
                result = 2;
            }
            else
            {
                // 20 / 300 Chance (6.66%)
                result = 3;
            }

            return result;
        }

        public void AssignRandomCurseTrait(Person person, out int roll)
        {
            Trait curseTrait = RollRandomCurseTrait(out roll);
            if (curseTrait != null)
            {
                person.receiveTrait(curseTrait);
            }
        }

        public Trait RollRandomCurseTrait(out int roll)
        {
            roll = Eleven.random.Next(300);

            if (roll < 9)
            {
                // 9 / 300 Chance (3%)
                T_CallOfTheAbyss deepOneCurse = new T_CallOfTheAbyss();
                deepOneCurse.strength = 5 + Eleven.random.Next(70);
                return deepOneCurse;
            }
            else if (roll < 12)
            {
                // 3 / 300 Chance (1%)
                T_TheHunger hunger = new T_TheHunger();
                hunger.strength = 5 + Eleven.random.Next(70);
                return hunger;
            }
            else if (roll < 21)
            {
                // 9 / 300 Chance (3%)
                if (CovensCore.Instance.TryGetModIntegrationData("LivingWilds", out ModIntegrationData intDataLW))
                {
                    if (intDataLW.TypeDict.TryGetValue("T_Lycanthropy", out Type lycanthropyType) && lycanthropyType != null)
                    {
                        int innerRoll = Eleven.random.Next(4);
                        object[] args = new object[] { false, false };
                        if (roll == 3)
                        {
                            args = new object[] { true, false };
                        }
                        else if (roll == 2)
                        {
                            args = new object[] { false, true };
                        }

                        Trait lycanthropy = (Trait)Activator.CreateInstance(lycanthropyType, args);
                        return lycanthropy;
                    }
                }
            }

            return null;
        }

        public void AssignRandomItems(Person person, bool allowUniqueSets, out int roll)
        {
            Item[] items = RollRandomItems(allowUniqueSets, out roll);

            if (roll == 300)
            {
                person.firstName = "Midas";
            }
            if (roll == 250) // This is the lowest value allocated to unique sets
            {
                if (CovensCore.Instance.TryGetModIntegrationData("DeepOnesPlus", out ModIntegrationData intDataDOP))
                {
                    if (intDataDOP.MethodInfoDict.TryGetValue("getAbyssalItem", out MethodInfo MI_getAbyssalItem) && MI_getAbyssalItem != null && intDataDOP.Kernel != null)
                    {
                        UAG_Warrior warrior = new UAG_Warrior(location.soc.lastTurnLocs[Eleven.random.Next(location.soc.lastTurnLocs.Count)], location.soc as Society, person);

                        for (int i = 0; i < person.items.Length; i++)
                        {
                            object[] args = new object[] { map, warrior };
                            Item abyssalItem = (Item)MI_getAbyssalItem.Invoke(intDataDOP.Kernel, args);
                            person.items[i] = abyssalItem;
                        }
                    }
                }
                
            }

            foreach (Item item in items)
            {
                person.gainItem(item, true);
            }
        }

        public Item[] RollRandomItems(bool allowUniqueSets, out int roll)
        {
            Item[] result = new Item[3];

            if (allowUniqueSets)
            {
                roll = Eleven.random.Next(300);
            }
            else
            {
                roll = Eleven.random.Next(250);
            }
            switch (roll)
            {
                case 299:
                    result[0] = new I_BagOfBoundlessWealth(map);
                    result[1] = new I_BagOfBoundlessWealth(map);
                    result[2] = new I_BagOfBoundlessWealth(map);
                    break;
                case 298:
                    result[0] = new I_PageFromTome(map);
                    result[1] = new I_EvilBook(map);
                    result[2] = new I_PoisonedDagger(map);
                    break;
                case 297:
                    result[0] =  new I_Poison(map);
                    result[1] = new I_ToxicVial(map);
                    result[2] = new I_StudentsManual(map);
                    break;
                case 296:
                    result[0] = new I_WarAxe(map);
                    result[1] = new I_ReliableShield(map);
                    result[2] = new I_ManticoreTrophy(map);
                    break;
                case 295:
                    result[0] = new I_RuinedPotionOfHealing(map);
                    result[1] = new I_RuinedPotionOfHealing(map);
                    result[2] = new I_RuinedPotionOfHealing(map);
                    break;
                case 294:
                    result[0] = new I_Deathstone(map);
                    result[1] = new I_SacrificialDagger(map);
                    result[2] = new I_UnassumingHood(map);
                    break;
                case 293:
                    result[0] = new I_BagOfPoverty(map);
                    result[1] = new I_DoomedProphetRing(map);
                    result[2] = new I_PageFromTome(map);
                    break;
                case 292:
                    result[0] = new I_SettlersWreath(map);
                    result[1] = new I_SettlersWreath(map);
                    result[2] = new I_SettlersWreath(map);
                    break;
                default:
                    break;
            }

            if (roll >= 180)
            {
                // Shortcuts the roll checks if value is above highest value used by random item rolls.
                // Value of in if statement should be the value used in the highest ineduality for random item roll results.
            }
            else if (roll < 100)
            {
                // 100 / 300 Chance (33.3%)
                result[0] = Item.getItemFromPool1(map, -1);

                for (int i = 1; i < 3; i++)
                {
                    int innerRoll = Eleven.random.Next(2);
                    if (innerRoll == 1)
                    {
                        result[i] = Item.getItemFromPool1(map, -1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (roll < 150)
            {
                // 50 / 300 CHance (16.6%)
                result[0] = Item.getItemFromPool2(map, -1);

                for (int i = 1; i < 3; i++)
                {
                    int innerRoll = Eleven.random.Next(2);
                    if (innerRoll == 1)
                    {
                        innerRoll = Eleven.random.Next(2);
                        if (innerRoll == 1)
                        {
                            result[i] = Item.getItemFromPool2(map, -1);
                        }
                        else
                        {
                            result[i] = Item.getItemFromPool1(map, -1);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (roll < 180)
            {
                // 30 / 300 Chance (10%)
                result[0] = Item.getItemFromPool3(map, -1);

                for (int i = 1; i < 3; i++)
                {
                    int innerRoll = Eleven.random.Next(2);
                    if (innerRoll == 1)
                    {
                        innerRoll = Eleven.random.Next(3);
                        if (innerRoll == 2)
                        {
                            result[i] = Item.getItemFromPool3(map, -1);
                        }
                        else if (innerRoll == 1)
                        {
                            result[i] = Item.getItemFromPool2(map, -1);
                        }
                        else
                        {
                            result[i] = Item.getItemFromPool1(map, -1);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
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
