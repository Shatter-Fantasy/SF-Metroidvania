# SF-Metroidvania-Package
There is a work in progress dedicated documentation website being made. Will update this section part way through alpha six that is currently in progress.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.
This package uses the burst compiler combined with NativeCollections for improved perforance.

## Current Alpha: Alpha Eight
Alpha Eight focused on a smaller set of qol api changes, simplifying the SavePoint logic,
and improvements to the SFRectangleShape scene editing tool. 

- SFShapeComponent scene tools
- Improved database registering to make sure they always are loaded first simplfying the initial game loading code flow. Mainly lowers errors for not ready databases.
- Reimplementing the UXML Schema that I purposely broke when merging SF UI Elements and SF Utilities to a dedicated single package called SF Core.
- Some more legacy code and class clean up.

## Future Features:

### Major Core Features
- Interactable Environment - think freezing water and burning grass. This relies on the GeometryIsland API in Unity 6.3.
- Full implementation of the Sprite Destructor to allow destructible sprites with physics. This wll be implementing the Sprite Fragmentation API added in Unity 6.3
- Updating the Data Editor for characters, items, and adding a level data editor tab to it.
- Create a core editor for SF tool related packages.

#### Future Data Editor Feature
Full implementation of the SF Data Editor with better UI Toolkit binding via serializable objects.
- Merge the Data Editor with the SF Metroidvania Editor window to create just the SF Metroidvania Editor window.
- Create a way to define regions in the database.
- Create ways to make data sections easier to be made.

#### Future Room Features
This might end up being a full on Room Editor tool to even help change spawned characters, items, and more.
- Rework how rooms are loaded from the database. 
  - Relies on implementing the SF Database Registration class that was already started. 
- Find a way to set up room transitions and connected loading rooms easier.
  - Make the rooms no care what their position in the prefab is set to.
  - Create a room anchor point to define the position rooms are able to be connected to each other.

### Future Scene Tool Improvements
- CinemachineRectangleConfiner Scene tools that would eliminate the need for an SFShapeComponent.
    - This would improve the set-up for confining cameras in rooms.
    - Remove some of the current limitations.
- Add more SFShapeComponent Scene tools for editing the shapes properties.