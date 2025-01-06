using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Pr_SpiritTree : Property
    {
        public int TargetArmyCount = 2;

        public int SpawnTimer = 25;

        public List<UM> Armies = new List<UM>();

        public Pr_SpiritTree(Location loc)
            : base(loc)
        {
            stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override string getName()
        {
            return $"Spirit Tree of {location.shortName}";
        }

        public override string getInvariantName()
        {
            return "Spirit Tree";
        }

        public override string getDesc()
        {
            return "A spirit tree resides here. It grants this location large boosts to prosperity and food production as well as forest spirit armies.";
        }

        public override Sprite getSprite(World world)
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_SpiritTree.png");
        }

        public override bool hasBackgroundHexView()
        {
            return true;
        }

        public override Sprite getHexBackgroundSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Hex_SpiritTree.png");
        }

        public override void turnTick()
        {
            for (int i = Armies.Count -1; i >= 0; i--)
            {
                if (Armies[i] == null || Armies[i].isDead)
                {
                    Armies.RemoveAt(i);
                }
                else
                {
                    Armies[i].homeLocation = location.index;
                }
            }

            if (Armies.Count < TargetArmyCount)
            {
                if (SpawnTimer > 0)
                {
                    SpawnTimer--;
                }

                if (location.settlement is SettlementHuman humanSettlement && location.soc is Society soc && SpawnTimer <= 0)
                {
                    UM_ForestArmy army = new UM_ForestArmy(location, soc, this);
                    location.units.Add(army);
                    map.units.Add(army);
                    Armies.Add(army);
                    SpawnTimer = 25;
                }
            }
        }

        public override double foodGenMult()
        {
            return 2.0;
        }
        public override double getProsperityInfluence()
        {
            return 0.5;
        }
    }
}
