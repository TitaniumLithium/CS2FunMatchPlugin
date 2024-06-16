namespace FunMatchPlugin;

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

public class FunWNoStop: FunBaseClass
{
    public float BurnAfterSecond = 0.5F;
    public override string Decription => "W No Stop 按住W不松手";
    public int BurnDamage = 2;
    private BasePlugin.GameEventHandler<EventRoundFreezeEnd>? EventRoundFreezeEndHandler;
    private BasePlugin.GameEventHandler<EventPlayerDisconnect>? EventPlayerDisconnectHandler;
    private BasePlugin.GameEventHandler<EventPlayerConnectFull>? EventPlayerConnectFullHandler;
    private Dictionary<int,Timer> playerTimersDict = new ();
    public FunWNoStop(FunMatchPlugin plugin) : base(plugin)
    {
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
                int playerid = (int)p.UserId;
                playerTimersDict.TryAdd((int)p.UserId,plugin.AddTimer(BurnAfterSecond,() => CheckPlayerForward(playerid),TimerFlags.REPEAT));
            }
            return HookResult.Stop;
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

        plugin.RegisterEventHandler <EventPlayerConnectFull> (EventPlayerConnectFullHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            Timer ?playerTimer;
            var oringin = @event.Userid!.OriginalControllerOfCurrentPawn.Get()!;
            if (oringin is null) return HookResult.Continue;
            CCSPlayerPawn ?pawn = oringin.PlayerPawn.Get();
            if (pawn is null) return HookResult.Continue;
            if (@event.Userid.UserId is null) {Console.WriteLine("null userid"); return HookResult.Continue;}
            int playerid = (int)@event.Userid.UserId;
            playerTimer = plugin.AddTimer(BurnAfterSecond,() => CheckPlayerForward(playerid),TimerFlags.REPEAT);
            playerTimersDict.TryAdd((int)@event.Userid.UserId!,playerTimer);
            return HookResult.Continue;
        });
        
    }

    public void CheckPlayerForward(int id)
    {
        CCSPlayerController? player = Utilities.GetPlayerFromUserid(id);
        if (player is null || (int)player.UserId! < 0 || !player.PawnIsAlive || player.PlayerPawn is null)
        {
            return;
        }
        if (!player.Buttons.HasFlag(PlayerButtons.Forward) || player.Buttons.HasFlag(PlayerButtons.Back) || player.Buttons.HasFlag(PlayerButtons.Duck) || player.Buttons.HasFlag(PlayerButtons.Walk))
        {
            CCSPlayerPawn ? pawn = player.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            BurnPlayer(pawn!);
        }
    }

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        ConVar.Find("mp_autokick")!.SetValue(true);
        foreach (var value in playerTimersDict.Values)
        {
            if (value is not null)
            value.Kill();
        }
        playerTimersDict.Clear();
        plugin.DeregisterEventHandler (EventRoundFreezeEndHandler!);
        plugin.DeregisterEventHandler (EventPlayerDisconnectHandler!);
        plugin.DeregisterEventHandler (EventPlayerConnectFullHandler!);
    }
}
