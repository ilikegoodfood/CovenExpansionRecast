using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class M_Owl : Minion
    {
        public M_Owl(Map map)
            : base(map)
        {
            traits.Add(new Mt_OwlNightVision());
        }

        public override string getName()
        {
            return "Owl";
        }

        public override Sprite getIcon()
        {
            return EventManager.getImg("CovenExpansion.Icon_Owl.png");
        }

        public override int getCommandCost()
        {
            return 1;
        }

        public override int getAttack()
        {
            return 3;
        }

        public override int getMaxDefence()
        {
            return 0;
        }

        public override int getMaxHP()
        {
            return 1;
        }

        public override int getRecruitmentTime()
        {
            return 1;
        }

        public override int getGoldCost()
        {
            return 25;
        }

        public override Minion getClone()
        {
            return new M_Owl(map);
        }
    }
}
