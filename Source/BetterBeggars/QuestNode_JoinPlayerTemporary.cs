// BetterBeggars.QuestNode_JoinPlayerTemporary

using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public class QuestNode_JoinPlayerTemporary : QuestNode
{
    private SlateRef<int> durationTicks = 60000;

    [NoTranslate] private SlateRef<string> inSignal;

    private SlateRef<bool> joinPlayer;

    private SlateRef<bool> makePrisoners;

    private SlateRef<IEnumerable<Pawn>> pawns;

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

        var previousFaction = pawns.GetValue(slate).FirstOrFallback().Faction;
        // Join Player
        var questPartJoinPlayer = new QuestPart_JoinPlayer
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ??
                       QuestGen.slate.Get<string>("inSignal"),
            joinPlayer = joinPlayer.GetValue(slate),
            makePrisoners = makePrisoners.GetValue(slate),
            mapParent = QuestGen.slate.Get<Map>("map").Parent
        };
        questPartJoinPlayer.pawns.AddRange(pawns.GetValue(slate));
        QuestGen.quest.AddPart(questPartJoinPlayer);

        // Delay and leave

        QuestGen.quest.Delay(durationTicks.GetValue(slate), delegate
        {
            var questPartLeavePlayer = new QuestPart_LeavePlayer
            {
                pawns = (List<Pawn>)pawns.GetValue(slate),
                replacementFaction = previousFaction
            };
            QuestGen.quest.AddPart(questPartLeavePlayer);

            var questPartLeave = new QuestPart_Leave
            {
                pawns = (List<Pawn>)pawns.GetValue(slate),
                sendStandardLetter = true,
                leaveOnCleanup = false
            };
            QuestGen.quest.AddPart(questPartLeave);
        });
    }
}