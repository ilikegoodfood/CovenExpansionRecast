using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_Soulstone : Item
    {
        public Person CapturedSoul;

        public Mg_Rti_TransposeSoul Rti_TransposeSoul;

        public List<Mg_Rti_TransposeSoul> TranspositionRituals = new List<Mg_Rti_TransposeSoul>();

        public I_Soulstone(Map map)
            : base(map)
        {
            Location location = map.locations[0];
            Rti_TransposeSoul = new Mg_Rti_TransposeSoul(location, this);
            challenges.Add(Rti_TransposeSoul);
            challenges.Add(new Rti_ReleaseSoul(location, this));
            challenges.Add(new Mg_Rti_RiteOfMasks(location, this));
            challenges.Add(new Mg_Rti_Curse_Toad(location, this));
            challenges.Add(new Mg_Rti_Curse_Flourishing(location, this));
            challenges.Add(new Mg_Rti_Curse_Mirror(location, this));
            
            if (CovensCore.Instance.TryGetModIntegrationData("LivingWilds", out _))
            {
                challenges.Add(new Mg_Rti_Curse_Lycanthropy(location, this));
            }
        }

        public override string getName()
        {
            if (CapturedSoul != null)
            {
                string name = $"Soulstone ({SoulTypeUtils.GetTitle(CapturedSoul)})";

                if (CapturedSoul.society == map.soc_dark)
                {
                    name += $" - Agent of The Dark";
                }
                else if (CapturedSoul.society is SG_AgentWanderers)
                {
                    name += $" - Monstrous Soul";
                }

                return name;
            }

            return "Soulstone";
        }

        public override string getShortDesc()
        {
            if (CapturedSoul != null)
            {
                string result = $"A gemstone carved into a magical cage. It contains the captured soul of {CapturedSoul.getName()} ({SoulTypeUtils.GetTitle(CapturedSoul)}).";

                if (CapturedSoul.society == map.soc_dark)
                {
                    result += $"They are an Agent of The Dark, and cannot be cursed.";
                }
                else if (CapturedSoul.society is SG_AgentWanderers)
                {
                    result += $"They have a Mounstrous Soul, and cannot be cursed.";
                }

                return result;
            }

            return "A gemstone carved into a magical cage that can be used to capture and release souls.";
        }

        public override Sprite getIconFore()
        {
            if (CapturedSoul != null)
            {
                switch(SoulTypeUtils.GetSoulType(CapturedSoul))
                {
                    case SoulType.Alienist:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Alienist.png");
                    case SoulType.DeepOneSpecialist:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Alienist.png");
                    case SoulType.Exorcist:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Exorcist.png");
                    case SoulType.Lightbringer:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Lightbringer.png");
                    case SoulType.Mediator:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Mediator.png");
                    case SoulType.Mage:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Mage.png");
                    case SoulType.OrcSlayer:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_OrcSlayer.png");
                    case SoulType.Physician:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Doctor.png");
                    case SoulType.Werewolf:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Werewolf.png");
                    default:
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone.png");
                }
            }

            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Inactive.png");
        }

        public SoulType GetSoulType()
        {
            if (CapturedSoul != null)
            {
                return SoulTypeUtils.GetSoulType(CapturedSoul);
            }

            return SoulType.Nothing;
        }

        public override Sprite getIconBack()
        {
            if (CapturedSoul != null)
            {
                if (CapturedSoul.shadow > 0.5)
                {
                    return CapturedSoul.getPortraitAlt();
                }
                return CapturedSoul.getPortrait();
            }

            return map.world.iconStore.standardBack;
        }

        public override List<Ritual> getRituals(UA ua)
        {
            if (!CovensCore.Opt_Curseweaving)
            {
                return new List<Ritual>();
            }

            Dictionary<I_Soulstone, Mg_Rti_TransposeSoul> existingTranspositionRituals = new Dictionary<I_Soulstone, Mg_Rti_TransposeSoul>();

            if (GetSoulType() != SoulType.Nothing)
            {
                foreach (Mg_Rti_TransposeSoul transpose in TranspositionRituals)
                {
                    I_Soulstone otherSoulstone;
                    if (transpose.SoulstoneA == this)
                    {
                        otherSoulstone = transpose.SoulstoneB;
                    }
                    else
                    {
                        otherSoulstone = transpose.SoulstoneA;
                    }

                    if (ua.person.items.Contains(otherSoulstone))
                    {
                        existingTranspositionRituals.Add(otherSoulstone, transpose);
                    }
                }
            }

            bool hasTranspositionScroll = false;
            TranspositionRituals.Clear();
            if (GetSoulType() != SoulType.Nothing)
            {
                for (int i = 0; i < ua.person.items.Length; i++)
                {
                    if (ua.person.items[i] is I_Soulstone soulstone)
                    {
                        if (soulstone != this && soulstone.GetSoulType() != SoulType.Nothing && soulstone.GetSoulType() != GetSoulType())
                        {
                            if (CovensCore.Instance.GetSoulcraftingItemID(GetSoulType(), soulstone.GetSoulType(), false) != string.Empty)
                            {
                                if (existingTranspositionRituals.TryGetValue(soulstone, out Mg_Rti_TransposeSoul transpose))
                                {
                                    TranspositionRituals.Add(transpose);
                                }
                                else
                                {
                                    transpose = new Mg_Rti_TransposeSoul(ua.location, this, soulstone);
                                    TranspositionRituals.Add(transpose);
                                }
                            }
                        }
                    }
                    else if (ua.person.items[i] is I_CraftList)
                    {
                        hasTranspositionScroll = true;
                    }
                }

                foreach (Mg_Rti_TransposeSoul transpose in TranspositionRituals)
                {
                    transpose.DisplayResult = hasTranspositionScroll;
                }
            }

            Rti_TransposeSoul.DisplayResult = hasTranspositionScroll;
            List<Ritual> result = new List<Ritual>(challenges);
            result.AddRange(TranspositionRituals);

            return result;
        }

        public override int getLevel()
        {
            return LEVEL_COMMON;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
