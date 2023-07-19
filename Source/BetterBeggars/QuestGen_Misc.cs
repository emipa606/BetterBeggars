// BetterBeggars.QuestGen_Misc

using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public static class QuestGen_Misc
{
    public static QuestPart_AddQuestBeggarsDelayedReward AddQuestBeggarsDelayedReward(this Quest quest, Pawn acceptee,
        Faction beggarFaction, IEnumerable<Pawn> pawns, FloatRange marketValueRange, string inSignalRemovePawn = null)
    {
        var questPart_AddQuestBeggarsDelayedReward = new QuestPart_AddQuestBeggarsDelayedReward
        {
            acceptee = quest.AccepterPawn,
            inSignal = QuestGen.slate.Get<string>("inSignal"),
            inSignalRemovePawn = inSignalRemovePawn,
            beggarFaction = beggarFaction
        };
        questPart_AddQuestBeggarsDelayedReward.beggars.AddRange(pawns);
        questPart_AddQuestBeggarsDelayedReward.marketValueRange = marketValueRange;
        quest.AddPart(questPart_AddQuestBeggarsDelayedReward);
        return questPart_AddQuestBeggarsDelayedReward;
    }
}