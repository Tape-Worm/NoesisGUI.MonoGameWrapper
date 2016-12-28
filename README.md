NoesisGUI 1.3 beta 3 MonoGame Integration experiment (forked from [aienabled](https://github.com/aienabled/NoesisGUI.MonoGameWrapper))
=============
This library provides a solution for integration [NoesisGUI 1.3](http://noesisengine.com) with [MonoGame 3.*](http://monogame.net) library.

Currently it supports only MonoGame projects for Windows DX11.

Example MonoGame project with integrated NoesisGUI is included.

The wrapper optionally supports some kind of **"XAML edit-and-continue"**; when any XAML file in the data folder is changed, the full UI is re-created. This is most useful if you use the MVVM pattern in your application, since you don't have to restart your application to tweak the UI.

Prerequisites
-----
* [Visual Studio 2015](https://www.visualstudio.com/), any edition will be fine.
* [MonoGame 3.* for VisualStudio](http://monogame.net)

Installation
-----
1. Download the **1.3 beta3 C# Windows SDK** from [NoesisGUI Forums](http://www.noesisengine.com/forums/viewtopic.php?f=3&t=947).
2. Extract the SDK to this project's folder `NoesisSDK`. The resulting directory tree should then look like:
```
         NoesisSDK
          |--Bin
          |--Doc
          |--Samples
          |--Src
          |--index.html
          |--version.txt
```
3. Open `NoesisGUI.MonoGameWrapper.sln` with Visual Studio 2015
4. Open context menu on `TestMonoGameNoesisGUI` project and select `Set as StartUp Project`.
5. Press F5 to launch the example game project.
6. You can change the XAML files while the game is running

Known issues
-----
* DeviceLost and DeviceReset are not implemented (not required for DX11 I guess, but might be for other renderers)
* Text editing is partially fixed in this issue (the repeat rate for non-printable keys is different from the OS)
* I experienced some layout and rendering problems when typing text; most likely this is a bug in Noesis GUI beta3, since it did not happen with beta2

License
-----
The code provided under MIT License. Please read [LICENSE.md](LICENSE.md) for details.
