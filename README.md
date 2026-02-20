# SF-Metroidvania-Package
There is a work in progress dedicated documentation website being made. Will update this section part way through alpha six that is currently in progress.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.

## Current Alpha: Alpha Seven
Alpha Seven is dedicated to removing old legacy code and improving API usage while simplifying how to use most low level physics api.

## Added:

### SFPhysicsManager:
Added a static class to manage certain properties of the SF Phsyics and Unity Low Level Physics 2D API.
- Started addding const ints for PhysicsMask layers to help know which bit needs flipped for a layer.
- Has debugging options for Physics Rendering
- Has debugging options for SF Physics Validation
  - PhysicsShape validation
  - PhysicsBody validation

PhysicsShapes Extensions:
- Added new ways to get Components on PhysicsShapes and PhysicsBodies.
- Added new ways to get Components during most Physics Events.
    - ITriggerBeginEvent
    - IContactBeginEvent
- Added a lot of helper methods for:
    - PhysicsAABB
    - 
### Camera Tools:
- Added a CinemachineRectangleConfiner to allow keeping a camera inside a certain room bounds.

## Updated:

### SFShapeComponent:
- Improved how it calls IShapeComponent callbacks.
- Created ways to sync the Transform and PhysicsTransform.
  - See the ITransformMonitor interface. for the transform syncing. 
- Implemented Hit Boxes using low level physics.


### AI Pathfinding
- Now implements low level physics for obstacle detection during grid creationg for the paths.


## Removed:
- All usages of the Low Level 2D Extras package was removed. It is no longer needed.
- Removed almost all legacy code or unuesed code. Some remain, but are being rewritten in upcoming alphas instead of being removed completely.


### Future Features:
- Interactible Enviorment - think freezing water and burning grass. This relies on the GeometryIsland API in Unity 6.3.
- Full implementation of the Sprite Destructor to allow destructible sprites with physics. This wll be implementing the Sprite Fragmentation API added in Unity 6.3
- Updating the Data Editor for characters, items, and adding a level data editor tab to it.
- Create a core editor for SF tool related packages.