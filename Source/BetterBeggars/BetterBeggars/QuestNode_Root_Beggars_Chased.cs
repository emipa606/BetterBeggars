// BetterBeggars.QuestNode_Root_Beggars_Chased
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars
{
    class QuestNode_Root_Beggars_Chased : QuestNode_Root_Beggars_Base
	{
		public const int JoinDelay = 1000;
        public const int RaidDelay = 2000;
		public static int StayDurationDays = UnityEngine.Random.Range(3, 6);
		public static int StayDurationTicks = StayDurationDays * 60000;
		private static FloatRange MutinyTimeRange = new FloatRange(0.2f, 1f);

		protected override void RunInt()
		{
			if (!ModLister.CheckIdeology("Beggars"))
			{
				return;
			}
			Quest quest = QuestGen.quest;
			Slate slate = QuestGen.slate;
			Map map = QuestGen_Get.GetMap();
			float num = slate.Get("points", 0f);
			slate.Set("map", map);
			slate.Set("stayDurationTicks", StayDurationTicks);

			slate.Set("stayDurationDays", StayDurationDays);

			int colonyPopulation = (slate.Exists("population") ? slate.Get("population", 0) : map.mapPawns.FreeColonistsSpawnedCount);
			int beggarCount = Mathf.Max(Mathf.RoundToInt(LodgerCountBasedOnColonyPopulationFactorRange.RandomInRange * (float)colonyPopulation), 1);
			List<FactionRelation> list = new List<FactionRelation>();
			foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
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
			Faction beggarFaction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, hidden: true);
			beggarFaction.temporary = true;
			Find.FactionManager.Add(beggarFaction);
			slate.Set("beggarFaction", beggarFaction);

			List<Pawn> pawns = new List<Pawn>();
			for (int i = 0; i < beggarCount; i++)
			{
				pawns.Add(quest.GeneratePawn(PawnKindDefOf.Beggar, beggarFaction, allowAddictions: true, null, 0f, mustBeCapableOfViolence: true, null, 0f, 0f, ensureNonNumericName: false, forceGenerateNewPawn: true));
			}


			slate.Set("beggars", pawns);
			slate.Set("beggarCount", beggarCount);

			beggarFaction.leader = pawns.First();
			quest.SetFactionHidden(beggarFaction, hidden: true);

			string beggarRecruitedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Recruited");

			QuestPart_ExtraFaction extraFactionPart = quest.ExtraFaction(beggarFaction, pawns, ExtraFactionType.MiniFaction, areHelpers: false, beggarRecruitedSignal);

			QuestPart_Choice questPart_Choice = quest.RewardChoice();
			QuestPart_Choice.Choice choice = new QuestPart_Choice.Choice
			{
				rewards =
				{
					(Reward)new Reward_VisitorsHelp(),
					(Reward)new Reward_PossibleFutureReward()
				}
			};
			questPart_Choice.choices.Add(choice);

			TryFindWalkInSpot(map, out IntVec3 walkInSpot);

			quest.Delay(JoinDelay, delegate
			{
				quest.PawnsArrive(pawns, null, map.Parent, null, joinPlayer: true, walkInSpot, "[beggarsArriveLetterLabel]", "[beggarsArriveLetterText]");
			});

			
			quest.SetAllApparelLocked(pawns);

			string beggarArrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Arrested");
			string beggarKilledSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Killed");
			string beggarDestroyedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Destroyed");
			string beggarKidnappedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Kidnapped");
			string beggarLeftSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.LeftMap");
			string beggarSurgeryViolatedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.SurgeryViolation");
			string beggarArrestedOrRecruitedSignal = QuestGen.GenerateNewSignal("beggar_ArrestedOrRecruited");
			string beggarArrestedOrRecruitedOrViolatedorDestroyedSignal = QuestGen.GenerateNewSignal("beggar_ArrestedOrRecruitedOrViolatedOrDestroyed");
			string beggarBanishedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Banished");
			string assaultColonySignal = QuestGen.GenerateNewSignal("AssaultColony");
			quest.AnySignal(new List<string> { beggarRecruitedSignal, beggarArrestedSignal }, null, new List<string> { beggarArrestedOrRecruitedSignal });
			quest.AnySignal(new List<string> { beggarRecruitedSignal, beggarArrestedSignal, beggarSurgeryViolatedSignal, beggarDestroyedSignal }, null, new List<string> { beggarArrestedOrRecruitedOrViolatedorDestroyedSignal });

			Action mutiny = delegate
			{
				int mutinyTimeTicks = Mathf.FloorToInt(MutinyTimeRange.RandomInRange * (float)StayDurationTicks);
				quest.Delay(mutinyTimeTicks, delegate
				{
					quest.Letter(LetterDefOf.ThreatBig, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarMutinyLetterText]", null, "[beggarMutinyLetterLabel]");
					quest.SignalPass(null, null, assaultColonySignal);
					QuestGen_End.End(quest, QuestEndOutcome.Unknown);
				}, null, null, null, reactivatable: false, null, null, isQuestTimeout: false, null, null, "Mutiny (" + mutinyTimeTicks.ToStringTicksToDays() + ")");
			};

			Faction enemyFaction = TryFindEnemyFaction();

			if (!enemyFaction.HostileTo(beggarFaction))
            {
				FactionRelation beggarEnemyRelation = new FactionRelation();
				beggarEnemyRelation.other = beggarFaction;
				beggarEnemyRelation.kind = FactionRelationKind.Hostile;
				enemyFaction.SetRelation(beggarEnemyRelation);
            }
			slate.Set("enemyFaction", enemyFaction);

			string stayDoneSignal = QuestGen.GenerateNewSignal("stayDone");

			quest.Delay(RaidDelay, delegate
			{
				quest.Raid(map, num, enemyFaction, walkInSpot: walkInSpot, customLetterLabel: "{BASELABEL} chasing beggars");

				/*
				QuestPart_JoinPlayer questPart_JoinPlayer = new QuestPart_JoinPlayer();
				questPart_JoinPlayer.pawns.AddRange(pawns);
				questPart_JoinPlayer.joinPlayer = true;
				questPart_JoinPlayer.makePrisoners = false;
				quest.AddPart(questPart_JoinPlayer);*/
			}, null, null, stayDoneSignal);

			string pawnLabelSingleOrPlural = ((beggarCount > 1) ? beggarFaction.def.pawnsPlural : beggarFaction.def.pawnSingular);


			quest.Delay(StayDurationTicks, delegate
			{
				quest.SignalPassWithFaction(beggarFaction, null, delegate
				{
					quest.AnyColonistWithCharityPrecept(delegate
					{
						quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, ("MessageCharityEventFulfilled".Translate() + ": " + "[beggarsLeavingLetterText]"), null, "[beggarsLeavingLetterLabel]");
						quest.RecordHistoryEvent(HistoryEventDefOf.CharityFulfilled_Beggars);
					}, delegate
					{
						quest.Letter(LetterDefOf.PositiveEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarsLeavingLetterText]", null, "[beggarsLeavingLetterLabel]");
					});
					
				});


				quest.Leave(pawns, null, sendStandardLetter: false, leaveOnCleanup: false, beggarArrestedOrRecruitedSignal, wakeUp: true);
			}, null, null, null, reactivatable: false, null, null, isQuestTimeout: false, "GuestsDepartsIn".Translate(), "GuestsDepartsOn".Translate(), "StayDelay");

			quest.End(QuestEndOutcome.Fail, 0, null, QuestGenUtility.HardcodedSignalWithQuestID("faction.BecameHostileToPlayer"));

			

			QuestPart_BeggarsInteractions questPart_BeggarsInteractions = new QuestPart_BeggarsInteractions();
			questPart_BeggarsInteractions.inSignalEnable = QuestGen.slate.Get<string>("inSignal");
			questPart_BeggarsInteractions.inSignalDestroyed = beggarDestroyedSignal;
			questPart_BeggarsInteractions.inSignalArrested = beggarArrestedSignal;
			questPart_BeggarsInteractions.inSignalSurgeryViolation = beggarSurgeryViolatedSignal;
			questPart_BeggarsInteractions.inSignalKidnapped = beggarKidnappedSignal;
			questPart_BeggarsInteractions.inSignalRecruited = beggarRecruitedSignal;
			questPart_BeggarsInteractions.inSignalAssaultColony = assaultColonySignal;
			questPart_BeggarsInteractions.inSignalLeftMap = beggarLeftSignal;
			questPart_BeggarsInteractions.inSignalBanished = beggarBanishedSignal;
			questPart_BeggarsInteractions.outSignalDestroyed_AssaultColony = QuestGen.GenerateNewSignal("BeggarDestroyed_AssaultColony");
			questPart_BeggarsInteractions.outSignalDestroyed_LeaveColony = QuestGen.GenerateNewSignal("BeggarDestroyed_LeaveColony");
			questPart_BeggarsInteractions.outSignalDestroyed_BadThought = QuestGen.GenerateNewSignal("BeggarDestroyed_BadThought");
			questPart_BeggarsInteractions.outSignalArrested_AssaultColony = QuestGen.GenerateNewSignal("BeggarArrested_AssaultColony");
			questPart_BeggarsInteractions.outSignalArrested_LeaveColony = QuestGen.GenerateNewSignal("BeggarArrested_LeaveColony");
			questPart_BeggarsInteractions.outSignalArrested_BadThought = QuestGen.GenerateNewSignal("BeggarArrested_BadThought");
			questPart_BeggarsInteractions.outSignalSurgeryViolation_AssaultColony = QuestGen.GenerateNewSignal("BeggarSurgeryViolation_AssaultColony");
			questPart_BeggarsInteractions.outSignalSurgeryViolation_LeaveColony = QuestGen.GenerateNewSignal("BeggarSurgeryViolation_LeaveColony");
			questPart_BeggarsInteractions.outSignalSurgeryViolation_BadThought = QuestGen.GenerateNewSignal("BeggarSurgeryViolation_BadThought");
			questPart_BeggarsInteractions.outSignalLast_Destroyed = QuestGen.GenerateNewSignal("LastBeggar_Destroyed");
			questPart_BeggarsInteractions.outSignalLast_Arrested = QuestGen.GenerateNewSignal("LastBeggar_Arrested");
			questPart_BeggarsInteractions.outSignalLast_Kidnapped = QuestGen.GenerateNewSignal("LastBeggar_Kidnapped");
			questPart_BeggarsInteractions.outSignalLast_Recruited = QuestGen.GenerateNewSignal("LastBeggar_Recruited");
			questPart_BeggarsInteractions.outSignalLast_LeftMapAllHealthy = QuestGen.GenerateNewSignal("LastBeggar_LeftMapAllHealthy");
			questPart_BeggarsInteractions.outSignalLast_LeftMapAllNotHealthy = QuestGen.GenerateNewSignal("LastBeggar_LeftMapAllNotHealthy");
			questPart_BeggarsInteractions.outSignalLast_Banished = QuestGen.GenerateNewSignal("LastBeggar_Banished");
			questPart_BeggarsInteractions.pawns.AddRange(pawns);
			questPart_BeggarsInteractions.faction = beggarFaction;
			questPart_BeggarsInteractions.mapParent = map.Parent;
			questPart_BeggarsInteractions.signalListenMode = QuestPart.SignalListenMode.Always;
			quest.AddPart(questPart_BeggarsInteractions);

			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalDestroyed_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarDiedMemoryThoughtLetterText]", null, "[beggarDiedMemoryThoughtLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalDestroyed_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarDiedAttackPlayerLetterText]", null, "[beggarDiedAttackPlayerLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalDestroyed_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarDiedLeaveMapLetterText]", null, "[beggarDiedLeaveMapLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalLast_Destroyed, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarsAllDiedLetterText]", null, "[beggarsAllDiedLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalArrested_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarArrestedMemoryThoughtLetterText]", null, "[beggarArrestedMemoryThoughtLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalArrested_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarArrestedAttackPlayerLetterText]", null, "[beggarArrestedAttackPlayerLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalArrested_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarArrestedLeaveMapLetterText]", null, "[beggarArrestedLeaveMapLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalLast_Arrested, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarsAllArrestedLetterText]", null, "[beggarsAllArrestedLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalSurgeryViolation_BadThought, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarViolatedMemoryThoughtLetterText]", null, "[beggarViolatedMemoryThoughtLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalSurgeryViolation_AssaultColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarViolatedAttackPlayerLetterText]", null, "[beggarViolatedAttackPlayerLetterLabel]");
			quest.Letter(LetterDefOf.NegativeEvent, questPart_BeggarsInteractions.outSignalSurgeryViolation_LeaveColony, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, null, filterDeadPawnsFromLookTargets: false, "[beggarViolatedLeaveMapLetterText]", null, "[beggarViolatedLeaveMapLetterLabel]");
			quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerDied, questPart_BeggarsInteractions.outSignalDestroyed_BadThought);
			quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerArrested, questPart_BeggarsInteractions.outSignalArrested_BadThought);
			quest.AddMemoryThought(pawns, ThoughtDefOf.OtherTravelerSurgicallyViolated, questPart_BeggarsInteractions.outSignalSurgeryViolation_BadThought);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalDestroyed_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalDestroyed_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Destroyed);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalArrested_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalArrested_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Arrested);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalSurgeryViolation_AssaultColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalSurgeryViolation_LeaveColony, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Kidnapped, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Fail, 0, null, questPart_BeggarsInteractions.outSignalLast_Banished, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Success, 0, null, questPart_BeggarsInteractions.outSignalLast_Recruited, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);
			quest.End(QuestEndOutcome.Success, 0, null, questPart_BeggarsInteractions.outSignalLast_LeftMapAllNotHealthy, QuestPart.SignalListenMode.OngoingOnly, sendStandardLetter: true);

			quest.AnyColonistWithCharityPrecept(delegate
			{
				quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars_Betrayed);
			}, null, beggarArrestedOrRecruitedOrViolatedorDestroyedSignal);



			quest.AllPawnsDespawned(pawns, delegate
			{
				AddDelayedReward(quest, pawns, beggarFaction, StayDurationDays, 0.5f);

				QuestGen_End.End(quest, QuestEndOutcome.Success);
			}, null, beggarLeftSignal);
		}

        protected override bool TestRunInt(Slate slate)
        {
			Map map = QuestGen_Get.GetMap();
			if (!Find.Storyteller.difficulty.allowViolentQuests || !TryFindWalkInSpot(map, out var _))
			{
				return false;
			}
			return true;
		}

		private bool TryFindWalkInSpot(Map map, out IntVec3 spawnSpot)
		{
			if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Fogged(map) && map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot))
			{
				return true;
			}
			if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot))
			{
				return true;
			}
			if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => true, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot))
			{
				return true;
			}
			return false;
		}

		private bool CanUseFaction(Faction faction)
		{
			if (!faction.temporary && !faction.defeated && !faction.IsPlayer && (faction.def.humanlikeFaction) && (faction.def == FactionDefOf.TribeRough))
			{
				return faction.HostileTo(Faction.OfPlayer);
			}
			List<FactionRelation> list = new List<FactionRelation>();
			foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
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
			return false;
		}

		private Faction TryFindEnemyFaction()
		{
			Faction enemyFaction = new Faction();
			Find.FactionManager.AllFactions.Where(CanUseFaction).TryRandomElement(out enemyFaction);
			if (enemyFaction == null)
            {
				List<FactionRelation> list = new List<FactionRelation>();
				foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
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
				enemyFaction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.TribeRough, list, hidden: true);
				enemyFaction.temporary = true;
				Find.FactionManager.Add(enemyFaction);
			}
			return enemyFaction;
		}

	}
}
