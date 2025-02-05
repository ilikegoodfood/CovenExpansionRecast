using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_ReleaseSoul : Ritual
    {
        public I_Soulstone Soulstone;

        public Rti_ReleaseSoul(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return "Release Soul";
            }

            return $"Release Soul of {Soulstone.CapturedSoul.getFullName()}";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return "Release the soul trapped in this soulstone (None).";
            }

            return $"Releases the soul of {Soulstone.CapturedSoul.getFullName()}.";
        }

        public override string getRestriction()
        {
            return "Requires the soulstone to have a human soul.";
        }

        public override Sprite getSprite()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null)
            {
                return EventManager.getImg("CovenExpansionRecast.Fore_Soulstone_Inactive.png");
            }

            if (Soulstone.CapturedSoul.shadow > 0.5)
            {
                return Soulstone.CapturedSoul.getPortraitAlt();
            }

            return Soulstone.CapturedSoul.getPortrait();
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null;
        }

        public override double getComplexity()
        {
            return 2.0;
        }

        public override Challenge.challengeStat getChallengeType()
        {
            return Challenge.challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override void complete(UA u)
        {
            u.location.properties.Add(new Pr_FallenHuman(u.location, Soulstone.CapturedSoul));
            Soulstone.CapturedSoul = null;
        }

        public override int[] buildNegativeTags()
        {
            return new int[]
            {
                Tags.CRUEL
            };
        }
    }
}
