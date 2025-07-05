// BetterBeggars.QuestNode_Root_Beggars_DelayedReward

using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

internal class QuestNode_Root_Beggars_DelayedReward : QuestNode
{
    protected override void RunInt()
    {
        var quest = QuestGen.quest;
        var slate = QuestGen.slate;
        var map = QuestGen_Get.GetMap();
        var faction = slate.Get<Faction>("faction");
        var marketValueRange = slate.Get<FloatRange>("marketValueRange");
        var val = slate.Get<Pawn>("rewardGiver");
        quest.ReservePawns(Gen.YieldSingle(val));
        quest.ReserveFaction(faction);
        var num = Rand.Range(5, 20) * 60000;
        slate.Set("rewardDelayTicks", num);
        quest.Delay(num, delegate
        {
            var parms = default(ThingSetMakerParams);
            parms.totalMarketValueRange = marketValueRange;
            parms.qualityGenerator = QualityGenerator.Reward;
            parms.makingFaction = faction;
            var list = ThingSetMakerDefOf.Reward_ItemsStandard.root.Generate(parms);
            slate.Set("listOfRewards", GenLabel.ThingsLabel(list));
            quest.DropPods(map.Parent, list, null, null, "[rewardLetterText]");
            QuestGen_End.End(quest, QuestEndOutcome.Unknown);
        }, null, null, null, false, null, null, false, null, null, "RewardDelay");
    }

    protected override bool TestRunInt(Slate slate)
    {
        if (slate.Get<Pawn>("rewardGiver") != null && slate.TryGet<FloatRange>("marketValueRange", out _))
        {
            return slate.Get<Faction>("faction") != null;
        }

        return false;
    }
}