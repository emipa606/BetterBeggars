// BetterBeggars.QuestNode_CreateFaction

using System.Collections.Generic;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace BetterBeggars;

public class QuestNode_CreateFaction : QuestNode
{
    public SlateRef<int> defaultFactionRelations = 1;

    public SlateRef<FactionDef> factionDef;

    public SlateRef<bool> isHidden = false;

    public SlateRef<bool> isTemporary = false;

    [NoTranslate] public SlateRef<string> storeAs;


    protected override bool TestRunInt(Slate slate)
    {
        return true;
    }

    protected override void RunInt()
    {
        var slate = QuestGen.slate;
        var list = new List<FactionRelation>();
        foreach (var item in Find.FactionManager.AllFactionsListForReading)
        {
            if (!item.def.permanentEnemy)
            {
                list.Add(new FactionRelation
                {
                    other = item,
                    kind = GetFactionRelationKind(defaultFactionRelations.GetValue(slate))
                });
            }
        }

        var faction =
            FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, isHidden.GetValue(slate));
        faction.temporary = isTemporary.GetValue(slate);
        Find.FactionManager.Add(faction);
        QuestGen.slate.Set(storeAs.GetValue(slate), faction);
    }

    private FactionRelationKind GetFactionRelationKind(int i)
    {
        switch (i)
        {
            case 0:
                return FactionRelationKind.Hostile;
            case 1:
                return FactionRelationKind.Neutral;
            case 2:
                return FactionRelationKind.Ally;
            default:
                return FactionRelationKind.Neutral;
        }
    }
}