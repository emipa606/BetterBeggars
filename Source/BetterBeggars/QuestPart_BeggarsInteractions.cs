// BetterBeggars.QuestPart_BeggarsInteractions

using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace BetterBeggars;

public class QuestPart_BeggarsInteractions : QuestPartActivable
{
    public Faction faction;

    public string inSignalArrested;

    public string inSignalAssaultColony;

    public string inSignalBanished;

    public string inSignalDestroyed;

    public string inSignalKidnapped;

    public string inSignalLeftMap;

    public string inSignalRecruited;

    public string inSignalSurgeryViolation;

    public MapParent mapParent;

    public string outSignalArrested_AssaultColony;

    public string outSignalArrested_BadThought;

    public string outSignalArrested_LeaveColony;

    public string outSignalDestroyed_AssaultColony;

    public string outSignalDestroyed_BadThought;

    public string outSignalDestroyed_LeaveColony;

    public string outSignalLast_Arrested;

    public string outSignalLast_Banished;

    public string outSignalLast_Destroyed;

    public string outSignalLast_Kidnapped;

    public string outSignalLast_LeftMapAllHealthy;

    public string outSignalLast_LeftMapAllNotHealthy;

    public string outSignalLast_Recruited;

    public string outSignalSurgeryViolation_AssaultColony;

    public string outSignalSurgeryViolation_BadThought;

    public string outSignalSurgeryViolation_LeaveColony;

    public List<Pawn> pawns = [];

    private int pawnsLeftUnhealthy;

    protected override void ProcessQuestSignal(Signal signal)
    {
        if (signal.tag == inSignalRecruited && signal.args.TryGetArg("SUBJECT", out Pawn arg) && pawns.Contains(arg))
        {
            pawns.Remove(arg);
            if (pawns.Count == 0)
            {
                Find.SignalManager.SendSignal(new Signal(outSignalLast_Recruited, signal.args));
            }
        }

        if (signal.tag == inSignalKidnapped && signal.args.TryGetArg("SUBJECT", out Pawn arg2) && pawns.Contains(arg2))
        {
            pawns.Remove(arg2);
            if (pawns.Count == 0)
            {
                Find.SignalManager.SendSignal(new Signal(outSignalLast_Kidnapped, signal.args));
            }
        }

        if (signal.tag == inSignalBanished && signal.args.TryGetArg("SUBJECT", out Pawn arg3) && pawns.Contains(arg3))
        {
            pawns.Remove(arg3);
            if (pawns.Count == 0)
            {
                Find.SignalManager.SendSignal(new Signal(outSignalLast_Banished, signal.args));
            }
        }

        if (signal.tag == inSignalLeftMap && signal.args.TryGetArg("SUBJECT", out Pawn arg4) && pawns.Contains(arg4))
        {
            pawns.Remove(arg4);
            if (arg4.Destroyed || arg4.InMentalState || arg4.health.hediffSet.BleedRateTotal > 0.001f)
            {
                pawnsLeftUnhealthy++;
            }

            var num = pawns.Count(p => p.Downed);
            if (pawns.Count - num <= 0)
            {
                if (pawnsLeftUnhealthy > 0 || num > 0)
                {
                    pawns.Clear();
                    pawnsLeftUnhealthy += num;
                    Find.SignalManager.SendSignal(new Signal(outSignalLast_LeftMapAllNotHealthy, signal.args));
                }
                else
                {
                    Find.SignalManager.SendSignal(new Signal(outSignalLast_LeftMapAllHealthy, signal.args));
                }
            }
        }

        if (signal.tag == inSignalDestroyed && signal.args.TryGetArg("SUBJECT", out Pawn arg5) && pawns.Contains(arg5))
        {
            pawns.Remove(arg5);
            arg5.SetFaction(faction);
            if (pawns.Count == 0)
            {
                Find.SignalManager.SendSignal(new Signal(outSignalLast_Destroyed, signal.args));
            }
            else
            {
                signal.args.Add(pawns.Count.Named("PAWNSALIVECOUNT"));
                switch (ChooseRandomInteraction())
                {
                    case InteractionResponseType.AssaultColony:
                        AssaultColony(HistoryEventDefOf.QuestPawnLost);
                        Find.SignalManager.SendSignal(new Signal(outSignalDestroyed_AssaultColony, signal.args));
                        break;
                    case InteractionResponseType.Leave:
                        LeavePlayer();
                        Find.SignalManager.SendSignal(new Signal(outSignalDestroyed_LeaveColony, signal.args));
                        break;
                    case InteractionResponseType.BadThought:
                        Find.SignalManager.SendSignal(new Signal(outSignalDestroyed_BadThought, signal.args));
                        break;
                }
            }
        }

        if (signal.tag == inSignalArrested && signal.args.TryGetArg("SUBJECT", out Pawn arg6) && pawns.Contains(arg6))
        {
            pawns.Remove(arg6);
            var inAggroMentalState = arg6.InAggroMentalState;
            arg6.SetFaction(null);
            if (pawns.Count == 0)
            {
                Find.SignalManager.SendSignal(new Signal(outSignalLast_Arrested, signal.args));
            }
            else if (!inAggroMentalState)
            {
                signal.args.Add(pawns.Count.Named("PAWNSALIVECOUNT"));
                switch (ChooseRandomInteraction())
                {
                    case InteractionResponseType.AssaultColony:
                        AssaultColony(HistoryEventDefOf.QuestPawnArrested);
                        Find.SignalManager.SendSignal(new Signal(outSignalArrested_AssaultColony, signal.args));
                        break;
                    case InteractionResponseType.Leave:
                        LeavePlayer();
                        Find.SignalManager.SendSignal(new Signal(outSignalArrested_LeaveColony, signal.args));
                        break;
                    case InteractionResponseType.BadThought:
                        Find.SignalManager.SendSignal(new Signal(outSignalArrested_BadThought, signal.args));
                        break;
                }
            }
        }

        if (signal.tag == inSignalSurgeryViolation && signal.args.TryGetArg("SUBJECT", out Pawn arg7) &&
            pawns.Contains(arg7))
        {
            signal.args.Add(pawns.Count.Named("PAWNSALIVECOUNT"));
            switch (ChooseRandomInteraction())
            {
                case InteractionResponseType.AssaultColony:
                    AssaultColony(HistoryEventDefOf.PerformedHarmfulSurgery);
                    Find.SignalManager.SendSignal(new Signal(outSignalSurgeryViolation_AssaultColony, signal.args));
                    break;
                case InteractionResponseType.Leave:
                    LeavePlayer();
                    Find.SignalManager.SendSignal(new Signal(outSignalSurgeryViolation_LeaveColony, signal.args));
                    break;
                case InteractionResponseType.BadThought:
                    Find.SignalManager.SendSignal(new Signal(outSignalSurgeryViolation_BadThought, signal.args));
                    break;
            }
        }

        if (inSignalAssaultColony != null && signal.tag == inSignalAssaultColony)
        {
            AssaultColony(null);
        }
    }

    private void LeavePlayer()
    {
        foreach (var pawn in pawns)
        {
            if (faction != pawn.Faction)
            {
                pawn.SetFaction(faction);
            }
        }

        LeaveQuestPartUtility.MakePawnsLeave(pawns, false, quest);
        Complete();
    }

    private void AssaultColony(HistoryEventDef reason)
    {
        if (faction.HasGoodwill)
        {
            Faction.OfPlayer.TryAffectGoodwillWith(faction, Faction.OfPlayer.GoodwillToMakeHostile(faction), true,
                false, reason);
        }
        else
        {
            faction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile, false);
        }

        foreach (var pawn in pawns)
        {
            pawn.GetLord()?.Notify_PawnLost(pawn, PawnLostCondition.ForcedByQuest);
        }

        foreach (var pawn in pawns)
        {
            pawn.SetFaction(faction);
            if (!pawn.Awake())
            {
                RestUtility.WakeUp(pawn);
            }
        }

        var lord = LordMaker.MakeNewLord(faction,
            new LordJob_AssaultColony(faction, true, true, false, false, true, false, true), mapParent.Map);
        foreach (var pawn in pawns)
        {
            if (!pawn.Dead)
            {
                lord.AddPawn(pawn);
            }
        }

        Complete();
    }

    private InteractionResponseType ChooseRandomInteraction()
    {
        return Gen.RandomEnumValue<InteractionResponseType>(false);
    }

    public override void Notify_FactionRemoved(Faction f)
    {
        if (faction == f)
        {
            faction = null;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignalDestroyed, "inSignalDestroyed");
        Scribe_Values.Look(ref inSignalArrested, "inSignalArrested");
        Scribe_Values.Look(ref inSignalSurgeryViolation, "inSignalSurgeryViolation");
        Scribe_Values.Look(ref inSignalRecruited, "inSignalRecruited");
        Scribe_Values.Look(ref inSignalKidnapped, "inSignalKidnapped");
        Scribe_Values.Look(ref inSignalAssaultColony, "inSignalAssaultColony");
        Scribe_Values.Look(ref inSignalLeftMap, "inSignalLeftMap");
        Scribe_Values.Look(ref inSignalBanished, "inSignalBanished");
        Scribe_Values.Look(ref outSignalDestroyed_AssaultColony, "outSignalDestroyed_AssaultColony");
        Scribe_Values.Look(ref outSignalDestroyed_LeaveColony, "outSignalDestroyed_LeaveColony");
        Scribe_Values.Look(ref outSignalDestroyed_BadThought, "outSignalDestroyed_BadThought");
        Scribe_Values.Look(ref outSignalArrested_AssaultColony, "outSignalArrested_AssaultColony");
        Scribe_Values.Look(ref outSignalArrested_LeaveColony, "outSignalArrested_LeaveColony");
        Scribe_Values.Look(ref outSignalArrested_BadThought, "outSignalArrested_BadThought");
        Scribe_Values.Look(ref outSignalSurgeryViolation_AssaultColony, "outSignalSurgeryViolation_AssaultColony");
        Scribe_Values.Look(ref outSignalSurgeryViolation_LeaveColony, "outSignalSurgeryViolation_LeaveColony");
        Scribe_Values.Look(ref outSignalSurgeryViolation_BadThought, "outSignalSurgeryViolation_BadThought");
        Scribe_Values.Look(ref outSignalLast_Arrested, "outSignalLastArrested");
        Scribe_Values.Look(ref outSignalLast_Destroyed, "outSignalLastDestroyed");
        Scribe_Values.Look(ref outSignalLast_Kidnapped, "outSignalLastKidnapped");
        Scribe_Values.Look(ref outSignalLast_Recruited, "outSignalLastRecruited");
        Scribe_Values.Look(ref outSignalLast_LeftMapAllHealthy, "outSignalLastLeftMapAllHealthy");
        Scribe_Values.Look(ref outSignalLast_LeftMapAllNotHealthy, "outSignalLastLeftMapAllNotHealthy");
        Scribe_Values.Look(ref outSignalLast_Banished, "outSignalLast_Banished");
        Scribe_Collections.Look(ref pawns, "pawns", LookMode.Reference);
        Scribe_References.Look(ref faction, "faction");
        Scribe_References.Look(ref mapParent, "mapParent");
        Scribe_Values.Look(ref pawnsLeftUnhealthy, "pawnsLeftUnhealthy");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            pawns.RemoveAll(x => x == null);
        }
    }

    private enum InteractionResponseType
    {
        AssaultColony,
        Leave,
        BadThought
    }
}