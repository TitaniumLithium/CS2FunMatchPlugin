namespace FunMatchPlugin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Timers;

// gameRules not working https://github.com/roflmuffin/CounterStrikeSharp/issues/489
public class FunC4EveryWhere : FunBaseClass
{
    public override string Decription => "C4 EveryWhere C4大战";
    public FunC4EveryWhere(FunMatchPlugin plugin) : base(plugin){}
    public CCSGameRules gameRules;
    private Timer roundTimer;
    public override void Fun(FunMatchPlugin plugin)
    {
        Enabled = true;
        //ConVar.Find("mp_weapons_allow_heavyassaultsuit").SetValue(true);
        ConVar.Find("mp_autokick")!.SetValue(false);
        ConVar.Find("mp_c4timer")!.SetValue(10);
        ConVar.Find("mp_plant_c4_anywhere")!.SetValue(true);
        ConVar.Find("mp_c4_cannot_be_defused")!.SetValue(true);
        ConVar.Find("mp_anyone_can_pickup_c4")!.SetValue(true);
        ConVar.Find("mp_ignore_round_win_conditions")!.SetValue(true);
        
        var Allplayers = Utilities.GetPlayers();
        foreach (var p in Allplayers)
        {
            p.RemoveWeapons();
            p.GiveNamedItem(CsItem.C4);
            p.GiveNamedItem(CsItem.Knife);
            //p.GiveNamedItem(CsItem.Kevlar);
            //p.GiveNamedItem(CsItem.KevlarHelmet);
            //p.GiveNamedItem<CItemHeavyAssaultSuit>("CItemHeavyAssaultSuit");
            //CCSPlayerController player = Utilities.GetPlayerFromUserid(id);
            // DesignerName == "cs_gamerules"  var gameRules = entity.As<CCSGameRules>(); CCSGameRulesProxy
            //var b = p.Buttons; mp_c4timer 20 mp_plant_c4_anywhere true mp_c4_cannot_be_defused true mp_anyone_can_pickup_c4 true
            //b.HasFlag(PlayerButtons.Forward); mp_ignore_round_win_conditions 0
        }

        roundTimer = plugin.AddTimer(30, ()=>CTWin() , TimerFlags.STOP_ON_MAPCHANGE);

        plugin.RegisterEventHandler<EventPlayerDeath> ((@event,info)=>
        {
            var entities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_gamerules").ToArray();
            foreach (var e in entities.Where(x => x.DesignerName == "cs_gamerules"))
            {
                gameRules = e.As<CCSGameRules>();
                Console.WriteLine($"PlayerKilled {gameRules.AccountCT} {gameRules.AccountTerrorist} {gameRules.NumCT} {gameRules.NumTerrorist}");
                Console.WriteLine($"{gameRules.RoundEndFunFactData1} {gameRules.RoundEndReason} {gameRules.RoundEndWinnerTeam}");
                //gameRules.TerminateRound(1,RoundEndReason.TerroristsWin);
                break;
            }

            return HookResult.Continue;
        });
        /*
        plugin.RegisterEventHandler<EventRoundFreezeEnd> ((@event,info)=>
        {
            var entities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity> ("cs_gamerules");
            foreach (var e in entities)
            {
                gameRules = e.As<CCSGameRules>();
                break;
            }
            Console.WriteLine($"FreezeEnd {gameRules.AccountCT} {gameRules.AccountTerrorist} {gameRules.NumCT} {gameRules.NumTerrorist}");
            //roundTimer = plugin.AddTimer(30, ()=>CTWin() , TimerFlags.STOP_ON_MAPCHANGE);
            return HookResult.Stop;
        });
        */
    }
    private void CTWin()
    {
        var csEntities = Utilities.FindAllEntitiesByDesignerName<CBaseEntity>("cs_").ToArray();
        foreach (var entity in csEntities.Where(x => x.DesignerName == "cs_gamerules"))
        {
            // gameRules not working https://github.com/roflmuffin/CounterStrikeSharp/issues/489
            var gameRules = entity.As<CCSGameRules>();
            gameRules.CTTimeOutActive = true;
            gameRules.TerminateRound(0,RoundEndReason.CTsWin);
        }
        ConVar.Find("mp_ignore_round_win_conditions")!.SetValue(false);
        Console.WriteLine("CTWIN");
    }
    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        ConVar.Find("mp_autokick")!.SetValue(true);
        ConVar.Find("mp_c4timer")!.SetValue(40);
        ConVar.Find("mp_plant_c4_anywhere")!.SetValue(false);
        ConVar.Find("mp_c4_cannot_be_defused")!.SetValue(false);
        ConVar.Find("mp_anyone_can_pickup_c4")!.SetValue(false);
        ConVar.Find("mp_ignore_round_win_conditions")!.SetValue(false);
        //roundTimer.Kill();
    }
}

