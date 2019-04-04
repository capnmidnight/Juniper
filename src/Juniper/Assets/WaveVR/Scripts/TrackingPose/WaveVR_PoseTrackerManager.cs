using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WaveVR_PoseTrackerManager))]
public class WaveVR_PoseTrackerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaveVR_PoseTrackerManager myScript = target as WaveVR_PoseTrackerManager;

        myScript.Type = (WVR_DeviceType)EditorGUILayout.EnumPopup ("Type", myScript.Type);
        myScript.TrackPosition = EditorGUILayout.Toggle ("Track Position", myScript.TrackPosition);
        if (true == myScript.TrackPosition)
        {
            if (myScript.Type == WVR_DeviceType.WVR_DeviceType_HMD)
            {
                myScript.EnableNeckModel = (bool)EditorGUILayout.Toggle ("    Enable Neck Model", myScript.EnableNeckModel);
            } else
            {
                myScript.SimulationOption = (WVR_SimulationOption)EditorGUILayout.EnumPopup ("    Simulate Position", myScript.SimulationOption);
                if (myScript.SimulationOption == WVR_SimulationOption.ForceSimulation || myScript.SimulationOption == WVR_SimulationOption.WhenNoPosition)
                {
                    myScript.FollowHead = (bool)EditorGUILayout.Toggle ("        Follow Head", myScript.FollowHead);
                }
            }
        }

        myScript.TrackRotation = EditorGUILayout.Toggle ("Track Rotation", myScript.TrackRotation);
        myScript.TrackTiming = (WVR_TrackTiming)EditorGUILayout.EnumPopup ("Track Timing", myScript.TrackTiming);

        if (GUI.changed)
            EditorUtility.SetDirty ((WaveVR_PoseTrackerManager)target);
    }
}
#endif

public enum WVR_TrackTiming {
    WhenUpdate,  // Pose will delay one frame.
    WhenNewPoses
};

public enum WVR_SimulationOption
{
    WhenNoPosition,
    ForceSimulation,
    NoSimulation
};

public class WaveVR_PoseTrackerManager : MonoBehaviour
{
    private const string LOG_TAG = "WaveVR_PoseTrackerManager";
    private void PrintDebugLog(string msg)
    {
        #if UNITY_EDITOR
        Debug.Log(LOG_TAG + ": Type: " + Type + ", " + msg);
        #endif
        Log.d (LOG_TAG, "Type: " + Type + ", " + msg);
    }

    public WVR_DeviceType Type = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public bool TrackPosition = true;
    public bool EnableNeckModel = true;
    public WVR_SimulationOption SimulationOption = WVR_SimulationOption.WhenNoPosition;
    public bool FollowHead = false;
    public bool TrackRotation = true;
    public WVR_TrackTiming TrackTiming = WVR_TrackTiming.WhenNewPoses;

    private GameObject[] IncludedObjects;
    private bool[] IncludedStates;

    /// <summary>
    /// We use 4 variables to determine whether to hide object with pose tracker or not.
    /// There are 2 kinds of pose tracked object:
    /// 1. Normal object
    /// 2. Controller
    /// 1 is shown when connected && has system focus && pose updated.
    /// 2 is shown when connected && has system focus && pose updated && not Gaze mode.
    /// </summary>
    private bool showTrackedObject = true;
    private bool connected = false;
    private bool mFocusCapturedBySystem = false;
    public bool poseUpdated = false;
    private bool hasNewPose = false;
    private bool gazeOnly = false;

    private WaveVR_DevicePoseTracker devicePoseTracker = null;
    private WaveVR_ControllerPoseTracker ctrlerPoseTracker = null;

    private bool ptmEnabled = false;
    #region Monobehaviour overrides
    void OnEnable()
    {
        if (!ptmEnabled)
        {
            int _children_count = transform.childCount;
            IncludedObjects = new GameObject[_children_count];
            IncludedStates = new bool[_children_count];
            for (int i = 0; i < _children_count; i++)
            {
                IncludedObjects [i] = transform.GetChild (i).gameObject;
                IncludedStates [i] = transform.GetChild (i).gameObject.activeSelf;
                PrintDebugLog ("OnEnable() " + gameObject.name + " has child: " + IncludedObjects [i].name + ", active? " + IncludedStates [i]);
            }

            #if UNITY_EDITOR
            if (Application.isEditor)
                this.connected = true; // WaveVR_Controller.Input (Type).connected;
            else
            #endif
            {
                // If pose is invalid, considering as disconnected and not show controller.
                WaveVR.Device _device = WaveVR.Instance.getDeviceByType (Type);
                this.connected = _device.connected;
            }

            // Always hide Pose Tracker objects when enabled.
            // Check whether to show object when:
            // 1. device connected
            // 2. pose updated
            // 3. system focus changed
            // 4. interaction mode changed
            ForceActivateTargetObjects (false);
            this.poseUpdated = false;
            this.hasNewPose = false;

            WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
            if (TrackTiming == WVR_TrackTiming.WhenNewPoses)
                WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);

            WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.INTERACTION_MODE_CHANGED, onInteractionModeChange);
            WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.SYSTEMFOCUS_CHANGED, onSystemFocusChanged);

            ptmEnabled = true;
        }
    }

    void Awake()
    {
        if (TrackPosition == false)
        {
            SimulationOption = WVR_SimulationOption.NoSimulation;
            FollowHead = false;
        }

        gameObject.SetActive (false);
        PrintDebugLog ("Awake() TrackPosition: " + TrackPosition + ", SimulationOption=" + SimulationOption +
            ", FollowHead: " + FollowHead + ", TrackRotation: " + TrackRotation + ", TrackTiming=" + TrackTiming);

        WaveVR_PointerCameraTracker pcTracker = gameObject.GetComponent<WaveVR_PointerCameraTracker>();

        if (pcTracker == null)
        {
            if (this.Type == WVR_DeviceType.WVR_DeviceType_HMD)
            {
                PrintDebugLog ("Awake() load WaveVR_DevicePoseTracker.");
                devicePoseTracker = (WaveVR_DevicePoseTracker)gameObject.AddComponent<WaveVR_DevicePoseTracker> ();
                if (null != devicePoseTracker)
                {
                    devicePoseTracker.type = Type;
                    devicePoseTracker.trackPosition = TrackPosition;
                    devicePoseTracker.EnableNeckModel = this.EnableNeckModel;
                    devicePoseTracker.trackRotation = TrackRotation;
                    devicePoseTracker.timing = TrackTiming;
                }
            } else
            {
                PrintDebugLog ("Awake() load WaveVR_ControllerPoseTracker.");
                ctrlerPoseTracker = (WaveVR_ControllerPoseTracker)gameObject.AddComponent<WaveVR_ControllerPoseTracker> ();
                if (null != ctrlerPoseTracker)
                {
                    ctrlerPoseTracker.Type = Type;
                    ctrlerPoseTracker.TrackPosition = TrackPosition;
                    ctrlerPoseTracker.SimulationOption = SimulationOption;
                    ctrlerPoseTracker.FollowHead = FollowHead;
                    ctrlerPoseTracker.TrackRotation = TrackRotation;
                    ctrlerPoseTracker.TrackTiming = TrackTiming;
                }
            }
        }
        gameObject.SetActive (true);
    }

    void Update()
    {
        if (!Application.isEditor)
        {
            if (gameObject != null)
                Log.gpl.d (LOG_TAG, "Update() showTrackedObject ? " + this.showTrackedObject + ", GameObject " + gameObject.name + " is " + (gameObject.activeSelf ? "shown." : "hidden."));
            foreach (var _obj in IncludedObjects)
            {
                Log.gpl.d (LOG_TAG, "Update() GameObject " + _obj.name + " is " + (_obj.activeSelf ? "shown." : "hidden."));
            }
        }

        if (!this.connected)
            return;

        bool _focus = false;
        if (WaveVR.Instance != null)
        {
            _focus = WaveVR.Instance.FocusCapturedBySystem;
        }

        if (this.mFocusCapturedBySystem != _focus)
        {
            // InputFocus changed!
            this.mFocusCapturedBySystem = _focus;
            PrintDebugLog ("Update() focus is " + (this.mFocusCapturedBySystem ? "captured by system." : "not captured by system."));
        }

        if (WaveVR_InputModuleManager.Instance != null)
        {
            WVR_InteractionMode _imode = WaveVR_InputModuleManager.Instance.GetInteractionMode ();
            bool _gazeOnly = (_imode == WVR_InteractionMode.WVR_InteractionMode_Gaze) ? true : false;
            if (this.gazeOnly != _gazeOnly)
            {
                this.gazeOnly = _gazeOnly;
                PrintDebugLog ("Update() interaction mode is " + (this.gazeOnly ? "Gaze only." : "NOT Gaze only."));
            }
        }

        ActivateTargetObjects ();

        // Update after the frame receiving 1st new pose.
        this.poseUpdated = this.hasNewPose;
    }

    void OnDisable()
    {
        // Consider a situation: no pose is updated and WaveVR_PoseTrackerManager is enabled <-> disabled multiple times.
        // At this situation, IncludedStates will be set to false forever since thay are deactivated at 1st time OnEnable()
        // and the deactivated state will be updated to IncludedStates in 2nd time OnEnable().
        // To prevent this situation, activate IncludedObjects in OnDisable to restore the state Children GameObjects.
        PrintDebugLog("OnDisable() restore children objects.");
        ForceActivateTargetObjects (true);

        WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
        if (TrackTiming == WVR_TrackTiming.WhenNewPoses)
            WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.NEW_POSES, OnNewPoses);
        WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.INTERACTION_MODE_CHANGED, onInteractionModeChange);
        WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.SYSTEMFOCUS_CHANGED, onSystemFocusChanged);

        ptmEnabled = false;
    }
    #endregion

    private bool hideEventController()
    {
        bool _hide = false;
        GameObject _model = WaveVR_EventSystemControllerProvider.Instance.GetControllerModel (this.Type);
        if (GameObject.ReferenceEquals (gameObject, _model))
        {
            _hide = this.gazeOnly;
        }

        return _hide;
    }

    public void ActivateTargetObjects()
    {
        bool _hide = hideEventController ();
        bool _has_input_module_enabled = true;
        if (WaveVR_InputModuleManager.Instance != null)
            _has_input_module_enabled = WaveVR_InputModuleManager.Instance.EnableInputModule;

        bool _active = (
            (this.connected == true)                                    // controller is connected (pose is valid).
            && (!this.mFocusCapturedBySystem)                           // scene has system focus.
            && this.poseUpdated                                         // already has pose.
            && (!_hide)                                                 // not Gaze or not event controller in Gaze
            && _has_input_module_enabled                                // has InputModuleManager and enabled
        );

        if (this.showTrackedObject == _active)
            return;

        PrintDebugLog ("ActivateTargetObjects() connected ? " + this.connected
            + ", mFocusCapturedBySystem? " + this.mFocusCapturedBySystem
            + ", controller in gaze? " + _hide
            + ", input module enabled? " + _has_input_module_enabled);

        ForceActivateTargetObjects (_active);
    }

    private void ForceActivateTargetObjects(bool active)
    {
        if (IncludedObjects == null)
            return;

        for (int i = 0; i < IncludedObjects.Length; i++)
        {
            if (IncludedObjects [i] == null)
                continue;

            if (IncludedStates [i])
            {
                PrintDebugLog ("ForceActivateTargetObjects() " + (active ? "activate" : "deactivate") + " " + IncludedObjects [i].name);
                IncludedObjects [i].SetActive (active);
            }
        }

        this.showTrackedObject = active;
    }

    private void onDeviceConnected(params object[] args)
    {
        if (!ptmEnabled)
        {
            PrintDebugLog ("onDeviceConnected() do NOTHING when disabled.");
            return;
        }

        bool _connected = false;
        WVR_DeviceType _type = this.Type;

        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            _connected = WaveVR_Controller.Input (this.Type).connected;
            _type = WaveVR_Controller.Input(this.Type).DeviceType;
        }
        else
        #endif
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType (this.Type);
            _connected = _device.connected;
            _type = _device.type;
        }

        PrintDebugLog ("onDeviceConnected() " + _type + " is " + (_connected ? "connected" : "disconnected")
            + ", left-handed? " + WaveVR_Controller.IsLeftHanded);

        if (this.connected != _connected)
        {
            this.connected = _connected;
            ActivateTargetObjects ();
        }
    }

    private void OnNewPoses(params object[] args)
    {
        if (!ptmEnabled)
        {
            PrintDebugLog ("OnNewPoses() do NOTHING when disabled.");
            return;
        }

        if (!this.hasNewPose)
        {
            // After 1st frame, pose has been updated.
            PrintDebugLog("OnNewPoses() pose updated.");
            this.hasNewPose = true;
        }
    }

    private void onSystemFocusChanged(params object[] args)
    {
        if (!ptmEnabled)
        {
            PrintDebugLog ("onSystemFocusChanged() do NOTHING when disabled.");
            return;
        }

        this.mFocusCapturedBySystem = (bool)args [0];
        PrintDebugLog ("onSystemFocusChanged() focus is " + (this.mFocusCapturedBySystem ? "captured by system." : "not captured by system."));

        ActivateTargetObjects ();
    }

    private void onInteractionModeChange(params object[] args)
    {
        if (!ptmEnabled)
        {
            PrintDebugLog ("onInteractionModeChange() do NOTHING when disabled.");
            return;
        }

        WVR_InteractionMode _imode = (WVR_InteractionMode)args [0];

        this.gazeOnly = (_imode == WVR_InteractionMode.WVR_InteractionMode_Gaze) ? true : false;
        bool _hide = hideEventController ();
        PrintDebugLog ("onInteractionModeChange() interaction mode is " + _imode + ", controller in gaze? " + _hide);

        ActivateTargetObjects ();
    }
}
