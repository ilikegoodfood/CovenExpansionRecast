using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class UAE_Spirit : UAE
    {
        public Person Source;

        public I_SpiritCage Cage;

        public UAE_Spirit(Location loc, Society sg, Person source, I_SpiritCage cage)
            : base(loc, sg)
        {
            Source = source;
            Cage = cage;

            person.shadow = 1.0;

            person.stat_might = source.stat_might;
            person.stat_lore = source.stat_lore;
            person.stat_intrigue = source.stat_intrigue;
            person.stat_command = source.stat_command;

            foreach (Trait trait in source.traits)
            {
                person.stat_might += trait.getMightChange();
                person.stat_lore += trait.getLoreChange();
                person.stat_intrigue += trait.getIntrigueChange();
                person.stat_command += trait.getCommandChange();
            }

            person.firstName = source.firstName;
            person.house = source.house;

            rituals.Add(new Rt_Haunt(loc, this));
        }

        public override string getName()
        {
            return $"{Source.getFullName()}'s Spirit";
        }

        public override Sprite getPortraitForeground()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_Spirit.png");
        }

        public override string getSpecialRuleDesc()
        {
            if (Cage?.Holder != null)
            {
                return $"Cannot move on its own. Moves with {Cage.Holder.getFullName()}, who bears the Spirit Cage.";
            }

            return "Cannot move on its own. Moves with the bearer of the Spirit Cage.";
        }

        public override int getMaxMoves()
        {
            return 0;
        }
    }
}
