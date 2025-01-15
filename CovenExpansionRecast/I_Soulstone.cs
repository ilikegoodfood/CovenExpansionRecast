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

        public List<Rti_TransposeSoul> TranspositionRituals = new List<Rti_TransposeSoul>();

        public I_Soulstone(Map map)
            : base(map)
        {
            challenges.Add(new Rti_TransposeSoul(map.locations[0], this));
            challenges.Add(new Rti_ReleaseSoul(map.locations[0], this));
            challenges.Add(new Rti_RiteOfMasks(map.locations[0], this));
        }

        public override string getName()
        {
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
            Dictionary<I_Soulstone, Rti_TransposeSoul> existingTranspositionRituals = new Dictionary<I_Soulstone, Rti_TransposeSoul>();

            if (GetSoulType() != "Nothing")
            {
                foreach (Rti_TransposeSoul transpose in TranspositionRituals)
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
                List<I_Soulstone> otherSoulstones = new List<I_Soulstone>();
                foreach (Item item in ua.person.items)
                {
                    if (item != this && item is I_Soulstone soulstone && soulstone.CapturedSoul != null && soulstone.GetSoulType() != GetSoulType())
                    {
                        otherSoulstones.Add(soulstone);
                    }
                }

                foreach (I_Soulstone otherSoulstone in otherSoulstones)
                {
                    if (otherSoulstone.GetSoulType() == "Nothing" || otherSoulstone.GetSoulType() == GetSoulType())
                    {
                        continue;
                    }

                    if (CovensCore.Instance.GetSoulcraftingItemID(GetSoulType(), otherSoulstone.GetSoulType()) != "")
                    {
                        if (existingTranspositionRituals.TryGetValue(otherSoulstone, out Rti_TransposeSoul transpose))
                        {
                            TranspositionRituals.Add(transpose);
                        }
                        else
                        {
                            transpose = new Rti_TransposeSoul(ua.location, this, otherSoulstone);
                            TranspositionRituals.Add(transpose);
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
