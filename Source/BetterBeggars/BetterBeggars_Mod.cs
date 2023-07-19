// BetterBeggars.BetterBeggars_Mod

using Mlie;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public class BetterBeggars_Mod : Mod
{
    public static BetterBeggars_Settings settings;
    public static string currentVersion;

    public BetterBeggars_Mod(ModContentPack content)
        : base(content)
    {
        settings = GetSettings<BetterBeggars_Settings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Better Beggars";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        settings.DoWindowContents(inRect);
    }
}