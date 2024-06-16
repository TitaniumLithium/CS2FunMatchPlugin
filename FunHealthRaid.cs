using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace FunMatchPlugin;

public class FunHealthRaid : FunBaseClass
{
    public override string Decription => "Health Raid 攻击吸血";

    public FunHealthRaid(FunMatchPlugin plugin) : base(plugin){}
    private BasePlugin.GameEventHandler<EventPlayerHurt>? EventPlayerHurtHandler;
    public int initHP = 100;
    public int maxRaid = 100;
    public float RaidScale = 0.5F;

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        var Allplayers = Utilities.GetPlayers();
        plugin.DeregisterEventHandler(EventPlayerHurtHandler!);
        foreach (var p in Allplayers)
        {
            if (!p.IsValid) continue;
            CCSPlayerPawn? pawn = p.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            if (!pawn!.IsValid) continue;
            pawn!.MaxHealth = 100;
            pawn!.Health = 100;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        }
    }

    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        var Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            if (!p.IsValid) continue;
            CCSPlayerPawn? pawn = p.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            if (!pawn!.IsValid) continue;
            p.GiveNamedItem(CsItem.Kevlar);
            p.GiveNamedItem(CsItem.KevlarHelmet);
            pawn!.MaxHealth = initHP;
            pawn!.Health = initHP;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
        }

        plugin.RegisterEventHandler<EventPlayerHurt> (EventPlayerHurtHandler = (@event, info)=>
        {
            if (Enabled == false) return HookResult.Stop;
            if (@event.Userid is null || @event.Attacker is null) return HookResult.Continue;
            if (@event.Userid == @event.Attacker) return HookResult.Continue;
            if (@event.Userid.Team == @event.Attacker.Team) return HookResult.Continue;;
            var attacker = @event.Attacker.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
            var damage = @event.DmgHealth * RaidScale;
            if (damage > maxRaid) damage = maxRaid;
            Server.NextFrame(() =>
            {
                attacker!.MaxHealth += (int)damage;
                attacker!.Health += (int)damage;
                Utilities.SetStateChanged(attacker, "CBaseEntity", "m_iMaxHealth");
                Utilities.SetStateChanged(attacker, "CBaseEntity", "m_iHealth");
            });
            return HookResult.Continue;
        });
    }
}
