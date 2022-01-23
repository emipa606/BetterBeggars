// BetterBeggars.BetterBeggars_Mod

using UnityEngine;
using Verse;

namespace BetterBeggars
{
    public class BetterBeggars_Mod : Mod
    {
        public static BetterBeggars_Settings settings;

        public override string SettingsCategory()
        {
            return "Better Beggars";
        }

        public BetterBeggars_Mod(ModContentPack content)
        : base(content)
        {
            settings = GetSettings<BetterBeggars_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }
    }
}
