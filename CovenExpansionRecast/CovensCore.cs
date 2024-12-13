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
            "I_barbDominion",
            "I_cagedSpirit",
            "I_casVeil",
            "I_chronoBauble",
            "I_evilBook",
            "I_heroicBoot",
            "I_panacea",
            "I_RazRatKing",
            "I_settlersClaim",
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
                    default:
                        break;
                }
            }
        }

        private void RegisterComLibHooks(Map map)
        {
            ComLib.RegisterHooks(new ComLibHooks(map));
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
    }
}
