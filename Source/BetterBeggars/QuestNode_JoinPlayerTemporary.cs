// BetterBeggars.QuestNode_JoinPlayerTemporary

using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public class QuestNode_JoinPlayerTemporary : QuestNode
{
    public SlateRef<int> durationTicks = 60000;

    [NoTranslate] public SlateRef<string> inSignal;

    public SlateRef<bool> joinPlayer;

    public SlateRef<bool> makePrisoners;

    public SlateRef<IEnumerable<Pawn>> pawns;

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
        var questPart_JoinPlayer = new QuestPart_JoinPlayer
        {
            inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ??
                       QuestGen.slate.Get<string>("inSignal"),
            joinPlayer = joinPlayer.GetValue(slate),
            makePrisoners = makePrisoners.GetValue(slate),
            mapParent = QuestGen.slate.Get<Map>("map").Parent
        };
        questPart_JoinPlayer.pawns.AddRange(pawns.GetValue(slate));
        QuestGen.quest.AddPart(questPart_JoinPlayer);

        // Delay and leave

        QuestGen.quest.Delay(durationTicks.GetValue(slate), delegate
        {
            var questPart_LeavePlayer = new QuestPart_LeavePlayer
            {
                pawns = (List<Pawn>)pawns.GetValue(slate),
                replacementFaction = previousFaction
            };
            QuestGen.quest.AddPart(questPart_LeavePlayer);

            var questPart_Leave = new QuestPart_Leave
            {
                pawns = (List<Pawn>)pawns.GetValue(slate),
                sendStandardLetter = true,
                leaveOnCleanup = false
            };
            QuestGen.quest.AddPart(questPart_Leave);
        });
    }
}