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

        public List<Mg_Rti_TransposeSoul> TranspositionRituals = new List<Mg_Rti_TransposeSoul>();

        public I_Soulstone(Map map)
            : base(map)
        {
            Location location = map.locations[0];
            challenges.Add(new Mg_Rti_TransposeSoul(location, this));
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
            switch (GetSoulType())
            {
                case "Werewolf":
                    return "Soulstone (Werewolf)";
                case "Mage":
                    return "Soulstone (Mage)";
                case "OrcSlayer":
                    return "Soulstone(Orc Slayer)";
                case "Mediateor":
                    return "Soulstone (Mediator)";
                case "Physician":
                    return "Soulstone (Physician)";
                case "Lightbringer":
                    return "Soulstone (Lightbringer)";
                case "Exorcist":
                    return "Soulstone (Exorcist)";
                case "Alienist":
                    return "Soulstone (Alienist)";
                case "Nothing":
                    return "Soulstone";
            }

            return "Soulstone";
        }

        public override string getShortDesc()
        {
            if (CapturedSoul != null)
            {
                return $"A gemstone carved into a magical cage. It contains the captured soul of {CapturedSoul.getName()}.";
            }

            return "A gemstone carved into a magical cage that can be used to capture and release souls.";
        }

        public override Sprite getIconFore()
        {
            if (CapturedSoul != null)
            {
                foreach (Trait trait in CapturedSoul.traits)
                {
                    if (trait.getName().Contains("Lycanthropy"))
                    {
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Werewolf.png");
                    }
                    
                    if (trait is T_MasteryGeomancy)
                    {
                        return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Mage.png");
                    }

                    if (trait is T_ChallengeBooster booster)
                    {
                        if (booster.target == Tags.ORC)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_OrcSlayer.png");
                        }

                        if (booster.target == Tags.DISCORD)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Mediator.png");
                        }

                        if (booster.target == Tags.DISEASE)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Doctor.png");
                        }

                        if (booster.target == Tags.SHADOW)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Lightbringer.png");
                        }

                        if (booster.target == Tags.UNDEAD)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Exorcist.png");
                        }

                        if (booster.target == Tags.DEEPONES)
                        {
                            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Alianist.png");
                        }
                    }
                }
            }

            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Inactive.png");
        }

        public string GetSoulType()
        {
            if (CapturedSoul != null)
            {
                foreach (Trait trait in CapturedSoul.traits)
                {
                    if (trait.getName().Contains("Lycanthropy"))
                    {
                        return "Werewolf";
                    }

                    if (trait is T_MasteryGeomancy)
                    {
                        return "Mage";
                    }

                    if (trait is T_ChallengeBooster booster)
                    {
                        if (booster.target == Tags.ORC)
                        {
                            return "OrcSlayer";
                        }

                        if (booster.target == Tags.DISCORD)
                        {
                            return "Mediator";
                        }

                        if (booster.target == Tags.DISEASE)
                        {
                            return "Physician";
                        }

                        if (booster.target == Tags.SHADOW)
                        {
                            return "Lightbringer";
                        }

                        if (booster.target == Tags.UNDEAD)
                        {
                            return "Exorcist";
                        }

                        if (booster.target == Tags.DEEPONES)
                        {
                            return "Alienist";
                        }
                    }
                }
            }

            return "Nothing";
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
            Dictionary<I_Soulstone, Mg_Rti_TransposeSoul> existingTranspositionRituals = new Dictionary<I_Soulstone, Mg_Rti_TransposeSoul>();

            if (GetSoulType() != "Nothing")
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

            TranspositionRituals.Clear();
            if (GetSoulType() != "Nothing")
            {
                for (int i = 0; i < ua.person.items.Length; i++)
                {
                    if (ua.person.items[i] is I_Soulstone soulstone)
                    {
                        if (soulstone != this && soulstone.GetSoulType() != "Nothing" && soulstone.GetSoulType() != GetSoulType())
                        {
                            if (CovensCore.Instance.GetSoulcraftingItemID(GetSoulType(), soulstone.GetSoulType()) != string.Empty)
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
                }
            }

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
