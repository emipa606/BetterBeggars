// BetterBeggars.QuestNode_LeavePlayer
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars
{
	public class QuestNode_LeavePlayer : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> inSignal;

		public SlateRef<IEnumerable<Pawn>> pawns;

		public SlateRef<Faction> replacementFaction;

		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			if (pawns.GetValue(slate) != null)
			{
				QuestPart_LeavePlayer questPart_LeavePlayer = new QuestPart_LeavePlayer();
				questPart_LeavePlayer.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
				questPart_LeavePlayer.replacementFaction = replacementFaction.GetValue(slate);
				questPart_LeavePlayer.pawns.AddRange(pawns.GetValue(slate));
				QuestGen.quest.AddPart(questPart_LeavePlayer);
			}
		}
	}
}
