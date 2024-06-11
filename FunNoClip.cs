using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace FunMatchPlugin;

public class FunNoClip : FunBaseClass
{
    public override string Decription => "NoClip ON 启用飞行";
    private List<CCSPlayerController> Allplayers = new();
    public FunNoClip(FunMatchPlugin plugin) : base(plugin){}
    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        ConVar.Find("sv_cheats")!.SetValue(Enabled);
        Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            //if (p.IsBot) continue;
            p.ExecuteClientCommandFromServer("noclip 1");
        }
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            if (p.IsValid && !p.IsBot)
            {
                p.ExecuteClientCommandFromServer("noclip 0");              
            }
        }
        Enabled = false;
        ConVar.Find("sv_cheats")!.SetValue(Enabled);
        Allplayers.Clear();
    }
}
