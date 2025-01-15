using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Pr_RobbedGraves : Property
    {
        public int Timer = 0;

        private int ChallengeBoostTag = 0;

        public T_ChallengeBooster ChallengeBooster =>  new T_ChallengeBooster(ChallengeBoostTag);

        public List<UM> Armies = new List<UM>();

        public Pr_RobbedGraves(Location loc)
            : base(loc)
        {
            ChallengeBoostTag = Eleven.random.Next(6) + 100;
            stackStyle = stackStyleEnum.TO_MAX_CHARGE;
        }

        public override string getName()
        {
            return $"Robbed Graves ({ChallengeBooster.getName()})";
        }

        public override string getInvariantName()
        {
            return "Robbed Graves";
        }

        public override string getDesc()
        {
            return $"Some of the graves at this locations catacomb have been picked clean. While this modifier is over 100% the unearth challenge can no longer be performed here. Decreases over time if there is a Death modifier with over 20% charge at this location as the graves are slowly refilled. The next body Exhumed here will be a {ChallengeBooster.getName()}.";
        }

        public override standardProperties getPropType()
        {
            return standardProperties.OTHER;
        }

        public override Sprite getSprite(World world)
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Graveyard.png");
        }

        public override bool deleteOnZero()
        {
            return false;
        }

        public T_ChallengeBooster GenerateChallengeBooster()
        {
            T_ChallengeBooster result = ChallengeBooster;
            ChallengeBoostTag = Eleven.random.Next(6) + 100;
            return result;
        }
    }
}
