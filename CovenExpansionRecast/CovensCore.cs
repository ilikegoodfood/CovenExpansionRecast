using Assets.Code;
using Assets.Code.Modding;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CovenExpansionRecast
{
    public class CovensCore : ModKernel
    {
        private static CovensCore instance;

        public static CovensCore Instance => instance;

        private static ModCore comLib;

        public static ModCore ComLib => comLib;

        private Dictionary<string, ModIntegrationData> _modIntegrationData;

        private HashSet<Type> _tenetDistributionTypeBalcklist = new HashSet<Type> { typeof(H_W_HumanSacrifice), typeof(H_Healers), typeof(H_MusicOfTheOuterSpheres), typeof(H_Doomsayers), typeof(H_IntransigentFaith) };

        public HashSet<HolyOrder_Witches> TenetDistributionCovensVisited = new HashSet<HolyOrder_Witches>();

        private Map _map;

        public P_OpenMind OpenMindPower;

        public List<Tuple<UA, Ritual>> RitualRemovalData;

        public static bool Opt_Curseweaving = true;

        public static bool Opt_LimitedInfluenceGain = true;

        public static int Opt_LimitedInfluenceGainCap = 2;

        public static bool Opt_FindableArtifacts = true;

        public static bool Opt_AdditionalTenets = true;

        public static bool Opt_RandomizeInitialTenetAlignments = true;

        public static int Opt_SoulLabel_None = 1;

        public static int Opt_SoulLabel_Cooperation = 1;

        public static int Opt_SoulLabel_DeepOnes = 1;

        public static int Opt_SoulLabel_Disease = 1;

        public static int Opt_SoulLabel_Madness = 1;

        public static int Opt_SoulLabel_Orc = 1;

        public static int Opt_SoulLabel_Shadow = 1;

        public static int Opt_SoulLabel_Undead = 1;

        // Soul Craftable Pairs //
        // The DUalSouls list is built dynamically in beforeMapGen
        // The lists of craftables are randomiised in beforeMapGen
        // Craftables lists must be of at least the length of the corresponding souls lists.
        public readonly List<SoulType> SingleSouls = new List<SoulType>
        {
            SoulType.Physician,
            SoulType.Mediator,
            SoulType.Exorcist,
            SoulType.Lightbringer,
            SoulType.OrcSlayer,
            SoulType.Mage,
            SoulType.Alienist
        };

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
        public readonly List<Tuple<SoulType, SoulType>> DualSouls = new List<Tuple<SoulType, SoulType>>();

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
            "I_SpiritSeed",
            "I_ToxicVial",
            "I_BagOfBoundlessWealth",
            "I_ExquisiteMask",
            "I_RuinousBlade",
            "I_HoodOfShadows",
            "I_BootsOfWealth",
            "I_BootsOfXP",
            "I_Elfstone",
            "I_PageFromTome",
            "I_SacrificialDagger",
            "I_WarAxe"
        });

        public readonly List<SoulType> DeepOneSouls = new List<SoulType>
        {
            SoulType.Physician,
            SoulType.Mediator,
            SoulType.Exorcist,
            SoulType.Lightbringer,
            SoulType.OrcSlayer,
            SoulType.Mage,
            SoulType.Alienist
        };

        public readonly List<string> DeepOneCraftables = new List<string>(new string[]
        {
            "I_AbyssalTome",
            "I_DrownedMemento",
            "I_MesmerizingShell",
            "I_RitualistShard",
            "I_WaterloggedCharm",
            "I_StrangeMeat",
            "I_AbyssalTome" // DUPLICATE
        });

        public readonly List<String> RecipeList = new List<String>();

        private HashSet<Type> _psychogenicIllnessPropertyBlacklist = new HashSet<Type>();

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
                case "Limited Influence":
                    Opt_LimitedInfluenceGain = value;
                    break;
                case "Findable Artifact Items":
                    Opt_FindableArtifacts = value;
                    break;
                case "Unique Coven Tenets":
                    Opt_AdditionalTenets = value;
                    break;
                case "Random Coven Initial Tenet Alignment":
                    Opt_RandomizeInitialTenetAlignments = value;
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
                    Opt_LimitedInfluenceGainCap = value;
                    break;
                case "Soul Label Style : None":
                    Opt_SoulLabel_None = value;
                    break;
                case "Soul Label Style : Co-Operation":
                    Opt_SoulLabel_Cooperation = value;
                    break;
                case "Soul Label Style : Deep Ones":
                    Opt_SoulLabel_DeepOnes = value;
                    break;
                case "Soul Label Style : Disease":
                    Opt_SoulLabel_Disease = value;
                    break;
                case "Soul Label Style : Madness":
                    Opt_SoulLabel_Madness = value;
                    break;
                case "Soul Label Style : Orc":
                    Opt_SoulLabel_Orc = value;
                    break;
                case "Soul Label Style : Shadow":
                    Opt_SoulLabel_Shadow = value;
                    break;
                case "Soul Label Style : Undead":
                    Opt_SoulLabel_Undead = value;
                    break;
                default:
                    break;
            }
        }

        public override void onStartGamePresssed(Map map, List<God> gods)
        {
            _modIntegrationData?.Clear();
            DualSouls?.Clear();
            RecipeList?.Clear();
        }

        public override void beforeMapGen(Map map)
        {
            instance = this;
            _map = map;

            GameInitialisation(map);
            OpenMindPower = new P_OpenMind(map);
            BuildSoulItemGroups();
            BuildSoulItemRecipeList(map);
        }

        public override void afterLoading(Map map)
        {
            instance = this;
            _map = map;

            GameInitialisation(map);
            UpdateSave(map);
        }

        private void GameInitialisation(Map map)
        {
            _modIntegrationData = new Dictionary<string, ModIntegrationData>();
            _psychogenicIllnessPropertyBlacklist = new HashSet<Type>();
            RitualRemovalData = new List<Tuple<UA, Ritual>>();
            GetModKernels(map.mods);
            RegisterComLibHooks(map);
            RegisterAgentAIs(map);

            BlacklistPropertyForPsychogenicIllness(typeof(Pr_PoliticalInstability));
        }

        private void UpdateSave(Map map)
        {
            if (RitualRemovalData == null)
            {
                RitualRemovalData = new List<Tuple<UA, Ritual>>();
            }

            foreach (Location loc in map.locations)
            {
                foreach (Property pr in loc.properties)
                {
                    if (!(pr is Pr_MagicPlague magicPlague))
                    {
                        continue;
                    }

                    foreach (Challenge challenge in magicPlague.Challenges)
                    {
                        if (challenge is Ch_TreatMagicDisease treat && treat.Plague == null)
                        {
                            treat.Plague = magicPlague;
                            continue;
                        }

                        if (challenge is Ch_BuyItem buy && buy.onSale is I_Soulstone soulstone)
                        {
                            Rti_ChooseSoulType chooseSoulRitual = null;
                            Mg_Rti_Curse_CollectMind collectMindRitual = null;
                            foreach (Ritual ritual in soulstone.challenges.ToList())
                            {
                                if (ritual is Rti_ChooseSoulType choose)
                                {
                                    chooseSoulRitual = choose;

                                }
                                else if (ritual is Mg_Rti_Curse_CollectMind collect)
                                {
                                    collectMindRitual = collect;
                                }
                            }

                            if (chooseSoulRitual == null)
                            {
                                soulstone.challenges.Add(new Rti_ChooseSoulType(buy.location, soulstone));
                            }
                            if (collectMindRitual == null)
                            {
                                soulstone.challenges.Add(new Mg_Rti_Curse_CollectMind(buy.location, soulstone));
                            }
                        }
                    }
                }
            }

            foreach (Person person in map.persons)
            {
                for (int i = 0; i < person.items.Length; i++)
                {
                    if (person.items[i] is I_Soulstone soulstone)
                    {
                        if (soulstone.Rti_TransposeSoul == null)
                        {
                            soulstone.Rti_TransposeSoul = (Mg_Rti_TransposeSoul)soulstone.challenges.FirstOrDefault(ch => ch is Mg_Rti_TransposeSoul transpose && transpose.SoulstoneB == null);
                        }

                        Rti_ChooseSoulType chooseSoulRitual = null;
                        Mg_Rti_Curse_CollectMind collectMindRitual = null;
                        foreach (Ritual ritual in soulstone.challenges.ToList())
                        {
                            if (ritual is Rti_ChooseSoulType choose)
                            {
                                chooseSoulRitual = choose;

                            }
                            else if (ritual is Mg_Rti_Curse_CollectMind collect)
                            {
                                collectMindRitual = collect;
                            }
                        }

                        if (chooseSoulRitual == null)
                        {
                            soulstone.challenges.Add(new Rti_ChooseSoulType(person.getLocation(), soulstone));
                        }
                        if (collectMindRitual == null)
                        {
                            soulstone.challenges.Add(new Mg_Rti_Curse_CollectMind(person.getLocation(), soulstone));
                        }
                    }
                }

                if (person.unit is UAEN_Pigeon pigeon && pigeon.task.GetType() == typeof(Task_GoToUnit))
                {
                    if (pigeon.returning)
                    {
                        pigeon.task = new Task_PigeonCarryToUnit(pigeon, pigeon.Owner);
                    }
                    else
                    {
                        pigeon.task = new Task_PigeonCarryToUnit(pigeon, pigeon.Target);
                    }
                }
            }

            if (TenetDistributionCovensVisited == null)
            {
                TenetDistributionCovensVisited = new HashSet<HolyOrder_Witches>();
                foreach (SocialGroup sg in map.socialGroups)
                {
                    if (!(sg is HolyOrder_Witches witches) || TenetDistributionCovensVisited.Contains(witches))
                    {
                        continue;
                    }

                    TenetDistributionCovensVisited.Add(witches);
                }
            }
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

                            Type abyssalTomeType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_AbyssalTome", false);
                            if (abyssalTomeType != null)
                            {
                                intDataDOP.TypeDict.Add("AbyssalTome", abyssalTomeType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Abyssal Tome item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_AbyssalTome`)");
                            }

                            Type momentoType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_DrownedMemento", false);
                            if (momentoType != null)
                            {
                                intDataDOP.TypeDict.Add("DrownedMomento", momentoType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Drowned Momento item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_DrownedMemento`)");
                            }

                            Type shellType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_MesmerizingShell", false);
                            if (shellType != null)
                            {
                                intDataDOP.TypeDict.Add("MesmerizingShell", shellType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Mesmerizing Shell item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_MesmerizingShell`)");
                            }

                            Type ritualShardType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_RitualistShard", false);
                            if (ritualShardType != null)
                            {
                                intDataDOP.TypeDict.Add("RitualistShard", ritualShardType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Ritualist Shard item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_RitualistShard`)");
                            }

                            Type strangeMeatType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_StrangeMeat", false);
                            if (strangeMeatType != null)
                            {
                                intDataDOP.TypeDict.Add("StrangeMeat", strangeMeatType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Strange Meat item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_StrangeMeat`)");
                            }

                            Type charmType = intDataDOP.Assembly.GetType("Wonderblunder_DeepOnes.I_WaterloggedCharm", false);
                            if (charmType != null)
                            {
                                intDataDOP.TypeDict.Add("WaterloggedCharm", charmType);
                            }
                            else
                            {
                                Console.WriteLine($"CovenExpansionRecast: Failed to get Waterlogged Charm item type from DeepOnes Plus (`Wonderblunder_DeepOnes.I_WaterloggedCharm`)");
                            }
                        }
                        break;
                    case "SOFGKeeperItemRebalanceAndAdditions":
                        Instance.TryAddModIntegrationData("KeeperItemMod", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: Keeper's Item Rebalance is Enabled");
                        break;
                    case "LivingSocieties":
                        Instance.TryAddModIntegrationData("LivingSocieties", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: Living Societies is Enabled");
                        break;
                    case "LivingWilds":
                        Instance.TryAddModIntegrationData("LivingWilds", new ModIntegrationData(kernel));
                        Console.WriteLine($"CovenExpansionRecast: Living Wilds is Enabled");

                        if (Instance.TryGetModIntegrationData("LivingWilds", out ModIntegrationData intDataLW))
                        {
                            Type lycanthropyTraitType = intDataLW.Assembly.GetType("LivingWilds.T_Nature_Lycanthropy", false);
                            if (lycanthropyTraitType != null)
                            {
                                intDataLW.TypeDict.Add("LycanthropyTrait", lycanthropyTraitType);
                                intDataLW.MethodInfoDict.Add("T_Lycanthropy.InfectPerson", lycanthropyTraitType.GetMethod("infectPerson", new Type[] { typeof(Person), typeof(bool), typeof(bool) }));
                                intDataLW.MethodInfoDict.Add("T_Lycanthropy.IsWerewolf", lycanthropyTraitType.GetMethod("isWerewolf", new Type[] { typeof(Person) }));
                            }
                            else
                            {
                                Console.WriteLine("CovenExpansionRecast: Failed to get Lycanthropy trait Type from Living Wilds (Living_Wilds.T_Nature_Lycanthropy)");
                            }

                            Type lycanthropyCurseTraitType = intDataLW.Assembly.GetType("LivingWilds.T_Nature_WerewolfInfectiousness", false);
                            if (lycanthropyCurseTraitType != null)
                            {
                                intDataLW.TypeDict.Add("LycanthropyCurseTrait", lycanthropyCurseTraitType);
                            }
                            else
                            {
                                Console.WriteLine("CovenExpansionRecast: Failed to get Lycanthropy Curse trait Type from Living Wilds (Living_Wilds.T_Nature_WerewolfInfectiousness)");
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
                    DualSouls.Add(new Tuple<SoulType, SoulType>(SingleSouls[i], SingleSouls[j]));
                }
            }

            // Shuffle craftables collection, ensuring that the soul-craftable pairs are unique each game.
            SingleCraftables.Shuffle();
            DualCraftables.Shuffle();
            DeepOneCraftables.Shuffle();

            // In any case where there are more SoulTypes than craftables, the soul types are shuffled instead of the craftables.
        }

        public void BuildSoulItemRecipeList(Map map)
        {
            for (int i = 0; i < SingleSouls.Count && i < SingleCraftables.Count; i++)
            {
                RecipeList.Add($"{SoulTypeUtils.GetTitle(SingleSouls[i])} => {GetSoulcraftingItemName(SingleCraftables[i])}");
            }

            for (int i = 0; i < DualSouls.Count && i < DualCraftables.Count; i++)
            {
                RecipeList.Add($"{SoulTypeUtils.GetTitle(DualSouls[i].Item1)} + {SoulTypeUtils.GetTitle(DualSouls[i].Item2)} => {GetSoulcraftingItemName(DualCraftables[i])}");
            }

            if (Instance.TryGetModIntegrationData("DeepOnesPlus", out ModIntegrationData intDataDO))
            {
                RecipeList.Add($"{SoulTypeUtils.GetTitle(SoulType.DeepOneSpecialist)} => {GetSoulcraftingItemName(GetSoulcraftingItemID(SoulType.DeepOneSpecialist))}");

                for (int i = 0; i < DeepOneSouls.Count && i < DeepOneCraftables.Count; i++)
                {
                    RecipeList.Add($"{SoulTypeUtils.GetTitle(SoulType.DeepOneSpecialist)} + {SoulTypeUtils.GetTitle(DeepOneSouls[i])} => {GetSoulcraftingItemName(DeepOneCraftables[i])}");
                }
            }

            if (Instance.TryGetModIntegrationData("LivingWilds", out _))
            {
                RecipeList.Add($"{SoulTypeUtils.GetTitle(SoulType.Werewolf)} => {GetSoulcraftingItemName(GetSoulcraftingItemID(SoulType.Werewolf))}");

                if (intDataDO != null)
                {
                    RecipeList.Add($"{SoulTypeUtils.GetTitle(SoulType.Werewolf)} + {SoulTypeUtils.GetTitle(SoulType.DeepOneSpecialist)} => {GetSoulcraftingItemName(GetSoulcraftingItemID(SoulType.Werewolf, SoulType.DeepOneSpecialist))}");
                }

                for (int i = 0; i < SingleSouls.Count; i++)
                {
                    RecipeList.Add($"{SoulTypeUtils.GetTitle(SoulType.Werewolf)} + {SoulTypeUtils.GetTitle(SingleSouls[i])} => {GetSoulcraftingItemName(GetSoulcraftingItemID(SoulType.Werewolf, SingleSouls[i]))}");
                }
            }
        }

        public override void afterMapGenAfterHistorical(Map map)
        {
            if (!Opt_AdditionalTenets && !Opt_Curseweaving)
            {
                return;
            }

            foreach(SocialGroup sg in map.socialGroups)
            {
                if (sg is HolyOrder_Witches witches)
                {
                    if (Opt_AdditionalTenets)
                    {
                        witches.tenets.Add(new H_SharedKnowledge(witches));
                        witches.tenets.Add(new H_OutcastShelters(witches));
                        witches.tenets.Add(new H_Aviaries(witches));
                        witches.tenets.Add(new H_Curseweavers(witches));

                        H_Dogmantic dogmatic = (H_Dogmantic)witches.tenets.FirstOrDefault(t => t is H_Dogmantic);
                        if (dogmatic == null)
                        {
                            witches.tenets.Add(new H_Initiation(witches));
                        }
                        else
                        {
                            int index = witches.tenets.IndexOf(dogmatic);
                            witches.tenets[index] = new H_Initiation(witches);
                            witches.tenets.Add(dogmatic);
                        }
                    }

                    if (!TenetDistributionCovensVisited.Contains(witches))
                    {
                        EstablishInitialTenetSpread(witches);
                        TenetDistributionCovensVisited.Add(witches);
                    }
                }
            }
        }

        public void EstablishInitialTenetSpread(HolyOrder_Witches witches)
        {
            if (!Opt_RandomizeInitialTenetAlignments)
            {
                return;
            }

            List<HolyTenet> positiveTenets = new List<HolyTenet>();
            List<HolyTenet> negativeTenets = new List<HolyTenet>();

            foreach (HolyTenet tenet in witches.tenets)
            {
                if (tenet.structuralTenet() || IsTenetTypeBlacklistedForRandomisedInitialStatus(tenet))
                {
                    continue;
                }

                if (tenet.status < tenet.getMaxPositiveInfluence())
                {
                    positiveTenets.Add(tenet);
                }
                if (tenet.status > tenet.getMaxNegativeInfluence())
                {
                    negativeTenets.Add(tenet);
                }
            }

            int tenetCount = 1 + Eleven.random.Next(2);
            for (int i = 0; i < tenetCount; i++)
            {
                if (positiveTenets.Count > 0)
                {
                    HolyTenet tenet = positiveTenets[Eleven.random.Next(positiveTenets.Count)];
                    tenet.status++;
                    positiveTenets.Remove(tenet);
                    negativeTenets.Remove(tenet);
                }
                

                if (negativeTenets.Count > 0)
                {
                    HolyTenet tenet = negativeTenets[Eleven.random.Next(negativeTenets.Count)];
                    tenet.status--;
                    positiveTenets.Remove(tenet);
                    negativeTenets.Remove(tenet);
                }
            }
        }

        public override void onTurnStart(Map map)
        {
            foreach (Unit unit in map.units)
            {
                if (!(unit is UA ua) || ua.isDead || ua.person == null)
                {
                    continue;
                }

                if (ua.person.traits.Any(t => t is T_ArcaneKnowledge || t is T_MasteryCurseweaving))
                {
                    if (!ua.rituals.Any(rt => rt is Rt_StudyCurseweaving))
                    {
                        ua.rituals.Add(new Rt_StudyCurseweaving(ua.location, ua));
                    }
                }
            }

            foreach (Tuple<UA, Ritual> tuple in RitualRemovalData)
            {
                tuple.Item1.rituals.Remove(tuple.Item2);
            }
            RitualRemovalData.Clear();

            if (map.burnInComplete)
            {
                foreach (SocialGroup sg in map.socialGroups)
                {
                    if (!(sg is HolyOrder_Witches witches) || TenetDistributionCovensVisited.Contains(witches))
                    {
                        continue;
                    }

                    EstablishInitialTenetSpread(witches);
                    TenetDistributionCovensVisited.Add(witches);
                }
            }
        }

        public override Item optionToReturnItemFromRandomPool(int itemRarity)
        {
            if (!Opt_FindableArtifacts || Eleven.random.NextDouble() >= 0.35)
            {
                return null;
            }

            if (itemRarity == Item.LEVEL_RARE && (!TryGetModIntegrationData("KeeperItemMod", out _) || !TryGetModIntegrationData("LivingSocieties", out _)))
            {
                switch (Eleven.random.Next(10))
                {
                    case 0:
                        return new I_SpiritSeed(_map);
                    case 1:
                        return new I_TomeOfSecrets(_map);
                    case 2:
                        return new I_MadBoots(_map);
                    case 3:
                        return new I_ToxicVial(_map);
                    case 4:
                        return new I_DoomedProphetRing(_map);
                    case 5:
                        return new I_ChronoBauble(_map);
                    case 6:
                        return new I_DominionBanner(_map);
                    case 7:
                        return new I_RatIcon(_map);
                    case 8:
                        return new I_Panacea(_map);
                    case 9:
                        return new I_SettlersWreath(_map);
                }
            }

            if (itemRarity == Item.LEVEL_RARE)
            {
                switch (Eleven.random.Next(5))
                {
                    case 0:
                        return new I_SpiritSeed(_map);
                    case 1:
                        return new I_TomeOfSecrets(_map);
                    case 2:
                        return new I_MadBoots(_map);
                    case 3:
                        return new I_ToxicVial(_map);
                    case 4:
                        return new I_DoomedProphetRing(_map);
                }
            }
            else if (itemRarity == Item.LEVEL_ARTEFACT)
            {
                switch (Eleven.random.Next(5))
                {
                    case 0:
                        return new I_ChronoBauble(_map);
                    case 1:
                        return new I_DominionBanner(_map);
                    case 2:
                        return new I_RatIcon(_map);
                    case 3:
                        return new I_Panacea(_map);
                    case 4:
                        return new I_SettlersWreath(_map);
                }
            }

            return null;
        }

        public override void populatingChallenges(Location location, List<Challenge> standardChallenges)
        {
            if (location.settlement == null)
            {
                return;
            }

            Sub_Catacombs catacombs = null;
            Sub_Temple temple = null;
            Sub_WitchCoven coven = null;
            if (location.settlement is Set_MinorOther)
            {
                foreach (Subsettlement sub in location.settlement.subs)
                {
                    if (sub is Sub_WitchCoven cov)
                    {
                        coven = cov;
                        break;
                    }
                    else if (temple == null && sub is Sub_Temple temp && temp.order is HolyOrder_Witches)
                    {
                        temple = temp;
                        continue;
                    }
                }
            }
            else if (location.settlement is SettlementHuman)
            {
                foreach (Subsettlement sub in location.settlement.subs)
                {
                    if (catacombs == null && sub is Sub_Catacombs cata)
                    {
                        catacombs = cata;
                        continue;
                    }
                    else if (temple == null && sub is Sub_Temple temp && temp.order is HolyOrder_Witches)
                    {
                        temple = temp;
                        continue;
                    }

                    if (catacombs != null && temple != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                return;
            }

            Ch_ExhumeGrave exhume = (Ch_ExhumeGrave)location.settlement.customChallenges.FirstOrDefault(ch => ch is Ch_ExhumeGrave);
            Pr_RobbedGraves robbed = (Pr_RobbedGraves)location.properties.FirstOrDefault(pr => pr is Pr_RobbedGraves);
            if (catacombs != null)
            {
                if (exhume == null)
                {
                    exhume = new Ch_ExhumeGrave(location);
                    location.settlement.customChallenges.Add(exhume);
                }

                if (robbed == null)
                {
                    robbed = new Pr_RobbedGraves(location);
                    robbed.charge = 0.0;
                    location.properties.Add(robbed);
                }
            }
            else
            {
                if (exhume != null)
                {
                    location.settlement.customChallenges.Remove(exhume);
                }

                if (robbed != null)
                {
                    location.properties.Remove(robbed);
                }
            }

            Ch_RecruitMinion recruitPigeon = (Ch_RecruitMinion)location.settlement.customChallenges.FirstOrDefault(ch => ch is Ch_RecruitMinion recruit && recruit.exemplar is M_Pigeon);
            Ch_BuySoulstone buySoulstone = (Ch_BuySoulstone)location.settlement.customChallenges.FirstOrDefault(ch => ch is Ch_BuySoulstone);
            Ch_BuyCraftList buyCraftList = (Ch_BuyCraftList)location.settlement.customChallenges.FirstOrDefault(c => c is Ch_BuyCraftList);
            if (temple != null || coven != null)
            {
                Subsettlement pigeonReqInf = (Subsettlement)temple ?? (Subsettlement)coven;
                if (recruitPigeon == null)
                {
                    recruitPigeon = new Ch_RecruitMinion(location, new M_Pigeon(location.map), -1, pigeonReqInf);
                    location.settlement.customChallenges.Add(recruitPigeon);
                }
                else
                {
                    recruitPigeon.reqInf = pigeonReqInf;
                }

                if (buySoulstone == null)
                {
                    buySoulstone = new Ch_BuySoulstone(location);
                    location.settlement.customChallenges.Add(buySoulstone);
                }

                if (buyCraftList == null)
                {
                    buyCraftList = new Ch_BuyCraftList(location);
                    location.settlement.customChallenges.Add(buyCraftList);
                }
            }
            else
            {
                if (recruitPigeon != null)
                {
                    location.settlement.customChallenges.Remove(recruitPigeon);
                }

                if (buySoulstone != null)
                {
                    location.settlement.customChallenges.Remove(buySoulstone);
                }

                if (buyCraftList != null)
                {
                    location.settlement.customChallenges.Remove(buyCraftList);
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

        public override void onChallengeComplete(Challenge challenge, UA ua, Task_PerformChallenge task_PerformChallenge)
        {
            if (ua.person.traits.Any(t => t is T_ArcaneKnowledge))
            {
                if (!ua.rituals.Any(rt => rt is Rt_StudyCurseweaving))
                {
                    ua.rituals.Add(new Rt_StudyCurseweaving(ua.location, ua));
                }
            }
        }

        public override int adjustHolyInfluenceDark(HolyOrder order, int inf, List<ReasonMsg> msgs)
        {
            if (!Opt_LimitedInfluenceGain || !(order is HolyOrder_Witches coven))
            {
                return 0;
            }

            if (inf > coven.nWorshippers  - coven.nAcolytes + Opt_LimitedInfluenceGainCap)
            {
                int result = coven.nWorshippers - coven.nAcolytes - inf + Opt_LimitedInfluenceGainCap;
                if (CommunityLib.ModCore.Get().checkIsProphetPlayerAligned(coven))
                {
                    result += 4;
                }

                if (result < 0)
                {
                    msgs?.Add(new ReasonMsg("Small Religion", result));
                    return result;
                }
            }

            return 0;
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

        public string GetSoulcraftingItemID(SoulType soulTypeA, SoulType soulTypeB = SoulType.Nothing, bool checkReverseRecipe = true)
        {
            if (soulTypeA != SoulType.DeepOneSpecialist && !SingleSouls.Contains(soulTypeA))
            {
                return string.Empty;
            }

            if (soulTypeA == soulTypeB)
            {
                return string.Empty;
            }

            int index;
            if (soulTypeB == SoulType.Nothing)
            {
                if (soulTypeA == SoulType.DeepOneSpecialist)
                {
                    return "I_Soulstone";
                }

                if (soulTypeA == SoulType.Werewolf)
                {
                    return "I_ConcealedDagger";
                }

                index = SingleSouls.IndexOf(soulTypeA);
                if (index < 0 || index >= SingleCraftables.Count)
                {
                    return string.Empty;
                }
                return SingleCraftables[index];
            }

            if (soulTypeB != SoulType.DeepOneSpecialist && soulTypeB != SoulType.Werewolf && !SingleSouls.Contains(soulTypeB))
            {
                return string.Empty;
            }

            if (soulTypeA == SoulType.DeepOneSpecialist || (checkReverseRecipe && soulTypeB == SoulType.DeepOneSpecialist))
            {
                SoulType otherSoulType;
                if (soulTypeA == SoulType.DeepOneSpecialist)
                {
                    otherSoulType = soulTypeB;
                }
                else
                {
                    otherSoulType = soulTypeA;
                }

                if (otherSoulType == SoulType.Werewolf)
                {
                    return "I_ManticoreTrophy";
                }

                index = DeepOneSouls.IndexOf(otherSoulType);
                if (index < 0 || index >= DeepOneCraftables.Count)
                {
                    return string.Empty;
                }

                return DeepOneCraftables[index];
            }

            if (soulTypeA == SoulType.Werewolf || (checkReverseRecipe && soulTypeB == SoulType.Werewolf))
            {
                return "I_WarAxe";
            }

            Tuple<SoulType, SoulType> tuple = new Tuple<SoulType, SoulType>(soulTypeA, soulTypeB);
            index = DualSouls.IndexOf(tuple);
            if (checkReverseRecipe && (index < 0 || index >= DualCraftables.Count))
            {
                tuple = new Tuple<SoulType, SoulType>(soulTypeB, soulTypeA);
                index = DualSouls.IndexOf(tuple);
            }

            if (index < 0 || index >= DualCraftables.Count)
            {
                return string.Empty;
            }

            return DualCraftables[index];
        }

        public Item GetSoulcraftingItem(Map map, string soulcraftingItemID, UA ua)
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
                case "I_SpiritSeed":
                    return new I_SpiritSeed(map);
                case "I_ToxicVial":
                    return new I_ToxicVial(map);
                case "I_BagOfBoundlessWealth":
                    return new I_BagOfBoundlessWealth(map);
                case "I_ExquisiteMask":
                    return new I_ExquisiteMask(map);
                case "I_RuinousBlade":
                    return new I_RuinousBlade(map);
                case "I_HoodOfShadows":
                    return new I_HoodOfShadows(map);
                case "I_BootsOfWealth":
                    return new I_BootsOfWealth(map);
                case "I_BootsOfXP":
                    return new I_BootsOfXP(map);
                case "I_Elfstone":
                    I_Elfstone elfstone = new I_Elfstone(map);
                    elfstone.corrupted = true;
                    Rti_CorruptElfstone corruptElfstone = (Rti_CorruptElfstone)elfstone.challenges.FirstOrDefault(c => c is Rti_CorruptElfstone);
                    if (corruptElfstone != null)
                    {
                        elfstone.challenges.Remove(corruptElfstone);
                    }
                    return elfstone;
                case "I_PageFromTome":
                    return new I_PageFromTome(map);
                case "I_SacrificialDagger":
                    return new I_SacrificialDagger(map);
                case "I_WarAxe":
                    return new I_WarAxe(map);
                case "I_ManticoreTrophy":
                    return new I_ManticoreTrophy(map);
                case "I_ConcealedDagger":
                    return new I_ConcealedDagger(map);
            }

            if (Instance.TryGetModIntegrationData("DeepOnesPlus", out ModIntegrationData intDataDOP))
            {
                switch(soulcraftingItemID)
                {
                    case "I_AbyssalTome":
                        if (ua != null && intDataDOP.TypeDict.TryGetValue("AbyssalTome", out Type abyssalTomeType))
                        {
                            return (Item)Activator.CreateInstance(abyssalTomeType, new object[] { map, ua });
                        }
                        return null;
                    case "I_DrownedMemento":
                        if (ua != null && intDataDOP.TypeDict.TryGetValue("DrownedMomento", out Type drownedMomentoType))
                        {
                            return (Item)Activator.CreateInstance(drownedMomentoType, new object[] { map, ua });
                        }
                        return null;
                    case "I_MesmerizingShell":
                        if (intDataDOP.TypeDict.TryGetValue("MesmerizingShell", out Type mesmirizingShellType))
                        {
                            return (Item)Activator.CreateInstance(mesmirizingShellType, new object[] { map });
                        }
                        return null;
                    case "I_RitualistShard":
                        if (ua != null && intDataDOP.TypeDict.TryGetValue("RitualistShard", out Type ritualistShardType))
                        {
                            return (Item)Activator.CreateInstance(ritualistShardType, new object[] { map, ua });
                        }
                        return null;
                    case "I_StrangeMeat":
                        if (ua != null && intDataDOP.TypeDict.TryGetValue("StrangeMeat", out Type strangeMeatType))
                        {
                            return (Item)Activator.CreateInstance(strangeMeatType, new object[] { map, ua });
                        }
                        return null;
                    case "I_WaterloggedCharm":
                        if (ua != null && intDataDOP.TypeDict.TryGetValue("WaterloggedCharm", out Type waterloggedCharmType))
                        {
                            return (Item)Activator.CreateInstance(waterloggedCharmType, new object[] { map, ua });
                        }
                        return null;
                    case "I_Soulstone":
                        return new I_Soulstone(map);
                }
            }

            return null;
        }

        public Item GetSoulcraftingItem(Map map, UA ua, SoulType soulTypeA, SoulType soulTypeB = SoulType.Nothing)
        {
            string itemID = GetSoulcraftingItemID(soulTypeA, soulTypeB);
            if (!string.IsNullOrEmpty(itemID))
            {
                return GetSoulcraftingItem(map, itemID, ua);
            }

            return null;
        }

        public string GetSoulcraftingItemName(string soulcraftingItemID)
        {
            switch (soulcraftingItemID)
            {
                case "I_SkeletonKey":
                    return "Skeleton Key";
                case "I_BagOfPoverty":
                    return "Jar of Poverty";
                case "I_Deathstone":
                    return "Deathstone";
                case "I_DarkStone":
                    return "Dark Stone";
                case "I_StudentsManual":
                    return "Student's Manual";
                case "I_PoisonedDagger":
                    return "Poisoned Dagger";
                case "I_PortableSkeleton":
                    return "Portable Skeleton";
                case "I_ReliableShield":
                    return "Reliable Shield";
                case "I_PotionOfHealing":
                    return "Potion of Healing";
                case "I_DominionBanner":
                    return "Banner of Barberous Dominion";
                case "I_SpiritCage":
                    return "Spirit Cage";
                case "I_DoomedProphetRing":
                    return "Doomed Prophet's Ring";
                case "I_ChronoBauble":
                    return "Chronobauble";
                case "I_TomeOfSecrets":
                    return $"Secrets of Life and Death";
                case "I_MadBoots":
                    return "Madcap Boots";
                case "I_Panacea":
                    return "The Panacea";
                case "I_RatIcon":
                    return "Razor Icon";
                case "I_SettlersWreath":
                    return "Wreath of Manifest";
                case "I_SpiritSeed":
                    return "Spirit Tree Seed";
                case "I_ToxicVial":
                    return "Phthisical Vial";
                case "I_BagOfBoundlessWealth":
                    return "Bag of Boundless Wealth";
                case "I_ExquisiteMask":
                    return "Exquisite Mask";
                case "I_RuinousBlade":
                    return "The Ruinous Blade";
                case "I_HoodOfShadows":
                    return "Hood of Shadows";
                case "I_BootsOfWealth":
                    return "Boots of Wealth";
                case "I_BootsOfXP":
                    return "Boots of the Scholar";
                case "I_Elfstone":
                    return "Corrupted Elfstone";
                case "I_PageFromTome":
                    return "Page from the Tome";
                case "I_SacrificialDagger":
                    return "Sacrificial Dagger";
                case "I_WarAxe":
                    return "War Axe";
                case "I_ManticoreTrophy":
                    return "Manticore Trophy";
                // DeepOnesPlus Itmes
                case "I_AbyssalTome":
                    return "Abyssal Tome";
                case "I_DrownedMemento":
                    return "Drowned Memento";
                case "I_MesmerizingShell":
                    return "Mesmerizing Shell";
                case "I_RitualistShard":
                    return "Ritualist Shard";
                case "I_StrangeMeat":
                    return "Strange Meat";
                case "I_WaterloggedCharm":
                    return "Waterlogged Charm";
            }

            return  string.Empty;
        }

        public string GetSoulcraftingItemName(SoulType soulTypeA, SoulType soulTypeB = SoulType.Nothing)
        {
            string itemID = GetSoulcraftingItemID(soulTypeA, soulTypeB);
            return GetSoulcraftingItemName(itemID);
        }

        public void BlacklistPropertyForPsychogenicIllness(Type propertyType)
        {
            _psychogenicIllnessPropertyBlacklist.Add(propertyType);
        }

        public void BlacklistPropertyForPsychogenicIllness(Property property)
        {
            BlacklistPropertyForPsychogenicIllness(property.GetType());
        }

        public bool IsBlacklistedForPsychogenicIllness(Type propertyType)
        {
            return _psychogenicIllnessPropertyBlacklist.Contains(propertyType);
        }

        public bool IsBlacklistedForPsychogenicIllness(Property property)
        {
            return IsBlacklistedForPsychogenicIllness(property.GetType());
        }

        public void BlacklistTenetTypeForRandomisedInitialStatus(Type type)
        {
            _tenetDistributionTypeBalcklist.Add(type);
        }

        public void BlacklistTenetTypeForRandomisedInitialStatus(HolyTenet tenet)
        {
            BlacklistTenetTypeForRandomisedInitialStatus(tenet.GetType());
        }

        public bool IsTenetTypeBlacklistedForRandomisedInitialStatus(Type type)
        {
            return _tenetDistributionTypeBalcklist.Contains(type);
        }

        public bool IsTenetTypeBlacklistedForRandomisedInitialStatus(HolyTenet tenet)
        {
            return IsTenetTypeBlacklistedForRandomisedInitialStatus(tenet.GetType());
        }

        public bool TryBlacklistPropertyTypeAsActionSourceForOpenMindPower(Type sourceType)
        {
            if (OpenMindPower == null)
            {
                return false;
            }

            if (OpenMindPower.PropertyTypeBlacklist.Contains(sourceType))
            {
                return false;
            }

            OpenMindPower.PropertyTypeBlacklist.Add(sourceType);
            return true;
        }

        public bool TryBlacklistPropertyTypeAsActionSourceForOpenMindPower(Property property)
        {
            return TryBlacklistPropertyTypeAsActionSourceForOpenMindPower(property.GetType());
        }

        public bool IsPropertyTypeBlacklistedForOpenMindPower(Type sourceType)
        {
            if (OpenMindPower == null)
            {
                return false;
            }

            return OpenMindPower.PropertyTypeBlacklist.Contains(sourceType);
        }

        public bool IsPropertyTypeBlacklistedForOpenMindPower(Property property)
        {
            return IsPropertyTypeBlacklistedForOpenMindPower(property.GetType());
        }
    }
}
