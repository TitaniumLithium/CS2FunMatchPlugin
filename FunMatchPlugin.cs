using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using System.Text.Json;

namespace FunMatchPlugin;

public class FunMatchPlugin: BasePlugin , IPluginConfig<FunMatchPluginConfig>
{
    public override string ModuleName => "Fun Match Plugin";
    public override string ModuleVersion => "1.1.1";
    public FunMatchPluginConfig Config {get;set;} = new();
    public override void Load(bool hotReload)
    {
        Console.WriteLine("Fun Match Plugin Load!");
        InstallFun(Config);
        InstallCustomModes();
        //FunC4EveryWhere funC4EveryWhere = new(this);
        //FunLists.Add(funC4EveryWhere);
    }
    public void OnConfigParsed(FunMatchPluginConfig config)
    {
        Console.WriteLine("Parsing config");
        Config = config;
    }

    private void InstallCustomModes()
    {
        var configdirectory = Path.Combine(Application.RootDirectory, "configs/plugins/FunMatchPlugin");
        if (!Path.Exists(configdirectory)) Directory.CreateDirectory(configdirectory);
        var configpath = Path.Combine(configdirectory,"CustomModes.json");
        if (!File.Exists(configpath)) return;

        var dir_addons = Directory.GetParent(Application.RootDirectory);
        var dir_csgo = Directory.GetParent(dir_addons!.FullName);
        var path_cfg = Path.Combine(dir_csgo!.FullName,"cfg");

        using (StreamReader r = new StreamReader(configpath))
        {
            string json = r.ReadToEnd();
            CustomModesConfig customModesConfig = JsonSerializer.Deserialize<CustomModesConfig>(json)!;
            foreach (var mode in customModesConfig.Modes)
            {
                var fun_cfg_game = Path.Combine(path_cfg,mode.Fun_cfgfilename);
                var fun_cfg_config = Path.Combine(configdirectory,mode.Fun_cfgfilename);
                var endfun_cfg_game = Path.Combine(path_cfg,mode.Endfun_cfgfilename);
                var endfun_cfg_config = Path.Combine(configdirectory,mode.Endfun_cfgfilename);
                File.Copy(fun_cfg_config,fun_cfg_game,true);
                File.Copy(endfun_cfg_config,endfun_cfg_game,true);
                FunLists.Add(new FunCustomConsoleMode(mode.Decr,mode.Fun_cfgfilename,mode.Endfun_cfgfilename));
            }
        }
    }

    private void InstallFun(FunMatchPluginConfig config)
    {
        if (config.IsFunBulletTeleportOn)
        {
            FunBulletTeleport funBulletTeleport = new (this);
            FunLists.Add(funBulletTeleport);
        }
        if (config.IsFunHealTeammatesOn)
        {
            FunHealTeammates funHealTeammates = new(this)
            {
                BurnAfterSecond = config.FunHealTeammatesBurnAfterSecond,
                BurnDamage = config.FunHealTeammatesBurnDamage,
                HealValue = config.FunHealTeammatesHealValue,
            };
            FunLists.Add(funHealTeammates);
        }
        if (config.IsFunHealthRaidOn)
        {
            FunHealthRaid funHealthRaid = new (this)
            {
                initHP = config.FunHealthRaidinitHP,
                maxRaid = config.FunHealthRaidmaxRaid,
                RaidScale = config.FunHealthRaidScale,
            };
            FunLists.Add(funHealthRaid);
        }
        if (config.IsFunHighHPOn)
        {
            FunHighHP funHighHP = new (this)
            {
                maxHP = config.FunHighHPmaxHP,
                armor = config.FunHighHParmor,
            };
            FunLists.Add(funHighHP);
        }
        if (config.IsFunInfiniteGrenadeOn)
        {
            FunInfiniteGrenade funFunInfiniteGrenade = new (this);
            FunLists.Add(funFunInfiniteGrenade);
        }
        if (config.IsFunJumpOrDieOn)
        {
            FunJumpOrDie funJumpOrDie = new(this)
            {
                BurnAfterSecond = config.FunJumpOrDieBurnAfterSecond,
                BurnDamage = config.FunJumpOrDieBurnDamage,
            };
            FunLists.Add(funJumpOrDie);
        }
        if (config.IsFunNoClipOn)
        {
            FunNoClip funNoClip = new (this){
                interval = config.FunNoClipinterval,
            };
            FunLists.Add(funNoClip);
        }
        if (config.IsFunPlayerShootExChangeOn)
        {
            FunPlayerShootExChange funPlayerShootExChange = new (this);
            FunLists.Add(funPlayerShootExChange);
        }
        if (config.IsFunToTheMoonOn)
        {
            FunToTheMoon funToTheMoon = new(this)
            {
                gravity = config.FunToTheMoongravity,
                BulletGiveAbsV = config.FunToTheMoonBulletGiveAbsV,
            };
            FunLists.Add(funToTheMoon);
        }
        if (config.IsFunWNoStopOn)
        {
            FunWNoStop funWNoStop = new (this)
            {
                BurnAfterSecond = config.FunWNoStopBurnAfterSecond,
                BurnDamage = config.FunWNoStopBurnDamage,
            };
            FunLists.Add(funWNoStop);
        }
        if (config.IsFunDropWeaponOnShootOn)
        {
            FunDropWeaponOnShoot funDropWeaponOnShoot = new();
            FunLists.Add(funDropWeaponOnShoot);
        }
        if (config.IsFunChangeWeaponOnShootOn)
        {
            FunChangeWeaponOnShoot funChangeWeaponOnShoot = new();
            FunLists.Add(funChangeWeaponOnShoot);         
        }
        if (config.IsFunFootBallOn)
        {
            FunFootBall funFootBall = new();
            FunLists.Add(funFootBall);
            RegisterListener<Listeners.OnServerPrecacheResources>((manifest) =>
            {
                manifest.AddResource("models/props/de_dust/hr_dust/dust_soccerball/dust_soccer_ball001.vmdl");
                manifest.AddResource("models/props_fairgrounds/fairgrounds_flagpole01.vmdl");
            });
        }
    }

    private Random rd = new Random();
    public void LoadRandomFun()
    {
        if (CurrentActiceFunIndex >= 0) return;
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
