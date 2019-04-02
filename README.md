# Juniper For Fun and Profit

[TOC]

# Introduction

Juniper is a framework within Unity 3D for managing and developing virtual, augmented, and mixed reality applications, within the context of a metaphor of the human body. It provides the following sub-modules:

- *Data* - Managing HTTP requests and streaming assets, all from background threads that do not lock up the main UI thread.
- *Ground* - Terrain rendering and AR surface reconstruction base code.
- *ImageTracking* - Image-marker tracking base code.
- *Input* - Base-code for handling user input.
- *NeuroSky* - Wrapper code for handling [the NeuroSky MindWave device](https://store.neurosky.com/pages/mindwave).
- *Progress* - Interfaces for representing progress meters.
- *Statistics* - Collections that perform statistical analysis on their contents.
- *Units* - Conversions between and formattings of values tagged with units of measure.
- *Unity* - Components and implementations for the Unity platform.
	- *Anchoring* - Components for fixing objects in space, with persistence on AR platforms.
	- *Animation* - Basic animations on specific, common object properties, without having to engage the full power of Mecanim.
	- *Audio* - Audio management and effects components.
	- *Collections* - Unity-specific collection manipulation code.
	- *Display* - Components for managing VR and AR display modes.
	- *Events* - Custom UnityEvent classes that pass a few primitive values as event arguments.
	- *Ground* - Concrete implementations of AR surface reconstruction and terrain rendering.
	- *Haptics* - Components for performing high-quality haptic feedback on systems that support it.
	- *ImageTracking* - Concrete implementations for platform-specific marker tracking.
	- *Imaging* - Loading and processing PNG and raw images.
	- *Input* - Handling user input: keyboard, mouse, touch, motion controllers, and speech.
	- *LightMapping* - Swapping light map sets at runtime to have different, baked lighting conditions without switching scenes.
	- *Progress* - Implementations of the IProgress interface for use in Unity experiences.
	- *Statistics* - Statistical analysis collections for Unity's Vector structures.
	- *Widgets* - Visual elements with which the user may interact.
	- *World* - Components for understanding and integrating with the real world: light estimation, global positioning, translating between spatial reference systems, receiving weather reports.
- *UnityEditor* - Components that run in the UnityEditor itself, with custom editors for a few of Juniper's components.
	- *ConfigurationManagement* - Listings of VR and AR platforms, their dependencies, and a dependency manager for automatically converting Unity projects between different platforms.
- *World* - Physical world units and data.
	- *Climate* - Code relating to weather reporting.
		- *OpenWeatherMap* - Service integration for [OpenWeatherMap](https://openweathermap.org/)'s API.
	- *GIS* - Latitude/Longitude points and conversions to different coordinate frames.

# Setup

## Installation

There are two ways to install Juniper.

- [Download the UnityPackage](Juniper.unitypackage) and install it in your project, or
- [Clone the repository](https://github.com/capnmidnight/Juniper) and copy the contents of the `src/Juniper/Assets/Juniper/` directory to your Assets folder in a directory named `Juniper`. This will also give you access to the `examples/` directory to see how projects are setup with Juniper.
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

# Code Metrics

```
───────────────────────────────────────────────────────────────────────────────
Language                     Files       Lines      Code    Comments     Blanks
───────────────────────────────────────────────────────────────────────────────
C#                             494       54928     33374       14850       6704
───────────────────────────────────────────────────────────────────────────────
Total                          494       54928     33374       14850       6704
───────────────────────────────────────────────────────────────────────────────
Estimated Cost to Develop $2,099,814
Estimated Schedule Effort 15.766558 months
Estimated People Required 8.072197
───────────────────────────────────────────────────────────────────────────────


```

