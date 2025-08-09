using Assets.Code;
using CommunityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class ComLibHooks : Hooks
    {
        private Map _map;

        public ComLibHooks(Map map) : base(map)
        {
            _map = map;
        }

        public override bool isUnitSubsumed(Unit uOriginal, Unit uSubsuming)
        {
            return uSubsuming is UAEN_Toad toad && toad.SubsumedUnit == uOriginal;
        }

        public override void onAgentAI_EndOfProcess(UA ua, AgentAI.AIData aiData, List<AgentAI.ChallengeData> validChallengeData, List<AgentAI.TaskData> validTaskData, List<Unit> visibleUnits)
        {
            if (ua is UAEN_Pigeon)
            {
                if (ua.task == null)
                {
                    if (ua.person.gold > 0 || ua.person.items.Any(i => i != null))
                    {
                        Pr_ItemCache pr_ItemCache = new Pr_ItemCache(ua.location);
                        foreach (Item item in ua.person.items)
                        {
                            if (item == null)
                            {
                                continue;
                            }

                            pr_ItemCache.addItemToSet(item);
                        }
                        pr_ItemCache.gold = ua.person.gold;
                    }

                    ua.map.addUnifiedMessage(ua, ua.location, "Carrier Pigeon flew away", $"After loosing its owner the pigeon has flown away, leaving any gold or items it was carrying behind.", "PigeonFlewAway");
                    if (GraphicalMap.selectedUnit == ua)
                    {
                        GraphicalMap.selectedUnit = null;
                    }
                    ua.disband(ua.map, "Ownerless carrier pigeon dissapeared into the wilds");
                    return;
                }
            }
        }

        public override void onAgentLevelup_GetTraits(UA ua, List<Trait> availableTraits, bool startingTraits)
        {
            if (!CovensCore.Opt_Curseweaving)
            {
                return;
            }

            if (startingTraits)
            {
                if (ua is UAE_Warlock)
                {
                    availableTraits.Add(new T_MasteryCurseweaving());
                }
            }
            else if (ua.isCommandable())
            {
                if (ua.corrupted && (ua is UAG || ua is UAA))
                {
                    bool knowsCurseweaving = false;
                    bool isMaster = false;

                    foreach (Trait trait in ua.person.traits)
                    {
                        if (trait is T_TransmutationMaster)
                        {
                            isMaster = true;
                            break;
                        }

                        if (trait is T_MasteryCurseweaving)
                        {
                            knowsCurseweaving = true;
                        }
                    }

                    if (!isMaster && knowsCurseweaving)
                    {
                        availableTraits.Add(new T_TransmutationMaster());
                    }
                }
            }
        }

        public override int onUnitAI_GetsDistanceToLocation(Unit u, Location target, Location[] pathTo, int travelTime)
        {
            if (u.person != null)
            {
                if (u.person.items.Any(i => i is I_MadBoots))
                {
                    travelTime = (int)Math.Ceiling(travelTime / (u.getMaxMoves() + 2.0));
                }
            }

            return travelTime;
        }
    }
}
