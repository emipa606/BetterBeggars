// BetterBeggars.QuestNode_Root_Beggars_Base

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public abstract class QuestNode_Root_Beggars_Base : QuestNode
{
    public static FloatRange LodgerCountBasedOnColonyPopulationFactorRange = new FloatRange(0.3f, 1f);

    public float BeggarRequestValueFactor = BetterBeggars_Mod.settings.BeggarRequestValueMultiplier;

    public static void AddDelayedReward(Quest quest, IEnumerable<Pawn> pawns, Faction faction, int DurationDays = -1,
        float chance = 0.5f)
    {
        if (!Rand.Chance(chance))
        {
            return;
        }

        var pawnCount = pawns.Count();
        var num2 = pawnCount * (DurationDays == -1 ? 1 : DurationDays) * 55f;
        var marketValueRange = new FloatRange(0.7f, 1.3f) * num2 *
                               Find.Storyteller.difficulty.EffectiveQuestRewardValueFactor;
        quest.AddQuestBeggarsDelayedReward(faction, pawns, marketValueRange);
    }
}