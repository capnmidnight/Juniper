using System.Text;
using UnityEngine;
using UnityEngine.UI;
using E7.Native;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class NativeTouchDemo : MonoBehaviour
{
#if UNITY_IOS
    private static NativeTouchDemo singleton;
#endif

    public ScrollRect scrollRect;
    public Text consoleText;
    public Text runningText;
    public RectTransform stopNativeTouchRect;

    public Transform nativeFollower;
    public Transform unityFollower;
    public AudioSource mainThreadOperation;

    private bool noCallbackEnabled;

    //These are `static` because I want to use them with the touch callback.
    //They are linking points that Unity's main thread would use them later too.
    //Touch callback must be `static` because that's a requirement on AOT platform,
    //for something in C# to be invoked by someone it must be `static`.
    private static int nativeTouchCount;
    private static Vector2 nativeFollowerPosition;
    private static StringBuilder stringBuilder;
    private static float rememberRealTimeSinceStartup;
    private static
#if UNITY_2017_2_OR_NEWER
    Vector2Int
#else
    Vector2
#endif
    cachedRealScreenResolution;

    public void GoToTrackerScene()
    {
        SceneManager.LoadScene("NativeTouchTrackerDemo");
    }

    private void Awake()
    {
#if UNITY_IOS
        singleton = this;
#endif

#if !UNITY_EDITOR
        //If we use resolution scaling, the `Screen.` API will be scaled down but the point from native side is as if it is not scaled.
        //RealScreenResolution is the `Screen.` resolution as if it hadn't scaled.
        cachedRealScreenResolution = NativeTouch.RealScreenResolution();
#endif

        Debug.Log("UNITY RESOLUTION " + Screen.currentResolution + " " + Screen.width + " " + Screen.height);
        Debug.Log("REAL SCREEN RESOLUTION " + cachedRealScreenResolution);
        Application.targetFrameRate = 60;

        stringBuilder = new StringBuilder();
    }

    /// <summary>
    /// Called by `Event Trigger`'s Pointer Down
    /// </summary>
    public void NormalTouch(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEventData;

        //Unity's input system does not include the timestamp, so we ask for the touch time from native side at this moment.
        //(But it is still not the real touch time, just that it can be compared with native touch's time.
        double nativeTouchTime = NativeTouch.GetNativeTouchTime();

        rememberRealTimeSinceStartup = Time.realtimeSinceStartup;

        //StringBuilder is a reference type that is also used in the touch callback. To prevent potentially 2 threads accessing at the same time, we are putting a lock on it.
        lock (stringBuilder)
        {
            if (lineCount > lineMax)
            {
                stringBuilder.Length = 0;
                lineCount = 0;
            }

            stringBuilder.AppendLine(string.Format("Unity : CallbackTime {0} CallbackTime in native timestamp {1} Frame# {2} Pos {3}", rememberRealTimeSinceStartup, nativeTouchTime, Time.frameCount, pointerEventData.position));

            lineCount++;
            consoleText.text = stringBuilder.ToString();
            scrollRect.verticalNormalizedPosition = 0;
        }
    }


    /// <summary>
    /// This is for using with the callback-based API.
    /// The other signature we can use, you replace `NativeTouchData` with `NativeTouchDataFull`, then start with full mode.
    /// 
    /// You have to **ALWAYS keep in mind** that everything in this code scope may **not** be in Unity's main thread. (On Android it is like that)
    /// Since this is really called by whatever thread that is handling touches at native side! Be careful of things you could not do outside of Unity's main thread.
    /// For example `Time.___` are mostly main thread dependent. Calling them in this method's scope in Android will hard crash the game. (But usable in iOS)
    /// 
    /// If you are accessing reference type, even if they are `static`, putting `lock(___)` statement on them is a good idea to ensure nothing in the main thread
    /// is doing something to it as the same time as this touch callback which might be in an other thread.
    /// 
    /// The objective of this static receiver is to calculate and remember position, so that main thread comes around, the `Update()` could use this position to move the graphic.
    /// </summary>
    public static void NativeTouchStaticReceiver(NativeTouchData ntd)
    {
        //First, try to flip Y axis.

        //Get the screen resolution again just in case you rotate the demo. (Should cache only once in the real game, in the case of not using Dynamic Resolution Scaling)
        cachedRealScreenResolution = NativeTouch.RealScreenResolution();

        //If we use `Screen.height` here it would be incorrect in the case of using Resolution Scaling.
        //Native Touch's data is unscaled, unlike Unity's `Touch` data.
        var convertedY = cachedRealScreenResolution.y - ntd.Y;
#if UNITY_IOS
        var convertedPreviousY = cachedRealScreenResolution.y - ntd.PreviousY;
#endif

        //StringBuilder is a reference type that is also used in Unity's main thread `NormalTouch` method.
        //To prevent potentially 2 threads accessing at the same time, we are putting a lock on it.
        lock (stringBuilder)
        {
            //Use the touch from native side as you like
            if (lineCount > lineMax)
            {
                stringBuilder.Length = 0;
                lineCount = 0;
            }

#if UNITY_IOS
            //iOS does not provide finger ID
            stringBuilder.AppendLine(string.Format("<color=red>NATIVE : Phase {0} Pos {1} {2} Movement {3} {4} Timestamp {5} CallbackTime {6} Frame# {7}</color>", ntd.Phase.ToString(), ntd.X, convertedY, ntd.X - ntd.PreviousX, convertedY - convertedPreviousY, ntd.Timestamp, rememberRealTimeSinceStartup, Time.frameCount));

            //iOS is on the main thread. We can just set the text here without fear of crashing the game.
            //But because this is static context, we need the NativeTouchDemo that is remembered to static.
            singleton.consoleText.text = stringBuilder.ToString();
            singleton.scrollRect.verticalNormalizedPosition = 0;

#elif UNITY_ANDROID
            //Android does not provide previous position and cannot get frame count since we are on the other thread.
            stringBuilder.AppendLine(string.Format("<color=red>NATIVE : Phase {0} Pos {1} {2} Timestamp {3} PointerID {4}</color>", ntd.Phase.ToString(), ntd.X, convertedY, ntd.Timestamp, ntd.PointerId));
            //We set text later on the main thread with Android
            //If we put the same thing as iOS here, you have about 10% chance of crashing uGUI because attempting to change text while in graphic rebuild loop.
#endif
        }

        //Be sure to scale to the current resolution to support resolution scaling.
        nativeFollowerPosition = new Vector2(Screen.width * (ntd.X / cachedRealScreenResolution.x), Screen.height * (convertedY / cachedRealScreenResolution.y));

        nativeTouchCount++;
        lineCount++;

#if UNITY_IOS
        //This is to show that even if you set the position here, on the callback that was called before the end of previous frame,
        //the position did not get submitted for drawing. It is too late.
        //And so in the demo scene you can never get the red square to be ahead of yellow no matter how fast you try to drag on the screen.
        Vector3 worldFollower = nativeFollowerPosition;
        singleton.nativeFollower.position = new Vector3(worldFollower.x, worldFollower.y, 0);
#endif

        //If you choose to disable Unity touch as start option,
        //Don't forget to prepare a way to get out of NativeTouch without relying on Unity's event system.
        //In this case we did not disable Unity touch so we can still press that Stop button.
    }

    public void StartNativeTouchButton()
    {
        NativeTouch.ClearCallbacks();
        NativeTouch.RegisterCallback(NativeTouchStaticReceiver);

        //We want only few essential data so that's "minimal" = not full
        NativeTouch.Start(new NativeTouch.StartOption { fullMode = false, disableUnityTouch = false });

        noCallbackEnabled = false;

#if !UNITY_EDITOR
		runningText.text = "Native touch is running (callback).";
#endif
    }

    public void StartNativeTouchButtonRingBuffer()
    {
        NativeTouch.ClearCallbacks();
        
        //This does nothing, for we disable the callback below...
        NativeTouch.RegisterCallback(NativeTouchStaticReceiver);

        //To iterate the ring buffer in main thread instead of handling the callback, we could potentially disable the callback to boost performance.
        //This is important in platform like Android IL2CPP where the callback is slow and you rather ONLY iterate the ring buffer.
        NativeTouch.Start(new NativeTouch.StartOption { fullMode = false, disableUnityTouch = false, noCallback = true });

        noCallbackEnabled = true;

#if !UNITY_EDITOR
		runningText.text = "Native touch is running (no callback).";
#endif
    }

    private const int lineMax = 40; //The demo lags too much with more lines
    private static int lineCount = 0;

    /// <summary>
    /// Thread could not use set text so for Android we move that logic to here.
    /// Also the follower object is updated here. Remember, Unity is a game engine and update in fixed interval.
    /// A touch hardware updates at more speed than this. If you use an on-screen touch debugger the follower object theoretically can
    /// never match the debug touch on-screen.
    /// </summary>
    public void Update()
    {
        if(noCallbackEnabled && NativeTouch.Started)
        {
            //In ring buffer iteration based approach, we turned on noCallback on the start option.
            //Usually we would try to handle the touches in the callback.
            //Instead, this is the moment we will use the collected touches in the ring buffer.

            NativeTouchData ntd;
            int movedThroughCount = 0;

            while(NativeTouch.touches.TryGetAndMoveNext(out ntd))
            {
                //If this while loop is run on every frame, it would at best get up to 5 move nexts.
                //The default ring buffer size in the start option is more than enough.

                //But if you have some use case that do not move next in some frames and want all previous touches in later frame,
                //you may want to increase the ring buffer size in StartOption.

                //We just send them to the same callback method that was disabled by noCallback mode. Then we get exactly the same behaviour.
                NativeTouchStaticReceiver(ntd);
                movedThroughCount++;
            }
            //Debug.Log("Moved through " + movedThroughCount + " touches from the ring buffer in the frame " + Time.frameCount);
        }

#if UNITY_ANDROID
        lock (stringBuilder)
        {
            consoleText.text = stringBuilder.ToString();
        }
        scrollRect.verticalNormalizedPosition = 0;

        //Debug.Log("Demo Update() Frame : " + Time.frameCount + " Elapsed Nanos : " + NativeTouchInterface.androidClassBridge.CallStatic<long>(NativeTouchInterface.AndroidElapsedRealtimeNanos) + " Native Touch count received before this : " + nativeTouchCount);
#endif

        // If you read the Native Touch docs, there are chances to receive more than 1 touch before the frame on BOTH platforms even with only 1 finger dragging around.
        nativeTouchCount = 0;

        // Move the follower. Smaller yellow ones ask the usual Unity API.
        // The bigger red ones use whatever the newest data from the callback that occured before this Update()
        Vector3 worldFollower = nativeFollowerPosition;
        nativeFollower.position = new Vector3(worldFollower.x, worldFollower.y, 0);

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 pos = t.position;
            //Debug.Log("Using Unity touch (.Length " + Input.touches.Length + " ) : " + string.Join(" | ", Input.touches.Select(x => TouchToString(x)).ToArray()));
            unityFollower.position = pos;
        }
    }


    private string TouchToString(Touch t)
    {
        return t.position.x + " " + t.position.y + " (delta : " + t.deltaPosition.x + " " + t.deltaPosition.y + ") " + t.phase + " ID " + t.fingerId;
    }

    public void StopNativeTouch()
    {
        NativeTouch.Stop();
        runningText.text = "Stopped Native Touch";
    }

}
