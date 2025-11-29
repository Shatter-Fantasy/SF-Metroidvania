# SF-Metroidvania-Package


> There is a work in progress dedicated documentation website being made. If you check the documentation and it gives a 404 error, that means a new build of it is being uploaded and deployed at the moment. It will be back up in less than two minutes.
> https://shatter-fantasy.github.io/SF-Metroidvania/manual/install-instructions/install.html

## Install Instructions are located in the manual page linked right above this sentence.

## Summary 
This is the Shatter Fantasy Metroidvania Unity package that can be used to create any game needing Metroidvania like controls. 
It is using Unity's low level physics that was added in Unity 6.3 creating a minimum required version for Unity 6.3 editors.

## Current Alpha: Alpha Five
Alpha Five and all development from now on goes to using Unity's low level physics 2D API introduced in Unity 6.3.
This has enabled a lot of new features not easily do able before with a massive, on average, 312% increase in performance in all scenes with physics in the Immortal Chronicles project that is using this package.

This means the use of all Collider2D and all Rigidbody2D are being removed in Alpha 5 release.
Alpha Five also brings forward the new streamlined SpawnSystem and removes all old struct events in favor of using C# event Actions where reasonable.


### Future Features:
- Interactible Enviorment - think freezing water and burning grass. This relies on the GeometryIsland API coming in Unity 6.3 first stable release out of beta.
- Updating the Data Editor for characters, items, and adding a level data editor tab to it.
