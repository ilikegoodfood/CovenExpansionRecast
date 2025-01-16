﻿using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class T_MasteryCurseweaving : Trait
    {
        public Person Person;

        public List<Mg_CaptureSoul>CaptureSoulRituals;

        public override string getName()
        {
            return $"Mastery of Curseweaving ({level})";
        }

        public override string getDesc()
        {
            return "Curseweaving is the art of sewing human souls into hexes and enchantments. Curseweaving allows the caster to bring entire families to ruin by using the souls of humans as thread and cloth to create intricate curses. Requires soulstones for powers to become available.";
        }

        public override void turnTick(Person p)
        {
            if (p.map.opt_allowMagicalArmsRace && p.unit != null && p.unit.isCommandable())
            {
                p.map.overmind.magicalArmsRace = Math.Max(level, p.map.overmind.magicalArmsRace);
            }

            ProcessCaptureRituals(p.getLocation());
        }

        public override void onAcquire(Person person)
        {
            Person = person;
            ProcessCaptureRituals(person.getLocation());

            UA ua = person.unit as UA;
            if (ua != null)
            {
                ua.rituals.Add(new Rt_soulTrap(ua.location));
                ua.rituals.Add(new Rt_studyCurseweaving(ua.location, ua));
            }

            if (level == 1)
            {
                I_Soulstone item = new I_Soulstone(person.map);
                person.gainItem(item, false);

                T_ArcaneKnowledge arcaneKnowledge = (T_ArcaneKnowledge)person.traits.FirstOrDefault(t => t is T_ArcaneKnowledge);
                if (arcaneKnowledge != null)
                {
                    arcaneKnowledge.level++;
                }
                else
                {
                    arcaneKnowledge = new T_ArcaneKnowledge();
                    arcaneKnowledge.level = 1;
                    person.receiveTrait(arcaneKnowledge);
                }
            }
        }

        public override void onMove(Location current, Location dest)
        {
            ProcessCaptureRituals(dest);
        }

        public void ProcessCaptureRituals(Location location)
        {
            if (Person == null || Person.unit == null)
            {
                CaptureSoulRituals.Clear();
                return;
            }

            // Get or create  Capture SOul ritual for each Soul present.
            List<Mg_CaptureSoul>  captureSoulRituals = new List<Mg_CaptureSoul>();
            foreach (Property property in location.properties)
            {
                if (!(property is Pr_FallenHuman soul))
                {
                    continue;
                }

                Mg_CaptureSoul captureSoul = CaptureSoulRituals.FirstOrDefault(rt => rt.Target == soul);
                if (captureSoul == null)
                {
                    captureSoul = new Mg_CaptureSoul(location, soul);
                }
                captureSoulRituals.Add(captureSoul);
            }

            CaptureSoulRituals.Clear();
            foreach (Ritual ritual in Person.unit.rituals.ToList())
            {
                if (ritual is Mg_CaptureSoul captureSoul)
                {
                    if (captureSoulRituals.Contains(captureSoul))
                    {
                        // Remove all Capure Soul rituals that the Person already has and should keep from the list of Capture Soul rituals to add and keep a reference to the ritual.
                        CaptureSoulRituals.Add(captureSoul);
                        captureSoulRituals.Remove(captureSoul);
                    }
                    else
                    {
                        // Remove all Capture Soul rituals for souls that are no longer present at the person's locatiion
                        Person.unit.rituals.Remove(captureSoul);
                        if ((Person.unit.task  is Task_PerformChallenge challenge && challenge.challenge == captureSoul) || (Person.unit.task is Task_GoToPerformChallenge goToChallenge && goToChallenge.challenge == captureSoul))
                        {
                            Person.unit.map.addUnifiedMessage(Person.unit, null, "Task Cancelled", Person.unit.getName() + " cancelled challenge " + ritual.getName() + ": No Longer Valid", UnifiedMessage.messageType.TASK_CANCELLED);
                            Person.unit.task = null;
                        }
                    }
                }
            }

            // Add any new Capture Soul reituals.
            foreach (Mg_CaptureSoul captureSoul in captureSoulRituals)
            {
                CaptureSoulRituals.Add(captureSoul);
                Person.unit.rituals.Add(captureSoul);
            }
        }

        public override int getMaxLevel()
        {
            return 3;
        }

        public override int[] getTags()
        {
            return new int[0];
        }
    }
}
