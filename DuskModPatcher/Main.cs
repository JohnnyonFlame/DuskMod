using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DuskMod
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new System.IO.StreamWriter("DuskModPatcher.log", false);
            log.AutoFlush = true;

            string curdir           = Directory.GetCurrentDirectory();
            string duskdir          = Path.GetFullPath(Path.Combine(curdir, ".."));
            string patchedAsm       = new[] { duskdir, "Dusk_Data", "Managed", "Assembly-CSharp.dll" }.Aggregate(Path.Combine);
            string patchedAsmBackup = Path.Combine(curdir, "Assembly-CSharp.dll.bak");
            string patchAsm         = Path.Combine(duskdir, "DuskMod.dll");

            if (!File.Exists(patchedAsmBackup))
            {
                log.WriteLine("No backup found, creating backup...");
                try
                {
                    File.Copy(patchedAsm, patchedAsmBackup);
                }
                catch (IOException err)
                {
                    log.WriteLine("Failed to create backup, {0}", err.Message);
                }
            }
            else
            {
                log.WriteLine("Backup found, restoring...");
                try
                {
                    File.Copy(patchedAsmBackup, patchedAsm, true);
                }
                catch (IOException err)
                {
                    log.WriteLine("Failed to restore backup, {0}", err.Message);
                }
            }

            log.WriteLine("Trying to patch {0} with {1}", patchedAsm, patchAsm);
            using (var hookedAsm = ModuleDefinition.ReadModule(patchedAsm, new ReaderParameters { ReadWrite = true }))
            {
                if (hookedAsm.AssemblyReferences.SingleOrDefault(x => x.Name == "DuskMod") != null)
                {
                    log.WriteLine("The assembly has already been patched, bailing out");
                    return;
                }

                using (var loaderAsm = ModuleDefinition.ReadModule(patchAsm))
                {
                    var hookMethod = loaderAsm.GetType("DuskMod").Methods.Single(x => x.Name == "Init");
                    var dosLoadingScreen = hookedAsm.Types.First(x => x.Name == "DosLoadingScreen");
                    var dosLoadTextField = dosLoadingScreen.Fields.First(x => x.Name == "textField");
                    var dosLoadStart     = dosLoadingScreen.Methods.First(x => x.Name == "Start");
                    var ilproc = dosLoadStart.Body.GetILProcessor();
                    var loc = dosLoadStart.Body.Instructions[0];

                    ilproc.InsertBefore(loc, Instruction.Create(OpCodes.Ldarg_0));
                    ilproc.InsertBefore(loc, Instruction.Create(OpCodes.Ldfld, dosLoadTextField));
                    ilproc.InsertBefore(loc, Instruction.Create(OpCodes.Call, hookedAsm.ImportReference(hookMethod)));
                    hookedAsm.Write();
                    log.WriteLine("Done");
                }
            }
        }
    }
}
