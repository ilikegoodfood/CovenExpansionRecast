using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_Panacea : Item
    {
        public I_Panacea(Map map)
            : base(map)
        {
            challenges.Add(new Rti_Panacea(map.locations[0]));
        }

        public override string getName()
        {
            return "The Panacea";
        }

        public override string getShortDesc()
        {
            return "Replaces temporary stat decreases with temporary stat increases and heals the holder over time. Can cure the woes of mankind to reduce an agents menace but increase their profile.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_Panacea.png");
        }

        public override List<Ritual> getRituals(UA ua)
        {
            return challenges;
        }

        public override void turnTick(Person owner)
        {
            if (owner.unit is UA)
            {
                if (owner.unit.hp < owner.unit.maxHp)
                {
                    owner.unit.hp++;
                }
            }
            else if (owner.rulerOf != -1)
            {
                foreach (Property property in owner.getLocation().properties)
                {
                    if (property is Pr_Unrest || property is Pr_Plague || property is Pr_Devastation || property is Pr_PoliticalInstability || property is Pr_Famine || property is Pr_Madness)
                    {
                        property.charge = 0.0;
                    }
                }
            }

            if (owner.sanity < owner.maxSanity)
            {
                owner.sanity++;
            }

            foreach (Trait trait in owner.traits.ToList())
            {
                if (trait is T_StatTempMight mightTrait)
                {
                    if (mightTrait.amount < 0)
                    {
                        owner.traits.Remove(mightTrait);
                        owner.receiveTrait(new T_StatTempMight(mightTrait.turnsLeft, -mightTrait.amount));
                    }
                    continue;
                }

                if (trait is T_StatTempLore loreTrait)
                {
                    if (loreTrait.amount < 0)
                    {
                        owner.traits.Remove(loreTrait);
                        owner.receiveTrait(new T_StatTempLore(loreTrait.turnsLeft, -loreTrait.amount));
                    }
                    continue;
                }

                if (trait is T_StatTempIntrigue intrigueTrait)
                {
                    if (intrigueTrait.amount < 0)
                    {
                        owner.traits.Remove(intrigueTrait);
                        owner.receiveTrait(new T_StatTempIntrigue(intrigueTrait.turnsLeft, -intrigueTrait.amount));
                    }
                    continue;
                }

                if (trait is T_StatTempCommand commandTrait)
                {
                    if (commandTrait.amount < 0)
                    {
                        owner.traits.Remove(commandTrait);
                        owner.receiveTrait(new T_StatTempCommand(commandTrait.turnsLeft, -commandTrait.amount));
                    }
                    continue;
                }
            }
        }

        public override int getLevel()
        {
            return LEVEL_ARTEFACT;
        }

        public override int getMorality()
        {
            return MORALITY_NEUTRAL;
        }
    }
}
