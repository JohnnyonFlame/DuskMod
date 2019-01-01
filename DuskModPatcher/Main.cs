using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DuskMod
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var file = File.OpenWrite("DuskModPatcher.log"))
            using (var log  = new StreamWriter(file, Encoding.UTF8) { AutoFlush = true })
            {
                file.SetLength(0);
                Patch(log);
            }
        }

        static void Patch(TextWriter log)
        {
            var curDir  = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var duskDir = Directory.GetParent(curDir).FullName;
            var asmPath = new[] { duskDir, "Dusk_Data", "Managed", "Assembly-CSharp.dll" }.Aggregate(Path.Combine);
            var bakPath = Path.Combine(curDir, "Assembly-CSharp.dll.bak");
            var injPath = Path.Combine(duskDir, "DuskMod.dll");

            if (!File.Exists(bakPath))
            {
                log.WriteLine("No backup found, creating backup...");
                File.Copy(asmPath, bakPath);
            }
            else
            {
                log.WriteLine("Backup found, restoring...");
                File.Copy(bakPath, asmPath, true);
            }

            log.WriteLine("Trying to patch {0} with {1}", asmPath, injPath);
            using (var asm = ModuleDefinition.ReadModule(asmPath, new ReaderParameters { ReadWrite = true }))
            {
                if (asm.AssemblyReferences.Any(r => r.Name == "DuskMod"))
                {
                    log.WriteLine("The assembly has already been patched, bailing out...");
                    return;
                }

                using (var inj = ModuleDefinition.ReadModule(injPath))
                {
                    var hook   = inj.GetType("DuskMod").Methods.First(m => m.Name == "Init");
                    var type   = asm.Types.First(t => t.Name == "DosLoadingScreen");
                    var field  = type.Fields.First(f => f.Name == "textField");
                    var method = type.Methods.First(m => m.Name == "Start");
                    var il     = method.Body.GetILProcessor();
                    var begin  = method.Body.Instructions.FirstOrDefault();
                    
                    foreach (var instruction in new[]
                        {
                            Instruction.Create(OpCodes.Ldarg_0),
                            Instruction.Create(OpCodes.Ldfld, field),
                            Instruction.Create(OpCodes.Call, asm.ImportReference(hook))
                        })
                    {
                        if (begin != null)
                            il.InsertBefore(begin, instruction);
                        else
                            il.Append(instruction);
                    }

                    asm.Write();
                    log.WriteLine("Done.");
                }
            }
        }
    }
}
