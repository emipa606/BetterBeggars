// BetterBeggars.BetterBeggars_Settings

using System;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public class BetterBeggars_Settings : ModSettings
{
    public const float BeggarRequestValueMultiplierBase = 0.85f;
    private static Vector2 scrollPosition = Vector2.zero;
    public float BeggarRequestValueMultiplier = 0.85f;
    public bool flagBeer = true;
    public bool flagCloth;
    public bool flagComponent;
    public bool flagFlake = true;
    public bool flagGold;
    public bool flagLuciferium = true;
    public bool flagMedicineHerbal = true;
    public bool flagMedicineIndustrial = true;

    public bool flagPenoxycyline = true;

    // Normal
    public bool flagSilver = true;
    public bool flagSmokeleafJoint = true;
    public bool flagSteel;

    public bool flagWood;

    // Drugs
    public bool flagYayo = true;

    public override void ExposeData()
    {
        base.ExposeData();
        // Normal
        Scribe_Values.Look(ref flagSilver, "flagSilver", true, true);
        Scribe_Values.Look(ref flagMedicineHerbal, "flagMedicineHerbal", true, true);
        Scribe_Values.Look(ref flagMedicineIndustrial, "flagMedicineIndustrial", true, true);
        Scribe_Values.Look(ref flagPenoxycyline, "flagPenoxycyline", true, true);
        Scribe_Values.Look(ref flagBeer, "flagBeer", true, true);
        Scribe_Values.Look(ref flagSteel, "flagSteel", false, true);
        Scribe_Values.Look(ref flagWood, "flagWood", false, true);
        Scribe_Values.Look(ref flagComponent, "flagComponent", false, true);
        Scribe_Values.Look(ref flagCloth, "flagCloth", false, true);
        // Drugs
        Scribe_Values.Look(ref flagYayo, "flagYayo", true, true);
        Scribe_Values.Look(ref flagFlake, "flagFlake", true, true);
        Scribe_Values.Look(ref flagLuciferium, "flagLuciferium", true, true);
        Scribe_Values.Look(ref flagSmokeleafJoint, "flagSmokeleafJoint", true, true);
        Scribe_Values.Look(ref BeggarRequestValueMultiplier, "BeggarRequestValueMultiplier", 0.85f, true);
    }

    internal void DoWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
        var rect = new Rect(0f, 0f, inRect.width - 30f, inRect.height);
        Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
        listing_Standard.Begin(rect);

        listing_Standard.Label("allowLabel_Text".Translate(), -1f, "allowLabel_Tooltip".Translate());
        listing_Standard.ColumnWidth = inRect.width / 2.2f;
        listing_Standard.CheckboxLabeled("allowSilver".Translate(), ref flagSilver);
        listing_Standard.CheckboxLabeled("allowMedicineHerbal".Translate(), ref flagMedicineHerbal);
        listing_Standard.CheckboxLabeled("allowMedicineIndustrial".Translate(), ref flagMedicineIndustrial);
        listing_Standard.CheckboxLabeled("allowPenoxycyline".Translate(), ref flagPenoxycyline);
        listing_Standard.CheckboxLabeled("allowBeer".Translate(), ref flagBeer);
        listing_Standard.CheckboxLabeled("allowGold".Translate(), ref flagGold);
        listing_Standard.CheckboxLabeled("allowSteel".Translate(), ref flagSteel);
        listing_Standard.CheckboxLabeled("allowWood".Translate(), ref flagWood);
        listing_Standard.CheckboxLabeled("allowComponent".Translate(), ref flagComponent);
        listing_Standard.CheckboxLabeled("allowCloth".Translate(), ref flagCloth);
        listing_Standard.CheckboxLabeled("allowYayo".Translate(), ref flagYayo);
        listing_Standard.CheckboxLabeled("allowFlake".Translate(), ref flagFlake);
        listing_Standard.CheckboxLabeled("allowLuciferium".Translate(), ref flagLuciferium);
        listing_Standard.CheckboxLabeled("allowSmokeleafJoint".Translate(), ref flagSmokeleafJoint);

        if (listing_Standard.Settings_Button("BetterBeggars_resetChecks".Translate(),
                new Rect(0f, listing_Standard.CurHeight, 180f, 29f)))
        {
            flagSilver = true;
            flagMedicineHerbal = true;
            flagMedicineIndustrial = true;
            flagPenoxycyline = true;
            flagBeer = true;
            flagGold = false;
            flagSteel = false;
            flagWood = false;
            flagComponent = false;
            flagCloth = false;
            flagYayo = true;
            flagFlake = true;
            flagLuciferium = true;
            flagSmokeleafJoint = true;
        }

        listing_Standard.Gap(50f);
        listing_Standard.Label(
            string.Concat("BeggarRequestValueMultiplier".Translate() + ": ", BeggarRequestValueMultiplier.ToString()),
            -1f, "BeggarRequestValueMultiplier_Tooltip".Translate());
        BeggarRequestValueMultiplier =
            (float)Math.Round(listing_Standard.Slider(BeggarRequestValueMultiplier, 0.1f, 2f), 2);
        if (listing_Standard.Settings_Button("BetterBeggars_resetMultiplier".Translate(),
                new Rect(0f, listing_Standard.CurHeight, 180f, 29f)))
        {
            BeggarRequestValueMultiplier = 0.85f;
        }

        if (BetterBeggars_Mod.currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("BetterBeggars_currentModVersion".Translate(BetterBeggars_Mod.currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Widgets.EndScrollView();

        Write();
    }
}