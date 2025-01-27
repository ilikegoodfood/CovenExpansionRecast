using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Mg_Rti_Curse_Mirror : Ritual
    {
        public I_Soulstone Soulstone;

        public Mg_Rti_Curse_Mirror(Location location, I_Soulstone soulstone)
            : base(location)
        {
            Soulstone = soulstone;
        }

        public override string getName()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curseweaving: Curse of Obsession";
            }

            return $"Curseweaving: Curse of Obsession (House {Soulstone.CapturedSoul.house.name})";
        }

        public override string getDesc()
        {
            if (Soulstone == null || Soulstone.CapturedSoul == null || Soulstone.CapturedSoul.house == null || Soulstone.CapturedSoul.society is SG_AgentWanderers || Soulstone.CapturedSoul.society == map.soc_dark)
            {
                return "Curses the House of the soul contained in this soulstone, changing all of their likes and dislikes to match those of the soul contained in this soulstone.";
            }

            string result = $"Curses House {Soulstone.CapturedSoul.house.name}, changing all of their likes and dislikes to match those of the soul contained in this soulstone.\n\n{Soulstone.CapturedSoul.getFullName()} holds the following opinions:";
            bool hasTags = false;

            if (Soulstone.CapturedSoul.extremeLikes.Count > 0)
            {
                result += $"\n<b>Loves:</b> ";
                hasTags = true;

                for (int i = 0; i < Soulstone.CapturedSoul.extremeLikes.Count; i++)
                {
                    result += $"{Tags.getName(Soulstone.CapturedSoul.extremeLikes[i])}";
                    if (i < Soulstone.CapturedSoul.extremeLikes.Count - 1)
                    {
                        result += ", ";
                    }
                    else
                    {
                        result += ".";
                    }
                }
            }

            if (Soulstone.CapturedSoul.likes.Count > 0)
            {
                result += $"\n<b>Likes:</b> ";
                hasTags = true;

                for (int i = 0; i < Soulstone.CapturedSoul.likes.Count; i++)
                {
                    result += $"{Tags.getName(Soulstone.CapturedSoul.likes[i])}";
                    if (i < Soulstone.CapturedSoul.likes.Count - 1)
                    {
                        result += ", ";
                    }
                    else
                    {
                        result += ".";
                    }
                }
            }

            if (Soulstone.CapturedSoul.hates.Count > 0)
            {
                result += $"\n<b>Dislikes:</b> ";
                hasTags = true;

                for (int i = 0; i < Soulstone.CapturedSoul.hates.Count; i++)
                {
                    result += $"{Tags.getName(Soulstone.CapturedSoul.hates[i])}";
                    if (i < Soulstone.CapturedSoul.hates.Count - 1)
                    {
                        result += ", ";
                    }
                    else
                    {
                        result += ".";
                    }
                }
            }

            if (Soulstone.CapturedSoul.extremeHates.Count > 0)
            {
                result += $"\n<b>Hates:</b> ";
                hasTags = true;

                for (int i = 0; i < Soulstone.CapturedSoul.extremeHates.Count; i++)
                {
                    result += $"{Tags.getName(Soulstone.CapturedSoul.extremeHates[i])}";
                    if (i < Soulstone.CapturedSoul.extremeHates.Count - 1)
                    {
                        result += ", ";
                    }
                    else
                    {
                        result += ".";
                    }
                }
            }

            if (!hasTags)
            {
                result += $" Holds no strong opinions.";
            }

            return result;
        }

        public override string getCastFlavour()
        {
            return "They exalt a single member of their family as if a god. Willingly, they crawl under their champions shadow, groveling to their legacy until their lives are a hollow imitation of their heroes.";
        }

        public override string getRestriction()
        {
            return "Requires Mastery of Curseweaving at least 2 and a soulstone containing a soul. The soul must not belong to The Dark, or to a monstrous population, and their house must not already suffer the curse.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Icon_FrogCurse.png");
        }

        public override int isGoodTernary()
        {
            return -1;
        }

        public override bool valid()
        {
            return Soulstone != null && Soulstone.CapturedSoul != null && Soulstone.CapturedSoul.house != null && !(Soulstone.CapturedSoul.society is SG_AgentWanderers) && Soulstone.CapturedSoul.society != map.soc_dark && !Soulstone.CapturedSoul.house.curses.Any(c => c is Curse_Mirror);
        }

        public override bool validFor(UA ua)
        {
            return ua.person.traits.Any(t => t is T_MasteryCurseweaving curseweaving && curseweaving.level > 1);
        }

        public override double getComplexity()
        {
            return 40.0;
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

        public override int getCompletionProfile()
        {
            return 8;
        }

        public override int getCompletionMenace()
        {
            return 20;
        }

        public override void complete(UA u)
        {
            Soulstone.CapturedSoul.house.curses.Add(new Curse_Mirror(Soulstone.CapturedSoul));
            Soulstone.CapturedSoul = null;
        }
    }
}
