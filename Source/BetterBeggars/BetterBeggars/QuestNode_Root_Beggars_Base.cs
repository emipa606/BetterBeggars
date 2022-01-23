// BetterBeggars.QuestNode_Root_Beggars_Base
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;

namespace BetterBeggars
{
    public abstract class QuestNode_Root_Beggars_Base : QuestNode
    {
        public static FloatRange LodgerCountBasedOnColonyPopulationFactorRange = new FloatRange(0.3f, 1f);

        public float BeggarRequestValueFactor = BetterBeggars_Mod.settings.BeggarRequestValueMultiplier;

        public static void AddDelayedReward(Quest quest, IEnumerable<Pawn> pawns, Faction faction, int DurationDays = -1, float chance = 0.5f)
        {
            if (Rand.Chance(chance))
            {
                int pawnCount = pawns.Count();
                float num2 = (float)(pawnCount * (DurationDays == -1 ? 1 : DurationDays)) * 55f;
                FloatRange marketValueRange = new FloatRange(0.7f, 1.3f) * num2 * Find.Storyteller.difficulty.EffectiveQuestRewardValueFactor;
                quest.AddQuestBeggarsDelayedReward(quest.AccepterPawn, faction, pawns, marketValueRange);
            }
        }
    }
}
