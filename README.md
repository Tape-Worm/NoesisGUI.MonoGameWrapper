NoesisGUI 1.3 beta MonoGame Integration experiment (forked from [aienabled](https://github.com/aienabled/NoesisGUI.MonoGameWrapper))
=============
This library provides a solution for integration [NoesisGUI 1.3](http://noesisengine.com) with [MonoGame 3.*](http://monogame.net) library.
Currently it supports only MonoGame projects for Windows DX11.
Example MonoGame project with integrated NoesisGUI is included.

Remarks
-----
This code contains some unofficial experiments with sharing Monogame textures with Noesis. 
You need to comment-out that code to get it running with 1.3 beta2

Prerequisites
-----
* [Visual Studio 2015](https://www.visualstudio.com/), any edition will be fine.
* [MonoGame 3.* for VisualStudio](http://monogame.net)

Installation
-----
1. Download the 1.3 C# API Windows SDK from [NoesisGUI Forums](http://www.noesisengine.com/forums/download/file.php?id=229).
2. Extract it to the folder `\NoesisSDK\`. The resulting directory tree should then contain the following subfolders:
        
        NoesisSDK
          |--Bin
          |--Doc
          |--Samples
          |--Src
        
3. Open `NoesisGUI.MonoGameWrapper.sln` with Visual Studio 2013/2015
4. Open context menu on `TestMonoGameNoesisGUI` project and select `Set as StartUp Project`.
5. Press F5 to launch the example game project.

Known issues
-----
* DeviceLost and DeviceReset are not implemented (not required for DX11 I guess, but might be for other renderers)
* Text editing is partially fixed in this issue (the repeat rate for non-printable keys is different from the OS)

License
-----
The code provided under MIT License. Please read [LICENSE.md](LICENSE.md) for details.
