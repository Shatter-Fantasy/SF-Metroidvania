# Changelog

## [0.0.3] - 2023-04-27

### Added 

### AI Section
- Added the StaeMachineBrain which controls the States of the controlling object.
- Added the StateCore class that is the core class for all states in the StateMachine system
- Added StateDecisionCore which is an automatic decision system to make AI change states based on certain conditions without having to manually call the state machine brain to update to a new state. 

## [0.0.2] - 2023-04-23

### Added

#### Physics Section
- Added the Controller2D class that calculates the physics for characters.
- Added a MovementProperties struct that is used to help set the current movement physics of characters. 
- Added PhysicsVolume that can be used to change the Physics of things that enter it. Ex. Character walking into water.

#### Input
- Created Controls class that is the Unity Input Action Class for the Toolkit.
- Created the InputManager class that acts like singleton static helper class for controlling player input.

### Changed

#### Package Settings
- Updated the Unity Registery Package Dependencies 
    - Input System 1.5.1
    - Cinemachine 3.0.0-pre.4


## [0.0.1] - 2023-03-31

### Added
- Initial example changelog data.