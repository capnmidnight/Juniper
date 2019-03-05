# Development

Clone the repository and copy the `Package/` directory to one of the `Test/<PlatformName>Test/Assets` directories.

## Tools

- [Unity Hub](https://unity3d.com/get-unity/download). Unity Hub is very handy for managing different versions of Unity between different projects. There is also a link in Unity Hub to the Unity Download Archive for any version that Unity does not advertise through the Available Installs screen.
	- **For Magic Leap**: [Magic Leap maintains their own fork of the Unity Editor](https://unity3d.com/partners/magicleap). It's currently an older version of Unity that is not compatible with the new Nestable Prefabs system, so be careful not to mix versions. You can upgrade from 2018.1 to later versions, but your scenes and prefabs will crash the editor if you try to downgrade. For that reason, there are copies of scenes and prefabs in the Juniper project specifically for Unity 2018.1. 
- [Visual Studio](https://visualstudio.microsoft.com/vs/). Unity can install this by default for you.
- [Blender](https://www.blender.org/download/). There are a few 3D models in the project repository that Unity requires Blender to be installed to be able to import them.
- [Toudor](https://nerdur.com/todour-pl/). The issues list for Juniper is maintained as a `todo.txt` file in the root of the project. Toudour provides a nice GUI for editing the list.
- [Doxygen](http://www.doxygen.nl/download.html). This is only necessary for updating the documentation in Juniper, but it's a good idea to use it on your projects, too. Make sure to add it to your PATH.
- [GraphViz](https://www.graphviz.org/download/). This is used by Doxygen to generate nicer diagrams for the documentation. Make sure to add it to your PATH.
- [scc](https://github.com/boyter/scc/releases). This is used by the documentation generation scripts to estimate the project complexity. Make sure to add it to your PATH.

## Debugging

The prefab at:

    Juniper/Prefabs/DebugUI.prefab

Contains a Canvas and Text element that is used by the various components to print out their status.

GPSLocation, SunPosition, Weather, and OutdoorLightEstimate all have boolean values controlling whether or not they provide real world values versus static values defined at design time. That boolean is called `Fake <X>`, where X is whatever property is being faked. They also include a boolean called `Print Debug Report` that enables showing the current status of the component on the ScreenDebugger.

## Platform Notes

These are notes specific to each AR subsystem and/or operating system

### Hololens

#### Build settings
The Hololens documentation talks about a Unity Build setting "C# Projects" that enables completing the build and deploying to the device from Visual Studio rather than from Unity. The rationale for this feature is that it makes it faster to iterate on *changes* to scripts. If you want to make changes to the scene, to any assets, or add any scripts, you need to still do it through Unity.

As of Version 2017.2 of Unity, this feature is broken, defaulting the Build Target Architecture to ARM. Just manually change the architecture to x86 (not x64).
