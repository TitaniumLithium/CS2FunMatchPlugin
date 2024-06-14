namespace FunMatchPlugin;

using System.Data.Common;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;



public class FunHealTeammates : FunBaseClass
{
    public override string Decription => "Heal Teammates 治疗队友";

    public FunHealTeammates(FunMatchPlugin plugin) : base(plugin){}
    public float BurnAfterSecond = 1.0F;
    public int BurnDamage = 5;
    public int HealValue = 10;
    private bool ?LastConvar_friendlyfire;
    private BasePlugin.GameEventHandler<EventRoundFreezeEnd>? EventRoundFreezeEndHandler;
    private BasePlugin.GameEventHandler<EventPlayerDisconnect>? EventPlayerDisconnectHandler;
    private BasePlugin.GameEventHandler<EventPlayerConnectFull>? EventPlayerConnectFullHandler;
    private BasePlugin.GameEventHandler<EventPlayerHurt>? EventPlayerHurtHandler;
    private Dictionary<int,Timer> playerTimersDict = new ();
    private void BurnPlayer(CCSPlayerPawn? pawn)
    {
        if (pawn is null) return;
        pawn.Health -= BurnDamage;
        pawn.ApplyStressDamage = true;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        if (pawn.Health <= 0)
        {
            pawn.CommitSuicide(true,true);
        }
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        ConVar.Find("mp_autokick")!.SetValue(true);
        ConVar.Find("ff_damage_reduction_bullets")!.SetValue(0.5F);
        foreach (var value in playerTimersDict.Values)
        {
            if (value is not null)
            value.Kill();
        }
        playerTimersDict.Clear();
        plugin.DeregisterEventHandler (EventRoundFreezeEndHandler!);
        plugin.DeregisterEventHandler (EventPlayerDisconnectHandler!);
        plugin.DeregisterEventHandler (EventPlayerConnectFullHandler!);
        plugin.DeregisterEventHandler (EventPlayerHurtHandler!);
        Server.ExecuteCommand($"mp_friendlyfire {LastConvar_friendlyfire!}");
    }

    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;

        ConVar.Find("mp_autokick")!.SetValue(false);
        LastConvar_friendlyfire = ConVar.Find("mp_friendlyfire")!.GetPrimitiveValue<bool>();
        ConVar.Find("mp_friendlyfire")!.SetValue(true);
        ConVar.Find("ff_damage_reduction_bullets")!.SetValue(0.0f);
        plugin.RegisterEventHandler <EventRoundFreezeEnd>(EventRoundFreezeEndHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            var Allplayers = Utilities.GetPlayers();
            foreach (var p in Allplayers)
            {
                if (p.UserId is null || (int)p.UserId < 0 || !p.PawnIsAlive || p.OriginalControllerOfCurrentPawn is null) continue;
                //if (p.IsBot) continue;
                var pawn = p.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
                playerTimersDict.TryAdd((int)p.UserId,plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn!),TimerFlags.REPEAT));
            }
            return HookResult.Stop;
        });
        
        plugin.RegisterEventHandler <EventPlayerConnectFull> (EventPlayerConnectFullHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            Timer ?playerTimer;
            CCSPlayerPawn ?pawn = @event.Userid!.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            if (pawn is null) return HookResult.Continue;
            playerTimer = plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn!),TimerFlags.REPEAT);
            playerTimersDict.TryAdd((int)@event.Userid.UserId!,playerTimer);
            return HookResult.Continue;
        });

        plugin.RegisterEventHandler <EventPlayerDisconnect> (EventPlayerDisconnectHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            Timer ?playerTimer;
            playerTimersDict.TryGetValue((int)@event.Userid!.UserId!,out playerTimer);
            if (playerTimer is null) return HookResult.Continue;
            playerTimer.Kill();
            playerTimersDict.Remove((int)@event.Userid.UserId);
            return HookResult.Continue;
        });

        //EventPlayerAvengedTeammate not working using playerhurtinstead
        plugin.RegisterEventHandler <EventPlayerHurt> (EventPlayerHurtHandler = (@event , info)=>
        {
            if (@event.Attacker is null && @event.Userid is null) return HookResult.Continue;
            if (@event.Attacker == @event.Userid) return HookResult.Continue;
            if (@event.Attacker!.Team != @event.Userid!.Team) return HookResult.Continue;
            CCSPlayerPawn pawn = @event.Userid.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get()!;
            pawn.Health += HealValue;
            if (pawn.Health >= 100) pawn.Health = 100;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
            return HookResult.Continue;
        });

    }
}
