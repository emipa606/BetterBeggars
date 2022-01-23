// BetterBeggars.QuestGen_Misc
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars
{
	public static class QuestGen_Misc
    {
		public static QuestPart_AddQuestBeggarsDelayedReward AddQuestBeggarsDelayedReward(this Quest quest, Pawn acceptee, Faction beggarFaction, IEnumerable<Pawn> pawns, FloatRange marketValueRange, string inSignalRemovePawn = null)
		{
			QuestPart_AddQuestBeggarsDelayedReward questPart_AddQuestBeggarsDelayedReward = new QuestPart_AddQuestBeggarsDelayedReward();
			questPart_AddQuestBeggarsDelayedReward.acceptee = quest.AccepterPawn;
			questPart_AddQuestBeggarsDelayedReward.inSignal = QuestGen.slate.Get<string>("inSignal");
			questPart_AddQuestBeggarsDelayedReward.inSignalRemovePawn = inSignalRemovePawn;
			questPart_AddQuestBeggarsDelayedReward.beggarFaction = beggarFaction;
			questPart_AddQuestBeggarsDelayedReward.beggars.AddRange(pawns);
			questPart_AddQuestBeggarsDelayedReward.marketValueRange = marketValueRange;
			quest.AddPart(questPart_AddQuestBeggarsDelayedReward);
			return questPart_AddQuestBeggarsDelayedReward;
		}
}

}