// BetterBeggars.QuestNode_Root_Beggars_WantThing_Vanilla

using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars;

public class QuestNode_Root_Beggars_WantThing_Vanilla : QuestNode_Root_Beggars_WantThing
{
    private static readonly List<ThingDef> AllowedThings = [];

    protected virtual void GetAllowedThings()
    {
        switch (BetterBeggars_Mod.Settings.flagSilver)
        {
            case true when !AllowedThings.Contains(ThingDefOf.Silver):
                AllowedThings.Add(ThingDefOf.Silver);
                break;
            case false when AllowedThings.Contains(ThingDefOf.Silver):
                AllowedThings.Remove(ThingDefOf.Silver);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagMedicineHerbal)
        {
            case true when !AllowedThings.Contains(ThingDefOf.MedicineHerbal):
                AllowedThings.Add(ThingDefOf.MedicineHerbal);
                break;
            case false when
                AllowedThings.Contains(ThingDefOf.MedicineHerbal):
                AllowedThings.Remove(ThingDefOf.MedicineHerbal);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagMedicineIndustrial)
        {
            case true when !AllowedThings.Contains(ThingDefOf.MedicineIndustrial):
                AllowedThings.Add(ThingDefOf.MedicineIndustrial);
                break;
            case false when
                AllowedThings.Contains(ThingDefOf.MedicineIndustrial):
                AllowedThings.Remove(ThingDefOf.MedicineIndustrial);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagPenoxycyline)
        {
            case true when !AllowedThings.Contains(ThingDefOf.Penoxycyline):
                AllowedThings.Add(ThingDefOf.Penoxycyline);
                break;
            case false when
                AllowedThings.Contains(ThingDefOf.Penoxycyline):
                AllowedThings.Remove(ThingDefOf.Penoxycyline);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagBeer)
        {
            case true when !AllowedThings.Contains(ThingDefOf.Beer):
                AllowedThings.Add(ThingDefOf.Beer);
                break;
            case false when AllowedThings.Contains(ThingDefOf.Beer):
                AllowedThings.Remove(ThingDefOf.Beer);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagGold)
        {
            case true when !AllowedThings.Contains(ThingDefOf.Gold):
                AllowedThings.Add(ThingDefOf.Gold);
                break;
            case false when AllowedThings.Contains(ThingDefOf.Gold):
                AllowedThings.Remove(ThingDefOf.Gold);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagSteel)
        {
            case true when !AllowedThings.Contains(ThingDefOf.Steel):
                AllowedThings.Add(ThingDefOf.Steel);
                break;
            case false when AllowedThings.Contains(ThingDefOf.Steel):
                AllowedThings.Remove(ThingDefOf.Steel);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagWood)
        {
            case true when !AllowedThings.Contains(ThingDefOf.WoodLog):
                AllowedThings.Add(ThingDefOf.WoodLog);
                break;
            case false when AllowedThings.Contains(ThingDefOf.WoodLog):
                AllowedThings.Remove(ThingDefOf.WoodLog);
                break;
        }

        switch (BetterBeggars_Mod.Settings.flagComponent)
        {
            case true when !AllowedThings.Contains(ThingDefOf.ComponentIndustrial):
                AllowedThings.Add(ThingDefOf.ComponentIndustrial);
                break;
            case false when
                AllowedThings.Contains(ThingDefOf.ComponentIndustrial):
                AllowedThings.Remove(ThingDefOf.ComponentIndustrial);
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
        BeggarRequestValueFactor = BetterBeggars_Mod.Settings.BeggarRequestValueMultiplier;
        var points = num * BeggarRequestValueFactor;
        if (BetterBeggars_Mod.Settings.LimitMaxValue)
        {
            points = Math.Min(points, BetterBeggars_Mod.Settings.MaxValue);
        }

        if (TryFindRandomRequestedThing(map, points, out var thingDef, out var count,
                AllowedThings))
        {
            slate.Set("requestedThing", thingDef);
            slate.Set("requestedThingDefName", thingDef.defName);
            slate.Set("requestedThingCount", count);
        }

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
        return AllowedThings.Count != 0 && TryFindRandomRequestedThing(map, num * 0.85f, out _, out _, AllowedThings);
    }
}