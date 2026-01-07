# SF-Metroidvania-Package
There is a work in progress dedicated documentation website being made. Will update this section part way through alpha six that is currently in progress.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.

## Current Alpha: Alpha Six
Alpha six is being dedicated to clean up post alpha five major package changes and adding some scene tools for making the physics components easier to work with.

- SFShapeComponent scene tools
- Improved database registering to make sure they always are loaded first simplfying the initial game loading code flow. Mainly lowers errors for not ready databases.
- Reimplementing the UXML Schema that I purposely broke when merging SF UI Elements and SF Utilities to a dedicated single package called SF Core.
- Some more legacy code and class clean up.

### Future Features:
- Interactible Enviorment - think freezing water and burning grass. This relies on the GeometryIsland API in Unity 6.3.
- Full implementation of the Sprite Destructor to allow destructible sprites with physics. This wll be implementing the Sprite Fragmentation API added in Unity 6.3
- Updating the Data Editor for characters, items, and adding a level data editor tab to it.
- Create a core editor for SF tool related packages.

## Known Issues
These issues are known and are currently being worked on if it is not a Unity Engine side bug.

### Unity Side Bug: Unity 6.4 beta to Unity 6.5 Rule Tile Crash
There are Tile Related crashes that are caused by EntityID related API implemented from Unity 6.4 and newer versions.
This is related to the OnTileRefreshPreview that is called by Unity.

### The Cinemachine Physics2D Shape is early WIP.
As of December 1st it is being worked on so not a smooth confiner yet for Cinemachine cameras.
Most likely be in alpha seven.
