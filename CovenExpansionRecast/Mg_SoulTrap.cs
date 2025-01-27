using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_SoulTrap : Ritual
    {
        public Mg_SoulTrap(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Curseweaving: Soul Snare";
        }

        public override string getDesc()
        {
            return "Creates a soul snare at this location. The next hero that starts a quest or nuetral challenge here loses their soul dropping it at this location. Basic quests like resting, completing training or recruiting minions will not trigger the snare. No effect on the chosen one.";
        }

        public override string getCastFlavour()
        {
            return "The city is adorned with strange figurines and charms, hidden in plain sight among the refuse. Even the keenest eye could not hope to guess their nefarious purpose.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_SoulSnare.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool validFor(UA ua)
        {
            return ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 0);
        }

        public override double getComplexity()
        {
            return 35.0;
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
            u.location.properties.Add(new Pr_SoulSnare(u.location));
        }
    }
}
