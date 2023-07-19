// BetterBeggars.QuestNode_Root_Beggars_Chased

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars;

internal class QuestNode_Root_Beggars_Chased : QuestNode_Root_Beggars_Base
{
    public const int JoinDelay = 1000;
    public const int RaidDelay = 4000;
    public static readonly int StayDurationDays = Rand.Range(3, 6);
    public static readonly int StayDurationTicks = StayDurationDays * 60000;
    private static FloatRange MutinyTimeRange = new FloatRange(0.2f, 1f);

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
        slate.Set("map", map);
        slate.Set("stayDurationTicks", StayDurationTicks);

        slate.Set("stayDurationDays", StayDurationDays);

        var colonyPopulation = slate.Exists("population")
            ? slate.Get("population", 0)
            : map.mapPawns.FreeColonistsSpawnedCount;
        var beggarCount =
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

        var beggarFaction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, true);
        beggarFaction.temporary = true;
        Find.FactionManager.Add(beggarFaction);
        slate.Set("beggarFaction", beggarFaction);

        var pawns = new List<Pawn>();
        for (var i = 0; i < beggarCount; i++)
        {
            pawns.Add(quest.GeneratePawn(PawnKindDefOf.Beggar, beggarFaction, true, null, 0f, true,
                null, 0f, 0f, false,
                true));
        }


        slate.Set("beggars", pawns);
        slate.Set("beggarCount", beggarCount);

        beggarFaction.leader = pawns.First();
        quest.SetFactionHidden(beggarFaction, true);

        var beggarRecruitedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Recruited");

        _ = quest.ExtraFaction(beggarFaction, pawns, ExtraFactionType.MiniFaction,
            false, beggarRecruitedSignal);

        var questPart_Choice = quest.RewardChoice();
        var choice = new QuestPart_Choice.Choice
        {
            rewards =
            {
                (Reward)new Reward_VisitorsHelp(),
                (Reward)new Reward_PossibleFutureReward()
            }
        };
        questPart_Choice.choices.Add(choice);

        TryFindWalkInSpot(map, out var walkInSpot);

        quest.Delay(JoinDelay,
            delegate
            {
                quest.PawnsArrive(pawns, null, map.Parent, null, true, walkInSpot,
                    "[beggarsArriveLetterLabel]", "[beggarsArriveLetterText]");
            });


        quest.SetAllApparelLocked(pawns);

        var beggarArrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Arrested");
        _ = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Killed");
        var beggarDestroyedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Destroyed");
        var beggarKidnappedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Kidnapped");
        var beggarLeftSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.LeftMap");
        var beggarSurgeryViolatedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.SurgeryViolation");
        var beggarArrestedOrRecruitedSignal = QuestGen.GenerateNewSignal("beggar_ArrestedOrRecruited");
        var beggarArrestedOrRecruitedOrViolatedorDestroyedSignal =
            QuestGen.GenerateNewSignal("beggar_ArrestedOrRecruitedOrViolatedOrDestroyed");
        var beggarBanishedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Banished");
        var assaultColonySignal = QuestGen.GenerateNewSignal("AssaultColony");
        quest.AnySignal(new List<string> { beggarRecruitedSignal, beggarArrestedSignal }, null,
            new List<string> { beggarArrestedOrRecruitedSignal });
        quest.AnySignal(new List<string>
                { beggarRecruitedSignal, beggarArrestedSignal, beggarSurgeryViolatedSignal, beggarDestroyedSignal },
            null, new List<string> { beggarArrestedOrRecruitedOrViolatedorDestroyedSignal });

        Action unused1 = delegate
        {
            var mutinyTimeTicks = Mathf.FloorToInt(MutinyTimeRange.RandomInRange * StayDurationTicks);
            quest.Delay(mutinyTimeTicks, delegate
                {
                    quest.Letter(LetterDefOf.ThreatBig, null, null, null, null, false,
                        QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarMutinyLetterText]", null,
                        "[beggarMutinyLetterLabel]");
                    quest.SignalPass(null, null, assaultColonySignal);
                    QuestGen_End.End(quest, QuestEndOutcome.Unknown);
                }, null, null, null, false, null, null, false, null, null,
                "Mutiny (" + mutinyTimeTicks.ToStringTicksToDays() + ")");
        };

        var enemyFaction = TryFindEnemyFaction();

        if (!enemyFaction.HostileTo(beggarFaction))
        {
            var beggarEnemyRelation = new FactionRelation
            {
                other = beggarFaction,
                kind = FactionRelationKind.Hostile
            };
            enemyFaction.SetRelation(beggarEnemyRelation);
        }

        slate.Set("enemyFaction", enemyFaction);

        var stayDoneSignal = QuestGen.GenerateNewSignal("stayDone");

        quest.Delay(RaidDelay, delegate
        {
            quest.Raid(map, num, enemyFaction, walkInSpot: walkInSpot,
                customLetterLabel: "{BASELABEL} chasing beggars");
        }, null, null, stayDoneSignal);


        quest.Delay(StayDurationTicks, delegate
            {
                quest.SignalPassWithFaction(beggarFaction, null, delegate
                {
                    quest.AnyColonistWithCharityPrecept(delegate
                        {
                            quest.Letter(LetterDefOf.PositiveEvent, null, null, null,
                                null,
                                false, QuestPart.SignalListenMode.OngoingOnly, null, false,
                                "MessageCharityEventFulfilled".Translate() + ": " + "[beggarsLeavingLetterText]", null,
                                "[beggarsLeavingLetterLabel]");
                            quest.RecordHistoryEvent(HistoryEventDefOf.CharityFulfilled_Beggars);
                        },
                        delegate
                        {
                            quest.Letter(LetterDefOf.PositiveEvent, null, null, null,
                                null,
                                false, QuestPart.SignalListenMode.OngoingOnly, null, false,
                                "[beggarsLeavingLetterText]", null,
                                "[beggarsLeavingLetterLabel]");
                        });
                });


                quest.Leave(pawns, null, false, false, beggarArrestedOrRecruitedSignal,
                    true);
            }, null, null, null, false, null, null, false, "GuestsDepartsIn".Translate(), "GuestsDepartsOn".Translate(),
            "StayDelay");

        quest.End(QuestEndOutcome.Fail, 0, null,
            QuestGenUtility.HardcodedSignalWithQuestID("faction.BecameHostileToPlayer"));


        var questPart_BeggarsInteractions = new QuestPart_BeggarsInteractions
        {
            inSignalEnable = QuestGen.slate.Get<string>("inSignal"),
            inSignalDestroyed = beggarDestroyedSignal,
            inSignalArrested = beggarArrestedSignal,
            inSignalSurgeryViolation = beggarSurgeryViolatedSignal,
            inSignalKidnapped = beggarKidnappedSignal,
            inSignalRecruited = beggarRecruitedSignal,
            inSignalAssaultColony = assaultColonySignal,
            inSignalLeftMap = beggarLeftSignal,
            inSignalBanished = beggarBanishedSignal,
            outSignalDestroyed_AssaultColony = QuestGen.GenerateNewSignal("BeggarDestroyed_AssaultColony"),
            outSignalDestroyed_LeaveColony = QuestGen.GenerateNewSignal("BeggarDestroyed_LeaveColony"),
            outSignalDestroyed_BadThought = QuestGen.GenerateNewSignal("BeggarDestroyed_BadThought"),
            outSignalArrested_AssaultColony = QuestGen.GenerateNewSignal("BeggarArrested_AssaultColony"),
            outSignalArrested_LeaveColony = QuestGen.GenerateNewSignal("BeggarArrested_LeaveColony"),
            outSignalArrested_BadThought = QuestGen.GenerateNewSignal("BeggarArrested_BadThought"),
            outSignalSurgeryViolation_AssaultColony =
                QuestGen.GenerateNewSignal("BeggarSurgeryViolation_AssaultColony"),
            outSignalSurgeryViolation_LeaveColony = QuestGen.GenerateNewSignal("BeggarSurgeryViolation_LeaveColony"),
            outSignalSurgeryViolation_BadThought = QuestGen.GenerateNewSignal("BeggarSurgeryViolation_BadThought"),
            outSignalLast_Destroyed = QuestGen.GenerateNewSignal("LastBeggar_Destroyed"),
            outSignalLast_Arrested = QuestGen.GenerateNewSignal("LastBeggar_Arrested"),
            outSignalLast_Kidnapped = QuestGen.GenerateNewSignal("LastBeggar_Kidnapped"),
            outSignalLast_Recruited = QuestGen.GenerateNewSignal("LastBeggar_Recruited"),
            outSignalLast_LeftMapAllHealthy = QuestGen.GenerateNewSignal("LastBeggar_LeftMapAllHealthy"),
            outSignalLast_LeftMapAllNotHealthy = QuestGen.GenerateNewSignal("LastBeggar_LeftMapAllNotHealthy"),
            outSignalLast_Banished = QuestGen.GenerateNewSignal("LastBeggar_Banished")
        };
        questPart_BeggarsInteractions.pawns.AddRange(pawns);
        questPart_BeggarsInteractions.faction = beggarFaction;
        questPart_BeggarsInteractions.mapParent = map.Parent;
        questPart_BeggarsInteractions.signalListenMode = QuestPart.SignalListenMode.Always;
        quest.AddPart(questPart_BeggarsInteractions);

        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalDestroyed_BadThought, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarDiedMemoryThoughtLetterText]", null,
            "[beggarDiedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalDestroyed_AssaultColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarDiedAttackPlayerLetterText]", null,
            "[beggarDiedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalDestroyed_LeaveColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarDiedLeaveMapLetterText]", null,
            "[beggarDiedLeaveMapLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalLast_Destroyed, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarsAllDiedLetterText]", null,
            "[beggarsAllDiedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalArrested_BadThought, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarArrestedMemoryThoughtLetterText]", null,
            "[beggarArrestedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalArrested_AssaultColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarArrestedAttackPlayerLetterText]", null,
            "[beggarArrestedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalArrested_LeaveColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarArrestedLeaveMapLetterText]", null,
            "[beggarArrestedLeaveMapLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalLast_Arrested, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarsAllArrestedLetterText]", null,
            "[beggarsAllArrestedLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_BadThought, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarViolatedMemoryThoughtLetterText]", null,
            "[beggarViolatedMemoryThoughtLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_AssaultColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarViolatedAttackPlayerLetterText]", null,
            "[beggarViolatedAttackPlayerLetterLabel]");
        quest.Letter(LetterDefOf.NegativeEvent,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_LeaveColony, null, null, null, false,
            QuestPart.SignalListenMode.OngoingOnly, null, false, "[beggarViolatedLeaveMapLetterText]", null,
            "[beggarViolatedLeaveMapLetterLabel]");
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerDied,
            questPart_BeggarsInteractions.outSignalDestroyed_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerArrested,
            questPart_BeggarsInteractions.outSignalArrested_BadThought);
        quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerSurgicallyViolated,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_BadThought);

        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalDestroyed_AssaultColony, QuestPart.SignalListenMode.OngoingOnly,
            true);
        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalDestroyed_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Destroyed);
        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalArrested_AssaultColony, QuestPart.SignalListenMode.OngoingOnly,
            true);
        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalArrested_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Arrested);
        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_AssaultColony,
            QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Fail, 0, null,
            questPart_BeggarsInteractions.outSignalSurgeryViolation_LeaveColony, QuestPart.SignalListenMode.OngoingOnly,
            true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Kidnapped,
            QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Banished,
            QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Success, 0, null, questPart_BeggarsInteractions.outSignalLast_Recruited,
            QuestPart.SignalListenMode.OngoingOnly, true);
        quest.End(QuestEndOutcome.Success, 0, null,
            questPart_BeggarsInteractions.outSignalLast_LeftMapAllNotHealthy, QuestPart.SignalListenMode.OngoingOnly,
            true);

        quest.AnyColonistWithCharityPrecept(
            delegate { quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars_Betrayed); }, null,
            beggarArrestedOrRecruitedOrViolatedorDestroyedSignal);


        quest.AllPawnsDespawned(pawns, delegate
        {
            AddDelayedReward(quest, pawns, beggarFaction, StayDurationDays);

            QuestGen_End.End(quest, QuestEndOutcome.Success);
        }, null, beggarLeftSignal);
    }

    protected override bool TestRunInt(Slate slate)
    {
        var map = QuestGen_Get.GetMap();
        return Find.Storyteller.difficulty.allowViolentQuests && TryFindWalkInSpot(map, out var _);
    }

    private bool TryFindWalkInSpot(Map map, out IntVec3 spawnSpot)
    {
        if (CellFinder.TryFindRandomEdgeCellWith(c => !c.Fogged(map) && map.reachability.CanReachColony(c), map,
                CellFinder.EdgeRoadChance_Neutral, out spawnSpot))
        {
            return true;
        }

        return CellFinder.TryFindRandomEdgeCellWith(c => !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral,
                   out spawnSpot) ||
               CellFinder.TryFindRandomEdgeCellWith(_ => true, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
    }

    private bool CanUseFaction(Faction faction)
    {
        if (!faction.temporary && !faction.defeated && !faction.IsPlayer && faction.def.humanlikeFaction &&
            faction.def == FactionDefOf.TribeRough)
        {
            return faction.HostileTo(Faction.OfPlayer);
        }

        //var list = new List<FactionRelation>();
        //foreach (var item in Find.FactionManager.AllFactionsListForReading)
        //{
        //    if (!item.def.permanentEnemy)
        //    {
        //        list.Add(new FactionRelation
        //        {
        //            other = item,
        //            kind = FactionRelationKind.Neutral
        //        });
        //    }
        //}

        return false;
    }

    private Faction TryFindEnemyFaction()
    {
        Find.FactionManager.AllFactions.Where(CanUseFaction).TryRandomElement(out var enemyFaction);
        if (enemyFaction != null)
        {
            return enemyFaction;
        }

        var list = new List<FactionRelation>();
        foreach (var item in Find.FactionManager.AllFactionsListForReading)
        {
            if (!item.def.permanentEnemy)
            {
                list.Add(new FactionRelation
                {
                    other = item,
                    kind = FactionRelationKind.Hostile
                });
            }
        }

        enemyFaction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.TribeRough, list, true);
        enemyFaction.temporary = true;
        Find.FactionManager.Add(enemyFaction);

        return enemyFaction;
    }
}