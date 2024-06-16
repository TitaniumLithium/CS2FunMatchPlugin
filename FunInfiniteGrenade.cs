using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace FunMatchPlugin;

public class FunInfiniteGrenade : FunBaseClass
{
    public override string Decription => "Infinite Grenade 无限火力";

    public FunInfiniteGrenade(FunMatchPlugin plugin) : base(plugin){}

    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        //ConVar.Find("mp_weapons_allow_heavyassaultsuit").SetValue(true);
        ConVar.Find("sv_cheats")!.SetValue(true);
        ConVar.Find("mp_autokick")!.SetValue(false);
        ConVar.Find("sv_infinite_ammo")!.SetValue(1);
        ConVar.Find("ammo_grenade_limit_total")!.SetValue(10);
        ConVar.Find("ammo_grenade_limit_default")!.SetValue(2);
        ConVar.Find("mp_weapons_allow_rifles")!.SetValue(0);
        ConVar.Find("mp_weapons_allow_pistols")!.SetValue(0);
        ConVar.Find("mp_weapons_allow_smgs")!.SetValue(0);
        ConVar.Find("mp_weapons_allow_heavy")!.SetValue(0);
        ConVar.Find("sv_cheats")!.SetValue(false);
        var Allplayers = Utilities.GetPlayers();
        bool BombHasGiven = false;
        foreach (var p in Allplayers)
        {
            if (!p.IsValid) continue;
            p.RemoveWeapons();
            p.GiveNamedItem(CsItem.HE);
            p.GiveNamedItem(CsItem.Knife);
            p.GiveNamedItem(CsItem.Kevlar);
            p.GiveNamedItem(CsItem.KevlarHelmet);
            if (p.Team == CounterStrikeSharp.API.Modules.Utils.CsTeam.Terrorist && !BombHasGiven)
            {
                p.GiveNamedItem(CsItem.C4);
                BombHasGiven = true;
            }
        }
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        ConVar.Find("sv_cheats")!.SetValue(true);
        ConVar.Find("mp_autokick")!.SetValue(true);
        ConVar.Find("sv_infinite_ammo")!.SetValue(0);
        ConVar.Find("ammo_grenade_limit_total")!.SetValue(5);
        ConVar.Find("ammo_grenade_limit_default")!.SetValue(1);
        ConVar.Find("mp_weapons_allow_rifles")!.SetValue(-1);
        ConVar.Find("mp_weapons_allow_pistols")!.SetValue(-1);
        ConVar.Find("mp_weapons_allow_smgs")!.SetValue(-1);
        ConVar.Find("mp_weapons_allow_heavy")!.SetValue(-1);
        ConVar.Find("sv_cheats")!.SetValue(false);
        Server.PrintToChatAll(StringExtensions.ReplaceColorTags("{RED}") + "[FunMatchPlugin] " + "If U Cannot Buy guns,Reconnect 如果手雷结束后买不了枪 请重新连接服务器");
    }
}
