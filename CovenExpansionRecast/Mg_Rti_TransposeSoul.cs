using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                name = $"Transpose Soul ({SoulTypeUtils.GetTitle(SoulstoneA.GetSoulType())})";
            }
            else
            {
                name = $"Transpose Souls ({SoulTypeUtils.GetTitle(SoulstoneA.GetSoulType())}, {SoulTypeUtils.GetTitle(SoulstoneB.GetSoulType())})";
            }

            if (DisplayResult)
            {
                name += $" into {CovensCore.Instance.GetSoulcraftingItemName(SoulstoneA.GetSoulType(), SoulstoneB?.GetSoulType() ?? SoulType.Nothing)}";
            }

            return name;
        }

        public override string getDesc()
        {
            if (SoulstoneB == null)
            {
                return "Forging an item from the essence of a soul. The rarity of the item produced will be low, with the highest quality items being composed of two souls with unique professions.";
            }
            return "Combines two souls together, forging an item from their combined essence. The rarity of the item will depend on the profession of the souls, with the highest quality items being composed of two souls with unique professions.";
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
            return SoulstoneA != null && SoulstoneA.GetSoulType() != SoulType.Nothing && !string.IsNullOrEmpty(CovensCore.Instance.GetSoulcraftingItemID(SoulstoneA.GetSoulType(), SoulstoneB?.GetSoulType() ?? SoulType.Nothing));
        }

        public override bool validFor(UA ua)
        {
            if ((ua.task is Task_PerformChallenge perform && perform.challenge == this) || (ua.task is Task_GoToPerformChallenge goPerform && goPerform.challenge == this))
            {
                SoulType soulType = SoulstoneA.GetSoulType();
                if (lastTurnTypeA != SoulType.Nothing)
                {
                    if (lastTurnTypeA != soulType)
                    {
                        map.addMessage($"{ua.getName()} cancelled Transpose {(SoulstoneB == null ? "Soul" : "Souls")} because the type of the{(SoulstoneB == null ? " " : " first ")}soul being used changed from {SoulTypeUtils.GetTitle(lastTurnTypeA)} to {SoulTypeUtils.GetTitle(soulType)}, invalidating the recipe.", 0.0, false, ua.location.hex);
                        lastTurnTypeA = soulType;
                        return false;
                    }
                }

                if (SoulstoneB != null)
                {
                    soulType = SoulstoneB.GetSoulType();
                    if (lastTurnTypeB != SoulType.Nothing)
                    {
                        if (lastTurnTypeB != soulType)
                        {
                            map.addMessage($"{ua.getName()} cancelled Transpose Souls because the type of the second soul being used changed from {SoulTypeUtils.GetTitle(lastTurnTypeB)} to {SoulTypeUtils.GetTitle(soulType)}, invalidating the recipe.", 0.0, false, ua.location.hex);
                            lastTurnTypeB = soulType;
                            return false;
                        }
                    }
                }
            }

            lastTurnTypeA = SoulstoneA.GetSoulType();
            lastTurnTypeB = SoulstoneB.GetSoulType();

            if (ua.location == null || ua.location.settlement == null)
            {
                return false;
            }

            if (!(ua.location.settlement is Set_MinorOther) && !(ua.location.settlement is SettlementHuman))
            {
                return false;
            }

            return ua.location.settlement.subs.Any(sub => sub is Sub_Temple temple && temple.order is HolyOrder_Witches);
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
            Item item = CovensCore.Instance.GetSoulcraftingItem(map, u, SoulstoneA.GetSoulType(), SoulstoneB.GetSoulType());

            if (SoulstoneB != null)
            {
                if (u.person.traits.Any(t => t is T_TransmutationMaster))
                {
                    List<string> labels = new List<string> { $"{SoulstoneA.CapturedSoul.getName()} ({SoulTypeUtils.GetTitle(SoulstoneA.GetSoulType())})", $"{SoulstoneB.CapturedSoul.getName()} ({SoulTypeUtils.GetTitle(SoulstoneB.GetSoulType())})" };
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
