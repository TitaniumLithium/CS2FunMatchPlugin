using System.Reflection;

namespace FunMatchPlugin;

//Unable to load seperated dlls, and type.IsSubclassOf(typeof(FunBaseClass)) not works properly
public class RoundFunManager
{
    /*
    public RoundFunManager(FunMatchPlugin plugin)
    {
        PluginDirectory = plugin.ModuleDirectory;
        string [] dllFilesNames = Directory.GetFiles(PluginDirectory,"*.dll");
        foreach (string dllFile in dllFilesNames)
        {
            if (dllFile == plugin.ModulePath) continue;
            Assembly assembly = Assembly.UnsafeLoadFrom(dllFile);
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                MethodInfo ?methodInfo = type.GetMethod("Fun");
                if (type.IsClass && methodInfo is not null && type.IsSubclassOf(typeof(FunBaseClass)))
                {
                    object instance = Activator.CreateInstance(type);
                    if (instance is not null)
                    {
                        methodInfo = type.GetMethod("RegisterCommand");
                        methodInfo.Invoke(instance, new object [] {plugin});
                        Console.WriteLine(type);
                    }
                }
            }
        }
        
    }
    */
    public List<FunBaseClass> FunLists = new List<FunBaseClass>();

    private string PluginDirectory;

    public void GetAllDllFiles(){}

    public void Load(){}

    public void UnLoad(){}
}
