﻿using Assets.Code;
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
    }
}
