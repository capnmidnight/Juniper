# Development

Clone the repository and copy the `Package/` directory to one of the `Test/<PlatformName>Test/Assets` directories.

## Tools

- [Unity Hub v0.20.1](https://blogs.unity3d.com/2018/01/24/streamline-your-workflow-introducing-unity-hub-beta/) (optional). Unity Hub is very handy for managing different versions of Unity between different projects.

- [Unity Editor v2018.2.0f2](https://unity3d.com/get-unity/download/archive). Unity 2018 has a package manager available that makes it easier to keep projects and their dependencies in sync without having to save all of the dependencies in the repository.

- [Visual Studio for Mac, Community Edition v7.5.2](https://visualstudio.microsoft.com/vs/mac/). Any editor is probably fine.

- **Vuforia v7.1.35**. This is the default version that comes with Unity 2018.2.01f2.

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
