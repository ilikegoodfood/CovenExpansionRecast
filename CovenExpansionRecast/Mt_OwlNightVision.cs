using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class Mt_OwlNightVision : MinionTrait
    {
        public override string getName()
        {
            return "An Eye for Sercrets";
        }

        public override string getDesc()
        {
            return "-5 <b>Profile</b>, Occasionally replaces midchallenge events with special positive ones, increased chance at locations with high <b>Shadow</b>.";
        }

        public override void turnTick(UA ua, Minion m)
        {
            if (!(ua.task is Task_PerformChallenge) || ua.midchallengeTimer < 4)
            {
               return;
            }

            if (ua.location.getShadow() < 0.5)
            {
                return;
            }

            int num = (int)Math.Floor((ua.location.getShadow() / 0.25) + 0.01);
            int roll = Eleven.random.Next(15);
            if (roll > num)
            {
                return;
            }

            roll = Eleven.random.Next(10);
            if (roll > 6)
            {
                if (EventManager.events.ContainsKey("covenexpansion.msdch_owlItem"))
                {
                    EventContext eventContext = EventContext.withPerson(ua.map, ua.person);
                    eventContext.map.world.prefabStore.popEvent(EventManager.events["covenexpansion.msdch_owlItem"].data, eventContext, null, true);
                }
            }
            else if (roll > 2)
            {
                if (EventManager.events.ContainsKey("covenexpansion.msdch_owlPassage"))
                {
                    EventContext eventContext2 = EventContext.withPerson(ua.map, ua.person);
                    eventContext2.map.world.prefabStore.popEvent(EventManager.events["covenexpansion.msdch_owlPassage"].data, eventContext2, null, true);
                }
            }
            else
            {
                if (EventManager.events.ContainsKey("covenexpansion.msdch_OwlEgg"))
                {
                    EventContext eventContext3 = EventContext.withPerson(ua.map, ua.person);
                    eventContext3.map.world.prefabStore.popEvent(EventManager.events["covenexpansion.msdch_OwlEgg"].data, eventContext3, null, true);
                }
            }

            ua.midchallengeTimer = 0;
        }

        public override double getProfile(UA ua, Minion m)
        {
            return -5.0;
        }
    }
}
