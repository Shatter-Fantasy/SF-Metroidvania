# Install Instructions

There are two ways to install the package. 

## Install via Unity Package Manager 

This package was built with the idea of using Unity's built in package manager to help make installing and choosing which version of the package you want to use easier. Here is the official Unity documentation page if you want to do a full read through instead of reading the short answer below.

[Unity Git Package Documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-git.html#extended)

### Step-by-step instructions
1. Open up Unity's package manager editor window and click the button with a plus sign at the top left of the package manager window. It should have a small dropdown arrow icon by the button with a plus sign on it.
![Unity GitHub Package.jpg](../../resources/images/Unity%20GitHub%20Package.jpg)

2. Choose install package from git url and paste in the following. Note the current Metroidvania package is being worked on in the alpha-one branch. I would advise following the optional steps below to grab it. https://github.com/Shatter-Fantasy/SF-Metroidvania.git

Optional for choosing a specific version of the SF Platformer package to install. Unity supports git revision syntax allowing you to add options at the end of the Git url to customize your package download. All options are added onto the git url after the pound symbol # is added to the end.

The options are as followed:

1. Specific branch - #name-of-branch
2. Specific version for package release. Note the letter v before the numbers - #v0.0.1 would give you the first alpha release while for pre-alphas you would type #pre-alpha.9
3. A specific commit if you want to try out a commit with a feature that hasn't been published in a release yet - #git-commit-hash Example for the specific commit hash #76c6efb35ac8d4226a22f974939f300231a3637f. This is the hash for the commit added right before pre-alpha 9 release.

Full example for wanting to get the SF Package that is a release version
https://github.com/Shatter-Fantasy/SF-Metroidvania.git#v0.0.1

Full example for wanting to get the SF Package that is being worked on in the alphas/alpha-five branch
[https://github.com/crowhound/SF-Platformer.git#alpha-one](https://github.com/Shatter-Fantasy/SF-Metroidvania.git#alphas/alpha-five)
