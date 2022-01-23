// BetterBeggars.QuestNode_Root_Beggars_WantThing
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars
{
    public abstract class QuestNode_Root_Beggars_WantThing : QuestNode_Root_Beggars_Base
    {
        public const int VisitDuration = 60000;

        public static Dictionary<ThingDef, int> requestCountDict = new Dictionary<ThingDef, int>();


        public static bool TryFindRandomRequestedThing(Map map, float value, out ThingDef thingDef, out int count, IEnumerable<ThingDef> allowedThings)
        {
            requestCountDict.Clear();
            Func<ThingDef, bool> globalValidator = delegate (ThingDef td)
            {
                if (!td.PlayerAcquirable)
                {
                    return false;
                }
                int num = ThingUtility.RoundedResourceStackCount(Mathf.Max(1, Mathf.RoundToInt(value / td.BaseMarketValue)));
                requestCountDict.Add(td, num);
                return PlayerItemAccessibilityUtility.Accessible(td, num, map) ? true : false;
            };
            if (allowedThings.Where((ThingDef def) => globalValidator(def)).TryRandomElement(out thingDef))
            {
                count = requestCountDict[thingDef];
                return true;
            }
            count = 0;
            return false;
        }

        protected abstract void GetAllowedThings();

    }
}
