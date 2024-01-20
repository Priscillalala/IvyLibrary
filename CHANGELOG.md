## 2.0.0
This update is huge; the change list will not be exhaustive
* Remove IvylPatcher
	* Remove `BepInConfig` attribute
	* Remove `PluginComponent` attribute and system
	* Add `BaseModuleAttribute`: this is an experimental feature intended replace plugin component functionality with a more customizable system built on searchable attributes
* Add XML documentation to relevant classes and methods
* Update file structure (introducing...folders!)
* "Ivyl" namespace renamed to "IvyLibrary"
* Shader swapping has been improved and moved to [ShaderSwapper](https://thunderstore.io/package/Smooth_Salad/ShaderSwapper/)
* Remove `ContentPackage` and `ExpansionPackage`
	* Add `BaseContentPlugin`: a specialized plugin with a generic implementation of `IContentPackProvider`. This comes with the goal of making asynchronous content loading more approachable 
	* Add content pack extensions to replace `ContentPackage` functionality
* Remove RecalculateStatsAPI dependency
	* Remove `IOnGetStatCoefficientsReciever`
* Remove a few other overly-specific util classes and merge the rest into the `Ivyl` class
* Remove `RoR2Asset` and `RoR2AssetGroup`
	* These didn't play nice with the RoR2 content loading systems
	* Add utils to `Ivyl` for loading both addressable and bundled assets
* Tons of other quality of life and bug fixes!

## 1.1.0 - Async all the way down
* Add `ArtifactCode.CopyToFormulaDisplayAsync`
* Content packages now generate asset ids for networked prefabs with empty asset ids
* Expansion package now loads the default expansion disabled icon asynchronously
* Add `ArtifactCompound.FindArtifactCompoundDefAsync'
* Fix `NetworkSoundEventDef.SetEventName` being mistakenly named `NetworkSoundEventDef.SetFlags`
* Add `RoR2Asset` and `RoR2AssetGroup` for managing addressable assets
* Add `Prefabs.CreatePrefab` and `Prefabs.ClonePrefab` for runtime prefab creation
* Minor stubbed shader swapping optimizations

## 1.0.0
* First release