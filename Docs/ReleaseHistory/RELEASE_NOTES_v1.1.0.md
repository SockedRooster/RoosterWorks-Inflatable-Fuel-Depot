# InflataDepot 1.1.0 - Stock resource storage

Close KSP and replace the entire GameData/InflataDepot folder. Keep ModuleManager
and B9 Part Switch installed. Do not merge with an older version.

In the VAB/SPH, right-click a depot and choose Resource Configuration:
LF + Oxidizer, Liquid Fuel Only, Oxidizer Only, Monopropellant, Ore, or Xenon Gas.
All three sizes support all six configurations. Cargo selection is editor-only.
Tanks launch empty, cannot store cargo while folded, and cannot deflate in flight
while any managed cargo remains. Resource capacities are listed in README.md.
The grey finish, VAB Inflate/Deflate, automatic crossfeed and no-staging changes
are included. Existing LF/LFOX identifiers and capacities are preserved.

Validation: compiled against installed KSP/Unity/B9 assemblies; existing asset
checks passed; 222 mock-API assertions passed across six configurations and three
sizes; package integrity checked. No dependency DLLs included.
In-game testing remains pending. Check selecting all resource types, folded and
inflated launches, transfer, loaded deflation protection, empty deflation,
save/reload, symmetry and old LF/LFOX craft. Existing craft can select the new
configuration only in the editor. No modded resource options are added.
