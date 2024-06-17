namespace FunMatchPlugin;

public class CustomModeInfo
{
    public string Decr {get;set;}
    public string Fun_cfgfilename {get;set;}
    public string Endfun_cfgfilename {get;set;}
}

public class CustomModesConfig
{
    public List<CustomModeInfo> Modes{get;set;}
}
