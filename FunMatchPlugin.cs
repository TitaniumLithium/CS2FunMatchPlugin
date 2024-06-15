using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;

namespace FunMatchPlugin;

public class FunMatchPlugin: BasePlugin
{
    public override string ModuleName => "Fun Match Plugin";
    public override string ModuleVersion => "1.0.6";
    public override void Load(bool hotReload)
    {
        Console.WriteLine("Fun Match Plugin Load!");
        FunBulletTeleport funBulletTeleport = new FunBulletTeleport(this);
        FunLists.Add(funBulletTeleport);
        FunHealTeammates funHealTeammates = new FunHealTeammates(this);
        FunLists.Add(funHealTeammates);
        FunHealthRaid funHealthRaid = new FunHealthRaid(this);
        FunLists.Add(funHealthRaid);
        FunHighHP funHighHP = new FunHighHP(this);
        FunLists.Add(funHighHP);
        FunInfiniteGrenade funFunInfiniteGrenade = new FunInfiniteGrenade(this);
        FunLists.Add(funFunInfiniteGrenade);
        FunJumpOrDie funJumpOrDie = new(this);
        FunLists.Add(funJumpOrDie);
        FunNoClip funNoClip = new FunNoClip(this);
        FunLists.Add(funNoClip);
        FunPlayerShootExChange funPlayerShootExChange = new FunPlayerShootExChange(this);
        FunLists.Add(funPlayerShootExChange);
        FunToTheMoon funToTheMoon = new(this);
        FunLists.Add(funToTheMoon);
        FunWNoStop funWNoStop = new FunWNoStop(this);
        FunLists.Add(funWNoStop);
        FunDropWeaponOnShoot funDropWeaponOnShoot = new();
        FunLists.Add(funDropWeaponOnShoot);
        //FunChangeWeaponOnShoot funChangeWeaponOnShoot = new();
        //FunLists.Add(funChangeWeaponOnShoot);
    }

    public void LoadRandomFun()
    {
        if (CurrentActiceFunIndex >= 0) return;
        Random rd = new Random();
        CurrentActiceFunIndex = rd.Next(0,FunLists.Count);
        FunLists[CurrentActiceFunIndex].Fun(this);
        if (DisPlayHelp) FunLists[CurrentActiceFunIndex].DisPlayHelp();
    }

    public void UnLoadFun()
    {
        if (CurrentActiceFunIndex < 0) return;
        FunLists[CurrentActiceFunIndex].EndFun(this);
    }

    public void LoadFunByIndex(int index)
    {
        if (index < 0 || index >=FunLists.Count) return;
        FunLists[index].Fun(this);
        if (DisPlayHelp) FunLists[index].DisPlayHelp();
    }

    public void UnLoadFunByIndex(int index)
    {
        if (index < 0 || index >=FunLists.Count) return;
        FunLists[index].EndFun(this);
    }


    public void LoadFunByName(string name)
    {

    }
    public bool DisPlayHelp = true;
    private int CurrentActiceFunIndex = -1;
    private List<FunBaseClass> FunLists = new List<FunBaseClass>();
    private bool EnableRandom = true;

    private GameEventHandler<EventRoundStart> ?ManualLoadHander;
    private int ManualLoadIndex = -1;

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (!EnableRandom) return HookResult.Continue;
        UnLoadFun();
        CurrentActiceFunIndex = -1;
        Server.NextFrame(()=>{
            LoadRandomFun();
        });
        return HookResult.Continue;
    }

    [ConsoleCommand("fun_load", "Load fun by num")]
    [CommandHelper(minArgs: 1, usage: "[num]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnLoadFunCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        
        int num;
        int.TryParse(commandInfo.GetArg(1),out num);
        if (num <= 0 || num >FunLists.Count)
        {
            commandInfo.ReplyToCommand($"Invalid num. pls input num from {1} - {FunLists.Count}");
            return;
        }
        if (ManualLoadHander is not null) 
        {
            commandInfo.ReplyToCommand($"Alraedy loaded {ManualLoadIndex + 1} Manually, Pls !fun_load first");
            return;
        }
        ManualLoadIndex = num - 1;
        LoadFunByIndex(ManualLoadIndex);
        RegisterEventHandler (ManualLoadHander = (@event, info) =>
        {
            UnLoadFunByIndex(ManualLoadIndex);
            Server.NextFrame(()=>{
                LoadFunByIndex(ManualLoadIndex);
            });
            return HookResult.Continue;
        });
    }

    [ConsoleCommand("!fun_load", "UnLoad fun Manually")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnUnLoadFunCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        UnLoadFunByIndex(ManualLoadIndex);
        DeregisterEventHandler(ManualLoadHander!);
        ManualLoadHander = null;
        commandInfo.ReplyToCommand($"Unloaded {ManualLoadIndex + 1}");
    }

    [ConsoleCommand("fun_lists", "Lists Avaliable Fun")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnListFunCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        for (int i = 0;i < FunLists.Count; i++)
        {
            commandInfo.ReplyToCommand($"{i + 1} {FunLists[i].Decription}");
        }
    }

    [ConsoleCommand("fun_displayhelp", "DisPlay Help Fun")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnDisPlayHelpCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        DisPlayHelp = true;
    }

    [ConsoleCommand("!fun_displayhelp", "Don't DisPlay Help Fun")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnDontDisPlayHelpCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        DisPlayHelp = false;
    }

    [ConsoleCommand("!fun_random", "Don't random Fun everyround")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnDontRandomCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        EnableRandom = false;
        UnLoadFun();
    }

    [ConsoleCommand("fun_random", "random Fun everyround")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public void OnRandomCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        EnableRandom = true;
    }
}
