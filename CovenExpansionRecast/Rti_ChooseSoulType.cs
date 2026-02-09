using Assets.Code;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_ChooseSoulType : Ritual
    {
        public I_Soulstone Soulstone;

        public Rti_ChooseSoulType(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone != null && Soulstone.CapturedSoul != null)
            {
                return $"Choose {Soulstone.CapturedSoul.getName()}'s Soul Type";
            }

            return "Choose Soul Type";
        }

        public override string getDesc()
        {
            StringBuilder result = new StringBuilder("Choose to manifest a specialization present within this soul.");
            if (Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return result.ToString();
            }

            List<SoulType> types = SoulTypeUtils.GetSoulTypes(Soulstone.CapturedSoul);
            if (types.Count > 1)
            {
                result.Append("You may choose from ");

                for (int i = 0; i < types.Count; i++)
                {
                    result.Append(SoulTypeUtils.GetTitle(types[i]));

                    if (i == types.Count - 1) // Last one
                    {
                        result.Append(".");
                    }
                    else if (i == types.Count - 2) // 2nd to last one
                    {
                        result.Append(", or ");
                    }
                    else
                    {
                        result.Append(", ");
                    }
                }
            }
            
            return result.ToString();
        }

        public override string getCastFlavour()
        {
            return "The soul within this soulstone is a swirl of conflicting potentials. With a moment's effort, it should be possible to bring one of your choosing to the fore.";
        }

        public override string getRestriction()
        {
            return "Requires a soulstone with a captured soul that has multiple specializations.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone.png");
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && SoulTypeUtils.GetSoulTypes(Soulstone.CapturedSoul).Count > 1;
        }

        public override bool validFor(UA ua)
        {
            return ua.isCommandable() && ua.person != null && ua.person.items.Any(i => i is I_Soulstone soulstone && soulstone.CapturedSoul != null && SoulTypeUtils.GetSoulTypes(soulstone.CapturedSoul).Count > 1);
        }

        public override bool validFor(UM ua)
        {
            return ua.isCommandable() & ua.person != null && ua.person.items.Contains(Soulstone);
        }

        public override void onBegin(Unit unit)
        {
            if (unit == null || !unit.isCommandable() || Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return;
            }

            List<SoulType> types = SoulTypeUtils.GetSoulTypes(Soulstone.CapturedSoul);
            if (types.Count <= 1)
            {
                return;
            }

            List<string> targetLabels = new List<string>();
            foreach (SoulType type in types)
            {
                if (Soulstone.SoulType == type)
                {
                    targetLabels.Add(SoulTypeUtils.GetTitle(type) + " (Current)");
                }

                targetLabels.Add(SoulTypeUtils.GetTitle(type));
            }

            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetText(targetLabels, false, new Sel2_SoulTypeSelector(Soulstone, types), "Select an intruding acolyte to curse").gameObject);
            unit.task = null;
        }

        public override double getComplexity()
        {
            return 0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }
    }
}
