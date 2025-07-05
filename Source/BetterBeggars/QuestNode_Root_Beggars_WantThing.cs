// BetterBeggars.QuestNode_Root_Beggars_WantThing

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public abstract class QuestNode_Root_Beggars_WantThing : QuestNode_Root_Beggars_Base
{
    protected const int VisitDuration = 60000;

    private static readonly Dictionary<ThingDef, int> requestCountDict = new();


    protected static bool TryFindRandomRequestedThing(Map map, float value, out ThingDef thingDef, out int count,
        IEnumerable<ThingDef> allowedThings)
    {
        requestCountDict.Clear();

        if (allowedThings.Where(globalValidator).TryRandomElement(out thingDef))
        {
            count = requestCountDict[thingDef];
            return true;
        }

        count = 0;
        return false;

        bool globalValidator(ThingDef td)
        {
            if (!td.PlayerAcquirable)
            {
                return false;
            }

            var num = ThingUtility.RoundedResourceStackCount(Mathf.Max(1,
                Mathf.RoundToInt(value / td.BaseMarketValue)));
            requestCountDict.Add(td, num);
            return PlayerItemAccessibilityUtility.Accessible(td, num, map);
        }
    }
}