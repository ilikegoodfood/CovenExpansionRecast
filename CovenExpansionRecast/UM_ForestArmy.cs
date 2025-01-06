using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class UM_ForestArmy : UM_HumanArmy
    {
        public Pr_SpiritTree SourceTree;

        public UM_ForestArmy(Location loc, Society sg, Pr_SpiritTree sourceTree)
            : base(loc, sg)
        {
            maxHp = 50;
            hp = 50;
            SourceTree = sourceTree;
        }

        public override string getName()
        {
            return $"Forest Guardians of {map.locations[this.homeLocation].shortName}";
        }

        public override Sprite getPortraitForeground()
        {
            return map.world.iconStore.vinervaSpiritOfWild;
        }

        public override void turnTickInner(Map map)
        {
            setMaxHP(map);
        }

        public void setMaxHP(Map map)
        {
            if (SourceTree == null || SourceTree.location.properties.Contains(SourceTree))
            {
                maxHp = 50;
                return;
            }

            maxHp = Math.Min(hp, 50);
            maxHp -= 5;

            if (hp > maxHp && hp <= maxHp + 5)
            {
                hp = maxHp;
            }

            if (maxHp <= 0)
            {
                SourceTree.Armies.Remove(this);
                disband(map, "Source spirit tree felled.");
            }
        }
    }
}
