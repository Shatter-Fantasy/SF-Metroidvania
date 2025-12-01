# Package Dependencies

## Unity Packages

Inside the SF Metroidvania package there is an automatic system that installs the required packages if you don't have them already instlled.
There are two scripts called SFHubPackageSystem and SFPackageData. They help install other SF packages that they rely on.

### Cinemachine 

Version 3.1.4

> We use 3.1.4 for everyone's sanity. There are several bugs in the previous versions with Confiner2D and a combination of the Input System package that made . 


## Other SF Packages

### Important Notice
Alpha Five is adding auto package set up implementating packages automatically.
Some packages requirements are being removed by adding Compilation Branching checks via scripting defines to allow the package to work without needing all of the other SF packages.
Most of the SF packages are utility packages with performance and quality of life functionality that makes adding new stuff easier.

Mainly the SF Utilities and the SF UI Elements.

### SF Utilities
As of writing this documentation out the main branch will be used, but moving to a specific branch system for making updates more stable once the package goes public.

https://github.com/Shatter-Fantasy/SF-Utilities

### SF Sprite Tools - Warning the most wip of them all.
This one has a lot of TileMap utilities and a wip custom tile map editor and also a custom Sprite Editor with one click create animation clip support.
This is used for high performance operations with the TileMapShape component inside of the SF Metroidvania toolkit.

https://github.com/crowhound/SF-Sprite-Tools

### SF UI Elements
Heavily used for the custom item and enemy data editor. Has a system being worked on to create custom editors for database with little code need.
Also used in the WIP Sprite Editor and WIP TileMap Editor.

https://github.com/Shatter-Fantasy/SF-UI-Elements
