using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

public class WaveVR_AdaptiveController : MonoBehaviour {
    private static string LOG_TAG = "WaveVR_AdaptiveController";
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

    private string renderModelNamePath = "";
    private uint sessionid = 0;
    private WaitForEndOfFrame wfef = null;
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;

    public bool makeupControllerModel(string renderModelName, uint sid)
    {
        sectionCount = 0;

        renderModelNamePath = renderModelName;
        sessionid = sid;

        ThreadStart threadStart = new ThreadStart(readNativeData);
        mthread = new Thread(threadStart);
        mthread.Start();

        string imageFile = renderModelNamePath + "/unity.png";

        byte[] imgByteArray = File.ReadAllBytes(imageFile);
        MatImage = new Texture2D(2, 2, TextureFormat.BGRA32, false);

        bool retLoad = MatImage.LoadImage(imgByteArray);
        Log.d(LOG_TAG, "load image ret: " + retLoad + " size: " + imgByteArray.Length);
        if (!retLoad)
        {
            Log.d(LOG_TAG, "failed to load texture");
            return false;
        }

        Log.d(LOG_TAG, "makeup controller model done");
        return true;
    }

    bool bLoadMesh = false;

    IEnumerator checkParams()
    {
        while (true)
        {
            if (!bLoadMesh)
                yield return wfef;
            else
                break;
        }
        string meshName = "";
        for (uint i = 0; i < sectionCount; i++)
        {
            meshName = Marshal.PtrToStringAnsi(FBXInfo[i].meshName);
            meshCom = null;
            meshGO = null;
            updateMesh = new Mesh();

            meshCom = Resources.Load("MeshComponent") as GameObject;
            meshGO = Instantiate(meshCom);
            meshGO.transform.parent = this.transform;
            meshGO.name = meshName;
            Matrix4x4 t = WaveVR_Utils.RigidTransform.toMatrix44(FBXInfo[i].matrix);

            Vector3 x = WaveVR_Utils.GetPosition(t);
            meshGO.transform.localPosition = new Vector3(-x.x, x.y, x.z);

            Log.d(LOG_TAG, " MeshGO = " + meshName + ", localPosition: " + meshGO.transform.localPosition.x + ", " + meshGO.transform.localPosition.y + ", " + meshGO.transform.localPosition.z);
            meshGO.transform.localRotation = WaveVR_Utils.GetRotation(t);

            float a = 0f;
            Vector3 b = Vector3.zero;
            meshGO.transform.localRotation.ToAngleAxis(out a, out b);
            Log.d(LOG_TAG, " MeshGO = " + meshName + ", localRotation: " + b);
            //Log.d(LOG_TAG, " MeshGO = " + meshName + ", localRotation: " + meshGO.transform.localRotation);
            meshGO.transform.localScale = WaveVR_Utils.GetScale(t);
            Log.d(LOG_TAG, " MeshGO = " + meshName + ", localScale: " + meshGO.transform.localScale);

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

            meshfilter.mesh = updateMesh;

            var meshRenderer = meshGO.GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = MatImage;
            meshRenderer.enabled = true;
        }

        WaveVR_Utils.Event.Send(WaveVR_Utils.Event.ADAPTIVE_CONTROLLER_READY, this.device);
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
        Log.d(LOG_TAG, "releaseMesh");
        WaveVR_Utils.Assimp.releaseMesh(sessionid);
    }

    void readNativeData()
    {
        bLoadMesh = false;
        bool ret = false;
        Log.d(LOG_TAG, "sessionid = " + sessionid);
        bool finishLoading = WaveVR_Utils.Assimp.getSectionCount(sessionid, ref sectionCount);

        if (!finishLoading || sectionCount == 0)
        {
            Log.d(LOG_TAG, "failed to load mesh");
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

        Log.d(LOG_TAG, "getMeshData start, sectionCount = " + sectionCount);
        ret = WaveVR_Utils.Assimp.getMeshData(sessionid, FBXInfo);
        Log.d(LOG_TAG, "getMeshData end");
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

        Log.d(LOG_TAG, "log and create array start");
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

            Log.d(LOG_TAG, "i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m0 + " , " + FBXInfo[i].matrix.m1 + " , " + FBXInfo[i].matrix.m2 + " , " + FBXInfo[i].matrix.m3 + "] ");
            Log.d(LOG_TAG, "i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m4 + " , " + FBXInfo[i].matrix.m5 + " , " + FBXInfo[i].matrix.m6 + " , " + FBXInfo[i].matrix.m7 + "] ");
            Log.d(LOG_TAG, "i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m8 + " , " + FBXInfo[i].matrix.m9 + " , " + FBXInfo[i].matrix.m10 + " , " + FBXInfo[i].matrix.m11 + "] ");
            Log.d(LOG_TAG, "i = " + i + ", relative transform = [" + FBXInfo[i].matrix.m12 + " , " + FBXInfo[i].matrix.m13 + " , " + FBXInfo[i].matrix.m14 + " , " + FBXInfo[i].matrix.m15 + "] ");
            Log.d(LOG_TAG, "i = " + i + ", vertice count = " + FBXInfo[i].verticeCount + ", normal count = " + FBXInfo[i].normalCount + ", uv count = " + FBXInfo[i].uvCount + ", indice count = " + FBXInfo[i].indiceCount);
        }
        Log.d(LOG_TAG, "log and create array end");

        bLoadMesh = true;
    }

    void OnEnable()
    {
        Log.d(LOG_TAG, "OnEnable");
        wfef = new WaitForEndOfFrame();
    }

    // Use this for initialization
    void Start () {
        Log.d(LOG_TAG, "Start");
        StartCoroutine(checkParams());
    }

    // Update is called once per frame
    void Update () {

    }
}
