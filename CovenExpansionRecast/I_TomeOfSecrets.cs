using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class I_TomeOfSecrets : Item
    {
        public int[] Secrets;

        public int Progress = 0;

        public bool Complete
        {
            get
            {
                return Progress >= 3;
            }
        }

        public I_TomeOfSecrets(Map map)
            : base(map)
        {
            Secrets = new int[8];
            for (int i = 0; i < Secrets.Length; i++)
            {
                Secrets[i] = i;
            }
            Secrets.Shuffle();
        }

        public override string getName()
        {
            return $"Secrets of Life and Death ({Progress}/3)";
        }

        public override string getShortDesc()
        {
            if (Progress >= 3)
            {
                return "An inscrutable tome that demands knowledge as much as it offers. +2 <b>Lore</b>. The tome is satiated.";
            }

            string taskString = "Nothing";
            switch(Secrets[Progress])
            {
                case 0:
                    taskString = "Bring the tome to a location with 100% or more Death.";
                    break;
                case 1:
                    taskString = "Have someone die while holding the tome.";
                    break;
                case 2:
                    taskString = "Bring the tome to a location with 100% or more Madness.";
                    break;
                case 3:
                    taskString = "Bring the tome to a location with 3 or more human souls.";
                    break;
                case 4:
                    taskString = "Bring the tome to a location with 150% or more Unrest.";
                    break;
                case 5:
                    taskString = "Bring the tome to a location with 10% prosperity.";
                    break;
                case 6:
                    taskString = "Bring the tome to a deep one sanctum.";
                    break;
                case 7:
                    taskString = "Bring the tome to the ruins of a city. The ruins must be fresh, not Ancient Ruins.";
                    break;
            }
            return $"An inscrutable tome that demands knowledge as much as it offers. +1 <b>Lore</b>. The tome will give you 3 tasks that when completed will empower the tome and grant 50 progress towards your gods awakening. Current task: {taskString}";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_CursedTome.png");
        }

        public override void onDeath(Person p)
        {
            if (Complete)
            {
                return;
            }

            if (Secrets[Progress] == 1)
            {
                Progress++;
                OnComplete(p.getLocation(), p);
            }
        }

        public override void onTravel(Person p, Location current, Location destination)
        {
            if (Complete)
            {
                return;
            }

            CheckLocalProgress(destination, p);
        }

        public override void turnTick(Person owner)
        {
            if (Complete)
            {
                return;
            }

            CheckLocalProgress(owner.getLocation(), owner);
        }

        public void CheckLocalProgress(Location location, Person person)
        {
            if (Complete)
            {
                return;
            }

            switch (Secrets[Progress])
            {
                case 0:
                    if (location.properties.Any(pr => pr is Pr_Death && pr.charge >= 100.0))
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 2:
                    if (location.properties.Any(pr => pr is Pr_Madness && pr.charge >= 100.0))
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 3:
                    if (location.properties.Any(pr => pr is Pr_FallenHuman))
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 4:
                    if (location.properties.Any(pr => pr is Pr_Unrest && pr.charge >= 150.0))
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 5:
                    if (location.settlement is SettlementHuman humanSettlement && humanSettlement.prosperity <= 0.104)
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 6:
                    if (location.settlement is Set_DeepOneSanctum)
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
                case 7:
                    if (location.settlement is Set_CityRuins)
                    {
                        Progress++;
                        OnComplete(location, person);
                    }
                    break;
            }
        }

        public void OnComplete(Location location, Person person)
        {
            if (Complete)
            {
                map.addUnifiedMessage(person, null, "Tome Satiated", $"{person.getName()} has completed their tomes request and has been rewarded for satiating the tombs hunger for knowledge.", "TOME QUEST", true);
            }
            else
            {
                map.addUnifiedMessage(person, null, "Tome Quest Completed", $"{person.getName()} has completed their tomes request and has been given a new request to fufill.", "TOME QUEST", true);
            }
        }

        public override int getLoreBonus()
        {
            if (Complete)
            {
                return 2;
            }

            return 0;
        }

        public override int getLevel()
        {
            return LEVEL_RARE;
        }

        public override int getMorality()
        {
            return MORALITY_EVIL;
        }
    }
}
