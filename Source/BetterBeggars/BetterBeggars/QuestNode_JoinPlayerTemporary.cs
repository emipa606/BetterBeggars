// BetterBeggars.QuestNode_JoinPlayerTemporary
using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars
{
	public class QuestNode_JoinPlayerTemporary : QuestNode
	{
		[NoTranslate]
		public SlateRef<string> inSignal;

		public SlateRef<IEnumerable<Pawn>> pawns;

		public SlateRef<bool> joinPlayer;

		public SlateRef<bool> makePrisoners;

		public SlateRef<int> durationTicks = 60000;

		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			if (pawns.GetValue(slate) != null)
			{
				Faction previousFaction = pawns.GetValue(slate).FirstOrFallback().Faction;
				// Join Player
				QuestPart_JoinPlayer questPart_JoinPlayer = new QuestPart_JoinPlayer();
				questPart_JoinPlayer.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
				questPart_JoinPlayer.joinPlayer = joinPlayer.GetValue(slate);
				questPart_JoinPlayer.makePrisoners = makePrisoners.GetValue(slate);
				questPart_JoinPlayer.mapParent = QuestGen.slate.Get<Map>("map").Parent;
				questPart_JoinPlayer.pawns.AddRange(pawns.GetValue(slate));
				QuestGen.quest.AddPart(questPart_JoinPlayer);

				// Delay and leave
				
				QuestGen.quest.Delay(durationTicks.GetValue(slate), delegate
				{
					QuestPart_LeavePlayer questPart_LeavePlayer = new QuestPart_LeavePlayer();
					questPart_LeavePlayer.pawns = (List<Pawn>)pawns.GetValue(slate);
					questPart_LeavePlayer.replacementFaction = previousFaction;
					QuestGen.quest.AddPart(questPart_LeavePlayer);

					QuestPart_Leave questPart_Leave = new QuestPart_Leave();
					questPart_Leave.pawns = (List<Pawn>)pawns.GetValue(slate);
					questPart_Leave.sendStandardLetter = true;
					questPart_Leave.leaveOnCleanup = false;
					QuestGen.quest.AddPart(questPart_Leave);
				});	
			}
		}
	}
}
