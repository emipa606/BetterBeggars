// BetterBeggars.QuestNode_CreateFaction
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
    public class QuestNode_CreateFaction : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeAs;

        public SlateRef<FactionDef> factionDef;

        public SlateRef<int> defaultFactionRelations = 1;

        public SlateRef<bool> isHidden = false;

        public SlateRef<bool> isTemporary = false;


        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            List<FactionRelation> list = new List<FactionRelation>();
            foreach (Faction item in Find.FactionManager.AllFactionsListForReading)
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
            Faction faction = FactionGenerator.NewGeneratedFactionWithRelations(FactionDefOf.Beggars, list, hidden: isHidden.GetValue(slate));
            faction.temporary = isTemporary.GetValue(slate);
            Find.FactionManager.Add(faction);
            QuestGen.slate.Set(storeAs.GetValue(slate), faction);
        }

        private FactionRelationKind GetFactionRelationKind(int i)
        {
            if (i == 0)
            {
                return FactionRelationKind.Hostile;
            }
            if (i == 1)
            {
                return FactionRelationKind.Neutral;
            }
            if (i == 2)
            {
                return FactionRelationKind.Ally;
            }
            return FactionRelationKind.Neutral;
        }

        
    }
}
