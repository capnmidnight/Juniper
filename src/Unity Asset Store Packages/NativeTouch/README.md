iOS Native Touch 
Sirawat Pitaksarit / Exceed7 Experiments
Contact : 5argon@exceed7.com
----

# How to use

All of instructions to use this is in `HowToUse > HowToUse.zip`. Please unzip it somewhere not in your project or Unity will import it as one of your game's asset.

It is an offline version of this website : http://exceed7.com/native-touch

# Demo scene

Also there is a demo scene in `Demo` folder. All of the button does not work in editor, you have to test them in the real iOS device.

# Extras

- Contains an Android Studio project that can produce the `.aar` in `Plugins/Android` folder.
- A bonus `NativeTouchTracker`, the touch processing wrapper over Native Touch. Use at your own risk, but I do use it in my game too. It requires C# 7.0 (Incremental Compiler package), `Unity.Mathematics`, and `Unity.Collections`. Unzip the zip file and you should find a self-contained assembly `E7.NativeTouch.Tracker`.