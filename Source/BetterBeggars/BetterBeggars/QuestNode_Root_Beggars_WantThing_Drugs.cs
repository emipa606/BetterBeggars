// BetterBeggars.QuestNode_Root_Beggars_WantThing_Drugs
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars
{
	public class QuestNode_Root_Beggars_WantThing_Drugs : QuestNode_Root_Beggars_WantThing
	{

		private static List<ThingDef> AllowedDrugs = new List<ThingDef>();

		protected override void GetAllowedThings()
		{
			if (BetterBeggars_Mod.settings.flagYayo == true && !AllowedDrugs.Contains(BeggarDefOf.Yayo))
			{
				AllowedDrugs.Add(BeggarDefOf.Yayo);
			}
			else if (BetterBeggars_Mod.settings.flagYayo == false && AllowedDrugs.Contains(BeggarDefOf.Yayo))
			{
				AllowedDrugs.Remove(BeggarDefOf.Yayo);
			}
			if (BetterBeggars_Mod.settings.flagFlake == true && !AllowedDrugs.Contains(BeggarDefOf.Flake))
			{
				AllowedDrugs.Add(BeggarDefOf.Flake);
			}
			else if (BetterBeggars_Mod.settings.flagFlake == false && AllowedDrugs.Contains(BeggarDefOf.Flake))
			{
				AllowedDrugs.Remove(BeggarDefOf.Flake);
			}
			if (BetterBeggars_Mod.settings.flagLuciferium == true && !AllowedDrugs.Contains(ThingDefOf.Luciferium))
			{
				AllowedDrugs.Add(ThingDefOf.Luciferium);
			}
			else if (BetterBeggars_Mod.settings.flagLuciferium == false && AllowedDrugs.Contains(ThingDefOf.Luciferium))
			{
				AllowedDrugs.Remove(ThingDefOf.Luciferium);
			}
			if (BetterBeggars_Mod.settings.flagSmokeleafJoint == true && !AllowedDrugs.Contains(ThingDefOf.SmokeleafJoint))
			{
				AllowedDrugs.Add(ThingDefOf.SmokeleafJoint);
			}
			else if (BetterBeggars_Mod.settings.flagSmokeleafJoint == false && AllowedDrugs.Contains(ThingDefOf.SmokeleafJoint))
			{
				AllowedDrugs.Remove(ThingDefOf.SmokeleafJoint);
			}
			if (BetterBeggars_Mod.settings.flagBeer == true && !AllowedDrugs.Contains(ThingDefOf.Beer))
			{
				AllowedDrugs.Add(ThingDefOf.Beer);
			}
			else if (BetterBeggars_Mod.settings.flagBeer == false && AllowedDrugs.Contains(ThingDefOf.Beer))
			{
				AllowedDrugs.Remove(ThingDefOf.Beer);
			}
		}

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
			slate.Set("visitDurationTicks", VisitDuration);
			slate.Set("valueFactor", BeggarRequestValueFactor);
			GetAllowedThings();
			BeggarRequestValueFactor = BetterBeggars_Mod.settings.BeggarRequestValueMultiplier;
			if (TryFindRandomRequestedThing(map, num * BeggarRequestValueFactor, out var thingDef, out var count, AllowedDrugs))
			{
				slate.Set("requestedThing", thingDef);
				slate.Set("requestedThingDefName", thingDef.defName);
				slate.Set("requestedThingCount", count);
			}
			CompProperties_Drug drugProperties = thingDef.GetCompProperties<CompProperties_Drug>();
			ChemicalDef chemicalDef = drugProperties.chemical;
			HediffDef hediffDef_Addiction = chemicalDef.addictionHediff;
			NeedDef needDef = hediffDef_Addiction.causesNeed;

			hediffDef_Addiction.initialSeverity = Rand.Range(0.20f, 0.80f);

			int colonyPopulation = (slate.Exists("population") ? slate.Get("population", 0) : map.mapPawns.FreeColonistsSpawnedCount);
			int numOfBeggars = Mathf.Max(Mathf.RoundToInt(LodgerCountBasedOnColonyPopulationFactorRange.RandomInRange * (float)colonyPopulation), 1);
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
			Faction faction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, hidden: true);
			faction.temporary = true;
			Find.FactionManager.Add(faction);
			slate.Set("faction", faction);
			List<Pawn> pawns = new List<Pawn>();
			for (int i = 0; i < numOfBeggars; i++)
			{
				pawns.Add(quest.GeneratePawn(PawnKindDefOf.Beggar, faction, allowAddictions: true, null, 0f, mustBeCapableOfViolence: true, null, 0f, 0f, ensureNonNumericName: false, forceGenerateNewPawn: true));
			}

			slate.Set("beggars", pawns);
			slate.Set("beggarCount", numOfBeggars);
			quest.SetFactionHidden(faction);
			quest.PawnsArrive(pawns, null, map.Parent, null, joinPlayer: false, null, null, null, null, null, isSingleReward: false, rewardDetailsHidden: false, sendStandardLetter: false);

			QuestPart_AddHediff questPart_AddHediff = new QuestPart_AddHediff();
			questPart_AddHediff.inSignal = QuestGen.slate.Get<string>("inSignal");
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

			string itemsReceivedSignal = QuestGen.GenerateNewSignal("ItemsReceived");
			QuestPart_BegForItems questPart_BegForItems = new QuestPart_BegForItems();
			questPart_BegForItems.inSignal = QuestGen.slate.Get<string>("inSignal");
			questPart_BegForItems.outSignalItemsReceived = itemsReceivedSignal;
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
			string pawnLabelSingleOrPlural = ((numOfBeggars > 1) ? faction.def.pawnsPlural : faction.def.pawnSingular);
			quest.Delay(60000, delegate
			{
				quest.Leave(pawns, null, sendStandardLetter: false, leaveOnCleanup: false);
				quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars);
				quest.AnyColonistWithCharityPrecept(delegate
				{
					quest.Message(string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(), "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)), MessageTypeDefOf.NegativeEvent, getLookTargetsFromSignal: false, null, pawns);
				});
			}, null, itemsReceivedSignal);
			string beggarArrestedSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Arrested");
			string beggarKilledSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.Killed");
			string beggarLeftSignal = QuestGenUtility.HardcodedSignalWithQuestID("beggars.LeftMap");
			quest.AnyColonistWithCharityPrecept(delegate
			{
				quest.Message("MessageCharityEventFulfilled".Translate() + ": " + "MessageBeggarsLeavingWithItems".Translate(pawnLabelSingleOrPlural), MessageTypeDefOf.PositiveEvent, getLookTargetsFromSignal: false, null, pawns);
				quest.RecordHistoryEvent(HistoryEventDefOf.CharityFulfilled_Beggars);
			}, delegate
			{
				quest.Message("MessageBeggarsLeavingWithItems".Translate(pawnLabelSingleOrPlural), MessageTypeDefOf.PositiveEvent, getLookTargetsFromSignal: false, null, pawns);
			}, itemsReceivedSignal);
			quest.AnySignal(new string[2] { beggarKilledSignal, beggarArrestedSignal }, delegate
			{
				quest.SignalPassActivable(delegate
				{
					quest.AnyColonistWithCharityPrecept(delegate
					{
						quest.Message(string.Format("{0}: {1}", "MessageCharityEventRefused".Translate(), "MessageBeggarsLeavingWithNoItems".Translate(pawnLabelSingleOrPlural)), MessageTypeDefOf.NegativeEvent, getLookTargetsFromSignal: false, null, pawns);
					});
				}, null, null, null, null, itemsReceivedSignal);
				quest.Letter(LetterDefOf.NegativeEvent, null, null, null, null, useColonistsFromCaravanArg: false, QuestPart.SignalListenMode.OngoingOnly, pawns, filterDeadPawnsFromLookTargets: false, "[letterTextBeggarsBetrayed]", null, "[letterLabelBeggarsBetrayed]");
				QuestPart_FactionRelationChange part = new QuestPart_FactionRelationChange
				{
					faction = faction,
					relationKind = FactionRelationKind.Hostile,
					canSendHostilityLetter = false,
					inSignal = QuestGen.slate.Get<string>("inSignal")
				};
				quest.AddPart(part);
				quest.RecordHistoryEvent(HistoryEventDefOf.CharityRefused_Beggars_Betrayed);
			});
			quest.End(QuestEndOutcome.Fail, 0, null, QuestGenUtility.HardcodedSignalWithQuestID("faction.BecameHostileToPlayer"));
			quest.AllPawnsDespawned(pawns, delegate
			{
				quest.SignalPassActivable(delegate
				{
					AddDelayedReward(quest, pawns, faction, -1, 0.5f);
				}, null, null, null, null, itemsReceivedSignal);
				QuestGen_End.End(quest, QuestEndOutcome.Success);
			}, null, beggarLeftSignal);
		}

		protected override bool TestRunInt(Slate slate)
		{
			Map map = QuestGen_Get.GetMap();
			float num = slate.Get("points", 0f);
			if (!FactionDefOf.Beggars.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp))
			{
				return false;
			}
			GetAllowedThings();
			for (int i = 0; i < AllowedDrugs.Count; i++)
            {
				slate.Set(AllowedDrugs[i].label, "yes");
            }
			if (AllowedDrugs.Count == 0)
			{
				return false;
			}
			ThingDef thingDef;
			int count;
			return TryFindRandomRequestedThing(map, num * 0.85f, out thingDef, out count, AllowedDrugs);
		}

    }
}
