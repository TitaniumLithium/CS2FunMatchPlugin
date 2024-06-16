namespace FunMatchPlugin;

using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

// gameRules not working https://github.com/roflmuffin/CounterStrikeSharp/issues/489
public class FunC4EveryWhere : FunBaseClass
{
    public override string Decription => "C4 EveryWhere 10s to explode C4大战 10s爆炸";
    public FunC4EveryWhere(FunMatchPlugin plugin) : base(plugin){}
    public CCSGameRules? gameRules = null;
    private Timer? roundTimer = null;

    private Timer? roundTimer90 = null;
    private Timer? roundTimer60 = null;
    private Timer? roundTimer110 = null;
    private bool TeamHasWon = false;
    private CTeam? Team_CT;
    private CTeam? Team_T;

    private BasePlugin.GameEventHandler<EventBombPlanted>? EventBombPlantedHandler=null;
    private BasePlugin.GameEventHandler<EventPlayerDeath>? EventPlayerDeathHandler=null;
    public override void Fun(FunMatchPlugin plugin)
    {
        Enabled = true;
        ConVar.Find("sv_cheats")!.SetValue(true);
        ConVar.Find("mp_autokick")!.SetValue(false);
        ConVar.Find("mp_c4timer")!.SetValue(10);
        ConVar.Find("mp_plant_c4_anywhere")!.SetValue(true);
        ConVar.Find("mp_c4_cannot_be_defused")!.SetValue(true);
        ConVar.Find("mp_anyone_can_pickup_c4")!.SetValue(true);
        //ConVar.Find("mp_ignore_round_win_conditions")!.SetValue(true);
        ConVar.Find("sv_cheats")!.SetValue(false);

        var gameRulesProxie = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
        var CTeamArray = Utilities.FindAllEntitiesByDesignerName<CTeam>("cs_team_manager").ToArray();
        gameRules = gameRulesProxie.First().GameRules!;
        foreach (var t in CTeamArray)
        {
            if (t.Teamname == "CT") 
            Team_CT = t;
            if (t.Teamname == "TERRORIST") 
            Team_T = t;
        }

        if (gameRules is null || Team_CT is null || Team_T is null)
        {
            Console.WriteLine("[C4 everywhere] Exception No Gamesrules/Team CTorT Found");
            return;
        }

        var Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            if (!p.IsValid) continue;
            p.RemoveWeapons();
            p.GiveNamedItem(CsItem.C4);
            p.GiveNamedItem(CsItem.Knife);
            p.RemoveAllItemsOnNextRoundReset = true;
        }
        plugin.RegisterEventHandler<EventBombPlanted> (EventBombPlantedHandler = (@event,info)=>
        {
            if (Enabled == false) return HookResult.Stop;
            var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
            if (player is null || !player.IsValid) return HookResult.Continue;
            player.GiveNamedItem(CsItem.C4);
            return HookResult.Continue;
        });

        if(gameRules.WarmupPeriod) return;

        roundTimer = plugin.AddTimer(120, ()=>CTWin() , TimerFlags.STOP_ON_MAPCHANGE);
        roundTimer60 = plugin.AddTimer(60, ()=>Server.PrintToChatAll(StringExtensions.ReplaceColorTags("{RED}") + "[C4 Everywhere] 60s Left 剩余60s") , TimerFlags.STOP_ON_MAPCHANGE);
        roundTimer90 = plugin.AddTimer(90, ()=>Server.PrintToChatAll(StringExtensions.ReplaceColorTags("{RED}") + "[C4 Everywhere] 30s Left 剩余30s") , TimerFlags.STOP_ON_MAPCHANGE);
        roundTimer110 = plugin.AddTimer(110, ()=>Server.PrintToChatAll(StringExtensions.ReplaceColorTags("{RED}") + "[C4 Everywhere] 10s Left 剩余10s") , TimerFlags.STOP_ON_MAPCHANGE);

        plugin.RegisterEventHandler<EventPlayerDeath> (EventPlayerDeathHandler = (@event,info)=>
        {
            if (Enabled == false) return HookResult.Stop;
            if (TeamHasWon == true) return HookResult.Stop;
            int ct_num = AlivePlayerNum(CsTeam.CounterTerrorist);
            int t_num = AlivePlayerNum(CsTeam.Terrorist);
            if (@event.Userid is null || !@event.Userid.IsValid) return HookResult.Continue;
            if (@event.Userid.Team == CsTeam.CounterTerrorist)
            ct_num --;
            if (@event.Userid.Team == CsTeam.Terrorist)
            t_num --;
            if (t_num == 0 && ct_num == 0)
            {
                //gameRules.TerminateRound(3.0f,RoundEndReason.RoundDraw);
                TerminateRoundFix(3.0f,RoundEndReason.RoundDraw);
                roundTimer!.Kill();
                roundTimer60!.Kill();
                roundTimer90!.Kill();
                roundTimer110!.Kill();
                TeamHasWon = true;
                var q = @event.Userid.Team;
            }
            else if (t_num == 0)
            {
                //gameRules.TerminateRound(3.0f,RoundEndReason.CTsWin);
                TerminateRoundFix(3.0f,RoundEndReason.TargetBombed);
                roundTimer!.Kill();
                roundTimer60!.Kill();
                roundTimer90!.Kill();
                roundTimer110!.Kill();
                TeamHasWon = true;
                Team_CT!.Score++;
                Utilities.SetStateChanged(Team_CT,"CTeam", "m_iScore");
            }
            else if (ct_num == 0)
            {
                //gameRules.TerminateRound(3.0f,RoundEndReason.TerroristsWin);
                TerminateRoundFix(3.0f,RoundEndReason.BombDefused);
                roundTimer!.Kill();
                roundTimer60!.Kill();
                roundTimer90!.Kill();
                roundTimer110!.Kill();
                TeamHasWon = true;
                Team_T!.Score++;
                Utilities.SetStateChanged(Team_T,"CTeam", "m_iScore");
            }
            return HookResult.Continue;
        });

    }
    private void CTWin()
    {
        //gameRules!.TerminateRound(0.1f,RoundEndReason.CTsWin);
        //gameRules!.RoundWinStatus = 8;
        //gameRules!.TotalRoundsPlayed++;
        //gameRules!.ITotalRoundsPlayed++;
        TerminateRoundFix(3.0f,RoundEndReason.CTsWin);
        roundTimer!.Kill();
        roundTimer60!.Kill();
        roundTimer90!.Kill();
        roundTimer110!.Kill();
        TeamHasWon = true;
        Team_CT!.Score++;
        Utilities.SetStateChanged(Team_CT,"CTeam", "m_iScore");

    }

    private int AlivePlayerNum(CsTeam csTeam)
    {
        int players = 0;
        foreach (var player in Utilities.GetPlayers().Where(player => player.IsValid && player.Connected == PlayerConnectedState.PlayerConnected && player.PawnIsAlive))
        {
            if (player.Team == csTeam)
            players++;
        }
        return players;
    } 

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        ConVar.Find("sv_cheats")!.SetValue(true);
        ConVar.Find("mp_autokick")!.SetValue(true);
        ConVar.Find("mp_c4timer")!.SetValue(40);
        ConVar.Find("mp_plant_c4_anywhere")!.SetValue(false);
        ConVar.Find("mp_c4_cannot_be_defused")!.SetValue(false);
        ConVar.Find("mp_anyone_can_pickup_c4")!.SetValue(false);
        ConVar.Find("mp_ignore_round_win_conditions")!.SetValue(false);
        ConVar.Find("sv_cheats")!.SetValue(false);
        if (roundTimer is not null)
        roundTimer!.Kill();
        if (roundTimer60 is not null)
        roundTimer60!.Kill();
        if (roundTimer90 is not null)
        roundTimer90!.Kill();
        if (roundTimer110 is not null)
        roundTimer110!.Kill();
        TeamHasWon = false;
        if (EventBombPlantedHandler is not null)
        plugin.DeregisterEventHandler(EventBombPlantedHandler);
        if (EventPlayerDeathHandler is not null)
        plugin.DeregisterEventHandler(EventPlayerDeathHandler);

    }

    // to be fixed in https://github.com/roflmuffin/CounterStrikeSharp/issues/489
    public void TerminateRoundFix(float delay, RoundEndReason roundEndReason)
    {
        if (gameRules is null) return;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            TerminateRound(gameRules!.Handle,delay, roundEndReason, 0, 0);
        else
            TerminateRoundLinux(gameRules!.Handle, roundEndReason, 0, 0, delay);
            
    }
    public static MemoryFunctionVoid<nint, float, RoundEndReason, nint, uint> TerminateRoundFunc =
        new(GameData.GetSignature("CCSGameRules_TerminateRound"));
    public static Action<IntPtr, float, RoundEndReason, nint, uint> TerminateRound = TerminateRoundFunc.Invoke;
    public static MemoryFunctionVoid<nint, RoundEndReason, nint, uint, float> TerminateRoundLinuxFunc =
    new(GameData.GetSignature("CCSGameRules_TerminateRound"));
    public static Action<IntPtr, RoundEndReason, nint, uint, float> TerminateRoundLinux = TerminateRoundLinuxFunc.Invoke;
    
}

