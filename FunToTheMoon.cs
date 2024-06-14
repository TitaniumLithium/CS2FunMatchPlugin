using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;

namespace FunMatchPlugin;

public class FunToTheMoon : FunBaseClass
{
    public override string Decription => "To The Moon 月球低重力";
    public FunToTheMoon(FunMatchPlugin plugin) : base(plugin){}
    public float gravity = (float)(800 * 0.166);
    public float BulletGiveAbsV = 100;
    private BasePlugin.GameEventHandler<EventBulletImpact>? EventBulletImpactHandler;
    private BasePlugin.GameEventHandler<EventPlayerHurt>? EventPlayerHurtHandler;
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        ConVar.Find("sv_gravity")!.SetValue(gravity);
        plugin.RegisterEventHandler <EventBulletImpact>(EventBulletImpactHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            if (@event.Userid is null) return HookResult.Continue;
            var pawn = @event.Userid.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            Vector bulletPosition = new Vector(@event.X,@event.Y,@event.Z);
            Vector playerPosition = pawn!.AbsOrigin!;
            double V2 = Math.Pow(playerPosition.X-bulletPosition.X,2) + Math.Pow(playerPosition.Y-bulletPosition.Y,2) + Math.Pow(playerPosition.Z-bulletPosition.Z,2);
            float Scale = (float)(BulletGiveAbsV /Math.Sqrt(V2));
            pawn.AbsVelocity.X += (playerPosition.X-bulletPosition.X)*Scale;
            pawn.AbsVelocity.Y += (playerPosition.Y-bulletPosition.Y)*Scale;
            pawn.AbsVelocity.Z += (playerPosition.Z-bulletPosition.Z)*Scale;
            return HookResult.Continue;
        });

        // if molotov hurt the player, will add V from Acctacker not from damage source... to be rewrite
        plugin.RegisterEventHandler <EventPlayerHurt>(EventPlayerHurtHandler = (@event, info) =>
        {
            
            if (Enabled == false) return HookResult.Stop;
            if (@event.Attacker is null) return HookResult.Continue;
            if (@event.Userid is null) return HookResult.Continue;

            if (@event.Userid == @event.Attacker) return HookResult.Continue;

            var attacker = @event.Attacker.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            var victim = @event.Userid.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            Vector PositionAttacker = new Vector(attacker!.AbsOrigin!.X,attacker.AbsOrigin.Y,attacker.AbsOrigin.Z);
            Vector PositionVictim = new Vector(victim!.AbsOrigin!.X,victim.AbsOrigin.Y,victim.AbsOrigin.Z);

            double V2 = Math.Pow(PositionAttacker.X-PositionVictim.X,2) + Math.Pow(PositionAttacker.Y-PositionVictim.Y,2) + Math.Pow(PositionAttacker.Z-PositionVictim.Z,2);
            float Scale = (float)(BulletGiveAbsV /Math.Sqrt(V2));
            victim.AbsVelocity.X += (PositionVictim.X-PositionAttacker.X)*Scale;
            victim.AbsVelocity.Y += (PositionVictim.Y-PositionAttacker.Y)*Scale;
            victim.AbsVelocity.Z += (PositionVictim.Z-PositionAttacker.Z)*Scale;
            
            return HookResult.Continue;
        });
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        plugin.DeregisterEventHandler (EventBulletImpactHandler!);
        plugin.DeregisterEventHandler (EventPlayerHurtHandler!);
        ConVar.Find("sv_gravity")!.SetValue((float)800);
    }
}

