using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API;

namespace FunMatchPlugin;

public class FunPlayerShootExChange : FunBaseClass
{
    public override string Decription => "Bullet ExChange 击中交换位置";
    public FunPlayerShootExChange (FunMatchPlugin plugin): base(plugin){}
    private BasePlugin.GameEventHandler<EventPlayerHurt>? EventPlayerHurtHandler;
    public override void Fun(FunMatchPlugin plugin)
    {   
        if (Enabled) return;
        Enabled = true;
        plugin.RegisterEventHandler <EventPlayerHurt>(EventPlayerHurtHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            //Console.WriteLine("Player Shoot ExChange Event");
            //To Avoid VScode Warning "might be null"
            if (@event is null) return HookResult.Continue;
            if (@event.Attacker is null) return HookResult.Continue;
            if (@event.Userid is null) return HookResult.Continue;

            if (@event.Userid == @event.Attacker) return HookResult.Continue;
            var attacker = @event.Attacker.PlayerPawn.Get();
            var victim = @event.Userid.PlayerPawn.Get();
            Vector PositionAttacker = new Vector(attacker!.AbsOrigin!.X,attacker.AbsOrigin.Y,attacker.AbsOrigin.Z);
            Vector PositionVictim = new Vector(victim!.AbsOrigin!.X,victim.AbsOrigin.Y,victim.AbsOrigin.Z);
            Server.NextFrame(() =>
            {
                victim.Teleport(PositionAttacker);
                attacker.Teleport(PositionVictim);
            });

            return HookResult.Continue;
        });
    }
    public override void RegisterCommand(FunMatchPlugin plugin)
    {
        /*
        plugin.AddCommand("fun_shoot_exchange", "Start shoot exchange", (player, info) =>
        {
            this.Fun(plugin);
            this.DisPlayHelp();
        });
        plugin.AddCommand("!fun_shoot_exchange", "Stop shoot exchange", (player, info) =>
        {
            this.EndFun(plugin);
        });
        */
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        plugin.DeregisterEventHandler<EventPlayerHurt> (EventPlayerHurtHandler!);
        Enabled = false;
    }
}
