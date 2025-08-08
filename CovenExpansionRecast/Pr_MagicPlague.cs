using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Pr_MagicPlague : Property
    {
        public readonly string PlaguePropertyName;

        public readonly Type PlaguePropertyType;

        public double PlaguePropertyCharge;

        public readonly List<Challenge> Challenges = new List<Challenge>();

        public readonly List<Assets.Code.Action> Actions = new List<Assets.Code.Action>();

        public Pr_MagicPlague(Location loc, Property property)
            : base(loc)
        {
            PlaguePropertyName = property.getName();
            PlaguePropertyType = property.GetType();
            PlaguePropertyCharge = property.charge;

            Challenges.Add(new Ch_TreatMagicDisease(loc, this));
            Challenges.Add(new Ch_CultivateMagicDisease(loc, this));
            Challenges.Add(new Ch_MagicNovelStrain(loc, this));

            Actions.Add(new Act_RaiseLucidity(loc));

            stackStyle = stackStyleEnum.NONE;
        }

        public Pr_MagicPlague(Location loc, string propertyName, Type propertyType, double propertyCharge)
            : base (loc)
        {
            PlaguePropertyName = propertyName;
            PlaguePropertyType = propertyType;
            PlaguePropertyCharge = propertyCharge;

            Challenges.Add(new Ch_TreatMagicDisease(loc, this));
            Challenges.Add(new Ch_CultivateMagicDisease(loc, this));
            Challenges.Add(new Ch_MagicNovelStrain(loc, this));

            Actions.Add(new Act_RaiseLucidity(loc));

            stackStyle = stackStyleEnum.NONE;
        }

        public override string getName()
        {
            return $"Psychogenic Illness ({PlaguePropertyName})";
        }

        public override string getDesc()
        {
            return $"This location is suffering from mass psychogenic illness. Once this modifier reaches 80%, {PlaguePropertyName} will be spread at this location and grow until it reaches {map.param.prop_plagueSpreadReq} charge, but this spread is reduced if the location is under quarantine (needs {map.param.prop_plagueSpreadReqQuarantine} charge.";
        }

        public override Sprite getSprite(World world)
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_PsychIllness.png");
        }

        public override bool hasHexView()
        {
            return true;
        }

        public override Sprite hexViewSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Hex_PsychIllnessFaded.png");
        }

        public override void turnTick()
        {
            if (charge > 300.0)
            {
                charge = 300.0;
            }

            if (!(location.settlement is SettlementHuman settlementHuman))
            {
                location.properties.Remove(this);
                return;
            }

            bool isDuplicate = true;
            Pr_MagicPlague firstPlagueInstance = null;
            for (int i = 0; i < location.properties.Count; i++)
            {
                Property property = location.properties[i];
                if (property == this)
                {
                    break;
                }
                else if (property is Pr_MagicPlague plague && plague.PlaguePropertyType == PlaguePropertyType)
                {
                    firstPlagueInstance = plague;
                    break;
                }
            }

            if (isDuplicate)
            {
                firstPlagueInstance.influences.Add(new ReasonMsg("De-duplicating psychogenic illnesses", charge / 3.0));
                firstPlagueInstance.PlaguePropertyCharge = Math.Max(firstPlagueInstance.PlaguePropertyCharge, PlaguePropertyCharge);
                location.properties.Remove(this);
                return;
            }

            influences.Add(new ReasonMsg("Constant Increase", 2.0 * map.difficultyMult_shrinkWithDifficulty));

            Pr_Unrest unrest = (Pr_Unrest)location.properties.FirstOrDefault(pr => pr is Pr_Unrest);
            if (unrest == null)
            {
                unrest = new Pr_Unrest(location);
                unrest.charge = 1.0;
                location.properties.Add(unrest);
            }

            ReasonMsg exposureMsg = unrest.influences.FirstOrDefault(msg => msg.msg == "Panic Over Psychogenic Illness");
            double deltaChargeUnrest = (int)((charge / 100.0) + 1.0);
            if (exposureMsg == null)
            {
                unrest.influences.Add(new ReasonMsg("Panic Over Psychogenic Illness", deltaChargeUnrest));
            }
            else
            {
                exposureMsg.value += deltaChargeUnrest * 0.5;
            }

            int spreadRequirement = map.param.prop_plagueSpreadReq;
            if (location.properties.Any(pr => pr is Pr_Quarantine && pr.charge > 0.0))
            {
                spreadRequirement = map.param.prop_plagueSpreadReqQuarantine;
            }

            if (charge > spreadRequirement)
            {
                foreach (Location neighbour in location.getNeighbours())
                {
                    if (!(neighbour.settlement is SettlementHuman))
                    {
                        continue;
                    }

                    Pr_MagicPlague neighbouringPlague = (Pr_MagicPlague)neighbour.properties.FirstOrDefault(pr => pr is Pr_MagicPlague nPlague && nPlague.PlaguePropertyType == PlaguePropertyType);
                    if (neighbouringPlague == null)
                    {
                        neighbouringPlague = new Pr_MagicPlague(neighbour, PlaguePropertyName, PlaguePropertyType, PlaguePropertyCharge);
                        neighbouringPlague.charge = 1.0;
                        neighbouringPlague.influences.Add(new ReasonMsg($"Spread from {location.getName()}", 1.0));
                        neighbour.properties.Add(neighbouringPlague);
                        return;
                    }
                    else
                    {
                        if (neighbouringPlague.charge < charge / 2.0)
                        {
                            neighbouringPlague.influences.Add(new ReasonMsg($"Spread from {location.getName()}", 1.0));
                        }

                        if (neighbouringPlague.PlaguePropertyCharge < PlaguePropertyCharge)
                        {
                            neighbouringPlague.PlaguePropertyCharge = Math.Min(neighbouringPlague.PlaguePropertyCharge + 2.0, PlaguePropertyCharge);
                        }
                    }
                }
            }

            Pr_Lucidity lucidity = (Pr_Lucidity)location.properties.FirstOrDefault(pr => pr is Pr_Lucidity);
            if (lucidity == null)
            {
                lucidity = new Pr_Lucidity(location);
                lucidity.charge = 1.0;
                location.properties.Add(lucidity);
            }

            double deltaChargeLucidity = 3.0;
            if (map.opt_rulerTraitsAffectModifiers && location.person() != null)
            {
                    deltaChargeLucidity = 3.0 * 0.5 * location.person().getStatLore();
            }

            exposureMsg = lucidity.influences.FirstOrDefault(msg => msg.msg == "Psychogenic Illness Exposure");
            if (exposureMsg == null)
            {
                lucidity.influences.Add(new ReasonMsg("Psychogenic Illness Exposure", deltaChargeLucidity));
            }
            else
            {
                exposureMsg.value += deltaChargeLucidity * 0.5;
            }

            if (charge >= 80.0)
            {
                Property targetProperty = location.properties.FirstOrDefault(pr => pr.GetType() == PlaguePropertyType);
                if (targetProperty == null)
                {
                    targetProperty = (Property)Activator.CreateInstance(PlaguePropertyType, new object[] { location });
                    targetProperty.charge = PlaguePropertyCharge / 10.0;
                    location.properties.Add(targetProperty);
                }

                if (targetProperty.charge < PlaguePropertyCharge)
                {
                    double chargeDelta = 3.0;
                    chargeDelta = Math.Min(chargeDelta, PlaguePropertyCharge - targetProperty.charge);

                    targetProperty.influences.Add(new ReasonMsg("Psychogenic Illness", chargeDelta));
                }
            }
        }

        public override List<Challenge> getChallenges()
        {
            return Challenges;
        }

        public override List<Assets.Code.Action> getActions()
        {
            return Actions;
        }

        public override double getProsperityInfluence()
        {
            return -0.003 * this.charge;
        }

        public static void addToProperty(string cause, double amount, Location loc, Property property)
        {
            Type propertyType = property.GetType();
            Pr_MagicPlague magicPlague = (Pr_MagicPlague)loc.properties.FirstOrDefault(pr => pr is Pr_MagicPlague plague && plague.PlaguePropertyType == propertyType);
            if (magicPlague == null)
            {
                magicPlague = new Pr_MagicPlague(loc, property);
                magicPlague.charge = 50.0 + (property.charge / 3.0);
                loc.properties.Add(magicPlague);
            }

            magicPlague.influences.Add(new ReasonMsg(cause, amount));
        }

        public static void addToProperty(string cause, double amount, Location loc, string propertyName, Type propertyType, double propertyCharge)
        {
            Pr_MagicPlague magicPlague = (Pr_MagicPlague)loc.properties.FirstOrDefault(pr => pr is Pr_MagicPlague plague && plague.PlaguePropertyType == propertyType);
            if (magicPlague == null)
            {
                magicPlague = new Pr_MagicPlague(loc, propertyName, propertyType, propertyCharge);
                magicPlague.charge = 50.0 + (propertyCharge / 3.0);
                loc.properties.Add(magicPlague);
            }

            magicPlague.influences.Add(new ReasonMsg(cause, amount));
        }
    }
}
