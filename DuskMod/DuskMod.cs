using System;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

public static class DuskMod
{
    public static void Init(Text text)
    {
        string rootDir = Directory.GetParent(Application.dataPath).FullName;
        string modsDir = Path.Combine(rootDir, "Mods");
        text.text     += "\nLoading mods...\n";
        
        try
        {
            foreach (string filePath in Directory.EnumerateFiles(modsDir, "*.dll"))
            {
                text.text += $"Loading mod {Path.GetFileNameWithoutExtension(filePath)}...\n";
                var assembly = Assembly.LoadFrom(filePath);

                foreach (var type in assembly.GetTypes()
                    .Where(t => Attribute.IsDefined(t, typeof(ModEntryPointAttribute))))
                {
                    type.GetMethod("Main")?.Invoke(null, null);
                }
            }
        }
        catch (Exception e)
        {
            text.text += e.Message + "\n";
        }

        text.text += "\n"; 
    }
}
