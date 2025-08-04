using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_SoulSearch : Ritual
    {
        public Rti_SoulSearch(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Soul Search";
        }

        public override string getDesc()
        {
            return "View a list of all souls present on the map, including information such as the soul's type and whether it is an agent of the dark.";
        }

        public override string getCastFlavour()
        {
            return "If viewed at just the right angle, illuminated by the shining light of the moon, the text on this simple piece of parchment shifts and swirls, revealing secrets of the dead.";
        }

        public override string getRestriction()
        {
            return "Cannot be performed if there are no souls to be found.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_SkullSense.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool valid()
        {
            return map.locations.Any(loc => loc.properties.Any(pr => pr is Pr_FallenHuman));
        }

        public override bool validFor(UA ua)
        {
            return ua.isCommandable() && ua.person.items.Any(i => i is I_CraftList);
        }

        public override double getComplexity()
        {
            return 0.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override void onImmediateBegin(UA uA)
        {
            complete(uA);
            uA.task = null;
        }

        public override void complete(UA u)
        {
            PopSoulSearchSelect(u.map);
        }

        public static void PopSoulSearchSelect(Map map)
        {
            Sel2_SoulFinder selector = new Sel2_SoulFinder();
            selector.TargetList = new List<Pr_FallenHuman>();
            List<string> optionLabels = new List<string>();
            Person person;
            foreach (Location loc in map.locations)
            {
                foreach (Property pr in loc.properties)
                {
                    if (pr is Pr_FallenHuman soul)
                    {
                        person = map.persons[soul.personIndex];
                        selector.TargetList.Add(soul);
                        optionLabels.Add($"{person.getName()} ({SoulTypeUtils.GetTitle(person)})");
                    }
                }
            }

            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetText(optionLabels, false, selector, "Souls", "This list displays all souls currently on the map. Selecting one will pan to it's location.").gameObject);
        }
    }
}
