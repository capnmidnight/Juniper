# Juniper For Fun and Profit

This is a bunch of code that I use in my VR projects. The following README block is very out of date, but I don't consider this project ready for other people to use, yet.

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

# Code Metrics

```
───────────────────────────────────────────────────────────────────────────────
Language                     Files       Lines      Code    Comments     Blanks
───────────────────────────────────────────────────────────────────────────────
C#                            1657      268811    189507       45443      33861
───────────────────────────────────────────────────────────────────────────────
Total                         1657      268811    189507       45443      33861
───────────────────────────────────────────────────────────────────────────────
Estimated Cost to Develop $17,735,319
Estimated Schedule Effort 31.526044 months
Estimated People Required 25.002706
───────────────────────────────────────────────────────────────────────────────


```

