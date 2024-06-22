namespace FunMatchPlugin;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
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

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
