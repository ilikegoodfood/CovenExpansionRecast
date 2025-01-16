using Assets.Code;
using Assets.Code.Modding;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class CovensCore : ModKernel
    {
        private static CovensCore instance;

        public static CovensCore Instance => instance;

        private static ModCore comLib;

        public static ModCore ComLib => comLib;

        private Dictionary<string, ModIntegrationData> _modIntegrationData;

        public static bool Opt_Curseweaving = true;

        public static bool Opt_SoulWeaving = true;

        public static bool Opt_LimitedInfluenceGain = true;

        public static int Opt_LimitedInfluenceGainMax = 2;

        public static bool Opt_FindableArtifacts = true;

        public static bool Opt_AdditionalTenets = true;

        // Soul Craftable Pairs //
        // The DUalSouls list is built dynamically in beforeMapGen
        // The lists of craftables are randomiised in beforeMapGen
        // Craftables lists must be of at least the length of the corresponding souls lists.
        public readonly List<string> SingleSouls = new List<string>(new string[]
        {
            "Physician",
            "Mediator",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage"
        });

        public readonly List<string> SingleCraftables = new List<string>(new string[]
        {
            "I_SkeletonKey",
            "I_BagOfPoverty",
            "I_Deathstone",
            "I_DarkStone",
            "I_StudentsManual",
            "I_PoisonedDagger",
            "I_PortableSkeleton",
            "I_ReliableShield",
            "I_PotionOfHealing"
        });

        // Given SimpleSouls has 6 items, DualSouls will have 15 items.
        public readonly List<Tuple<string, string>> DualSouls = new List<Tuple<string, string>>();

        public readonly List<string> DualCraftables = new List<string>(new string[]
        {
            "I_DominionBanner",
            "I_SpiritCage",
            "I_DoomedProphetRing",
            "I_ChronoBauble",
            "I_TomeOfSecrets",
            "I_MadBoots",
            "I_Panacea",
            "I_RatIcon",
            "I_SettlersWreath",
            "I_spiritSeedcone",
            "I_toxVial",
            "I_BagOfBoundlessWealth",
            "I_ExquisiteMask",
            "I_RuinousBlade",
            "I_HoodOfShadows"
        });

        public readonly List<string> DeepOneSouls = new List<string>(new string[]
        {
            "Physician",
            "Mediator",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage"
        });

        public readonly List<string> DeepOneCraftables = new List<string>(new string[]
        {
            "I_AbyssalTome",
            "I_DrownedMemento",
            "I_MesmerizingShell",
            "I_RitualistShard",
            "I_WaterloggedCharm",
            "I_StrangeMeat"
        });

        public CovensCore()
        {
            instance = this;
        }

        public override void receiveModConfigOpts_bool(string optName, bool value)
        {
            switch(optName)
            {
                case "Curseweaving":
                    Opt_Curseweaving = value;
                    break;
                case "Soulweaving":
                    Opt_SoulWeaving = value;
                    break;
                case "Limited Influence":
                    Opt_LimitedInfluenceGain = value;
                    break;
                case "Findable Artifact Items":
                    Opt_FindableArtifacts = value;
                    break;
                case "Unique Coven Tenets":
                    Opt_AdditionalTenets = value;
                    break;
                default:
                    break;
            }
        }

        public override void receiveModConfigOpts_int(string optName, int value)
        {
            switch(optName)
            {
                case "Influence Cap":
                    Opt_LimitedInfluenceGainMax = value;
                    break;
                default:
                    break;
            }
        }

        public override void beforeMapGen(Map map)
        {
            instance = this;

            GameInitialisation(map);
            BuildSoulItemGroups();
        }

        public override void afterLoading(Map map)
        {
            instance = this;

            GameInitialisation(map);
        }

        private void GameInitialisation(Map map)
        {
            _modIntegrationData = new Dictionary<string, ModIntegrationData>();
            GetModKernels(map.mods);
            RegisterComLibHooks(map);
            RegisterAgentAIs(map);
        }

        private void GetModKernels(List<ModKernel> kernels)
        {
            foreach(ModKernel kernel in kernels)
            {
                switch(kernel.GetType().Namespace)
                {
                    case "CommunityLib":
                        if (!(kernel is ModCore modCore))
                        {
                            throw new InvalidOperationException($"Kernel from the \"CommunityLib\" namespace is not of type \"CommunityLib.ModCore\". It's type is '{kernel.GetType().AssemblyQualifiedName}'");
                        }
                        comLib = modCore;
                        comLib.registerMagicType(typeof(T_MasteryCurseweaving));
                        break;
                    case "Wonderblunder_DeepOnes":
                        Instance.TryAddModIntegrationData("DeepOnesPlus", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: DeepOnesPlus is Enabled");

                        if (Instance.TryGetModIntegrationData("DeepOnesPlus", out ModIntegrationData intDataDOP))
                        {
                            if (intDataDOP.TypeDict.TryGetValue("Kernel", out Type kernelType))
                            {
                                intDataDOP.MethodInfoDict.Add("getAbyssalItem", kernelType.GetMethod("getItemFromAbyssalPool", new Type[] { typeof(Map), typeof(UA) }));
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast - InvalidOperatinException: '{typeof(ModIntegrationData).Name}' for Key \"DeepOnesPlus\" did not contain Key \"Kernel\" in TypeDict");
                            }
                        }
                        break;
                    case "LivingWilds":
                        Instance.TryAddModIntegrationData("LivingWilds", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: Living Wilds is Enabled");

                        if (Instance.TryGetModIntegrationData("LivingWilds", out ModIntegrationData intDataLW))
                        {
                            Type lycanthropyTraitType = intDataLW.Assembly.GetType("Living_Wilds.T_Nature_Lycanthropy", false);
                            if (lycanthropyTraitType != null)
                            {
                                intDataLW.MethodInfoDict.Add("T_Lycanthropy.InfectPerson", lycanthropyTraitType.GetMethod("infectPerson", new Type[] { typeof(Person), typeof(bool), typeof(bool) }));
                                intDataLW.MethodInfoDict.Add("T_Lycanthropy.IsWerewolf", lycanthropyTraitType.GetMethod("isWerewolf", new Type[] { typeof(Person) }));
                            }
                            else
                            {
                                Console.WriteLine("CovenExpansionRecast: Failed to get Lycanthropy trait Type from Living Wilds (Living_Wilds.T_Nature_Lycanthropy)");
                            }
                        }
                        break;
                    case "Orcs_Plus":
                        Instance.TryAddModIntegrationData("OrcsPlus", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: OrcsPlus is Enabled");

                        if (Instance.TryGetModIntegrationData("OrcsPlus", out ModIntegrationData intDataOP))
                        {
                            Type orcCultureType = intDataOP.Assembly.GetType("Orcs_Plus.HolyOrder_Orcs", false);
                            if (orcCultureType != null)
                            {
                                intDataOP.TypeDict.Add("OrcCulture", orcCultureType);
                            }
                            else
                            {
                                Console.WriteLine("CovenExpansionRecast: Failed to get orc culture Type from OrcsPlus (Orcs_Plus.HolyOrder_Orcs)");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void RegisterComLibHooks(Map map)
        {
            ComLib.RegisterHooks(new ComLibHooks(map));
        }

        private void RegisterAgentAIs(Map map)
        {
            new UniversalAgentAIs(map);
        }

        private void BuildSoulItemGroups()
        {
            // Builds all possible combinations of Pairs of SimpleSouls, exlcuding those where both values of the pair are the same.
            for (int i = 0; i < SingleSouls.Count; i++)
            {
                for (int j = i + 1; j < SingleSouls.Count; j++)
                {
                    DualSouls.Add(new Tuple<string, string>(SingleSouls[i], SingleSouls[j]));
                }
            }

            // Shuffle craftables collection, ensuring that the soul-craftable pairs are unique each game.
            SingleCraftables.Shuffle();
            DualCraftables.Shuffle();
            DeepOneCraftables.Shuffle();
        }

        public override void onTurnStart(Map map)
        {
            foreach (House house in map.houses)
            {
                Curse_Toad curse = (Curse_Toad)house.curses.FirstOrDefault(c => c is Curse_Toad toad);
                if (curse != null)
                {
                    curse.Timer--;
                    if (curse.Timer  <= 0)
                    {
                        house.curses.Remove(curse);
                    }
                }
            }
        }

        public override double unitAgentAIAttack(Map map, UA ua, Unit other, List<ReasonMsg> reasons, double initialUtility)
        {
            double utility = initialUtility;
            T_Wanderer wanderer = (T_Wanderer)ua.person.traits.FirstOrDefault(t => t is T_Wanderer);
            if (wanderer != null)
            {
                double val = wanderer.AttackCount * 20.0;
                utility -= val;
                reasons?.Add(new ReasonMsg("Wanderlust", -val));
            }

            return utility;
        }

        public override double unitAgentAIBodyguard(Map map, UA ua, Unit other, List<ReasonMsg> reasons, double initialUtility)
        {
            double utility = initialUtility;
            T_Wanderer wanderer = (T_Wanderer)ua.person.traits.FirstOrDefault(t => t is T_Wanderer);
            if (wanderer != null)
            {
                double val = wanderer.GuardCount * 20.0;
                utility -= val;
                reasons?.Add(new ReasonMsg("Wanderlust", -val));
            }

            return utility;
        }

        public override double unitAgentAIDisrupt(Map map, UA ua, List<ReasonMsg> reasons, double initialUtility)
        {
            double utility = initialUtility;
            T_Wanderer wanderer = (T_Wanderer)ua.person.traits.FirstOrDefault(t => t is T_Wanderer);
            if (wanderer != null)
            {
                double val = wanderer.DisruptCount * 20.0;
                utility -= val;
                reasons?.Add(new ReasonMsg("Wanderlust", -val));
            }

            return utility;
        }

        private bool TryAddModIntegrationData(string name, ModIntegrationData intData)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"CovenExpansionRecast: Argument Exception - '{nameof(name)}' is null or empty.");
                return false;
            }

            if (_modIntegrationData == null)
            {
                Console.WriteLine($"CovenExpansionRecast: Operation Exception - '{nameof(_modIntegrationData)}` is null");
                return false;
            }

            if (_modIntegrationData.ContainsKey(name))
            {
                Console.WriteLine($"CovenExpansionRecast: DuplicateKeyException - '{nameof(_modIntegrationData)}` already contains Key {name}");
                return false;
            }

            _modIntegrationData.Add(name, intData);
            return true;
        }

        public bool TryGetModIntegrationData(string name, out ModIntegrationData intData)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"CovenExpansionRecast: Argument Exception - '{nameof(name)}' is null or empty.");
                intData = null;
                return false;
            }

            if (_modIntegrationData == null)
            {
                Console.WriteLine($"CovenExpansionRecast: Operation Exception - '{nameof(_modIntegrationData)}` is null");
                intData = null;
                return false;
            }

            return _modIntegrationData.TryGetValue(name, out intData);
        }

        public string GetSoulcraftingItemID(string soulTypeA, string soulTypeB = "Nothing")
        {
            if (soulTypeA != "Alienist" && !SingleSouls.Contains(soulTypeA))
            {
                return string.Empty;
            }

            int index;
            if (soulTypeB == "Nothing")
            {
                index = SingleSouls.IndexOf(soulTypeB);
                if (index != -1)
                {
                    return SingleCraftables[index];
                }
            }

            if (soulTypeB != "Alienist" && !SingleSouls.Contains(soulTypeB))
            {
                return string.Empty;
            }

            if  (soulTypeA == "Alienist" || soulTypeB == "Alienist")
            {
                string otherSoulType;
                if (soulTypeA == "Alienist")
                {
                    otherSoulType = soulTypeB;
                }
                else
                {
                    otherSoulType = soulTypeA;
                }

                index = DeepOneSouls.IndexOf(otherSoulType);
                if (index < 0 || index >= DeepOneSouls.Count)
                {
                    return string.Empty;
                }

                return DeepOneCraftables[index];
            }

            Tuple<string, string> tuple = new Tuple<string, string>(soulTypeA, soulTypeB);
            index = DualSouls.IndexOf(tuple);
            if (index == -1)
            {
                tuple = new Tuple<string, string>(soulTypeB, soulTypeA);
                index = DualSouls.IndexOf(tuple);
            }

            if (index < 0 || index >= DualCraftables.Count)
            {
                return string.Empty;
            }

            return DualCraftables[index];
        }

        public Item GetSoulcraftingItem(Map map, string soulcraftingItemID, UA ua = null)
        {
            switch (soulcraftingItemID)
            {
                case "I_SkeletonKey":
                    return new I_SkeletonKey(map);
                case "I_BagOfPoverty":
                    return new I_BagOfPoverty(map);
                case "I_Deathstone":
                    return new I_Deathstone(map);
                case "I_DarkStone":
                    return new I_DarkStone(map);
                case "I_StudentsManual":
                    return new I_StudentsManual(map);
                case "I_PoisonedDagger":
                    return new I_PoisonedDagger(map);
                case "I_PortableSkeleton":
                    return new I_PortableSkeleton(map);
                case "I_ReliableShield":
                    return new I_ReliableShield(map);
                case "I_PotionOfHealing":
                    return  new I_PotionOfHealing(map);
                case "I_DominionBanner":
                    return new I_DominionBanner(map);
                case "I_SpiritCage":
                    if (ua == null)
                    {
                        return new I_SpiritCage(map);
                    }
                    I_SpiritCage cage = new I_SpiritCage(map);
                    UAE_Spirit spirit = new UAE_Spirit(ua.location, map.soc_dark, new Person(map.soc_dark, null), cage);
                    return cage;
                case "I_DoomedProphetRing":
                    return new I_DoomedProphetRing(map);
                case "I_ChronoBauble":
                    return  new I_ChronoBauble(map);
                case "I_TomeOfSecrets":
                    return new I_TomeOfSecrets(map);
                case "I_MadBoots":
                    return new I_MadBoots(map);
                case "I_Panacea":
                    return new I_Panacea(map);
                case "I_RatIcon":
                    return new I_RatIcon(map);
                case "I_SettlersWreath":
                    return  new I_SettlersWreath(map);
                case "I_spiritSeedcone":
                    return null;
                case "I_toxVial":
                    return null;
                case "I_BagOfBoundlessWealth":
                    return new I_BagOfBoundlessWealth(map);
                case "I_ExquisiteMask":
                    return new I_ExquisiteMask(map);
                case "I_RuinousBlade":
                    return new I_RuinousBlade(map);
                case "I_HoodOfShadows":
                    return new I_HoodOfShadows(map);
            }

            if (Instance.TryGetModIntegrationData("DeepOnesPlus", out ModIntegrationData intDataDOP))
            {
                /*"I_AbyssalTome",
            "I_DrownedMemento",
            "I_MesmerizingShell",
            "I_RitualistShard",
            "I_WaterloggedCharm",
            "I_StrangeMeat"*/
            }

            return null;
        }

        public Item GetSoulcraftingItem(Map map, string soulTypeA, string soulTypeB = "Nothing", UA ua = null)
        {
            string itemID = GetSoulcraftingItemID(soulTypeA, soulTypeB);
            if (!string.IsNullOrEmpty(itemID))
            {
                return GetSoulcraftingItem(map, itemID, ua);
            }

            return null;
        }
    }
}
