using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Cvars;

namespace FunMatchPlugin;

public class FunJumpOrDie : FunBaseClass
{
    public float BurnAfterSecond = 1F;
    public override string Decription => "Jump OR Die 地板烫脚";
    public int BurnDamage = 5;

    private class playerTimer
    {
        public CounterStrikeSharp.API.Modules.Timers.Timer? timer;

        public playerTimer(){}

        public playerTimer(CounterStrikeSharp.API.Modules.Timers.Timer? t) {timer = t;}
    }

    private Dictionary<int,playerTimer> playerTimersDict = new ();
    private BasePlugin.GameEventHandler<EventPlayerJump>? EventPlayerJumpHandler;
    private BasePlugin.GameEventHandler<EventRoundFreezeEnd>? EventRoundFreezeEndHandler;
    private BasePlugin.GameEventHandler<EventPlayerDisconnect>? EventPlayerDisconnectHandler;
    private BasePlugin.GameEventHandler<EventPlayerConnectFull>? EventPlayerConnectFullHandler;
    public FunJumpOrDie (FunMatchPlugin plugin) : base (plugin){}
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        ConVar.Find("mp_autokick")!.SetValue(false);
        plugin.RegisterEventHandler <EventRoundFreezeEnd>(EventRoundFreezeEndHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            var Allplayers = Utilities.GetPlayers();
            foreach (var p in Allplayers)
            {
                if (p.UserId is null || (int)p.UserId < 0 || !p.PawnIsAlive || p.PlayerPawn is null) continue;
                //if (p.IsBot) continue;
                CCSPlayerPawn ?pawn = p.PlayerPawn.Get();
                if (pawn is null) continue;
                playerTimer playerTimer = new();
                playerTimer.timer = plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn),TimerFlags.REPEAT);
                playerTimersDict.TryAdd((int)p.UserId,playerTimer);
            }
            return HookResult.Stop;
        });

        plugin.RegisterEventHandler <EventPlayerDisconnect> (EventPlayerDisconnectHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            playerTimer ?playerTimer = new();
            playerTimersDict.TryGetValue((int)@event.Userid!.UserId!,out playerTimer);
            if (playerTimer is null) return HookResult.Continue;
            if (playerTimer.timer is not null) playerTimer.timer.Kill();
            playerTimersDict.Remove((int)@event.Userid.UserId);
            return HookResult.Continue;
        });

        plugin.RegisterEventHandler <EventPlayerConnectFull> (EventPlayerConnectFullHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            playerTimer playerTimer = new();
            CCSPlayerPawn ?pawn = @event.Userid!.PlayerPawn.Get();
            if (pawn is null) return HookResult.Continue;
            playerTimer.timer = plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn),TimerFlags.REPEAT);
            playerTimersDict.TryAdd((int)@event.Userid.UserId!,playerTimer);
            return HookResult.Continue;
        });

        plugin.RegisterEventHandler <EventPlayerJump>(EventPlayerJumpHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            if (@event is null) return HookResult.Continue;
            if (@event.Userid is null) return HookResult.Continue;
            if (@event.Userid.UserId is null) return HookResult.Continue;
            var pawn = @event.Userid.PlayerPawn.Get();
            if (pawn is null) return HookResult.Continue;
            playerTimer? playerTimer = new();
            if (playerTimersDict.ContainsKey((int)@event.Userid.UserId))
            {
                playerTimersDict.TryGetValue((int)@event.Userid.UserId, out playerTimer);
                if (playerTimer!.timer is not null) playerTimer.timer.Kill();
                playerTimer.timer = plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn),TimerFlags.REPEAT);
            }
            else
            {
                Console.WriteLine("[Jump Or Die] Unbinding Timers player detected");
                playerTimer.timer = plugin.AddTimer(BurnAfterSecond,() => BurnPlayer(pawn),TimerFlags.REPEAT);
                playerTimersDict.TryAdd((int)@event.Userid.UserId,playerTimer);
            }
            return HookResult.Continue;
        });
    }

    public override void EndFun(FunMatchPlugin plugin)
    {
        ConVar.Find("mp_autokick")!.SetValue(true);
        plugin.DeregisterEventHandler (EventPlayerJumpHandler!);
        plugin.DeregisterEventHandler (EventRoundFreezeEndHandler!);
        plugin.DeregisterEventHandler (EventPlayerDisconnectHandler!);
        plugin.DeregisterEventHandler (EventPlayerConnectFullHandler!);

        foreach (var value in playerTimersDict.Values)
        {
            if (value.timer is not null)
            value.timer.Kill();
        }
        playerTimersDict.Clear();
        Enabled = false;
    }
    private void BurnPlayer(CCSPlayerPawn pawn)
    {
        pawn.Health -= BurnDamage;
        pawn.ApplyStressDamage = true;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        if (pawn.Health <= 0)
        {
            pawn.CommitSuicide(true,true);
        }
    }
}
