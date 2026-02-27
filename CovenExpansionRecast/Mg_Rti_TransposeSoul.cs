using Assets.Code;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_TransposeSoul : Ritual
    {
        public I_Soulstone SoulstoneA;

        public I_Soulstone SoulstoneB;

        public bool DisplayResult;

        public SoulType lastTurnTypeA = SoulType.Nothing;

        public SoulType lastTurnTypeB = SoulType.Nothing;

        public Mg_Rti_TransposeSoul(Location location, I_Soulstone soulstoneA, I_Soulstone soulstoneB = null)
            : base(location)
        {
            SoulstoneA = soulstoneA;
            SoulstoneB = soulstoneB;
        }

        public override string getName()
        {
            if (SoulstoneA == null)
            {
                return $"Transpose Soul(s)";
            }

            string name;
            if (SoulstoneB == null)
            {
                name = $"Transpose Soul ({SoulTypeUtils.GetTitle(SoulstoneA.SoulType)})";
            }
            else
            {
                name = $"Transpose Souls ({SoulTypeUtils.GetTitle(SoulstoneA.SoulType)}, {SoulTypeUtils.GetTitle(SoulstoneB.SoulType)})";
            }

            if (DisplayResult)
            {
                name += $" into {CovensCore.Instance.GetSoulcraftingItemName(SoulstoneA.SoulType, SoulstoneB?.SoulType ?? SoulType.Nothing)}";
            }

            return name;
        }

        public override string getDesc()
        {
            if (SoulstoneB == null)
            {
                if (SoulstoneA.CapturedSoul != null)
                {
                    return $"Forges an item from the essence of {SoulstoneA.CapturedSoul.getName()}'s {SoulTypeUtils.GetTitle(SoulstoneA.SoulType)} soul. The rarity of the item produced will be low, with the highest quality items being composed of two souls with unique professions.";
                }

                return "Forges an item from the essence of a soul. The rarity of the item produced will be low, with the highest quality items being composed of two souls with unique professions.";
            }
            if (SoulstoneA.CapturedSoul != null && SoulstoneB.CapturedSoul != null)
            {
                return $"Forges the souls of {SoulstoneA.CapturedSoul.getName()}'s {SoulTypeUtils.GetTitle(SoulstoneA.SoulType)} soul and {SoulstoneB.CapturedSoul.getName()}'s {SoulTypeUtils.GetTitle(SoulstoneB.SoulType)} soul together, forging an item from their combined essence. The rarity of the item will depend on the profession of the souls, with the highest quality items being composed of two souls with unique professions.";
            }
            return $"Forges two souls together, forging an item from their combined essence. The rarity of the item will depend on the profession of the souls, with the highest quality items being composed of two souls with unique professions.";
        }

        public override string getCastFlavour()
        {
            if (SoulstoneB == null)
            {
                return "The fire crackles and pops as the soul coalesces, and a new relic is born from its essence.";
            }
            return "The fire crackles and pops as the two souls coalesce, and a new relic is born from their fused essences.";
        }

        public override string getRestriction()
        {
            if (SoulstoneB == null)
            {
                return "Requires a soulstones that contains a soul of a specific type. Must be performed at a witches coven.";
            }
            return "Requires two soulstones that contain souls of a specific type. Must be performed at a witches coven.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Transpose.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return SoulstoneA != null && SoulstoneA.SoulType != SoulType.Nothing && !string.IsNullOrEmpty(CovensCore.Instance.GetSoulcraftingItemID(SoulstoneA.SoulType, (SoulstoneB?.SoulType ?? SoulType.Nothing)));
        }

        public override bool validFor(UA ua)
        {
            if (ua.task != null && (ua.task is Task_PerformChallenge perform && perform.challenge == this) || (ua.task is Task_GoToPerformChallenge goPerform && goPerform.challenge == this))
            {
                SoulType soulType = SoulstoneA.SoulType;
                if (lastTurnTypeA != SoulType.Nothing && lastTurnTypeA != soulType)
                {
                    map.addMessage($"{ua.getName()} cancelled Transpose {(SoulstoneB == null ? "Soul" : "Souls")} because the type of the{(SoulstoneB == null ? " " : " first ")}soul being used changed from {SoulTypeUtils.GetTitle(lastTurnTypeA)} to {SoulTypeUtils.GetTitle(soulType)}, invalidating the recipe.", 0.0, false, ua.location.hex);
                    lastTurnTypeA = soulType;
                    return false;
                }

                if (SoulstoneB != null)
                {
                    soulType = SoulstoneB.SoulType;
                    if (lastTurnTypeB != SoulType.Nothing && lastTurnTypeB != soulType)
                    {
                        map.addMessage($"{ua.getName()} cancelled Transpose Souls because the type of the second soul being used changed from {SoulTypeUtils.GetTitle(lastTurnTypeB)} to {SoulTypeUtils.GetTitle(soulType)}, invalidating the recipe.", 0.0, false, ua.location.hex);
                        lastTurnTypeB = soulType;
                        return false;
                    }
                }
            }

            lastTurnTypeA = SoulstoneA.SoulType;
            lastTurnTypeB = SoulstoneB?.SoulType ?? SoulType.Nothing;

            if (ua.location == null || ua.location.settlement == null)
            {
                return false;
            }

            if (!(ua.location.settlement is Set_MinorOther) && !(ua.location.settlement is SettlementHuman))
            {
                return false;
            }

            return ua.location.settlement.subs.Any(sub => (sub is Sub_Temple temple && temple.order is HolyOrder_Witches) || sub is Sub_WitchCoven coven);
        }

        public override double getComplexity()
        {
            return 10.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatLore();
            if (val >= 1.0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override void complete(UA u)
        {
            Item item = CovensCore.Instance.GetSoulcraftingItem(map, u, SoulstoneA.SoulType, SoulstoneB?.SoulType ?? SoulType.Nothing);

            if (SoulstoneB != null)
            {
                if (u.person.traits.Any(t => t is T_TransmutationMaster))
                {
                    List<string> labels = new List<string> { $"{SoulstoneA.CapturedSoul.getName()} ({SoulTypeUtils.GetTitle(SoulstoneA.SoulType)}){(SoulstoneA.CapturedSoul.society == map.soc_dark ? " - Agent of The Dark" : SoulstoneA.CapturedSoul.society is SG_AgentWanderers ? " - Monstrous Soul" : "")}", $"{SoulstoneB.CapturedSoul.getName()} ({SoulTypeUtils.GetTitle(SoulstoneB.SoulType)}){(SoulstoneB.CapturedSoul.society == map.soc_dark ? " - Agent of The Dark" : SoulstoneB.CapturedSoul.society is SG_AgentWanderers ? " - Monstrous Soul" : "")}" };
                    Sel2_SaveSoulSelector selector = new Sel2_SaveSoulSelector(u.map, new List<I_Soulstone> { SoulstoneA, SoulstoneB });
                    map.world.ui.addBlocker(map.world.prefabStore.getScrollSetText(labels, false, selector, "Save Soul", "Choose which soul will not be consumed by this ritual. On dismiss, a random soul will not be consumed.").gameObject);
                }
                else
                {
                    SoulstoneA.CapturedSoul = null;
                    SoulstoneB.CapturedSoul = null;
                }
            }
            else
            {
                SoulstoneA.CapturedSoul = null;
            }
            

            if (item == null)
            {
                msgString = "ERROR: The available souls did not match any known recipe.";
                u.person.gainItem(new I_Shield(u.map));
                return;
            }

            msgString = $"The soulstones fuse together in a dazzling light, leaving behind {(item.getName().StartsWithVowel() ? "an " + item.getName() : "a " + item.getName())}.";
            u.person.gainItem(item);
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.CRUEL
            };
        }
    }
}
