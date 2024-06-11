using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace FunMatchPlugin;

public class FunBulletTeleport : FunBaseClass
{
    public override string Decription => "Bullet Teleport 瞬移子弹";
    public FunBulletTeleport (FunMatchPlugin plugin): base(plugin){}
    private BasePlugin.GameEventHandler<EventBulletImpact>? EventBulletImpactHandler;
    public override void Fun(FunMatchPlugin plugin)
    {   
        if (Enabled) return;
        Enabled = true;
        plugin.RegisterEventHandler <EventBulletImpact>(EventBulletImpactHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            CCSPlayerPawn? pawn = @event.Userid!.PlayerPawn.Get();
            Vector Position = new Vector(@event.X,@event.Y,@event.Z);
            if (pawn == null)   return HookResult.Continue;
            pawn.Teleport(Position);
            return HookResult.Continue;
        });
    }

    public override void RegisterCommand(FunMatchPlugin plugin)
    {
        /*
        plugin.AddCommand("fun_bullet_teleport", "Start bullet teleport", (player, info) =>
        {
            this.Fun(plugin);
            this.DisPlayHelp();
        });
        plugin.AddCommand("!fun_bullet_teleport", "Stop bullet teleport", (player, info) =>
        {
            this.EndFun(plugin);
        });
        */
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        plugin.DeregisterEventHandler<EventBulletImpact> (EventBulletImpactHandler!);
        Enabled = false;
    }
}

