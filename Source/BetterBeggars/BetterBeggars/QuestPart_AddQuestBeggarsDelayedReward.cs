// BetterBeggars.QuestPart_AddQuestBeggarsDelayedReward
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars
{
	public class QuestPart_AddQuestBeggarsDelayedReward : QuestPart_AddQuest
	{
		public List<Pawn> beggars = new List<Pawn>();

		public FloatRange marketValueRange;

		public Faction beggarFaction;

		public string inSignalRemovePawn;

		public override QuestScriptDef QuestDef => QuestScriptDefOf.RefugeeDelayedReward;

		public override Slate GetSlate()
		{
			Slate slate = new Slate();
			slate.Set("marketValueRange", marketValueRange);
			slate.Set("beggarFaction", beggarFaction);
			for (int i = 0; i < beggars.Count; i++)
			{
				if (!beggars[i].Dead && beggars[i].Faction != Faction.OfPlayer && !beggars[i].IsPrisoner)
				{
					slate.Set("rewardGiver", beggars[i]);
					break;
				}
			}
			return slate;
		}

		public override void Notify_FactionRemoved(Faction f)
		{
			if (beggarFaction == f)
			{
				beggarFaction = null;
			}
		}

		public override void Notify_QuestSignalReceived(Signal signal)
		{
			base.Notify_QuestSignalReceived(signal);
			if (signal.tag == inSignalRemovePawn && signal.args.TryGetArg("SUBJECT", out Pawn arg) && beggars.Contains(arg))
			{
				beggars.Remove(arg);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref beggarFaction, "beggarFaction");
			Scribe_Collections.Look(ref beggars, "beggars", LookMode.Reference);
			Scribe_Values.Look(ref inSignalRemovePawn, "inSignalRemovePawn");
			Scribe_Values.Look(ref marketValueRange, "marketValueRange");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				beggars.RemoveAll((Pawn x) => x == null);
			}
		}
	}
}

