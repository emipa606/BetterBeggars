// BetterBeggars.BetterBeggars_Settings
using System;
using BetterBeggars;
using UnityEngine;
using Verse;

namespace BetterBeggars
{
    public class BetterBeggars_Settings : ModSettings
    {
        private static Vector2 scrollPosition = Vector2.zero;
        // Normal
        public bool flagSilver = true;
        public bool flagMedicineHerbal = true;
        public bool flagMedicineIndustrial = true;
        public bool flagPenoxycyline = true;
        public bool flagBeer = true;
        public bool flagGold = false;
        public bool flagSteel = false;
        public bool flagWood = false;
        public bool flagComponent = false;
        public bool flagCloth = false;
        // Drugs
        public bool flagYayo = true;
        public bool flagFlake = true;
        public bool flagLuciferium = true;
        public bool flagSmokeleafJoint = true;
        public const float BeggarRequestValueMultiplierBase = 0.85f;
        public float BeggarRequestValueMultiplier = 0.85f;

        public override void ExposeData()
        {
            base.ExposeData();
            // Normal
            Scribe_Values.Look(ref flagSilver, "flagSilver", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagMedicineHerbal, "flagMedicineHerbal", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagMedicineIndustrial, "flagMedicineIndustrial", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagPenoxycyline, "flagPenoxycyline", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagBeer, "flagBeer", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagSteel, "flagSteel", defaultValue: false, forceSave: true);
            Scribe_Values.Look(ref flagWood, "flagWood", defaultValue: false, forceSave: true);
            Scribe_Values.Look(ref flagComponent, "flagComponent", defaultValue: false, forceSave: true);
            Scribe_Values.Look(ref flagCloth, "flagCloth", defaultValue: false, forceSave: true);
            // Drugs
            Scribe_Values.Look(ref flagYayo, "flagYayo", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagFlake, "flagFlake", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagLuciferium, "flagLuciferium", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref flagSmokeleafJoint, "flagSmokeleafJoint", defaultValue: true, forceSave: true);
            Scribe_Values.Look(ref BeggarRequestValueMultiplier, "BeggarRequestValueMultiplier", 0.85f, forceSave: true);
        }

        internal void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect = new Rect(0f, 0f, inRect.width - 30f, inRect.height);
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

            if (listing_Standard.Settings_Button("BetterBeggars_resetChecks".Translate(), new Rect(0f, listing_Standard.CurHeight, 180f, 29f)))
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
            listing_Standard.Label(string.Concat("BeggarRequestValueMultiplier".Translate() + ": ", BeggarRequestValueMultiplier.ToString()), -1f, "BeggarRequestValueMultiplier_Tooltip".Translate());
            BeggarRequestValueMultiplier = (float)Math.Round(listing_Standard.Slider(BeggarRequestValueMultiplier, 0.1f, 2f), 2);
            if (listing_Standard.Settings_Button("BetterBeggars_resetMultiplier".Translate(), new Rect(0f, listing_Standard.CurHeight, 180f, 29f)))
            {
                BeggarRequestValueMultiplier = 0.85f;
            }
            listing_Standard.End();
            Widgets.EndScrollView();

            Write();
        }


    }
}