using System.Reflection;
using Harmony;

[ModEntryPoint]
public static class NoInfighting
{
    public static void Main()
    {
        var harmony = HarmonyInstance.Create("com.github.johnnyonflame.noinfighting");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(BasicAIScript))]
    [HarmonyPatch("Update")]
    class BasicAIScript_Update_Patch
    {
        static bool Prefix(BasicAIScript __instance)
        {
            if (__instance.beingcrazy == false)
            {
                __instance.MyTarget = null;
            }
            return true;
        }
    }
}
