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

        public Mg_Rti_TransposeSoul(Location location, I_Soulstone soulstoneA, I_Soulstone soulstone_B = null)
            : base(location)
        {
            SoulstoneA = soulstoneA;
            SoulstoneB = soulstone_B;
        }

        public override string getName()
        {
            if (SoulstoneA == null)
            {
                return $"Transpose Souls";
            }

            if (SoulstoneB == null)
            {
                return $"Transpose Souls ({SoulstoneA.GetSoulType()})";
            }
            return $"Transpose Souls ({SoulstoneA.GetSoulType()}, {SoulstoneB.GetSoulType()})";
        }

        public override string getDesc()
        {
            return "Combines two souls together forging an item from their combined essence. The rarity of the item will depend on the profession of the souls with the highest quality items being composed of two souls with unique professions.";
        }

        public override string getCastFlavour()
        {
            return "The fire crackles and pops as the two souls coalesce, and a new relic is born from their fused essence.";
        }

        public override string getRestriction()
        {
            return "Requires two soulstones that contain souls.  Must be performed at a witches coven";
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
            return SoulstoneA != null && !string.IsNullOrEmpty(CovensCore.Instance.GetSoulcraftingItemID(SoulstoneA.GetSoulType(), SoulstoneB?.GetSoulType() ?? "Nothing"));
        }

        public override bool validFor(UA ua)
        {
            if (ua.location == null)
            {
                return false;
            }

            if (!(ua.location.settlement is Set_MinorOther) && !(ua.location.settlement is SettlementHuman))
            {
                return false;
            }

            return location.settlement.subs.Any(sub => sub is Sub_Temple temple && temple.order.tenets.Any(ht => ht is H_Soulweavers && ht.status < 0));
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
            Item item = CovensCore.Instance.GetSoulcraftingItem(map, u, SoulstoneA.GetSoulType(), SoulstoneB?.GetSoulType()  ?? "Nothing");
            
            for (int i = 0; i < u.person.items.Length; i++)
            {
                if (u.person.items[i] == SoulstoneA || u.person.items[i] == SoulstoneB)
                {
                    u.person.items[i] = null;
                }
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
