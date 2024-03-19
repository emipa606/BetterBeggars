// BetterBeggars.QuestNode_Root_Beggars_WantThing

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public abstract class QuestNode_Root_Beggars_WantThing : QuestNode_Root_Beggars_Base
{
    public const int VisitDuration = 60000;

    public static readonly Dictionary<ThingDef, int> requestCountDict = new Dictionary<ThingDef, int>();


    public static bool TryFindRandomRequestedThing(Map map, float value, out ThingDef thingDef, out int count,
        IEnumerable<ThingDef> allowedThings)
    {
        requestCountDict.Clear();

        if (allowedThings.Where(GlobalValidator).TryRandomElement(out thingDef))
        {
            count = requestCountDict[thingDef];
            return true;
        }

        count = 0;
        return false;

        bool GlobalValidator(ThingDef td)
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