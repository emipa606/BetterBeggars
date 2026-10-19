// BettarBeggars.BeggarDefOf

using RimWorld;
using Verse;

namespace BetterBeggars;

[DefOf]
public static class BeggarDefOf
{
    public static ThingDef Yayo;
    public static ThingDef Flake;

    static BeggarDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(BeggarDefOf));
    }
}