> [!WARNING]
> Unity 6.4 and 6.5 has a set of Native Crashes that are part of the Engine related to the Tilemap updates. These are not caused by the the package.
> Use Unity 6.4 patch 4 or 6.5 beta 5 at least to avoid these crashes. I am helping Unity Technologies with them and we got all, but one fixed involving update to SRP batcher.
> Link to our public forum posts for people to stay updated on the crashes.
> https://discussions.unity.com/t/tilemap-crashes-and-rendering-bugs-in-unity-6-4-6-5-and-6-6-collection-post/1715496/14

# SF-Metroidvania-Package
There is a work in progress dedicated documentation website being made.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's Physics Core 2D that was added in Unity 6.5, (originally Low Lvel Physics 2D introduced in 6.3) creating a minimum required version for Unity 6.5 editors.
The Physics Core 2D package is used to link into Box2D Version 3.1 to create a custom Physics System from the ground up.
We are aiming to have the package be released for 6.7 LTS. This package uses the burst compiler combined with NativeCollections for improved perforance.
Read section "Why Unity 6.7 and not Unity 6.3" for more information on version requirements

## Current Alpha: Alpha Ten Changes

### Important Changes:
- We now have the Physics Core 2D implemented so Unity 6.5 is the min version as mentioned we are aiming for a Unity 6.7 release with no technical debt.
This includes a lot of namespace changes from Unity's side. 
See Unity post here for the namespaces changes for Box2D 3.1 related API.

https://discussions.unity.com/t/physics-core-2d-in-unity-6-5/1715178


### Added Stuff

#### Region System replacing the need for a room database.
Alpha Ten includes the ground up rework for the region and room sytem. This simplies a lot of set up for both of them.
There is now a Region Database scriptable object that replaces the Room Database.
Note Room System is still used, but implements helper logic via the Region Database now.

This adds a lot including, but not limited to:
- Custom room position spawning when transitioning between regions.
- Simple set up for choosing which room to load when changing regions.
- A region Data asset to help quickly set up rooms and room transitions easily.


### Fixed Bugs
- Lot of the Spawn System issues caused by the room system messing up the spawn point for loading rooms.


### Planned for Alpha Ten
- Update Slope calculations. Implement PhysicsQuery with burst compilation.


## Future Features:

### Future Major Core Features
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
