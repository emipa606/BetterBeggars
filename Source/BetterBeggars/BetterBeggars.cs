// BetterBeggars.BetterBeggars

using UnityEngine;
using Verse;

namespace BetterBeggars;

[StaticConstructorOnStartup]
public static class BetterBeggars
{
    public static bool Settings_Button(this Listing_Standard ls, string label, Rect rect)
    {
        var result = Widgets.ButtonText(rect, label);
        ls.Gap(2f);
        return result;
    }
}