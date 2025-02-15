﻿using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_MagicPlague : Ritual
    {
        private static HashSet<Type> _compatibleProperties = new HashSet<Type>();

        public static HashSet<Type> CompatibleProperties
        {
            get
            {
                if (_compatibleProperties == null)
                {
                    _compatibleProperties = new HashSet<Type>();
                }

                return _compatibleProperties;
            }
        }

        public Mg_MagicPlague(Location location)
            : base(location)
        {
            
        }

        public override string getName()
        {
            return $"Curseweaving: Psychogenic Epidemic";
        }

        public override string getDesc()
        {
            return "Choose almost any modifier at this agents location and create a psychogenic illness that carries the modifier with it. The illness will spread much like a plague. When the modifier has at least 80 charge, the illness will create the modfiier you chose at the illness' locaton. The higher the charge of the initial modifier the more effective the illness will be.";
        }

        public override string getCastFlavour()
        {
            return "They claw at their own eyes desperate to be free from the visions that writhe in their skulls. At last their prayers are answered as their paranoias spill forth from their heads, made corporeal only by the shared belief that they are real.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 2 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_PsychIllness.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return true;
        }

        public override bool validFor(UA ua)
        {
            return ua.isCommandable() && ua.location.settlement is SettlementHuman && ua.person != null && ua.person.items.Any(i => i is I_Soulstone soulstone && soulstone.CapturedSoul != null && soulstone.CapturedSoul.traits.Any(t => t is T_ChallengeBooster booster && (booster.target == Tags.DISCORD || booster.target == Tags.DISEASE)));
        }

        public override double getComplexity()
        {
            return 80.0;
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

        public override bool isChannelled()
        {
            return true;
        }

        public override int getCompletionProfile()
        {
            return 14;
        }

        public override int getCompletionMenace()
        {
            return 30;
        }

        public override void complete(UA u)
        {
            List<I_Soulstone> soulstones = u.person.items.OfType<I_Soulstone>().Where(i => i is I_Soulstone stone && stone.CapturedSoul != null && stone.CapturedSoul.traits.Any(t => t is T_ChallengeBooster booster && (booster.target == Tags.DISCORD || booster.target == Tags.DISEASE))).ToList();

            if (soulstones.Count == 0)
            {
                msgString = "You did not not posses a Soulstone carrying the required soul.";
                return;
            }
            else if (soulstones.Count == 1)
            {
                PopPlaguePropertySelect(u, soulstones[0]);
            }
            else
            {
                PopSoulSelect(u);
                return;
            }
        }

        public static void PopSoulSelect(UA caster)
        {
            List<I_Soulstone> soulstones = caster.person.items.OfType<I_Soulstone>().Where(i => i is I_Soulstone stone && stone.CapturedSoul != null && stone.CapturedSoul.traits.Any(t => t is T_ChallengeBooster booster && (booster.target == Tags.DISCORD || booster.target == Tags.DISEASE))).ToList();

            List<string> optionLabels = new List<string>();
            foreach (I_Soulstone soulstone in soulstones)
            {
                optionLabels.Add($"{soulstone.getName()} ({soulstone.CapturedSoul.getFullName()})");
            }

            Sel2_MagicPlague_SoulstoneSelector selector = new Sel2_MagicPlague_SoulstoneSelector();
            selector.Caster = caster;
            selector.TargetList = soulstones;
            caster.map.world.ui.addBlocker(caster.map.world.prefabStore.getScrollSetText(optionLabels, false, selector, "Select Soul", "Select the Soul which will be consumed to create the psychogenic illness.").gameObject);
        }

        public static void PopPlaguePropertySelect(UA caster, I_Soulstone soulstone)
        {
            List<Property> properties = new List<Property>();
            List<string> optionLabels = new List<string>();
            foreach (Property property in caster.location.properties)
            {
                Type propertyType = property.GetType();
                if (CovensCore.Instance.IsBlacklistedForPsychogenicIllness(propertyType))
                {
                    continue;
                }

                if (CompatibleProperties.Contains(propertyType))
                {
                    properties.Add(property);
                    continue;
                }

                if (property.GetType().GetConstructors().Any(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(Location)))
                {
                    CompatibleProperties.Add(propertyType);
                    properties.Add(property);
                    optionLabels.Add($"{property.getName()} ({property.charge})");
                }
            }

            if (properties.Count == 0)
            {
                caster.inner_profile -= 14;
                caster.inner_menace -= 30;
            }
            else
            {
                Sel2_MagicPlague_PropertySelector selector = new Sel2_MagicPlague_PropertySelector();
                selector.Caster = caster;
                selector.Soulstone = soulstone;
                selector.TargetList = properties;
                caster.map.world.ui.addBlocker(caster.map.world.prefabStore.getScrollSetText(optionLabels, false, selector, "Select Property", "Select the property that this phsychogenic illenss will spread.").gameObject);
            }
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.DISEASE
            };
        }
    }
}
