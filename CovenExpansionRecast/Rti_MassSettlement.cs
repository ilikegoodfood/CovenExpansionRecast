using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_MassSettlement : Ritual
    {
        public I_SettlersWreath Wreath;

        public Rti_MassSettlement(Location location, I_SettlersWreath wreath)
            : base(location)
        {
            Wreath = wreath;
        }

        public override string getName()
        {
            return "Mass Settlement";
        }

        public override string getDesc()
        {
            return "Founds a new outpost at this location and all locations within 3 steps that have adequate habitability. They will quickly grow into full human settlements, rapidly expanding the territory of the holders society.";
        }

        public override string getRestriction()
        {
            return $"Requires an empty or ruined land location with habitability > {(int)Math.Round(map.param.mapGen_minHabitabilityForHumans * 100)}% and for the settlers to have been organized.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_SettlersWreath.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override double getMenace()
        {
            return 50.0;
        }

        public override bool validFor(UA ua)
        {
            if (Wreath.Society == null)
            {
                return false;
            }

            return ValidAt(ua.location);
        }

        public override double getComplexity()
        {
            return 50.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.COMMAND;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatCommand();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Command", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));
            return val;
        }

        public override int getCompletionProfile()
        {
            return 16;
        }

        public override void complete(UA u)
        {
            for (int i = 0; i < u.person.items.Length; i++)
            {
                if (u.person.items[i] is I_SettlersWreath wreath && wreath.getRituals(u).Contains(this))
                {
                    u.person.items[i] = null;
                    break;
                }
            }

            HashSet<Location> locationHashSet = new HashSet<Location> { u.location };
            List<Location> outerLocations = new List<Location> { u.location };
            for (int i = 0; i <= 3; i++)
            {
                List<Location> newLocations = new List<Location>();

                foreach (Location location in outerLocations)
                {
                    if (ValidAt(location))
                    {
                        Pr_HumanOutpost pr_HumanOutpost = new Pr_HumanOutpost(location, Wreath.Society);
                        pr_HumanOutpost.charge += 150.0;
                        location.properties.Add(pr_HumanOutpost);
                    }

                    foreach (Location neighbour in location.getNeighbours())
                    {
                        if (locationHashSet.Contains(neighbour))
                        {
                            continue;
                        }

                        newLocations.Add(neighbour);
                    }
                }

                if (newLocations.Count == 0)
                {
                    break;
                }

                outerLocations = newLocations;
            }
        }

        public bool ValidAt(Location location)
        {
            if (location.isOcean || location.soc != null)
            {
                return false;
            }

            if (location.hex.getHabilitability() < map.param.mapGen_minHabitabilityForHumans || (location.settlement != null && !(location.settlement is Set_CityRuins)))
            {
                return false;
            }

            if (location.properties.Any(pr => pr is Pr_HumanOutpost))
            {
                return false;
            }

            return true;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
            {
                Tags.COOPERATION
            };
        }
    }
}
