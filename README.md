## Disclaimers
--------------
This project is ***not* official**, and thus **not** supported by *New Blood Interactive* or any of it's affiliates. Use the "Issues" provided in the *GitHub* page.
This project **DOES NOT** aim to be an alternative of the upcoming *Dusk SDK*. This is a hobby project meant for learning purposes. Do *not* use it for anything you're serious about, wait for the official SDK.

## Installation
---------------
- Download the latest version from the Releases, extract it into your "Dusk\\" folder
- Open the "Patcher\\" directory, run the "DuskModPatcher.exe" file.
- Copy any desired mod files (".dll" assemblies) into the "mods\\" directory
- Have Fun!

In case the process works, you should get a message on the fakedos startup telling you about the loaded mods, otherwise, check the Patcher\\DuskModPatcher.log file for errors.

## Development
--------------
If Steam is not installed in the default location on your main drive, and/or Dusk is installed in a separate Steam library folder, set the `DuskDir` environment variable to your Dusk game directory.

You may alternatively set the path when building via the `dotnet` CLI:  
`dotnet build -p:DuskDir="path/to/game/directory" -c Release`

Add the "DuskMod.dll" assembly into your mod and any necessary assembly references from the game's Managed directory.

Check the included mods and *Harmony's* documentation for a QuickStart.

## License
----------
This software accompanies the "3-Clause BSD license", read the "license.txt" file. I am not affiliated with New Blood, it's developers, affiliates or Dusk. Do not ask New Blood for support in using this, use the provided GitHub "issues" page.
