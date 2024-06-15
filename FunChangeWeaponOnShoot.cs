using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace FunMatchPlugin;

public class FunChangeWeaponOnShoot : FunBaseClass
{
    public override string Decription => "Change Weapon OnShoot 射击换枪";
    private BasePlugin.GameEventHandler<EventBulletImpact>? EventBulletImpactHandler=null;
    private Random random = new Random();
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        //EventPlayerShoot Not Working
        plugin.RegisterEventHandler <EventBulletImpact>(EventBulletImpactHandler = (@event, info) =>
        {
            if (Enabled == false) return HookResult.Stop;
            if (@event.Userid == null) return HookResult.Continue;
            //200-209 300-312 400-410
            //give random weapon ON shoot
            var guntype = random.Next(2,5);
            int guntype_add = 0;
            switch (guntype)
            {
                case 2:
                {
                    guntype_add = random.Next(0,10);
                    break;
                }
                case 3:
                {
                    guntype_add = random.Next(0,13);
                    break;
                }
                case 4:
                {
                    guntype_add = random.Next(0,11);
                    break;
                }
            }
            CsItem csItem = (CsItem)(guntype * 100 + guntype_add);

            var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
            var pawn = player!.PlayerPawn.Get();
            var cur_weapon = pawn!.WeaponServices!.ActiveWeapon.Get();
            if (cur_weapon is null) return HookResult.Continue;
            if (cur_weapon.DesignerName == "weapon_c4" || cur_weapon.DesignerName == "weapon_knife" || cur_weapon.DesignerName == "weapon_knife_t")
            return HookResult.Continue;

            @event.Userid!.DropActiveWeapon();

            @event.Userid!.GiveNamedItem(csItem);
            Server.NextFrame(()=>{
                cur_weapon!.Remove();
                if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
                {
                    @event.Userid!.GiveNamedItem(CsItem.Knife);
                }                
            });
            return HookResult.Continue;
        });
    }

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        plugin.DeregisterEventHandler(EventBulletImpactHandler!);
    }
}

