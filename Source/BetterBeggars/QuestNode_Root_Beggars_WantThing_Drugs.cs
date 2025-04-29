// BetterBeggars.QuestNode_Root_Beggars_WantThing_Drugs

using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public class QuestNode_Root_Beggars_WantThing_Drugs : QuestNode_Root_Beggars_WantThing
{
    private static readonly List<ThingDef> AllowedDrugs = [];

    protected virtual void GetAllowedThings()
    {
        switch (BetterBeggars_Mod.settings.flagYayo)
        {
            case true when !AllowedDrugs.Contains(BeggarDefOf.Yayo):
                AllowedDrugs.Add(BeggarDefOf.Yayo);
                break;
            case false when AllowedDrugs.Contains(BeggarDefOf.Yayo):
                AllowedDrugs.Remove(BeggarDefOf.Yayo);
                break;
        }

        switch (BetterBeggars_Mod.settings.flagFlake)
        {
            case true when !AllowedDrugs.Contains(BeggarDefOf.Flake):
                AllowedDrugs.Add(BeggarDefOf.Flake);
                break;
            case false when AllowedDrugs.Contains(BeggarDefOf.Flake):
                AllowedDrugs.Remove(BeggarDefOf.Flake);
                break;
        }

        switch (BetterBeggars_Mod.settings.flagLuciferium)
        {
            case true when !AllowedDrugs.Contains(ThingDefOf.Luciferium):
                AllowedDrugs.Add(ThingDefOf.Luciferium);
                break;
            case false when AllowedDrugs.Contains(ThingDefOf.Luciferium):
                AllowedDrugs.Remove(ThingDefOf.Luciferium);
                break;
        }

        var smokeLeafDef = DefDatabase<ThingDef>.GetNamedSilentFail("SmokeleafJoint");
        switch (BetterBeggars_Mod.settings.flagSmokeleafJoint)
        {
            case true when !AllowedDrugs.Contains(smokeLeafDef):
                AllowedDrugs.Add(smokeLeafDef);
                break;
            case false when
                AllowedDrugs.Contains(smokeLeafDef):
                AllowedDrugs.Remove(smokeLeafDef);
                break;
        }

        switch (BetterBeggars_Mod.settings.flagBeer)
        {
            case true when !AllowedDrugs.Contains(ThingDefOf.Beer):
                AllowedDrugs.Add(ThingDefOf.Beer);
                break;
            case false when AllowedDrugs.Contains(ThingDefOf.Beer):
                AllowedDrugs.Remove(ThingDefOf.Beer);
                break;
        }
    }

    protected override void RunInt()
    {
        if (!ModLister.CheckIdeology("Beggars"))
        {
            return;
        }

        var quest = QuestGen.quest;
        var slate = QuestGen.slate;
        var map = QuestGen_Get.GetMap();
        var num = slate.Get("points", 0f);
        slate.Set("visitDurationTicks", VisitDuration);
        slate.Set("valueFactor", BeggarRequestValueFactor);
        GetAllowedThings();
        BeggarRequestValueFactor = BetterBeggars_Mod.settings.BeggarRequestValueMultiplier;
        var points = num * BeggarRequestValueFactor;
        if (BetterBeggars_Mod.settings.LimitMaxValue)
        {
            points = Math.Min(points, BetterBeggars_Mod.settings.MaxValue);
        }

        if (TryFindRandomRequestedThing(map, points, out var thingDef, out var count,
                AllowedDrugs))
        {
            slate.Set("requestedThing", thingDef);
            slate.Set("requestedThingDefName", thingDef.defName);
            slate.Set("requestedThingCount", count);
        }

        var drugProperties = thingDef.GetCompProperties<CompProperties_Drug>();
        var chemicalDef = drugProperties.chemical;
        var hediffDef_Addiction = chemicalDef.addictionHediff;

        hediffDef_Addiction.initialSeverity = Rand.Range(0.20f, 0.80f);

        var colonyPopulation = slate.Exists("population")
            ? slate.Get("population", 0)
            : map.mapPawns.FreeColonistsSpawnedCount;
        var numOfBeggars =
            Mathf.Max(Mathf.RoundToInt(LodgerCountBasedOnColonyPopulationFactorRange.RandomInRange * colonyPopulation),
                1);
        var list = new List<FactionRelation>();
        foreach (var item in Find.FactionManager.AllFactionsListForReading)
        {
            if (!item.def.permanentEnemy)
            {
                list.Add(new FactionRelation
                {
                    other = item,
                    kind = FactionRelationKind.Neutral
                });
            }
        }

        var faction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, true);
        faction.temporary = true;
        Find.FactionManager.Add(faction);
        slate.Set("faction", faction);
        var pawns = new List<Pawn>();
        for (var i = 0; i < numOfBeggars; i++)
        {
            pawns.Add(
                quest.GeneratePawn(PawnKindDefOf.Beggar, faction, true, null, 0f, true, null, 0f, 0f, false, true));
        }

        slate.Set("beggars", pawns);
        slate.Set("beggarCount", numOfBeggars);
        quest.SetFactionHidden(faction);
        quest.PawnsArrive(pawns, null, map.Parent, null, false, null, null, null, null, null, false, false, false);

        var questPart_AddHediff = new QuestPart_AddHediff
        {
            inSignal = QuestGen.slate.Get<string>("inSignal")
        };
        questPart_AddHediff.pawns.AddRange(pawns);
        questPart_AddHediff.hediffDef = hediffDef_Addiction;
        quest.AddPart(questPart_AddHediff);

        /*
         * Doesn't work for some reason :(
        foreach (Pawn pawn in pawns) {
            QuestPart_ChangeNeed questPart_ChangeNeed = new QuestPart_ChangeNeed();
            questPart_ChangeNeed.pawn = pawn;
            questPart_ChangeNeed.need = needDef;
            questPart_ChangeNeed.offset = -0.5f;
            quest.AddPart(questPart_ChangeNeed);
        }
        */

        var itemsReceivedSignal = QuestGen.GenerateNewSignal("ItemsReceived");
        var questPart_BegForItems = new QuestPart_BegForItems
        {
            inSignal = QuestGen.slate.Get<string>("inSignal"),
            outSignalItemsReceived = itemsReceivedSignal
        };
        questPart_BegForItems.pawns.AddRange(pawns);
        questPart_BegForItems.target = pawns[0];
        questPart_BegForItems.faction = faction;
        questPart_BegForItems.mapParent = map.Parent;
        questPart_BegForItems.thingDef = thingDef;
        questPart_BegForItems.amount = count;
        quest.AddPart(questPart_BegForItems);
        quest.GiveRewards(new RewardsGeneratorParams
        {
            allowDevelopmentPoints = true,
            thingRewardDisallowed = true
        });
        var pawnLabelSingleOrPlural = faction.def.pawnSingular;
        var translationKey = "MessageBeggarsLeavingWithItemsSingular";
        if (numOfBeggars > 1)
        {
            pawnLabelSingleOrPlural = faction.def.pawnsPlural;
            translationKey = "MessageBeggarsLeavingWithItemsPlural";
        }

        quest.Delay(60000, delegate
        {
            quest.Leave(pawns, null, false, false);
            quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars);
            quest.AnyColonistWithCharityPrecept(delegate
            {
                quest.Message(
                    string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(),
                        "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)),
                    MessageTypeDefOf.NegativeEvent, false, null, pawns);
            });
        }, null, itemsReceivedSignal);
        var beggarArrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Arrested");
        var beggarKilledSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Killed");
        var beggarLeftSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.LeftMap");
        quest.AnyColonistWithCharityPrecept(delegate
            {
                quest.Message(
                    "MessageCharityEventFulfilled".Translate() + ": " +
                    translationKey.Translate(pawnLabelSingleOrPlural), MessageTypeDefOf.PositiveEvent,
                    false, null, pawns);
                quest.RecordHistoryEvent(HistoryEventDefOf.CharityFulfilled_Beggars);
            },
            delegate
            {
                quest.Message(translationKey.Translate(pawnLabelSingleOrPlural),
                    MessageTypeDefOf.PositiveEvent, false, null, pawns);
            }, itemsReceivedSignal);
        quest.AnySignal([beggarKilledSignal, beggarArrestedSignal], delegate
        {
            quest.SignalPassActivable(
                delegate
                {
                    quest.AnyColonistWithCharityPrecept(delegate
                    {
                        quest.Message(
                            string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(),
                                "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)),
                            MessageTypeDefOf.NegativeEvent, false, null, pawns);
                    });
                }, null, null, null, null, itemsReceivedSignal);
            quest.Letter(LetterDefOf.NegativeEvent, null, null, null, null, false,
                QuestPart.SignalListenMode.OngoingOnly, pawns, false, "[letterTextBeggarsBetrayed]", null,
                "[letterLabelBeggarsBetrayed]");
            var part = new QuestPart_FactionRelationChange
            {
                faction = faction,
                relationKind = FactionRelationKind.Hostile,
                canSendHostilityLetter = false,
                inSignal = QuestGen.slate.Get<string>("inSignal")
            };
            quest.AddPart(part);
            quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars_Betrayed);
        });
        quest.End(QuestEndOutcome.Fail, 0, null,
            QuestGenUtility.HardcodedSignalWithQuestID("faction.BecameHostileToPlayer"));
        quest.AllPawnsDespawned(pawns, delegate
        {
            quest.SignalPassActivable(delegate { AddDelayedReward(quest, pawns, faction); }, null, null, null, null,
                itemsReceivedSignal);
            QuestGen_End.End(quest, QuestEndOutcome.Success);
        }, null, beggarLeftSignal);
    }

    protected override bool TestRunInt(Slate slate)
    {
        var map = QuestGen_Get.GetMap();
        var num = slate.Get("points", 0f);
        if (!FactionDefOf.Beggars.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp))
        {
            return false;
        }

        GetAllowedThings();
        foreach (var def in AllowedDrugs)
        {
            slate.Set(def.label, "yes");
        }

        return AllowedDrugs.Count != 0 && TryFindRandomRequestedThing(map, num * 0.85f, out _, out _, AllowedDrugs);
    }
}