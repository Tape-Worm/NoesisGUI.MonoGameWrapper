NoesisGUI 2.0 MonoGame Integration experiment (forked from [aienabled](https://github.com/aienabled/NoesisGUI.MonoGameWrapper))
=============
This library provides a solution for integrating [NoesisGUI 2.0](http://noesisengine.com) with the [MonoGame 3.*](http://monogame.net) library.

Currently it supports only MonoGame projects for Windows DX11.

An example MonoGame project with integrated NoesisGUI is included.

This library supports the following new features:
* **"XAML hot reloading"**; when any XAML file in the data folder is changed, the full UI is re-created. This is most useful if you use the MVVM pattern in your application, since you don't have to restart your application to tweak the UI.
* Helper method to place the console and game windows on the leftmost resp rightmost monitor (useful if you have a multi-monitor setup, like many game developers)
* Exposes a method to regenerate the mipmaps of a monogame render target (see demo)
* Lots of ugly hacks to get monogame keyboard input working with Noesis textboxes (need to dig deeper into this)

Prerequisites
-----
* [Visual Studio 2017](https://www.visualstudio.com/), any edition will be fine (2015 might work, untested)
* [MonoGame 3.* for VisualStudio](http://monogame.net) (tested with 3.5.1)

Installation
-----
1. Download the **2.0 C# Windows SDK** from [NoesisGUI Forums](http://www.noesisengine.com/developers/downloads.php).
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
3. Open `NoesisGUI.MonoGameWrapper.sln` with Visual Studio
4. Open context menu on `TestMonoGameNoesisGUI` project and select `Set as StartUp Project`.
5. Press F5 to launch the example game project.
6. You can change the XAML files while the game is running

Known issues
-----
* DeviceLost and DeviceReset are not implemented (not required for DX11 I guess, but might be for other renderers)
* Text editing is partially fixed in this issue (the repeat rate for non-printable keys is different from the OS)

License
-----
The code provided under MIT License. Please read [LICENSE.md](LICENSE.md) for details.
