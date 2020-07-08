# AutoGunfireReborn
A tool that adds easy mode to Gunfire Reborn.

This project shows how to inject Unity modules during runtime into il2cpp Unity games.

## Features
 - AutoAim - adds the auto-aim functionality from controller usage when playing with keyboard & mouse
 - ExtraSensoryPerception - shows where portals, chests, vendors, and enemies are located (through walls)
 - FreeCam - not functional yet. Goal is to add a free camera to wander the world.
 - GameSpeed - speeds up the game
 - JumpHeight - changes the jump height of your character, making all puzzles trivial.
 - MovementSpeed - lets you run as fast as the Flash.
 - UnlimitedAmmo - no need to reload or find ammo anymore.
 - UnlockAll - not functional yet. Goal is to unlock all weapons/scrolls/etc., but this is server-sided for the most part.
 - WeaponMod - not functional. Currently only adds map-wide AOE range to explosive weapons.
 
## Todo / current issues
 - Occasionally crashes on injection
 - Deployment / packaging requries lots of DLL's, would be nice to have a single exe. Fody doesn't work as is.
 - Dumper needs to be manually run on game update
 - Most of the logic of Gunfire Reborn is done outside of Unity in a native dll / python.
 - App that does the injection is just a simple winform - need to clean this up.

## Credits
https://github.com/Perfare/Il2CppDumper - dumps out dummy DLLs from Unity il2cpp game

https://github.com/knah/Il2CppAssemblyUnhollower - converts dummy DLLs from Il2CppDumper into native calls. knah also helped me debug along the way

https://github.com/warbler/SharpMonoInjector - calls mono functions after mono is loaded


https://github.com/HerpDerpinstine/MelonLoader - not used in this project directly, but I leveraged the workflow to inject mono into the target app