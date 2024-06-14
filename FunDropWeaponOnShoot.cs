using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace FunMatchPlugin;

public class FunDropWeaponOnShoot : FunBaseClass
{
    public override string Decription => "Drop Weapon OnShoot 射击丢枪";
    private BasePlugin.GameEventHandler<EventBulletImpact>? EventBulletImpactHandler=null;
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        //EventPlayerShoot Not Working
        plugin.RegisterEventHandler <EventBulletImpact>(EventBulletImpactHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
            var pawn = player!.PlayerPawn.Get();
            var cur_weapon = pawn!.WeaponServices!.ActiveWeapon.Get();
            if (cur_weapon is null) return HookResult.Continue;
            if (cur_weapon.DesignerName == "weapon_c4" || cur_weapon.DesignerName == "weapon_knife" || cur_weapon.DesignerName == "weapon_knife_t")
            return HookResult.Continue;

            @event.Userid!.DropActiveWeapon();

            if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
            {
                @event.Userid!.GiveNamedItem(CsItem.Knife);
            }
            return HookResult.Continue;
        });
    }

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        plugin.DeregisterEventHandler(EventBulletImpactHandler!);
    }
}
