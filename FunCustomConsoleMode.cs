using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Translations;

namespace FunMatchPlugin;

public class FunCustomConsoleMode : FunBaseClass
{
    public string DecriptionCustom;
    private string CFGLoadFile;
    private string CFGUnLoadFile;
    public override string Decription => "Custom Modes";

    public FunCustomConsoleMode(string decr,string loadFile, string unLoadFile)
    {
        DecriptionCustom = decr;
        CFGLoadFile = loadFile;
        CFGUnLoadFile = unLoadFile;
    }

    public override void EndFun(FunMatchPlugin plugin)
    {
        Enabled = false;
        Server.ExecuteCommand("exec " + CFGUnLoadFile);
    }

    public override void Fun(FunMatchPlugin plugin)
    {
        if (Enabled == true) return;
        Enabled = true;
        Server.ExecuteCommand("exec " + CFGLoadFile);
    }
    public override void DisPlayHelp()
    {
        Server.PrintToChatAll(StringExtensions.ReplaceColorTags("{RED}") + "[FunMatchPlugin] " + DecriptionCustom);
    }
}
