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
            Vector Position = new Vector(@event.X,@event.Y,@event.Z);
            var oringin = @event.Userid!.OriginalControllerOfCurrentPawn.Get()!;
            var oringinpawn = oringin.PlayerPawn.Get();
            if (oringinpawn is null) return HookResult.Continue;
            oringinpawn!.Teleport(Position);
            return HookResult.Continue;
        });
    }

    public override void RegisterCommand(FunMatchPlugin plugin)
    {
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        plugin.DeregisterEventHandler<EventBulletImpact> (EventBulletImpactHandler!);
        Enabled = false;
    }
}

