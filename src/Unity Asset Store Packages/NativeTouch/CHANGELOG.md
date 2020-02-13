# Native Touch Changelog
Sirawat Pitaksarit / Exceed7 Experiments (Contact : 5argon@exceed7.com)

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

I am keeping the **Unreleased** section at the bottom of this document. You can look at it to learn about what's sorely missing and would be coming next.

# [4.0.1] - 2019-06-06

## Fixed

- A rounding error in `NativeTouchRecognizer.mm` causing "plus" iOS device which have 3x native coordinate scaling to return a coordinate error by -1 from an intended place. If you was trying to write a touch tracker that connect up points and wondering why they didn't connect properly on plus devices, this is the reason. I am incredibly sorry.

# [4.0.0] - 2019-03-20

## Added

### New underlying interop mechanism : ring buffer

The talk back from native to C# is improved. Relying less on delegate and more "communication via memory area". It is a straight upgrade and you have nothing to lose from user's perspective regardless of modes of operation. (It's a new backend of sorts) The callback based API you use is still the same.

How it was working previously : 

- C# send a delegate to `static` method to native.
- Native wait for touch callback from OS.
- Native call that delegate with the touch data. Its argument is **a single** touch struct, and to be allocated from platform-specific touch structure to allow cross platform. (`MotionEvent` for Android, and `UITouch` for iOS) Plus it is not easy to send the whole object from Java or Objective C that C# can understand so I must make a `struct`.
- So you receive tons of callback when you mash the screen.

This is mostly not a problem on iOS IL2CPP and Android Mono, the callback is as fast as "just a method call". However on Android IL2CPP, the Java -> C# method call is **very slow**, as slow as 16ms (1 frame of 60 FPS). This is not acceptable. Be able to handle touch in a "callback style" in Unity is the core function of Native Touch. But it is no use if the callback is so slow that it arrives later than Unity's touch.

The new way it is working right now : 

- C# allocates a fixed length array of touches as a ring buffer.
- This is pinned and its address is sent to native side to use.
- C# send a delegate to `static` method to native also, but this time its argument is no longer touch struct, but just 2 `int` specifying where in the ring buffer to check out the data and how long from that point. (Wraps around to beginning, as per ring buffer definition.)
- Native wait for touch callback from OS.
- The native side writes those touches to the ring buffer boundary. It knows the fixed length allocated at managed side. Along the way it remembers where it start writing and how much it had written.
- When finished one set of touches, native side use the delegate to tell C# to "check out" the ring buffer at where and how long.
- C# check the ring buffer it owned and found that there are new touch data written from the native side.
- C# iterates through them and present you each touch as the same as before. You still get to see `NativeTouchData` given to you one by one, but the iteration has been moved from native side to managed C#. Each touch is no longer gated by native to managed delegate callback.

All C# side still remains in safe code. It uses `IntPtr` which is usable in safe context to give native side the address.

This approach aims to solve several things :

- Previously IL2CPP on Android (but not iOS, and not on Mono Android) had abysmal delegate callback performance. This patch greatly reduces amount of callbacks needed. (As high as 5x lower when the screen is really busy.) On Android by compiling with Mono backend the delegate performance improved instantly. This might be because native to managed delegate in Android is assisted by JIT mechanism. When on AOT only backend like IL2CPP something may have become more difficult to do. I suspect that Java View call to invoke that remembered C# delegate is not passed through IL2CPP, and so it is not "just a function pointer call" anymore. Something heavy is added. Still, even with a mere 1 callback on Android IL2CPP it is unacceptable that it takes 1 frame or more. But ring buffer allows a new API that could be used instead of callback style. More in its own section.
- Previously each callback is weighted by a full touch struct on the parameter. Now it is just 2 integers. Lighter is always better.
- Previously there is a touch struct allocation at native side. Now it wrote directly to the ring buffer which C# owns using a struct-mapped pointer which is maintained to match the C# side. There is no allocation at all not counting the native touch struct coming from the OS's callback. (`MotionEvent` and `UITouch`)

### New StartOption added : `noCallback`

Be able to handle touch in a "callback style" in Unity is the core function of Native Touch. I am adding one more approach more similar to `Input.touches` you used to do.

To recap, the previous "callback based" API sends C# `static` method to be "reverse p-invoked" by Java's `View` or iOS's `UIGestureRecognizer` when it receive a touch. With ring buffer shared memory upgrade in this version, the reverse p-invoke is just telling C# to look at specific spot in those memory areas. The native had written something new (touch data) to it. C# see new data and understand that those are new touches. Greatly improve interop speed by talking via memory content plus lesser callback, rather than purely via callbacks with a lot of parameters.

But still, platform like Android IL2CPP is very bad at calling even one Java -> C# delegate. And this call is blocking the Java `View`'s touch handler. Though, if I tell C# to tell C to call delegate back to C#, this is fast, in fact 2x faster than Mono. Likely IL2CPP optimized these cross platform complexity into "just a function call".

Native Touch is special because the originator of action is Java's view. Java is JNI-ing to C to invoke the delegate because Java could not do direct memory write (required for the ring buffer optimization) and Java is too coward to do direct method pointer call. This C invoking C# works, but probably IL2CPP knows nothing about it. So when it jumbles into IL2CPP'ed Android, something heavy must be in place to make it work. (Probably something heavy like C#'s reflection?)

Native Touch 4.0.0 allows **opting out completely from receiving callbacks** with `noCallback` on the `StartOption` used when you call `NativeTouch.Start()`. Previously Native Touch will not allow you to start without registering any callback. Now even with callback registered, it will let you start and completely ignores the callbacks.

What's the point? How to receive touches then? This bring us to the next feature...

### New touch handling style added : Ring buffer iteration API `NativeTouch.touches`.

Instead of relying on callbacks, you wait for your touches in `Update()` as usual. BUT instead of checking `Input.touches`, you now have an access to `NativeTouch.touches`.

This `NativeTouch.touches` is essentially that ring buffer but with a wrapper to help you read from the correct place on it. It contains `.TryGetAndMoveNext(out NativeTouchData)`, an easy interface that you could `while` until you have catch up with the latest data on the ring buffer.

One advantage over the callback based is that you have no worry about thread. Remember that the callback way is initiated by native side. The thread in that callback scope is depending on what thread the native side is using. On Android, it is not in the same thread as Unity's main thread and is completely not in sync. This make it a bit difficult to migrate and use those `NativeTouchData` for things waiting in the main thread. Potentially involving some mutex locks to make it safe.

When you use `NativeTouch.touches` API, you are reading from the same ring buffer that native side is writing new touches to, which might be in parallel. But not to worry as I have made sufficient mutex locks that it properly waits or avoiding each other. (This lock is not enabled on iOS, as the write is not in parallel unlike in Android)

When you was using the callback way, you are likely doing some kind of caching of those touches in order to make the main thread's `Update()` to be able to use them. Now you can think that this caching and making it main thread compatible is already done for you. have Except you didn't pay the cost of native callback with `noCallback` start option! On platform like Android IL2CPP where callback is expensive, using `noCallback` with waiting to do ring buffer iteration in the next `Update()` is often faster than the usual callback way.

Also, did we completely defeat the point of Native Touch? **No**, for several reasons :

- **Still could be faster** : The data may still appear *earlier or equal* to `Input.touches`. I had a report that some phone have `Input.touches` data appears as late as 3 frames from the touch. You may see data appearing in `NativeTouch.touches` earlier. It cannot be later, the code is instructed to write to ring buffer area even before handing the touch to Unity.
- **Still more data** : The touches waiting in the ring buffer of `NativeTouch.touches` have touch timestamps. Even if they arrive at the same time as `Input.touches`, you have some missing information that Unity discarded.
- **Still customizable and source available** : The touches is "native". We can modify the plugin to include more from the native side that you fear Unity is discarding or processed out to fit the `Touch` data structure. Touch timestamp is one such thing that we added back. Unity's touch processing is in closed source. We could do nothing about it. The "native" in the name is guaranteeing you can do whatever native Android or iOS can do about those events equally. But Native Touch make them appears in Unity easier without any processing.

What we lose by using ring buffer iteration-based API :

- Previously it is possible to make it in time to use data from the callback for moving object, that is still earlier than rendering submission in the game loop. This make it *visibly* faster that Native Touch could speed up things. With this the chance of being able to do so still exists but lower, since we wait until the next main thread `Update()` to use the touches.
- Previously it is possible to do a thread safe thing in that callback. Everything there is 100% faster than waiting for Unity main thread's `Update()`. For example, if you want to make an app that plays sound instantly you touch the screen without any other logic (don't care where you touch the screen), nothing could beat placing native audio playing code in the Native Touch `static` callback. (No, `AudioSource` is not thread safe. The audio playing must be main thread independent for platform like Android which the callback comes in an another thread.)

## Changed

### `ClearCallbacks()` now cannot be used while in `Start()` state.

- It is now **not possible** to use `NativeTouch.ClearCallbacks()` while you are in Native Touch enabled state. Stopping Native Touch on platform like iOS can cause a remaining queued touches to be dispatched for the last time. If you clear the callbacks before stopping it could lead to null reference error. (Because Native Touch is intentionally not doing null check, for speed.)

## Fixed

- Better error throwing in the code.
- Demo scene is fixed to show how to do ring buffer iteration based API together with `noCallback` start option.
- [iOS] No longer replays tons of touches on calling `NativeTouch.Stop()`. The fix for this is achieved by never removing the touch recognizer but just temporarily turn it off. The strange touch replay through `UIView` was caused when removing the recognizer.

# [3.0.0] - 2019-02-28

## Added

### `NativeTouch.RealScreenResolution`

Native Touch's coordinate is unscaled, if your game uses Resolution Scaling (Or dynamic resolution scaling) then `Screen.width/height/resolution/etc` will be scaled down too. If you use that to calculate something with Native Touch's returned coordinate then it is wrong. (Unity's `Input.___` API will be scaled down too)

Since native side has no idea how small Unity is scaling things, the approach is to use this method to return resolution of `Screen.__` API as if it wasn't scaled. It got the same bounds as what coordinate that is returning from Native Touch. Then you could calculate the touch coordinate into the scaled coordinate.

## Changed

### Native Touch now returns "Native Scale" instead of just (virtual) scale.

This is a breaking change and a fix at the same time. Previously there is a mistake that returns just `scale` in `NativeTouchRecognizer.mm` line 92. Now it is instead `nativeScale`.

The reason is Unity's coordinate is going by the **native scale**. [See this table](https://developer.apple.com/library/archive/documentation/DeviceInformation/Reference/iOSDeviceCompatibility/Displays/Displays.html#//apple_ref/doc/uid/TP40013599-CH108-SW2), the -Plus device have a smaller multiplier from 3 to 2.608.

That means the previous version of Native Touch returns coordinate higher than Unity's bound when it goes to 3x where actually it is just 2.608x, for example. This update brings them down to the same bound.

## Fixed

- Demo is updated to work correctly in the case of using Resolution Scaling.
- NativeTouchTracker demo is updated to work correctly in the case of using Resolution Scaling.
- Fixed `extern` methods intended for iOS side mistakenly getting into Android build and cause linker error.
- Better XML code documentation.

# [2.1.0] - 2018-10-10

## Added

### Bonus content added : `NativeTouchTracker`

It is zipped in the Extra folder. Feed it a lot of `NativeTouchData` in sequence and you would be able to poll data from it just like Unity's `Input.touches`. There are tons of preprocessor directives inside that make the whole thing works magically on both Android and iOS.

This struct is an example of how my take of interpreting native touches would go. There are several discarded data still, based on the need of my own game, you can take a look at it as a guideline for creating your own wrapper.

Requirements (And why it stays in a zip file)

- A reference to `Unity.Collections` and `Unity.Mathematics` package, making it goes well with C# Jobs, ECS, and Burst compilation.
- An understanding of how native collection works, this is a struct full of them. You also have to `Dispose` it.
- Incremental Compiler package for C#7.0 syntax.

### New demo scene

It now has a colored square following your finger's movement.

## Fixed

- The previous version's Android touch sends wrong touch phase in the case of 3 or more touches. It is corrected now.
- Previously iOS sends duplicated touch events and it is possible to get Stationary phase because I wrongly used `[event allTouches]` instead of `touches` in the native side.
- Previously Android sends Cancelled phase only for the "main" touch in the pack and other touches fixed as "ambiguous Moved" (Might be Moved or implied Stationary). Now only for Cancelled phase there is a special rule so that all touches in the pack are considered Cancelled instead of Moved. Otherwise there's no way to tell which touch came together with the Cancelled touch. 

# [2.0.0] - 2018-09-08

## Added

### Android support

If you put `#if` to get Android out of the way before, now it is not needed.

## Changed

### Static callback signature changed to just one `struct`

It is much easier than before that you have to declare 5 `int`s or something and that was very error prone.
This struct has a nice C# property so that each platform access only its valid fields.

Also because of this Unity's `Touch` support has been dropped. The mode is just full mode or not-full mode. (minimal mode)

### Minimal or full mode specified via `StartOption` instead

There is a new overload for `Start()` which accepts some options, `Start(startOption)`.

### The touch continues to Unity

Previous version completely disable Unity touch. This version to get the same behaviour you have to use the new `StartOption` when calling `Start(startOption)`.

## Fixed

### iOS now correctly returns stationary phase touch on multitouch scenario

The previous version has a bug. When you have multiple fingers down and move one finger you only get one move phase in the callback.
Now in addition, you get other fingers as stationary phase as well. Outside of multitouch scenario you can never receive a stationary phase.

In Android there is no stationary phase at all. Please read [Callback Details](http://exceed7.com/native-touch/callback-details.html) in the website.

# [1.0.0] - 2018-04-13

The first release. Supports only iOS.

# Unreleased

### iPad Pro and Apple Pencil 2 support

I am getting an iPad Pro this year. After that I could confirm if we can fully utilize the ProMotion 120Hz input fully or not, and if there are any missing information from the new Apple Pencil that Unity discards but we could recover or not.

However, I am suspecting that it is already supported currently, but without able to confirm by myself I can't guarantee the support status of these.

### ECS Support  

The touch could optionally produce an `Entity` to any world you want at any `BarrierSystem` you like in the case of Android, or immediately with `EntityManager` of your choice the case of iOS. The touch struct will be implementing `IComponentData` interface and attached to that `Entity`.

Then any system wish to use the touch can inject them and attach `ISystemStateComponentData` which signify processed touch to it. A utility method to clean up all processed touch will be provided as well to save memory space.

I will wait for `Unity.Entities` package to be out of preview and be more widespread first or else the `asmdef` would have to refer to preview assembly.

At first to make the onboarding optional, the support might come in the form of preprocessor directive like having `NATIVE_TOUCH_ECS` and it changes the using in the code to use Unity ECS library and adds `: IComponentData` to the touch struct.

### [Android] Historical values 

Many values from Android has variable amount of "historical values". Currently Native Touch discards them. I have to find a nice way to send variable values from native to managed and make it usable with the current workflow.