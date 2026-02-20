# SF-Metroidvania-Package
There is a work in progress dedicated documentation website being made. Will update this section part way through alpha six that is currently in progress.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.
This package uses the burst compiler combined with NativeCollections for improved perforance.

## Current Alpha: Alpha Eight
Alpha Eight feature outline is still being worked out. 
Currently planned are more Low Level Physics 2D utilities and qol updates.

### Planned
- Done: Add more utlities for Low Level Physics API.
    - Done: Extension methods for setting PhysicsShapes.ContactFilter.categories  
    - Done: Extension methods for setting PhysicsShapes.ContactFilter.contacts 
- Done: Update the Save System.cs - Sooner I get this done the less chance of future alphas breaking save files in builds.
  - Done: Remove old Checkpoint and CheckPointManager class

### Wanting to implement 
Goals for this alpha, but not guaranteed.
- CinemachineRectangleConfiner Scene tools that would eliminate the need for an SFShapeComponent.
  - This would improve the set up for confining cameras in rooms.
  - Remove some of the current limiations.
- Add more SFShapeComponent Scene tools for editing the shapes properties.

#### Wanted Room Features
- Rework how rooms are loaded from the database. 
  - Relies on implementing the SF Database Registration class that was already started. 
- Find a way to set up room transitions and connected loading rooms easier.
  - Make the rooms no care what their position in the prefab is set to.
  - Create a room anchor point to define the position rooms are able to be connected to each other.

#### Have to implement in the future. Sooner the better.

Full implementation of the SF Data Editor with better UI Toolkit binding via serializable objects.
    - Merge the Data Editor with the SF Metroidvania Editor window.
    -  


## Future Features:
- Interactible Enviorment - think freezing water and burning grass. This relies on the GeometryIsland API in Unity 6.3.
- Full implementation of the Sprite Destructor to allow destructible sprites with physics. This wll be implementing the Sprite Fragmentation API added in Unity 6.3
- Updating the Data Editor for characters, items, and adding a level data editor tab to it.
- Create a core editor for SF tool related packages.