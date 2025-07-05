// BetterBeggars.BetterBeggars_Mod

using Mlie;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public class BetterBeggars_Mod : Mod
{
    public static BetterBeggars_Settings Settings;
    public static string CurrentVersion;

    public BetterBeggars_Mod(ModContentPack content)
        : base(content)
    {
        Settings = GetSettings<BetterBeggars_Settings>();
        CurrentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "Better Beggars";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DoWindowContents(inRect);
    }
}