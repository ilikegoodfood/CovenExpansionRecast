using Assets.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CovenExpansionRecast
{
    public class Rti_ReadTransposingScroll : Ritual
    {
        public Rti_ReadTransposingScroll(Location location)
            : base(location)
        {

        }

        public override string getName()
        {
            return "Read Transposing Scroll";
        }

        public override string getDesc()
        {
            return "View a list of all soul transposition recipes.";
        }

        public override string getCastFlavour()
        {
            return "Centuries of study and experimentaion have gone into descovering the recipes transcribed on this simple sheet of parchment.";
        }

        public override Sprite getSprite()
        {
            return EventManager.getImg("CovenExpansionRecast.Fore_List.png");
        }

        public override int isGoodTernary()
        {
            return 0;
        }

        public override bool valid()
        {
            return true;
        }

        public override bool validFor(UA ua)
        {
            return ua.isCommandable() && ua.person.items.Any(i => i is I_CraftList);
        }

        public override double getComplexity()
        {
            return 0.0;
        }

        public override challengeStat getChallengeType()
        {
            return challengeStat.OTHER;
        }

        public override double getProgressPerTurnInner(UA unit, List<ReasonMsg> msgs)
        {
            msgs?.Add(new ReasonMsg("Base", 1.0));
            return 1.0;
        }

        public override void onImmediateBegin(UA uA)
        {
            complete(uA);
            uA.task = null;
        }

        public override void complete(UA u)
        {
            Sel2_DisplayInertSelection selector = new Sel2_DisplayInertSelection();
            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetText(CovensCore.Instance.RecipeList, false, selector, "Soul Transposition Recipes", "This list displays all soul transposition recipes.").gameObject);
        }
    }
}
