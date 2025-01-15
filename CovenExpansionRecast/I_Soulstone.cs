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
        Person CapturedSoul;

        public I_Soulstone(Map map)
            : base(map)
        {

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

        public string getSoulType()
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
                            return "Alianist";
                        }
                    }
                }
            }

            return "Nothing";
        }
    }
}
