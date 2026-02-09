using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Ch_H_CurseIntrudingAcolyte : ChallengeHoly
    {
        public HolyOrder Order;

        public int Cooldown = 0;

        public UAA Target;

        public Ch_H_CurseIntrudingAcolyte(Location loc, HolyOrder order)
            : base(loc)
        {
            Order = order;
        }

        public override string getName()
        {
            if (Target != null)
            {
                return "Holy: Curse " + Target.getName();
            }

            return "Holy: Curse Intruder";
        }

        public override string getDesc()
        {
            return "Places a curse on an acolyte performing a quest at this location. Cannot be used again if used recently.";
        }

        public override string getRestriction()
        {
            return "Can only be performed by a member of the Holy Order followed at this settlemnt. The Holy Order's curseweaver status must be negative. Must target an acolyte that is performing a challenge at this location.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Curseweave.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool valid()
        {
            if (Cooldown > 0)
            {
                return false;
            }

            if (claimedBy != null && (claimedBy.isDead || !(claimedBy.task is Task_PerformChallenge performChallenge) || performChallenge.challenge != this))
            {
                claimedBy = null;
                Target = null;
            }

            if (Target != null)
            {
                if (Target.isDead || Target.location != location || (Target.task != null && !(Target.task is Task_PerformChallenge)))
                {
                    Target = null;
                }
                else
                {
                    return true;
                }
            }

            foreach (Unit unit in location.units)
            {
                if (!(unit is UAA uaa) || uaa.isCommandable() || uaa.order == Order || uaa.order is HolyOrder_Ophanim || (uaa.task  != null && !(uaa.task is Task_PerformChallenge)))
                {
                    continue;
                }

                if (CovensCore.Instance.TryGetModIntegrationData("OrcsPlus", out ModIntegrationData intDataOP) && intDataOP.TypeDict.TryGetValue("OrcCulture", out Type orcCultureType))
                {
                    if (uaa.order.GetType() == orcCultureType || uaa.order.GetType().IsSubclassOf(orcCultureType))
                    {
                        continue;
                    }
                }

                return true;
            }

            return false;
        }

        public override bool validFor(UA ua)
        {
            if (ua.society != Order)
            {
                return false;
            }

            H_Curseweavers curseweavers = (H_Curseweavers)Order.tenets.FirstOrDefault(t => t is H_Curseweavers);
            if (curseweavers != null && curseweavers.status < 0)
            {
                return true;
            }

            return false;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.LORE;
        }

        public override double getComplexity()
        {
            return 1.0;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            double val = unit.getStatLore();
            if (val > 0)
            {
                msgs?.Add(new ReasonMsg("Stat: Lore", val));
                return val;
            }

            val = 1.0;
            msgs?.Add(new ReasonMsg("Base", val));

            return val;
        }

        public override void onImmediateBegin(UA uA)
        {
            ChooseTarget(uA);
        }

        public void ChooseTarget(UA ua)
        {
            List<UAA> targets = new List<UAA>();
            List<string> targetLabels = new List<string>();

            foreach (Unit unit in location.units)
            {
                if (!(unit is UAA uaa) || uaa.isCommandable() || uaa.order == Order || (uaa.task != null && !(uaa.task is Task_PerformChallenge)))
                {
                    continue;
                }

                targets.Add(uaa);
                targetLabels.Add($"{uaa.getName()} of the {uaa.society.getName()}");
            }

            if (targets.Count == 0)
            {
                ua.task = null;
                if (ua.isCommandable())
                {
                    map.addMessage($"{ua.getName()} has cancelled {getName()}. No valid targets are present.", 0.5);
                }
                return;
            }

            if (targets.Count == 1)
            {
                Target = targets[0];
                return;
            }

            if (!ua.isCommandable())
            {
                Target = targets[Eleven.random.Next(targets.Count)];
                return;
            }

            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetText(targetLabels, false, new Sel2_CurseTargetSelector(map, this, targets), "Select an intruding acolyte to curse").gameObject);
        }

        public override void complete(UA u)
        {
            if (Target == null)
            {
                ChooseTarget(u);

                if (Target == null)
                {
                    return;
                }
            }

            Cooldown = 10;

            int curseCount = 0;
            foreach (Trait trait in Target.person.traits)
            {
                if (trait is T_ThroughTheirEyes || trait is T_Generosity || trait is T_Soulless || trait is T_Wanderer)
                {
                    curseCount++;
                }
            }

            switch (curseCount)
            {
                case 0:
                    T_Generosity generosity = new T_Generosity(Order);
                    if (u.isCommandable())
                    {
                        msgString = $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with the curse of generosity, and will constantly donate funds to the coven that cursed them.";
                    }
                    else
                    {
                        map.addUnifiedMessage(Target, u, "Curse of Generosity", $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with the curse of generosity, and will constantly donate funds to the coven that cursed them.", "Witch Curses Acolyte", true);
                    }
                    Target.person.receiveTrait(generosity);
                    break;
                case 1:
                    if (!Target.person.hasSoul)
                    {
                        break;
                    }

                    T_Soulless soulless = new T_Soulless();
                    if (u.isCommandable())
                    {
                        msgString = $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has driven their their soul from their body. An implacable emptiness washes over them.";
                    }
                    else
                    {
                        map.addUnifiedMessage(Target, u, "Soul Stolen by Curse", $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has driven their their soul from their body. An implacable emptiness washes over them.", "Witch Curses Acolyte", true);
                    }
                    Target.person.receiveTrait(soulless);
                    break;
                case 2:
                    T_ThroughTheirEyes eyes = new T_ThroughTheirEyes();
                    if (u.isCommandable())
                    {
                        msgString = $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with the see through their eyes curse, allowing dark forces to see through their eyes. Security at their current location is reduced by 1.";
                    }
                    else
                    {
                        map.addUnifiedMessage(Target, u, "Curseed with See Through Their Eyes", $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with the see through their eyes curse, allowing dark forces to see through their eyes. Security at their current location is reduced by 1.", "Witch Curses Acolyte", true);
                    }
                    Target.person.receiveTrait(eyes);
                    break;
                case 3:
                    T_Wanderer wanderer = new T_Wanderer();
                    if (u.isCommandable())
                    {
                        msgString = $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with an insatiable wanderlust, driving them to explore and perform as many challenges as they can, even to their own detriment.";
                    }
                    else
                    {
                        map.addUnifiedMessage(Target, u, "Curse of Wanderlust", $"{Target.getName()} has disrupted the faith at {location.getName()} and has been cursed as a result. {u.getName()} has cursed them with an insatiable wanderlust, driving them to explore and perform as many challenges as they can, even to their own detriment.", "Witch Curses Acolyte", true);
                    }
                    Target.person.receiveTrait(wanderer);
                    break;
                default:
                    break;
            }

            Target = null;
        }

        public override int[] buildPositiveTags()
        {
            return new int[]
                {
                    Tags.CRUEL,
                    Tags.RELIGION,
                    Tags.SHADOW
                };
        }
    }
}
