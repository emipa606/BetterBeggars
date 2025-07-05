// BetterBeggars.QuestNode_LeavePlayer

using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public class QuestNode_LeavePlayer : QuestNode
{
    [NoTranslate] private SlateRef<string> inSignal;

    private SlateRef<IEnumerable<Pawn>> pawns;

    private SlateRef<Faction> replacementFaction;

    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        var slate = QuestGen.slate;
        if (pawns.GetValue(slate) == null)
        {
            return;
        }

        var questPartLeavePlayer = new QuestPart_LeavePlayer
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ??
                       QuestGen.slate.Get<string>("inSignal"),
            replacementFaction = replacementFaction.GetValue(slate)
        };
        questPartLeavePlayer.pawns.AddRange(pawns.GetValue(slate));
        QuestGen.quest.AddPart(questPartLeavePlayer);
    }
}