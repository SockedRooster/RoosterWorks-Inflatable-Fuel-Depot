# Build the plugin

Players install the release ZIP; no build is needed.
Run Build-Plugin.ps1 -KSPDir "path to KSP" on Windows with KSP 1.12.x and
B9 Part Switch installed. The script uses the .NET Framework C# compiler and
references local KSP, Unity and GameData/B9PartSwitch/Plugins/B9PartSwitch.dll.
Only InflataDepotPlugin.dll is distributed; dependency binaries are not bundled.

Source/Tests/ResourceRegression.cs is an offline mock-API harness. Compile it
with Source/InflataDepotPlugin/ModuleInflataFuelLock.cs into a separate executable
without game references, then run it. Never use that test executable as the plugin.
The harness covers six configurations and three sizes. It does not simulate
KSP/Unity/B9 initialization, UI or actual resource transfer.

Source/test_assets.py validates models, textures and config basics.
Source/build.py regenerates procedural assets/configs; do not run it for a
plugin-only release, as it replaces released models. Tank definitions and
subtypes in that generator are kept aligned with the packaged configs.
