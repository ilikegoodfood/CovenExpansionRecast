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

            foreach (Trait trait in p.traits)
            {
                if (trait.getName().Contains("Lycanthropy"))
                {
                    return SoulType.Werewolf;
                }

                if (CovensCore.ComLib.checkKnowsMagic(p))
                {
                    return SoulType.Mage;
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

        public static string GetTitle(SoulType type)
        {
            switch (type)
            {
                case SoulType.Alienist:
                    return "Alienist";
                case SoulType.Exorcist:
                    return "Excorcist";
                case SoulType.Lightbringer:
                    return "Lightbringer";
                case SoulType.Mage:
                    return "Mage";
                case SoulType.Mediator:
                    return "Mediator";
                case SoulType.OrcSlayer:
                    return "Orc-slayer";
                case SoulType.DeepOneSpecialist:
                    return "Deep One Specialist";
                case SoulType.Physician:
                    return "Physician";
                case SoulType.Werewolf:
                    return "Werewolf";
                default:
                    return "Unremarkable";
            }
        }

        public static string GetTitle(Person p)
        {
            return GetTitle(GetSoulType(p));
        }
    }
}
