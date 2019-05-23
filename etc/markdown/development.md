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

