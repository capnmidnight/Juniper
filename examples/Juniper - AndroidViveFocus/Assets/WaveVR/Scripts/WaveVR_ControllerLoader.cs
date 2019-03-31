using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WaveVR_ControllerLoader))]
public class WaveVR_ControllerLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaveVR_ControllerLoader myScript = target as WaveVR_ControllerLoader;

        myScript.WhichHand = (WaveVR_ControllerLoader.ControllerHand)EditorGUILayout.EnumPopup ("Type", myScript.WhichHand);
        myScript.ControllerComponents = (WaveVR_ControllerLoader.CComponent)EditorGUILayout.EnumPopup ("Controller Components", myScript.ControllerComponents);

        myScript.TrackPosition = EditorGUILayout.Toggle ("Track Position", myScript.TrackPosition);
        if (true == myScript.TrackPosition)
        {
            myScript.SimulationOption = (WVR_SimulationOption)EditorGUILayout.EnumPopup ("    Simulate Position", myScript.SimulationOption);
            if (myScript.SimulationOption == WVR_SimulationOption.ForceSimulation || myScript.SimulationOption == WVR_SimulationOption.WhenNoPosition)
            {
                myScript.FollowHead = (bool)EditorGUILayout.Toggle ("        Follow Head", myScript.FollowHead);
            }
        }

        myScript.TrackRotation = EditorGUILayout.Toggle ("Track Rotation", myScript.TrackRotation);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Controller model");
        myScript.adaptiveLoading = EditorGUILayout.Toggle("  Adaptive loading", myScript.adaptiveLoading);
        if (true == myScript.adaptiveLoading)
        {
            EditorGUILayout.LabelField("  Button effect");
            myScript.enableButtonEffect = EditorGUILayout.Toggle("    Enable button effect", myScript.enableButtonEffect);
            if (true == myScript.enableButtonEffect)
            {
                myScript.useEffectSystemConfig = EditorGUILayout.Toggle("      Apply button effect system config", myScript.useEffectSystemConfig);
                if (true != myScript.useEffectSystemConfig)
                {
                    myScript.buttonEffectColor = EditorGUILayout.ColorField("          Button effect color", myScript.buttonEffectColor);
                }
            }

            EditorGUILayout.LabelField("  Beam");
            myScript.useBeamSystemConfig = EditorGUILayout.Toggle("    Apply beam system config", myScript.useBeamSystemConfig);
            if (true != myScript.useBeamSystemConfig)
            {
                myScript.updateEveryFrame = EditorGUILayout.Toggle("       Need to update every frame", myScript.updateEveryFrame);
                myScript.startWidth = EditorGUILayout.FloatField("       Start width ", myScript.startWidth);
                myScript.endWidth = EditorGUILayout.FloatField("       End width ", myScript.endWidth);
                myScript.startOffset = EditorGUILayout.FloatField("       Start offset ", myScript.startOffset);
                myScript.endOffset = EditorGUILayout.FloatField("       End offset ", myScript.endOffset);
                myScript.StartColor = EditorGUILayout.ColorField("       Start color", myScript.StartColor);
                myScript.EndColor = EditorGUILayout.ColorField("       End color", myScript.EndColor);
            }

            EditorGUILayout.LabelField("  Indication feature");
            myScript.overwriteIndicatorSettings = true;
            myScript.showIndicator = EditorGUILayout.Toggle("    Show Indicator", myScript.showIndicator);
            if (true == myScript.showIndicator)
            {
                myScript.hideIndicatorByRoll = EditorGUILayout.Toggle("      Hide Indicator when roll angle > 90 ", myScript.hideIndicatorByRoll);
                myScript.showIndicatorAngle = EditorGUILayout.FloatField("      Show When Angle > ", myScript.showIndicatorAngle);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("      Line customization");
                myScript.lineLength = EditorGUILayout.FloatField("        Line Length", myScript.lineLength);
                myScript.lineStartWidth = EditorGUILayout.FloatField("        Line Start Width", myScript.lineStartWidth);
                myScript.lineEndWidth = EditorGUILayout.FloatField("        Line End Width", myScript.lineEndWidth);
                myScript.lineColor = EditorGUILayout.ColorField("        Line Color", myScript.lineColor);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("      Text customization");
                myScript.textCharacterSize = EditorGUILayout.FloatField("        Text Character Size", myScript.textCharacterSize);
                myScript.zhCharactarSize = EditorGUILayout.FloatField("        Chinese Character Size", myScript.zhCharactarSize);
                myScript.textFontSize = EditorGUILayout.IntField("        Text Font Size", myScript.textFontSize);
                myScript.textColor = EditorGUILayout.ColorField("        Text Color", myScript.textColor);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("      Key indication");
                var list = myScript.buttonIndicationList;

                int newCount = Mathf.Max(0, EditorGUILayout.IntField("        Button indicator size", list.Count));

                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(new ButtonIndication());

                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.LabelField("        Button indication " + i);
                    myScript.buttonIndicationList[i].keyType = (ButtonIndication.KeyIndicator)EditorGUILayout.EnumPopup("        Key Type", myScript.buttonIndicationList[i].keyType);
                    myScript.buttonIndicationList[i].alignment = (ButtonIndication.Alignment)EditorGUILayout.EnumPopup("        Alignment", myScript.buttonIndicationList[i].alignment);
                    myScript.buttonIndicationList[i].indicationOffset = EditorGUILayout.Vector3Field("        Indication offset", myScript.buttonIndicationList[i].indicationOffset);
                    myScript.buttonIndicationList[i].useMultiLanguage = EditorGUILayout.Toggle("        Use multi-language", myScript.buttonIndicationList[i].useMultiLanguage);
                    if (myScript.buttonIndicationList[i].useMultiLanguage)
                        myScript.buttonIndicationList[i].indicationText = EditorGUILayout.TextField("        Indication key", myScript.buttonIndicationList[i].indicationText);
                    else
                        myScript.buttonIndicationList[i].indicationText = EditorGUILayout.TextField("        Indication text", myScript.buttonIndicationList[i].indicationText);
                    myScript.buttonIndicationList[i].followButtonRotation = EditorGUILayout.Toggle("        Follow button rotation", myScript.buttonIndicationList[i].followButtonRotation);
                    EditorGUILayout.Space();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("Indication feature");

            myScript.overwriteIndicatorSettings = EditorGUILayout.Toggle("  Overwrite Indicator Settings", myScript.overwriteIndicatorSettings);
            if (true == myScript.overwriteIndicatorSettings)
            {
                myScript.showIndicator = EditorGUILayout.Toggle("    Show Indicator", myScript.showIndicator);
                if (true == myScript.showIndicator)
                {
                    myScript.hideIndicatorByRoll = EditorGUILayout.Toggle("    Hide Indicator when roll angle > 90 ", myScript.hideIndicatorByRoll);
                    myScript.showIndicatorAngle = EditorGUILayout.FloatField("    Show When Angle > ", myScript.showIndicatorAngle);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("    Line customization");
                    myScript.lineLength = EditorGUILayout.FloatField("      Line Length", myScript.lineLength);
                    myScript.lineStartWidth = EditorGUILayout.FloatField("      Line Start Width", myScript.lineStartWidth);
                    myScript.lineEndWidth = EditorGUILayout.FloatField("      Line End Width", myScript.lineEndWidth);
                    myScript.lineColor = EditorGUILayout.ColorField("      Line Color", myScript.lineColor);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("    Text customization");
                    myScript.textCharacterSize = EditorGUILayout.FloatField("      Text Character Size", myScript.textCharacterSize);
                    myScript.zhCharactarSize = EditorGUILayout.FloatField("      Chinese Character Size", myScript.zhCharactarSize);
                    myScript.textFontSize = EditorGUILayout.IntField("      Text Font Size", myScript.textFontSize);
                    myScript.textColor = EditorGUILayout.ColorField("      Text Color", myScript.textColor);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("    Key indication");
                    var list = myScript.buttonIndicationList;

                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("      Button indicator size", list.Count));

                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add(new ButtonIndication());

                    for (int i = 0; i < list.Count; i++)
                    {
                        EditorGUILayout.LabelField("      Button indication " + i);
                        myScript.buttonIndicationList[i].keyType = (ButtonIndication.KeyIndicator)EditorGUILayout.EnumPopup("        Key Type", myScript.buttonIndicationList[i].keyType);
                        myScript.buttonIndicationList[i].alignment = (ButtonIndication.Alignment)EditorGUILayout.EnumPopup("        Alignment", myScript.buttonIndicationList[i].alignment);
                        myScript.buttonIndicationList[i].indicationOffset = EditorGUILayout.Vector3Field("        Indication offset", myScript.buttonIndicationList[i].indicationOffset);
                        myScript.buttonIndicationList[i].useMultiLanguage = EditorGUILayout.Toggle("        Use multi-language", myScript.buttonIndicationList[i].useMultiLanguage);
                        if (myScript.buttonIndicationList[i].useMultiLanguage)
                            myScript.buttonIndicationList[i].indicationText = EditorGUILayout.TextField("        Indication key", myScript.buttonIndicationList[i].indicationText);
                        else
                            myScript.buttonIndicationList[i].indicationText = EditorGUILayout.TextField("        Indication text", myScript.buttonIndicationList[i].indicationText);
                        myScript.buttonIndicationList[i].followButtonRotation = EditorGUILayout.Toggle("        Follow button rotation", myScript.buttonIndicationList[i].followButtonRotation);
                        EditorGUILayout.Space();
                    }
                }
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty ((WaveVR_ControllerLoader)target);
    }
}
#endif

public class WaveVR_ControllerLoader : MonoBehaviour {
    private static string LOG_TAG = "WaveVR_ControllerLoader";
    private void PrintDebugLog(string msg)
    {
        #if UNITY_EDITOR
        Debug.Log(LOG_TAG + "  Hand: " + WhichHand + ", " + msg);
        #endif
        Log.d (LOG_TAG, "Hand: " + WhichHand + ", " + msg);
    }

    private void PrintInfoLog(string msg)
    {
        #if UNITY_EDITOR
        PrintDebugLog(msg);
        #endif
        Log.i (LOG_TAG, "Hand: " + WhichHand + ", " + msg);
    }

    private void PrintWarningLog(string msg)
    {
#if UNITY_EDITOR
        PrintDebugLog(msg);
#endif
        Log.w(LOG_TAG, "Hand: " + WhichHand + ", " + msg);
    }

    public enum ControllerHand
    {
        Controller_Right,
        Controller_Left
    };

    public enum CComponent
    {
        One_Bone,
        Multi_Component
    };

    public enum CTrackingSpace
    {
        REAL_POSITION_ONLY,
        FAKE_POSITION_ONLY,
        AUTO_POSITION_ONLY,
        ROTATION_ONLY,
        ROTATION_AND_REAL_POSITION,
        ROTATION_AND_FAKE_POSITION,
        ROTATION_AND_AUTO_POSITION,
        CTS_SYSTEM
    };

    public enum ControllerType
    {
        ControllerType_None,
        ControllerType_Generic,
        ControllerType_Resources,
        ControllerType_AssetBundles,
        ControllerType_AdaptiveController
    }

    [Header("Loading options")]
    public ControllerHand WhichHand = ControllerHand.Controller_Right;
    public CComponent ControllerComponents = CComponent.Multi_Component;
    public bool TrackPosition = true;
    public WVR_SimulationOption SimulationOption = WVR_SimulationOption.WhenNoPosition;
    public bool FollowHead = false;
    public bool TrackRotation = true;

    [Header("Indication feature")]
    public bool overwriteIndicatorSettings = true;
    public bool showIndicator = false;
    public bool hideIndicatorByRoll = true;

    [Range(0, 90.0f)]
    public float showIndicatorAngle = 30.0f;

    [Header("Line customization")]
    [Range(0.01f, 0.1f)]
    public float lineLength = 0.03f;
    [Range(0.0001f, 0.1f)]
    public float lineStartWidth = 0.0004f;
    [Range(0.0001f, 0.1f)]
    public float lineEndWidth = 0.0004f;
    public Color lineColor = Color.white;

    [Header("Text customization")]
    [Range(0.01f, 0.2f)]
    public float textCharacterSize = 0.08f;
    [Range(0.01f, 0.2f)]
    public float zhCharactarSize = 0.07f;
    [Range(50, 200)]
    public int textFontSize = 100;
    public Color textColor = Color.white;

    [Header("Indications")]
    public List<ButtonIndication> buttonIndicationList = new List<ButtonIndication>();

    [Header("AdaptiveLoading")]
    public bool adaptiveLoading = true;  // flag to describe if enable adaptive controller loading feature

    [Header("ButtonEffect")]
    public bool enableButtonEffect = true;
    public bool useEffectSystemConfig = true;
    public Color32 buttonEffectColor = new Color32(0, 179, 227, 255);

    [Header("Beam")]
    public bool enableBeam = true;
    public bool useBeamSystemConfig = true;
    public bool updateEveryFrame = false;
    public float startWidth = 0.000625f;    // in x,y axis
    public float endWidth = 0.00125f;       // let the bean seems the same width in far distance.
    public float startOffset = 0.015f;
    public float endOffset = 0.65f;
    public Color32 StartColor = new Color32(255, 255, 255, 255);
    public Color32 EndColor = new Color32(255, 255, 255, 77);

    private ControllerType controllerType = ControllerType.ControllerType_None;
    private GameObject controllerPrefab = null;
    private GameObject originalControllerPrefab = null;
    private string controllerFileName = "";
    private string controllerModelFoler = "Controller/";
    private string genericControllerFileName = "Generic_";
    private List<AssetBundle> loadedAssetBundle = new List<AssetBundle>();
    private string renderModelNamePath = "";
    private WVR_DeviceType deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private bool connected = false;
    private uint sessionid = 0;
    private string renderModelName = "";
    private IntPtr ptrParameterName = IntPtr.Zero;
    private IntPtr ptrResult = IntPtr.Zero;
    private bool isChecking = false;
    private WaitForSeconds wfs = null;

#if UNITY_EDITOR
    public delegate void ControllerModelLoaded(GameObject go);
    public static event ControllerModelLoaded onControllerModelLoaded = null;
#endif

    void OnEnable()
    {
        controllerPrefab = null;
        controllerFileName = "";
        genericControllerFileName = "Generic_";
        if (WhichHand == ControllerHand.Controller_Right)
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
        }
        else
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
        }
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            WVR_DeviceType _type = WaveVR_Controller.Input(this.deviceType).DeviceType;
            onLoadController(_type);
            return;
        }
#endif

        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return;
        }
#endif
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }
    // Use this for initialization
    void Start()
    {
        wfs = new WaitForSeconds(1.0f);
        loadedAssetBundle.Clear();
        if (checkConnection () != connected)
            connected = !connected;

        if (connected)
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType (this.deviceType);
            onLoadController (_device.type);
        }

        WaveVR_EventSystemControllerProvider.Instance.MarkControllerLoader (deviceType, true);
        PrintDebugLog("start a coroutine to check connection and render model name periodly");
        StartCoroutine(checkRenderModelAndDelete());
    }

    private void onDeviceConnected(params object[] args)
    {
        bool _connected = false;
        WVR_DeviceType _type = this.deviceType;

        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            _connected = WaveVR_Controller.Input (this.deviceType).connected;
            _type = WaveVR_Controller.Input(this.deviceType).DeviceType;
        }
        else
        #endif
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType (this.deviceType);
            _connected = _device.connected;
            _type = _device.type;
        }

        PrintDebugLog ("onDeviceConnected() " + _type + " is " + (_connected ? "connected" : "disconnected") + ", left-handed? " + WaveVR_Controller.IsLeftHanded);

        if (connected != _connected)
        {
            connected = _connected;
        }

        if (connected)
        {
            if (controllerPrefab == null) onLoadController (_type);
        }
    }

    private const string VRACTIVITY_CLASSNAME = "com.htc.vr.unity.WVRUnityVRActivity";
    private const string FILEUTILS_CLASSNAME = "com.htc.vr.unity.FileUtils";

    private void onLoadController(WVR_DeviceType type)
    {
        controllerFileName = "";
        controllerModelFoler = "Controller/";
        genericControllerFileName = "Generic_";

        // Make up file name
        // Rule =
        // ControllerModel_TrackingMethod_CComponent_Hand
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            genericControllerFileName = "Generic_";

            genericControllerFileName += "MC_";

            if (WhichHand == ControllerHand.Controller_Right)
            {
                genericControllerFileName += "R";
            }
            else
            {
                genericControllerFileName += "L";
            }

            originalControllerPrefab = Resources.Load(controllerModelFoler + genericControllerFileName) as GameObject;
            if (originalControllerPrefab == null)
            {
                PrintDebugLog("Cant load generic controller model, Please check file under Resources/" + controllerModelFoler + genericControllerFileName + ".prefab is exist!");
            }
            else
            {
                PrintDebugLog(genericControllerFileName + " controller model is found!");
                SetControllerOptions(originalControllerPrefab);
                controllerPrefab = Instantiate(originalControllerPrefab);
                controllerPrefab.transform.parent = this.transform.parent;

                PrintDebugLog("Controller model loaded");
                ApplyIndicatorParameters();
                if (onControllerModelLoaded != null)
                {
                    PrintDebugLog("trigger delegate");
                    onControllerModelLoaded(controllerPrefab);
                }

                WaveVR_EventSystemControllerProvider.Instance.SetControllerModel(deviceType, controllerPrefab);
            }
            return;
        }
#endif
        string parameterName = "GetRenderModelName";
        ptrParameterName = Marshal.StringToHGlobalAnsi(parameterName);
        ptrResult = Marshal.AllocHGlobal(64);
        uint resultVertLength = 64;
        Interop.WVR_GetParameters(type, ptrParameterName, ptrResult, resultVertLength);
        renderModelName = Marshal.PtrToStringAnsi(ptrResult);

        int deviceIndex = -1;
        parameterName = "backdoor_get_device_index";
        ptrParameterName = Marshal.StringToHGlobalAnsi(parameterName);
        IntPtr ptrResultDeviceIndex = Marshal.AllocHGlobal(2);
        Interop.WVR_GetParameters(type, ptrParameterName, ptrResultDeviceIndex, 2);

        int _out = 0;
        bool _ret = int.TryParse (Marshal.PtrToStringAnsi (ptrResultDeviceIndex), out _out);
        if (_ret)
            deviceIndex = _out;

        PrintInfoLog("get controller id from runtime is " + renderModelName);

        controllerFileName += renderModelName;
        controllerFileName += "_";

        if (ControllerComponents == CComponent.Multi_Component)
        {
            controllerFileName += "MC_";
        }
        else
        {
            controllerFileName += "OB_";
        }

        if (WhichHand == ControllerHand.Controller_Right)
        {
            controllerFileName += "R";
        }
        else
        {
            controllerFileName += "L";
        }

        PrintInfoLog("controller file name is " + controllerFileName);
        var found = false;
        controllerType = ControllerType.ControllerType_None;

        if (adaptiveLoading)
        {
            if (Interop.WVR_GetWaveRuntimeVersion() >= 2)
            {
                PrintInfoLog("Start adaptive loading");
                // try to adaptive loading
                bool folderPrepared = true;
                bool loadControllerAssets = false;

                // 1. check if there are assets in private folder
                string renderModelFolderPath = Application.temporaryCachePath + "/";
                string renderModelUnzipFolder = renderModelFolderPath + renderModelName + "/";
                renderModelNamePath = renderModelFolderPath + renderModelName + "/Unity";

                // delete old asset
                if (Directory.Exists(renderModelNamePath))
                {
                    try
                    {
                        Directory.Delete(renderModelNamePath, true);
                    }
                    catch (Exception e)
                    {
                        PrintInfoLog("delete folder exception: " + e);
                        folderPrepared = false;
                    }
                }

                // unzip assets from runtime
                if (folderPrepared)
                {
                    PrintWarningLog(renderModelName + " assets, start to deploy");
                    loadControllerAssets = deployZIPFile(deviceIndex, renderModelUnzipFolder);
                }

                // load model from runtime
                if (loadControllerAssets) {
                    found = loadMeshAndImageByDevice(renderModelNamePath);

                    if (!found)
                    {
                        string UnityVersion = Application.unityVersion;
                        PrintInfoLog("Application built by Unity version : " + UnityVersion);

                        int assetVersion = checkAssetBundlesVersion(UnityVersion);

                        if (assetVersion == 1)
                        {
                            renderModelNamePath += "/5.6";
                        }
                        else if (assetVersion == 2)
                        {
                            renderModelNamePath += "/2017.3";
                        }

                        // try root path
                        found = tryLoadModelFromRuntime(renderModelNamePath, controllerFileName);

                        // try to load generic from runtime
                        if (!found)
                        {
                            PrintInfoLog("Try to load generic controller model from runtime");
                            string tmpGeneric = genericControllerFileName;
                            if (WhichHand == ControllerHand.Controller_Right)
                            {
                                tmpGeneric += "MC_R";
                            }
                            else
                            {
                                tmpGeneric += "MC_L";
                            }
                            found = tryLoadModelFromRuntime(renderModelNamePath, tmpGeneric);
                        }
                    }
                }

                // load model from package
                if (!found)
                {
                    PrintWarningLog("Can not find controller model from runtime");
                    originalControllerPrefab = Resources.Load(controllerModelFoler + controllerFileName) as GameObject;
                    if (originalControllerPrefab == null)
                    {
                        Log.e(LOG_TAG, "Can't load preferred controller model from package: " + controllerFileName);
                    }
                    else
                    {
                        PrintInfoLog(controllerFileName + " controller model is found!");
                        controllerType = ControllerType.ControllerType_Resources;
                        found = true;
                    }
                }
            } else
            {
                PrintInfoLog("API Level(2) is larger than Runtime Version (" + Interop.WVR_GetWaveRuntimeVersion() + ")");
            }
        } else
        {
            PrintInfoLog("Start package resource loading");
            if (Interop.WVR_GetWaveRuntimeVersion() >= 2) {
                // load resource from package
                originalControllerPrefab = Resources.Load(controllerModelFoler + controllerFileName) as GameObject;
                if (originalControllerPrefab == null)
                {
                    Log.e(LOG_TAG, "Can't load preferred controller model: " + controllerFileName);
                }
                else
                {
                    PrintInfoLog(controllerFileName + " controller model is found!");
                    controllerType = ControllerType.ControllerType_Resources;
                    found = true;
                }
            } else
            {
                PrintInfoLog("API Level(2) is larger than Runtime Version (" + Interop.WVR_GetWaveRuntimeVersion() + "), use generic controller model!");
            }
        }

        // Nothing exist, load generic
        if (!found)
        {
            PrintInfoLog(controllerFileName + " controller model is not found from runtime and package!");

            originalControllerPrefab = loadGenericControllerModelFromPackage(genericControllerFileName);
            if (originalControllerPrefab == null)
            {
                Log.e(LOG_TAG, "Can't load generic controller model, Please check file under Resources/" + controllerModelFoler + genericControllerFileName + ".prefab is exist!");
            }
            else
            {
                PrintInfoLog(genericControllerFileName + " controller model is found!");
                controllerType = ControllerType.ControllerType_Generic;
                found = true;
            }
        }

        if (found && (originalControllerPrefab != null))
        {
            PrintInfoLog("Instantiate controller model, controller type: " + controllerType);
            SetControllerOptions(originalControllerPrefab);
            controllerPrefab = Instantiate(originalControllerPrefab);
            controllerPrefab.transform.parent = this.transform.parent;
            if (controllerType == ControllerType.ControllerType_AdaptiveController) ApplyAdaptiveControllerParameters();
            ApplyIndicatorParameters();

            WaveVR_Utils.Event.Send(WaveVR_Utils.Event.CONTROLLER_MODEL_LOADED, deviceType, controllerPrefab);
            WaveVR_EventSystemControllerProvider.Instance.SetControllerModel(deviceType, controllerPrefab);
        }

        if (adaptiveLoading && controllerType == ControllerType.ControllerType_AssetBundles)
        {
            PrintInfoLog("loadedAssetBundle length: " + loadedAssetBundle.Count);
            foreach (AssetBundle tmpAB in loadedAssetBundle)
            {
                tmpAB.Unload(false);
            }
            loadedAssetBundle.Clear();
        }
        Marshal.FreeHGlobal(ptrParameterName);
        Marshal.FreeHGlobal(ptrResult);
        Marshal.FreeHGlobal(ptrResultDeviceIndex);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        isChecking = true;
    }

    private bool loadMeshAndImageByDevice(string renderModelNamePath)
    {
        IntPtr ptrError = Marshal.AllocHGlobal(64);
        string FBXFile = renderModelNamePath + "/" + "unity.fbx";
        bool ret = false;
        string errorCode = "";

        if (File.Exists(FBXFile))
        {
            ret = WaveVR_Utils.Assimp.OpenMesh(FBXFile, ref sessionid, ptrError);
            errorCode = Marshal.PtrToStringAnsi(ptrError);
        } else
        {
            ret = false;
            errorCode = "unity.fbx is not found!";
        }

        Log.d(LOG_TAG, "ret = " + ret + " error code = " + errorCode);
        if (ret)
        {
            string imageFile = renderModelNamePath + "/" + "unity.png";
            bool fileExist = File.Exists(imageFile);
            PrintInfoLog("unity.png exist: " + fileExist);
            ret = fileExist;

            if (ret)
            {
                originalControllerPrefab = Resources.Load("AdaptiveController") as GameObject;
                ret = (originalControllerPrefab != null) ? true : false;
            }
        }
        PrintInfoLog("loadMeshAndImageByDevice ret: " + ret);

        if (ret)
        {
            controllerType = ControllerType.ControllerType_AdaptiveController;
        }
        return ret;
    }

    // used for asset bundles
    private bool tryLoadModelFromRuntime(string renderModelNamePath, string modelName)
    {
        string renderModelAssetBundle = renderModelNamePath + "/" + "Unity";
        PrintInfoLog("tryLoadModelFromRuntime, path is " + renderModelAssetBundle);
        // clear unused asset bundles
        foreach (AssetBundle tmpAB in loadedAssetBundle)
        {
            tmpAB.Unload(false);
        }
        loadedAssetBundle.Clear();
        // check root folder
        AssetBundle ab = AssetBundle.LoadFromFile(renderModelAssetBundle);
        if (ab != null)
        {
            loadedAssetBundle.Add(ab);
            AssetBundleManifest abm = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            if (abm != null)
            {
                PrintDebugLog(renderModelAssetBundle + " loaded");
                string[] assetsName = abm.GetAllAssetBundles();

                for (int i = 0; i < assetsName.Length; i++)
                {
                    string subRMAsset = renderModelNamePath + "/" + assetsName[i];
                    ab = AssetBundle.LoadFromFile(subRMAsset);

                    loadedAssetBundle.Add(ab);
                    PrintDebugLog(subRMAsset + " loaded");
                }
                PrintInfoLog("All asset Bundles loaded, start loading asset");
                originalControllerPrefab = ab.LoadAsset<GameObject>(modelName);

                if (originalControllerPrefab != null)
                {
                    PrintInfoLog("adaptive load controller model " + modelName + " success");
                    controllerType = ControllerType.ControllerType_AssetBundles;
                    return true;
                }
            }
            else
            {
                PrintWarningLog("Can't find AssetBundleManifest!!");
            }
        }
        else
        {
            PrintWarningLog("Load " + renderModelAssetBundle + " failed");
        }
        PrintInfoLog("adaptive load controller model " + modelName + " from " + renderModelNamePath + " fail!");
        return false;
    }

    private bool deployZIPFile(int deviceIndex, string renderModelUnzipFolder)
    {
        AndroidJavaClass ajc = new AndroidJavaClass(VRACTIVITY_CLASSNAME);

        if (ajc == null || deviceIndex == -1)
        {
            PrintWarningLog("AndroidJavaClass vractivity is null, deviceIndex" + deviceIndex);
            return false;
        }
        else
        {
            AndroidJavaObject activity = ajc.CallStatic<AndroidJavaObject>("getInstance");
            if (activity != null)
            {
                AndroidJavaObject afd = activity.Call<AndroidJavaObject>("getControllerModelFileDescriptor", deviceIndex);
                if (afd != null)
                {
                    AndroidJavaObject fileUtisObject = new AndroidJavaObject(FILEUTILS_CLASSNAME, activity, afd);

                    if (fileUtisObject != null)
                    {
                        bool retUnzip = fileUtisObject.Call<bool>("doUnZIPAndDeploy", renderModelUnzipFolder);

                        fileUtisObject = null;
                        if (!retUnzip)
                        {
                            PrintWarningLog("doUnZIPAndDeploy failed");
                        }
                        else
                        {
                            PrintInfoLog("doUnZIPAndDeploy success");
                            ajc = null;
                            return true;
                        }
                    }
                    else
                    {
                        PrintWarningLog("fileUtisObject is null");
                    }
                }
                else
                {
                    PrintWarningLog("get fd failed");
                }
            }
            else
            {
                PrintWarningLog("getInstance failed");
            }
        }
        ajc = null;
        return false;
    }

    // used for asset bundles
    private int checkAssetBundlesVersion(string version)
    {
        if (version.StartsWith("5.6.3") || version.StartsWith("5.6.4") || version.StartsWith("5.6.5") || version.StartsWith("5.6.6") || version.StartsWith("2017.1") || version.StartsWith("2017.2"))
        {
            return 1;
        }

        if (version.StartsWith("2017.3") || version.StartsWith("2017.4") || version.StartsWith("2018.1"))
        {
            return 2;
        }

        return 0;
    }

    private GameObject loadGenericControllerModelFromPackage(string tmpGeneric)
    {
        if (WhichHand == ControllerHand.Controller_Right)
        {
            tmpGeneric += "MC_R";
        }
        else
        {
            tmpGeneric += "MC_L";
        }
        Log.w(LOG_TAG, "Can't find preferred controller model, load generic controller : " + tmpGeneric);
        if (adaptiveLoading) PrintInfoLog("Please update controller models from device service to have better experience!");
        return Resources.Load(controllerModelFoler + tmpGeneric) as GameObject;
    }

    private void SetControllerOptions(GameObject controller_prefab)
    {
        WaveVR_PoseTrackerManager _ptm = controller_prefab.GetComponent<WaveVR_PoseTrackerManager> ();
        if (_ptm != null)
        {
            _ptm.TrackPosition = TrackPosition;
            _ptm.SimulationOption = SimulationOption;
            _ptm.FollowHead = FollowHead;
            _ptm.TrackRotation = TrackRotation;
            _ptm.Type = this.deviceType;
            PrintInfoLog("set " + this.deviceType + " to WaveVR_PoseTrackerManager");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
#endif
        if (!pauseStatus) // resume
        {
            PrintInfoLog("App resume and check controller connection");
            isChecking = DeleteControllerWhenDisconnect();
        } else
        {
            isChecking = false;
        }
    }

    // Update is called once per frame
    void Update () {
    }

    IEnumerator checkRenderModelAndDelete()
    {
        while (true)
        {
            if (isChecking)
            {
                //PrintDebugLog("check connection and render model name per 1 second");
                isChecking = DeleteControllerWhenDisconnect();
            }
            yield return wfs;
        }
    }

    private bool DeleteControllerWhenDisconnect()
    {
        if (controllerPrefab == null) return false;

        bool _connected = WaveVR_Controller.Input(this.deviceType).connected;

        if (_connected)
        {
            WVR_DeviceType type = WaveVR_Controller.Input(this.deviceType).DeviceType;
            string parameterName = "GetRenderModelName";
            ptrParameterName = Marshal.StringToHGlobalAnsi(parameterName);
            ptrResult = Marshal.AllocHGlobal(64);
            uint resultVertLength = 64;

            Interop.WVR_GetParameters(type, ptrParameterName, ptrResult, resultVertLength);
            string tmprenderModelName = Marshal.PtrToStringAnsi(ptrResult);

            if (tmprenderModelName != renderModelName)
            {
                PrintInfoLog("Destroy controller prefeb because render model is different");
                Destroy(controllerPrefab);
                controllerPrefab = null;
                Marshal.FreeHGlobal(ptrParameterName);
                Marshal.FreeHGlobal(ptrResult);
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                return false;
            }
            Marshal.FreeHGlobal(ptrParameterName);
            Marshal.FreeHGlobal(ptrResult);
        }
        else
        {
            PrintInfoLog("Destroy controller prefeb because it is disconnect");
            Destroy(controllerPrefab);
            controllerPrefab = null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            return false;
        }
        return true;
    }

    private bool checkConnection()
    {
        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            return false;
        } else
        #endif
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType (this.deviceType);
            return _device.connected;
        }
    }

    private void ApplyAdaptiveControllerParameters()
    {
        var ch = controllerPrefab.transform.childCount;

        for (int i = 0; i < ch; i++)
        {
            PrintInfoLog(controllerPrefab.transform.GetChild(i).gameObject.name);
            // get controller pointer
            if (controllerPrefab.transform.GetChild(i).gameObject.name == "ControllerPointer")
            {
                GameObject CM = controllerPrefab.transform.GetChild(i).gameObject;

                WaveVR_ControllerPointer cp = CM.GetComponent<WaveVR_ControllerPointer>();

                if (cp != null)
                {
                    cp.device = this.deviceType;
                    PrintInfoLog("ControllerPointer is found");
                }
            }
            // get model
            if (controllerPrefab.transform.GetChild(i).gameObject.name == "Model")
            {
                GameObject CM = controllerPrefab.transform.GetChild(i).gameObject;

                WaveVR_AdaptiveController ac = CM.GetComponent<WaveVR_AdaptiveController>();

                if (ac != null)
                {
                    ac.device = this.deviceType;
                    var ret = ac.makeupControllerModel(renderModelNamePath, sessionid);
                    PrintInfoLog("AdaptiveController is active: " + ret);
                }

                WaveVR_AdaptiveControllerActions aca = CM.GetComponent<WaveVR_AdaptiveControllerActions>();

                if (aca != null)
                {
                    aca.enableButtonEffect = this.enableButtonEffect;
                    if (aca.enableButtonEffect)
                    {
                        PrintInfoLog("AdaptiveController button effect is active");
                        aca.device = this.deviceType;
                        aca.useSystemConfig = this.useEffectSystemConfig;
                        if (!this.useEffectSystemConfig) aca.buttonEffectColor = this.buttonEffectColor;
                    }
                }
            }

            // get beam
            if (controllerPrefab.transform.GetChild(i).gameObject.name == "Beam")
            {
                GameObject CM = controllerPrefab.transform.GetChild(i).gameObject;
                WaveVR_Beam wb = CM.GetComponent<WaveVR_Beam>();

                if (wb != null)
                {
                    wb.useSystemConfig = this.useBeamSystemConfig;
                    if (!this.useBeamSystemConfig)
                    {
                        PrintInfoLog("Beam doesn't use system config");
                        wb.updateEveryFrame = this.updateEveryFrame;
                        wb.startWidth = this.startWidth;
                        wb.endWidth = this.endWidth;
                        wb.startOffset = this.startOffset;
                        wb.endOffset = this.endOffset;
                        wb.StartColor = this.StartColor;
                        wb.EndColor = this.EndColor;
                    }
                }
            }
        }
    }

    private void ApplyIndicatorParameters()
    {
        if (!overwriteIndicatorSettings) return;
        WaveVR_ShowIndicator si = null;

        var ch = controllerPrefab.transform.childCount;
        bool found = false;

        for (int i = 0; i < ch; i++)
        {
            PrintInfoLog(controllerPrefab.transform.GetChild(i).gameObject.name);

            GameObject CM = controllerPrefab.transform.GetChild(i).gameObject;

            si = CM.GetComponentInChildren<WaveVR_ShowIndicator>();

            if (si != null)
            {
                found = true;
                break;
            }
        }


        if (found)
        {
            PrintInfoLog("WaveVR_ControllerLoader forced update WaveVR_ShowIndicator parameter!");
            si.showIndicator = this.showIndicator;

            if (showIndicator != true)
            {
                PrintInfoLog("WaveVR_ControllerLoader forced don't show WaveVR_ShowIndicator!");
                return;
            }
            si.showIndicator = this.showIndicator;
            si.showIndicatorAngle = showIndicatorAngle;
            si.hideIndicatorByRoll = hideIndicatorByRoll;
            si.lineColor = lineColor;
            si.lineEndWidth = lineEndWidth;
            si.lineStartWidth = lineStartWidth;
            si.lineLength = lineLength;
            si.textCharacterSize = textCharacterSize;
            si.zhCharactarSize = zhCharactarSize;
            si.textColor = textColor;
            si.textFontSize = textFontSize;

            if (buttonIndicationList.Count == 0)
            {
                PrintInfoLog("WaveVR_ControllerLoader uses controller model default button indication!");
                return;
            }
            PrintInfoLog("WaveVR_ControllerLoader uses customized button indication!");
            si.buttonIndicationList.Clear();
            foreach (ButtonIndication bi in buttonIndicationList)
            {
                PrintInfoLog("use multilanguage: " + bi.useMultiLanguage);
                PrintInfoLog("indication: "+ bi.indicationText);
                PrintInfoLog("alignment: " + bi.alignment);
                PrintInfoLog("offset: " + bi.indicationOffset);
                PrintInfoLog("keyType: " + bi.keyType);
                PrintInfoLog("followRotation: " + bi.followButtonRotation);

                si.buttonIndicationList.Add(bi);
            }

            si.createIndicator();
        } else
        {
            PrintInfoLog("Controller model doesn't support button indication feature!");
        }
    }
}
