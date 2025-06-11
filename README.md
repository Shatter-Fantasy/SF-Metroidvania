# SF-Metroidvania-Package

## Install Instructions are located at the bottom of the read me in detail.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
At the moment it is for 2D only physics, but we will be adding 3D hysics support when Unity 6.3 comes out. 
This is because there is a major rework from the ground up of Unity's 2D physics coming that allow making custom 3D world like interactions with 2D physics colliders.



## Current Alpha: Alpha One
Currently the first alpha is being worked on a seperate branch from the main branch. 
Look into the alpha-one branch if you want to try out the package. 


### Already Completed Upcoming Features:
Please note there is more than the typed below, but instead of creating a wall of text for people to read, I am currently working on a manual with videos to show how to do stuff.

#### Physics

##### General Physics
- Collision Controller that can be used both in and outside the CharaterControllers to implement custom Collision events and callbacks.
##### Character Controller
- Custom Character Controller that works for grounded, flying, and swimming characters.
- The custom controller also supports playable, enemy, and NPC characters without any extra changes needed.
- Quite of a bit of improvements brought to the custom collision detection calculations in the Controller for physics.
  - Note the Playable character has a special component called PlayerController2D that helps auto set up some input stuff.

##### Global Interaction Controllers
- Interaction Controllers allow anything to interact with anything that has a component that inherit from IInteractable.
  - There is a Player only PlayerController to link up input systems for interactions.
- Interaction controllers can interact with switches, NPC to start a dialogue, or other things that might need a custom interaction event.

#### Gloabl Event System - Does not require any references in the scene between objects to work.
- Global Event Manager allows listening and reacting to events called from any gameobject without a reference.
- Can create custom events that pass in any value and sends it to things listening.
- Can create custom validation for events before and after telling listeners the event is invoked to make sure any event data being sent is not of an incorrentformat or null.
- There are several events already premade for things like respawning, pausing game, changing game control from player to menus/in dialogue, and a lot more.

### Currently In Progress Features
- AIState for giving characters the ability to do detection systems like seeing while patrolling an area in games.
- AIState for NPC characters to patroll an area back and forth.
- Options menu system for volume control and some graphic setting event listeners.
   - Fullscreen, windowed, borderless window
   - Background music, master, sound effect, and ambient sound volume settings.

## Demo Videos
There will be demo videos as soon as I finished the options menu events.

## WIP API Documentation.
Please note the documentation is very early wip. The manual link at the top left is not ready yet. So clicking it does nothing. We are working on videos currently for the manual. 
Currently somethings are not in the final location for namespaces. In the root SF namespace you will see some classes, structs, or interfaces needing moved to their proper namespaces.

[WIP SF Platformer Documentation](https://crowhound.github.io/SF-Platformer/api/SF.Physics.CollisionInfo.html)

## Install Instructions
This package was built with the idea of using Unity's built in package manager to help make installing and choosing which version of the package you want to use easier.
Here is the official Unity documentation page if you want to do a full read through instead of reading the short answer below.

[Unity Custom Git Package Documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-git.html#extended)

1. Open up Unity's package manager editor window and click the button with a plus sign at the top left of the package manager window. It should have a small dropdown arrow icon by the button with a plus sign on it.

![install-instructions-1](https://github.com/user-attachments/assets/de316cc8-5498-4496-b702-221b6f2b73f7)

   
2. Choose install package from git url and paste in the following.

https://github.com/crowhound/SF-Platformer.git

Optional for choosing a specific version of the SF Platformer package to install.
Unity supports git revision syntax allowing you to add options at the end of the Git url to customize your package download.
All options are added onto the git url after the pound symbol # is added to the end.

The options are as followed:
1. Specific branch  - #name-of-branch
2. Specific version for package release. Note the letter v before the numbers - #v0.0.1 would give you the first alpha release while for pre-alphas you would type #pre-alpha.9
3. A specific commit if you want to try out a commit with a feature that hasn't been published in a release yet - #git-commit-hash
Example for the specific commit hash #76c6efb35ac8d4226a22f974939f300231a3637f. This is the hash for the commit added right before pre-alpha 9 release.

Full example for wanting to get the SF Package that is release version alpha 1
https://github.com/crowhound/SF-Platformer.git#v0.0.1

Full example for wanting to get the SF Package that is being worked on in the alpha-two branch
https://github.com/crowhound/SF-Platformer.git#alpha-two

