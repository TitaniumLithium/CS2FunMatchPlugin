using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace FunMatchPlugin;

using CounterStrikeSharp.API.Modules.Timers;
public class FunNoClip : FunBaseClass
{
    public override string Decription => "NoClip ON 启用飞行";
    private List<CCSPlayerController> Allplayers = new();
    public FunNoClip(FunMatchPlugin plugin) : base(plugin){}
    private Timer ?NoclipOnTimer = null;
    private bool IsNoClipON = false;
    public float interval = 2.0f;

    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled) return;
        Enabled = true;
        IsNoClipON = false;
        NoclipOnTimer = plugin.AddTimer(interval,SetNoclip,TimerFlags.REPEAT);
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            if (p.IsValid)
            {
                p.ExecuteClientCommandFromServer("noclip 0");              
            }
        }
        Enabled = false;
        IsNoClipON = false;
        ConVar.Find("sv_cheats")!.SetValue(false);
        Allplayers.Clear();
        if (NoclipOnTimer is not null) NoclipOnTimer.Kill();
    }

    private void SetNoclip()
    {
        if (IsNoClipON is false)
        {
            ConVar.Find("sv_cheats")!.SetValue(true);
            Allplayers = Utilities.GetPlayers();
            foreach (var p in Allplayers)
            {
                if (p.IsBot) continue;
                if (p.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get()!.IsDefusing || p.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get()!.InBombZone) 
                {
                    continue;
                }
                p.ExecuteClientCommandFromServer("noclip 1");
            }
            IsNoClipON = true;
            ConVar.Find("sv_cheats")!.SetValue(false);
        }
        else
        {
            ConVar.Find("sv_cheats")!.SetValue(true);
            Allplayers = Utilities.GetPlayers();
            foreach (var p in Allplayers)
            {
                if (p.IsBot) continue;
                p.ExecuteClientCommandFromServer("noclip 0");
            }
            IsNoClipON = false;
            ConVar.Find("sv_cheats")!.SetValue(false);
        }
    }
}
