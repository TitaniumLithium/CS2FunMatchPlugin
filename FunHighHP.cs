using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace FunMatchPlugin;

public class FunHighHP : FunBaseClass
{
    public override string Decription => "1000 HP 1000生命值";
    private List<CCSPlayerController> Allplayers = new();
    public FunHighHP(FunMatchPlugin plugin) : base(plugin){}
    public int maxHP = 1000;
    public int armor = 200;
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            var oringin = p!.OriginalControllerOfCurrentPawn.Get()!;
            if (oringin is null) continue;
            CCSPlayerPawn ?pawn = oringin.PlayerPawn.Get();
            if (pawn is null) continue;
            if (!pawn!.IsValid) continue;
            p.GiveNamedItem(CsItem.Kevlar);
            p.GiveNamedItem(CsItem.KevlarHelmet);
            pawn!.MaxHealth = maxHP;
            pawn!.Health = maxHP;
            pawn!.ArmorValue = armor;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawn", "m_ArmorValue");
        }
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            var oringin = p!.OriginalControllerOfCurrentPawn.Get()!;
            if (oringin is null) continue;
            CCSPlayerPawn ?pawn = oringin.PlayerPawn.Get();
            if (pawn is null) continue;
            if (!pawn!.IsValid) continue;
            pawn!.MaxHealth = 100;
            pawn!.Health = 100;
            pawn!.ArmorValue = 100;
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
            Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
            Utilities.SetStateChanged(pawn, "CCSPlayerPawn", "m_ArmorValue");
        }
        Enabled = false;
        Allplayers.Clear();
    }
}
