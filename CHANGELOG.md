# Changelog

## Alpha Seven:
There are more misc changes not shown below. Too many to mention during a alpha release version.

---

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

---

## Updated:

### SFShapeComponent:
- Improved how it calls IShapeComponent callbacks.
- Created ways to sync the Transform and PhysicsTransform.
    - See the ITransformMonitor interface. for the transform syncing.
- Implemented Hit Boxes using low level physics.


### AI Pathfinding
- Now implements low level physics for obstacle detection during grid creationg for the paths.

---

## Removed:
- All usages of the Low Level 2D Extras package was removed. It is no longer needed.
- Removed almost all legacy code or unuesed code. Some remain, but are being rewritten in upcoming alphas instead of being removed completely.
