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


        public readonly List<string> SimpleSoul = new List<string>(new string[]
        {
            "Physician",
            "Mediator",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage"
        });

        public readonly List<string> FirstSoul = new List<string>(new string[]
        {
            "Physician",
            "Physician",
            "Physician",
            "Physician",
            "Physician",
            "Mediator",
            "Mediator",
            "Mediator",
            "Mediator",
            "Exorcist",
            "Exorcist",
            "Exorcist",
            "Lightbringer",
            "Lightbringer",
            "Orc-slayer"
        });

        public readonly List<string> SecondSoul = new List<string>(new string[]
        {
            "Mediator",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage",
            "Lightbringer",
            "Orc-slayer",
            "Mage",
            "Orc-slayer",
            "Mage",
            "Mage"
        });

        public readonly List<string> DeepOneSecondSoul = new List<string>(new string[]
        {
            "Physician",
            "Mediator",
            "Exorcist",
            "Lightbringer",
            "Orc-slayer",
            "Mage"
        });

        public readonly List<string> BaseCraftables = new List<string>(new string[]
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

        public readonly List<string> SimpleCraftables = new List<string>(new string[]
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
                            throw new InvalidOperationException($"Kernel from the \"CommunityLib\" namespace is not of type \"CommunityLib.ModCore\". It's type is {kernel.GetType().AssemblyQualifiedName}");
                        }
                        comLib = modCore;
                        break;
                    case "Wonderblunder_DeepOnes":
                        _modIntegrationData.Add("DeepOnesPlus", new ModIntegrationData(kernel));
                        break;
                    case "LivingWilds":
                        _modIntegrationData.Add("LivingWilds", new ModIntegrationData(kernel));
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
