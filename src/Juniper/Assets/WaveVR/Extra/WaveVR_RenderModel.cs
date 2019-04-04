using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

public class WaveVR_RenderModel : MonoBehaviour
{
    private static string LOG_TAG = "WaveVR_RenderModel";
    private void PrintDebugLog(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(LOG_TAG + "  Hand: " + WhichHand + ", " + msg);
#endif
        Log.d(LOG_TAG, "Hand: " + WhichHand + ", " + msg);
    }

    private void PrintInfoLog(string msg)
    {
#if UNITY_EDITOR
        PrintDebugLog(msg);
#endif
        Log.i(LOG_TAG, "Hand: " + WhichHand + ", " + msg);
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

    public ControllerHand WhichHand = ControllerHand.Controller_Right;
    public GameObject defaultModel = null;
    public bool updateDynamically = false;

    private GameObject controllerSpawned = null;
    private WVR_DeviceType deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    private bool connected = false;
    private string renderModelNamePath = "";
    private string renderModelName = "";
    private IntPtr ptrParameterName = IntPtr.Zero;
    private IntPtr ptrResult = IntPtr.Zero;
    private uint sessionid = 0;
    private const string VRACTIVITY_CLASSNAME = "com.htc.vr.unity.WVRUnityVRActivity";
    private const string FILEUTILS_CLASSNAME = "com.htc.vr.unity.FileUtils";

    private List<Color32> colors = new List<Color32>();
    private GameObject meshCom = null;
    private GameObject meshGO = null;
    private Mesh updateMesh;
    private Texture2D MatImage = null;
    private Material modelMat;

    private FBXInfo_t[] FBXInfo;
    private MeshInfo_t[] SectionInfo;
    private uint sectionCount;
    private Thread mthread;
    private bool isChecking = false;

    private Material ImgMaterial;
    private static bool isRenderModelEnable = false;
    private WaitForEndOfFrame wfef = null;
    private WaitForSeconds wfs = null;

    void OnEnable()
    {
        PrintDebugLog("OnEnable");

        if (WhichHand == ControllerHand.Controller_Right)
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Right;
        }
        else
        {
            deviceType = WVR_DeviceType.WVR_DeviceType_Controller_Left;
        }

        connected = checkConnection();

        if (connected)
        {
            PrintDebugLog("Controller load when connected!");

            WaveVR.Device _device = WaveVR.Instance.getDeviceByType(this.deviceType);
            onLoadController(_device.type);
        }

        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }

    void OnDisable()
    {
        PrintDebugLog("OnDisable");

        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DEVICE_CONNECTED, onDeviceConnected);
    }

    private void onDeviceConnected(params object[] args)
    {
        bool _connected = false;
        WVR_DeviceType _type = this.deviceType;

#if UNITY_EDITOR
        if (Application.isEditor)
        {
            _connected = WaveVR_Controller.Input(this.deviceType).connected;
            _type = WaveVR_Controller.Input(this.deviceType).DeviceType;
        }
        else
#endif
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType(this.deviceType);
            _connected = _device.connected;
            _type = _device.type;
        }

        PrintDebugLog("onDeviceConnected() " + _type + " is " + (_connected ? "connected" : "disconnected") + ", left-handed? " + WaveVR_Controller.IsLeftHanded);

        if (connected != _connected)
        {
            connected = _connected;
        }

        if (connected)
        {
            if (!checkChild()) onLoadController(_type);
        }
    }
    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            bool _connected = false;
            WVR_DeviceType _type = this.deviceType;
            _connected = WaveVR_Controller.Input(this.deviceType).connected;
            _type = WaveVR_Controller.Input(this.deviceType).DeviceType;
            onLoadController(_type);
            return;
        }
#endif
        PrintDebugLog("start() connect: " + connected + " Which hand: " + WhichHand);
        ImgMaterial = new Material(Shader.Find("Unlit/Texture"));
        wfef = new WaitForEndOfFrame();
        wfs = new WaitForSeconds(1.0f);

        if (updateDynamically)
        {
            PrintDebugLog("updateDynamically, start a coroutine to check connection and render model name periodly");
            StartCoroutine(checkRenderModelAndDelete());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Log.gpl.d(LOG_TAG, "Update() render model " + WhichHand + " connect ? " + this.connected + ", child object count ? " + transform.childCount);
    }

    private void onLoadController(WVR_DeviceType type)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            PrintDebugLog("onLoadController in play mode");
            if (defaultModel != null)
            {
                controllerSpawned = Instantiate(defaultModel, this.transform);
                controllerSpawned.transform.parent = this.transform;
            }
            return;
        }
#endif

        if (Interop.WVR_GetWaveRuntimeVersion() < 2)
        {
            PrintDebugLog("onLoadController in old service");
            if (defaultModel != null)
            {
                controllerSpawned = Instantiate(defaultModel, this.transform);
                controllerSpawned.transform.parent = this.transform;
            }
            return;
        }

        bool loadControllerAssets = true;
        var found = false;

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
        bool _ret = int.TryParse(Marshal.PtrToStringAnsi(ptrResultDeviceIndex), out _out);
        if (_ret)
            deviceIndex = _out;

        PrintInfoLog("get controller id from runtime is " + renderModelName);

        // 1. check if there are assets in private folder
        string renderModelFolderPath = Application.temporaryCachePath + "/";
        string renderModelUnzipFolder = renderModelFolderPath + renderModelName + "/";
        renderModelNamePath = renderModelFolderPath + renderModelName + "/Unity";

        if (!Directory.Exists(renderModelNamePath))
        {
            PrintWarningLog(renderModelName + " assets, start to deploy");
            loadControllerAssets = deployZIPFile(deviceIndex, renderModelUnzipFolder);
        }

        if (loadControllerAssets)
        {
            found = loadMeshAndImageByDevice(renderModelNamePath);

            if (found)
            {
                bool renderModelReady = makeupControllerModel(renderModelNamePath, sessionid);
                PrintDebugLog("renderModelReady = " + renderModelReady);
                Marshal.FreeHGlobal(ptrParameterName);
                Marshal.FreeHGlobal(ptrResult);
                return;
            }
        }

        if (defaultModel != null)
        {
            PrintDebugLog("Can't load controller model from DS, load default model");
            controllerSpawned = Instantiate(defaultModel, this.transform);
            controllerSpawned.transform.parent = this.transform;
        }

        Marshal.FreeHGlobal(ptrParameterName);
        Marshal.FreeHGlobal(ptrResult);
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
                            ajc = null;
                            PrintInfoLog("doUnZIPAndDeploy success");
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
        }
        else
        {
            ret = false;
            errorCode = "unity.fbx is not found!";
        }

        PrintDebugLog("ret = " + ret + " error code = " + errorCode);
        if (ret)
        {
            string imageFile = renderModelNamePath + "/" + "unity.png";
            bool fileExist = File.Exists(imageFile);
            PrintInfoLog("unity.png exist: " + fileExist);
            ret = fileExist;
        }
        PrintInfoLog("loadMeshAndImageByDevice ret: " + ret);
        Marshal.FreeHGlobal(ptrError);

        return ret;
    }

    public bool makeupControllerModel(string renderModelNamePath, uint sid)
    {
        sectionCount = 0;
        sessionid = sid;
        if (checkChild()) deleteChild();

        string imageFile = renderModelNamePath + "/unity.png";

        if (!File.Exists(imageFile))
        {
            PrintDebugLog("failed to load texture");
            return false;
        }
        byte[] imgByteArray = File.ReadAllBytes(imageFile);
        MatImage = new Texture2D(2, 2, TextureFormat.BGRA32, false);
        bool retLoad = MatImage.LoadImage(imgByteArray);
        PrintDebugLog("load image ret: " + retLoad + " size: " + imgByteArray.Length);
        if (!retLoad)
        {
            PrintDebugLog("failed to load texture");
            return false;
        }
        bLoadMesh = false;
        PrintDebugLog("reset bLoadMesh, start to spawn game object after new connection");
        StartCoroutine(SpawnRenderModel());
        ThreadStart threadStart = new ThreadStart(readNativeData);
        mthread = new Thread(threadStart);
        mthread.Start();

        isChecking = true;
        return true;
    }

    IEnumerator SpawnRenderModel()
    {
        while(true)
        {
            if (bLoadMesh) break;
            PrintDebugLog("SpawnRenderModel is waiting");
            yield return wfef;
        }
        spawnMesh();
    }

    bool bLoadMesh = false;

    void spawnMesh()
    {
        if (!bLoadMesh)
        {
            PrintDebugLog("bLoadMesh is false, skipping spawn objects");
            return;
        }
        string meshName = "";
        for (uint i = 0; i < sectionCount; i++)
        {
            meshName = Marshal.PtrToStringAnsi(FBXInfo[i].meshName);
            meshCom = null;
            meshGO = null;

            updateMesh = new Mesh();
            meshCom = new GameObject();
            meshCom.AddComponent<MeshRenderer>();
            meshCom.AddComponent<MeshFilter>();
       //      meshCom = Resources.Load("MeshComponent") as GameObject;
            meshGO = Instantiate(meshCom);
            meshGO.transform.parent = this.transform;
            meshGO.name = meshName;
            Matrix4x4 t = WaveVR_Utils.RigidTransform.toMatrix44(FBXInfo[i].matrix);

            Vector3 x = WaveVR_Utils.GetPosition(t);
            meshGO.transform.localPosition = new Vector3(-x.x, x.y, x.z);

            PrintDebugLog("i = " + i + " MeshGO = " + meshName + ", localPosition: " + meshGO.transform.localPosition.x + ", " + meshGO.transform.localPosition.y + ", " + meshGO.transform.localPosition.z);
            meshGO.transform.localRotation = WaveVR_Utils.GetRotation(t);

            float a = 0f;
            Vector3 b = Vector3.zero;
            meshGO.transform.localRotation.ToAngleAxis(out a, out b);
            PrintDebugLog("i = " + i + " MeshGO = " + meshName + ", localRotation: " + b);
            //PrintDebugLog(" MeshGO = " + meshName + ", localRotation: " + meshGO.transform.localRotation);
            meshGO.transform.localScale = WaveVR_Utils.GetScale(t);
            PrintDebugLog("i = " + i + " MeshGO = " + meshName + ", localScale: " + meshGO.transform.localScale);
            meshGO.transform.Rotate(new Vector3(0, 180, 0));

            var meshfilter = meshGO.GetComponent<MeshFilter>();

            updateMesh.Clear();
            updateMesh.vertices = SectionInfo[i]._vectice;
            updateMesh.uv = SectionInfo[i]._uv;
            updateMesh.uv2 = SectionInfo[i]._uv;
            updateMesh.colors32 = colors.ToArray();
            updateMesh.normals = SectionInfo[i]._normal;
            updateMesh.SetIndices(SectionInfo[i]._indice, MeshTopology.Triangles, 0);
            updateMesh.name = meshName;
            if (meshfilter != null)
            {
                meshfilter.mesh = updateMesh;
            }

            var meshRenderer = meshGO.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = ImgMaterial;
                meshRenderer.material.mainTexture = MatImage;
                meshRenderer.enabled = true;
            }

            if (meshName == "__CM__TouchPad_Touch") meshGO.SetActive(false);

        }
        WaveVR_Utils.Event.Send(WaveVR_Utils.Event.ADAPTIVE_CONTROLLER_READY, deviceType);

        cleanNativeData();
        Resources.UnloadUnusedAssets();
    }

    void cleanNativeData()
    {
        for (int i = 0; i < sectionCount; i++)
        {
            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._vectice[j] = Vector3.zero;
            }
            SectionInfo[i]._vectice = null;

            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._normal[j] = Vector3.zero;
            }
            SectionInfo[i]._normal = null;

            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._uv[j] = Vector2.zero;
            }
            SectionInfo[i]._uv = null;

            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._indice[j] = 0;
            }
            SectionInfo[i]._indice = null;

            Marshal.FreeHGlobal(FBXInfo[i].meshName);
        }
        SectionInfo = null;
        FBXInfo = null;
        WaveVR_Utils.Assimp.releaseMesh(sessionid);
    }

    void readNativeData()
    {
        bool ret = false;
        PrintDebugLog("sessionid = " + sessionid);
        bool finishLoading = WaveVR_Utils.Assimp.getSectionCount(sessionid, ref sectionCount);

        if (!finishLoading || sectionCount == 0)
        {
            PrintDebugLog("failed to load mesh");
            return;
        }

        FBXInfo = new FBXInfo_t[sectionCount];
        SectionInfo = new MeshInfo_t[sectionCount];
        for (int i = 0; i < sectionCount; i++)
        {
            FBXInfo[i] = new FBXInfo_t();
            SectionInfo[i] = new MeshInfo_t();

            FBXInfo[i].meshName = Marshal.AllocHGlobal(64);
        }

        ret = WaveVR_Utils.Assimp.getMeshData(sessionid, FBXInfo);
        if (!ret)
        {
            for (int i = 0; i < sectionCount; i++)
            {
                Marshal.FreeHGlobal(FBXInfo[i].meshName);
            }

            SectionInfo = null;
            FBXInfo = null;
            WaveVR_Utils.Assimp.releaseMesh(sessionid);
            return;
        }

        for (uint i = 0; i < sectionCount; i++)
        {
            SectionInfo[i]._vectice = new Vector3[FBXInfo[i].verticeCount];
            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._vectice[j] = new Vector3();
            }
            SectionInfo[i]._normal = new Vector3[FBXInfo[i].normalCount];
            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._normal[j] = new Vector3();
            }
            SectionInfo[i]._uv = new Vector2[FBXInfo[i].uvCount];
            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._uv[j] = new Vector2();
            }
            SectionInfo[i]._indice = new int[FBXInfo[i].indiceCount];
            for (int j = 0; j < FBXInfo[i].verticeCount; j++)
            {
                SectionInfo[i]._indice[j] = new int();
            }

            bool tret = WaveVR_Utils.Assimp.getSectionData(sessionid, i, SectionInfo[i]._vectice, SectionInfo[i]._normal, SectionInfo[i]._uv, SectionInfo[i]._indice);
            if (!tret) continue;

            PrintDebugLog("i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m0 + " , " + FBXInfo[i].matrix.m1 + " , " + FBXInfo[i].matrix.m2 + " , " + FBXInfo[i].matrix.m3 + "] ");
            PrintDebugLog("i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m4 + " , " + FBXInfo[i].matrix.m5 + " , " + FBXInfo[i].matrix.m6 + " , " + FBXInfo[i].matrix.m7 + "] ");
            PrintDebugLog("i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m8 + " , " + FBXInfo[i].matrix.m9 + " , " + FBXInfo[i].matrix.m10 + " , " + FBXInfo[i].matrix.m11 + "] ");
            PrintDebugLog("i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m12 + " , " + FBXInfo[i].matrix.m13 + " , " + FBXInfo[i].matrix.m14 + " , " + FBXInfo[i].matrix.m15 + "] ");
            PrintDebugLog("i = " + i + ", vertice count = " + FBXInfo[i].verticeCount + ", normal count = " + FBXInfo[i].normalCount + ", uv count = " + FBXInfo[i].uvCount + ", indice count = " + FBXInfo[i].indiceCount);
        }

        bLoadMesh = true;
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
#endif

        if (updateDynamically)
        {
            if (!pauseStatus) // resume
            {
                isChecking = DeleteControllerWhenDisconnect();
            }
            else
            {
                isChecking = false;
            }
        }
    }

    IEnumerator checkRenderModelAndDelete()
    {
        while (true)
        {
            if (isChecking)
            {
                isChecking = DeleteControllerWhenDisconnect();
            }
            yield return wfs;
        }
    }

    private void deleteChild()
    {
        var ch = transform.childCount;

        for (int i = 0; i < ch; i++)
        {
            PrintInfoLog("deleteChild: " + transform.GetChild(i).gameObject.name);

            GameObject CM = transform.GetChild(i).gameObject;

            Destroy(CM);
        }
    }

    private bool checkChild()
    {
        var ch = transform.childCount;

        return (ch > 0) ? true : false;
    }

    private bool DeleteControllerWhenDisconnect()
    {
        if (!checkChild()) return false;

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
                deleteChild();
                Marshal.FreeHGlobal(ptrParameterName);
                Marshal.FreeHGlobal(ptrResult);
                return false;
            }
            Marshal.FreeHGlobal(ptrParameterName);
            Marshal.FreeHGlobal(ptrResult);
        }
        else
        {
            PrintInfoLog("Destroy controller prefeb because it is disconnect");
            deleteChild();
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
        }
        else
#endif
        {
            WaveVR.Device _device = WaveVR.Instance.getDeviceByType(this.deviceType);
            return _device.connected;
        }
    }
}
