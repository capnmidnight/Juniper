// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using wvr;
using WaveVR_Log;

public class WaveVR_Init : MonoBehaviour
{
    private const string LOG_TAG = "WaveVR_Init";

    /// <summary>
    /// The singleton instance of the <see cref="WaveVR_Init"/> class, there only be one instance in a scene.
    /// </summary>
    public static WaveVR_Init Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WaveVR_Init>();
                if (_instance == null)
                {
                    Log.d(LOG_TAG, "WaveVR_Init create an instance");
                    _instance = new GameObject("[WaveVR]").AddComponent<WaveVR_Init>();
                }
            }
            return _instance;
        }
    }
    private static WaveVR_Init _instance;

    void signalSurfaceState(string msg) {
        WaveVR_Render.signalSurfaceState(msg);
    }

    void Start()
    {
        if (WaveVR.Instance != null)
        {
            WaveVR.Instance.onLoadLevel ();

            // if system boots with default controller role left, set left-handed mode to true
            bool _lefthanded = false;
#if UNITY_EDITOR
            if (Application.isEditor) 
            {
            }
            else
#endif
            {
                _lefthanded = Interop.WVR_GetDefaultControllerRole() == WVR_DeviceType.WVR_DeviceType_Controller_Left ? true : false;
            }
            #if UNITY_EDITOR
            Debug.Log (LOG_TAG + " Start() Set left-handed mode to " + _lefthanded);
            #endif
            Log.i (LOG_TAG, "Start() Set left-handed mode to " + _lefthanded);
            WaveVR_Controller.SetLeftHandedMode (_lefthanded);
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        if (_instance != this)
        {
            Log.w(LOG_TAG, "Has another [WaveVR] object in a scene. Destory this.");
            Destroy(this);
            return;
        }

#if UNITY_EDITOR
        if (Application.isEditor) return;
#endif
        if (WaveVR.Instance != null)
        {
            Log.d(LOG_TAG, "Initialized");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isEditor && !WaveVR.Instance.isSimulatorOn) return;
#endif

        bool ret = false;
        do
        {
            WVR_Event_t vrevent = new WVR_Event_t();
#if UNITY_EDITOR
            if (Application.isEditor) 
            {
                ret = WaveVR_Utils.WVR_PollEventQueue_S(ref vrevent);
            }
            else
#endif
            {
                ret = Interop.WVR_PollEventQueue(ref vrevent);
            }
            if (ret)
                processVREvent(vrevent);
        } while (ret);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Log.i (LOG_TAG, "OnApplicationPause() pauseStatus: " + pauseStatus);
        if (!pauseStatus)
        {
            // Application resume.
            SetLeftHandedMode ();
            ResetButtonStates ();
        }
    }

    private void ResetButtonStates()
    {
        Log.i (LOG_TAG, "ResetButtonStates() Reset button states.");
        foreach (WVR_DeviceType _type in WaveVR.DeviceTypes)
        {
            WaveVR_Controller.Input (_type).ResetButtonStates ();
        }
    }

    private void SetLeftHandedMode()
    {
        if (WaveVR.Instance != null)
        {
                bool _lefthanded = false;
#if UNITY_EDITOR
                if (Application.isEditor) 
                {
                }
                else
#endif
                {
                _lefthanded = Interop.WVR_GetDefaultControllerRole () == WVR_DeviceType.WVR_DeviceType_Controller_Left ? true: false;
                }

            Log.i (LOG_TAG, "SetLeftHandedMode() Set left-handed mode to " + _lefthanded);
            WaveVR_Controller.SetLeftHandedMode (_lefthanded);
        }
    }

    private void processVREvent(WVR_Event_t vrEvent)
    {
        // Process events used by plugin
        switch ((WVR_EventType)vrEvent.common.type)
        {
            case WVR_EventType.WVR_EventType_IpdChanged:
                {
                    WaveVR_Utils.Event.Send("IpdChanged");
                    if (WaveVR_Render.Instance != null)
                        WaveVR_Render.Expand(WaveVR_Render.Instance);
                }
                break;
            case WVR_EventType.WVR_EventType_DeviceStatusUpdate:
               {
                    WaveVR_Utils.Event.Send("TrackedDeviceUpdated", vrEvent.device.common.type);
               }
               break;
            case WVR_EventType.WVR_EventType_BatteryStatusUpdate:
                {
                    WaveVR_Utils.Event.Send("BatteryStatusUpdate");
                }
                break;
        case WVR_EventType.WVR_EventType_LeftToRightSwipe:
        case WVR_EventType.WVR_EventType_RightToLeftSwipe:
        case WVR_EventType.WVR_EventType_DownToUpSwipe:
        case WVR_EventType.WVR_EventType_UpToDownSwipe:
            WaveVR_Utils.Event.Send("SWIPE_EVENT", vrEvent.common.type);
            break;
        case WVR_EventType.WVR_EventType_DeviceRoleChanged:
            if (WaveVR.Instance != null)
            {
                SetLeftHandedMode ();
                ResetButtonStates ();

                Log.i (LOG_TAG, "processVREvent() Resend connection notification after switching hand.");
                WaveVR.Instance.OnControllerRoleChange ();
            }
            break;
        default:
            break;
        }

        // Send event to developer for all kind of event if developer don't want to add callbacks for every event.
        WaveVR_Utils.Event.Send(WaveVR_Utils.Event.ALL_VREVENT, vrEvent);

        // Send event to developer by name.
        WaveVR_Utils.Event.Send(vrEvent.common.type.ToString(), vrEvent);
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

