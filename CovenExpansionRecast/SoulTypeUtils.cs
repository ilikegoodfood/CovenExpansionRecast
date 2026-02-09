using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public static class SoulTypeUtils
    {
        public static SoulType GetSoulTypeFromTypeIndex(int soulTypeIndex)
        {
            if (Enum.IsDefined(typeof(SoulType), soulTypeIndex))
            {
                return (SoulType)soulTypeIndex;
            }

            return SoulType.Nothing;
        }

        public static SoulType GetSoulType(Person p)
        {
            if (p == null)
            {
                return SoulType.Nothing;
            }

            if (CovensCore.ComLib.checkKnowsMagic(p))
            {
                return SoulType.Mage;
            }

            foreach (Trait trait in p.traits)
            {
                if (trait.getName().Contains("Lycanthropy"))
                {
                    return SoulType.Werewolf;
                }

                if (trait is T_ChallengeBooster booster)
                {
                    if (booster.target == Tags.ORC)
                    {
                        return SoulType.OrcSlayer;
                    }

                    if (booster.target == Tags.DISCORD)
                    {
                        return SoulType.Mediator;
                    }

                    if (booster.target == Tags.DISEASE)
                    {
                        return SoulType.Physician;
                    }

                    if (booster.target == Tags.SHADOW)
                    {
                        return SoulType.Lightbringer;
                    }

                    if (booster.target == Tags.UNDEAD)
                    {
                        return SoulType.Exorcist;
                    }

                    if (booster.target == Tags.DEEPONES)
                    {
                        return SoulType.DeepOneSpecialist;
                    }
                }
            }

            return SoulType.Nothing;
        }

        public static List<SoulType> GetSoulTypes(Person p)
        {
            List<SoulType> types = new List<SoulType>();

            if (CovensCore.ComLib.checkKnowsMagic(p))
            {
                types.Add(SoulType.Mage);
            }

            foreach (Trait trait in p.traits)
            {
                if (trait.getName().Contains("Lycanthropy"))
                {
                    types.Add(SoulType.Werewolf);
                    continue;
                }

                if (trait is T_ChallengeBooster booster)
                {
                    if (booster.target == Tags.ORC)
                    {
                        types.Add(SoulType.OrcSlayer);
                    }
                    else if (booster.target == Tags.DISCORD)
                    {
                        types.Add(SoulType.Mediator);
                    }
                    else if (booster.target == Tags.DISEASE)
                    {
                        types.Add(SoulType.Physician);
                    }
                    else if (booster.target == Tags.SHADOW)
                    {
                        types.Add(SoulType.Lightbringer);
                    }
                    else if (booster.target == Tags.UNDEAD)
                    {
                        types.Add(SoulType.Exorcist);
                    }
                    else if (booster.target == Tags.DEEPONES)
                    {
                        types.Add(SoulType.DeepOneSpecialist);
                    }
                }
            }

            if (types.Count <= 1)
            {
                return types;
            }

            return types.Distinct().ToList();
        }

        public static string GetTitle(SoulType type)
        {
            switch (type)
            {
                case SoulType.Alienist:
                    switch(CovensCore.Opt_SoulLabel_Madness)
                    {
                        case 1:
                            return "Alienist";
                        default:
                            return "Madness Specialist";
                    }
                case SoulType.Exorcist:
                    switch (CovensCore.Opt_SoulLabel_Undead)
                    {
                        case 1:
                            return "Excorcist";
                        default:
                            return "Undead Specialist";
                    }
                case SoulType.Lightbringer:
                    switch (CovensCore.Opt_SoulLabel_Shadow)
                    {
                        case 1:
                            return "Lightbringer";
                        default:
                            return "Shadow Specialist";
                    }
                case SoulType.Mage:
                    return "Mage";
                case SoulType.Mediator:
                    switch (CovensCore.Opt_SoulLabel_Cooperation)
                    {
                        case 1:
                            return "Mediator";
                        default:
                            return "Co-Operation Specialist";
                    }
                case SoulType.OrcSlayer:
                    switch (CovensCore.Opt_SoulLabel_Orc)
                    {
                        case 1:
                            return "Orc-slayer";
                        default:
                            return "Orc Specialist";
                    }
                case SoulType.DeepOneSpecialist:
                    switch (CovensCore.Opt_SoulLabel_DeepOnes)
                    {
                        case 2:
                            return "Harpoonist";
                        case 1:
                            return "Pelagist";
                        default:
                            return "Deep One Specialist";
                    }
                case SoulType.Physician:
                    switch (CovensCore.Opt_SoulLabel_Disease)
                    {
                        case 1:
                            return "Physician";
                        default:
                            return "Disease Specialist";
                    }
                case SoulType.Werewolf:
                    return "Werewolf";
                default:
                    switch (CovensCore.Opt_SoulLabel_Cooperation)
                    {
                        case 1:
                            return "Unremarkable";
                        default:
                            return "Unspecialized";
                    }
            }
        }

        public static string GetTitle(Person p)
        {
            return GetTitle(GetSoulType(p));
        }
    }
}
