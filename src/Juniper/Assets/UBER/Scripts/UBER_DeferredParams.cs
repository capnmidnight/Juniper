using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
struct UBER_RGBA_ByteArray
{
    [FieldOffset(0)]
    public byte Byte0;
    [FieldOffset(1)]
    public byte Byte1;
    [FieldOffset(2)]
    public byte Byte2;
    [FieldOffset(3)]
    public byte Byte3;
    [FieldOffset(4)]
    public byte Byte4;
    [FieldOffset(5)]
    public byte Byte5;
    [FieldOffset(6)]
    public byte Byte6;
    [FieldOffset(7)]
    public byte Byte7;
    [FieldOffset(8)]
    public byte Byte8;
    [FieldOffset(9)]
    public byte Byte9;
    [FieldOffset(10)]
    public byte Byte10;
    [FieldOffset(11)]
    public byte Byte11;
    [FieldOffset(12)]
    public byte Byte12;
    [FieldOffset(13)]
    public byte Byte13;
    [FieldOffset(14)]
    public byte Byte14;
    [FieldOffset(15)]
    public byte Byte15;

    [FieldOffset(0)]
    public float R;
    [FieldOffset(4)]
    public float G;
    [FieldOffset(8)]
    public float B;
    [FieldOffset(12)]
    public float A;
}

[AddComponentMenu("UBER/Deferred Params")]
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class UBER_DeferredParams : MonoBehaviour {
    [Header("Translucency setup 1")]
	[ColorUsageAttribute(false)]
    public Color TranslucencyColor1=new Color(1,1,1,1);
	[Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
	public float Strength1=4;
	[Range(0.0f, 1.0f)] public float PointLightsDirectionality1=0.7f;
	[Range(0.0f, 0.5f)] public float Constant1=0.1f;
	[Range(0.0f, 0.3f)] public float Scattering1=0.05f;
	[Range(0.0f, 100f)] public float SpotExponent1=30f;
	[Range(0.0f, 20f)] public float SuppressShadows1=0.5f;
	[Range(0.0f, 1.0f)] public float NdotLReduction1=0f;

    [Space]
    [Header("Translucency setup 2")]
    [ColorUsageAttribute(false)]
    public Color TranslucencyColor2 = new Color(1, 1, 1, 1);
    [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
    public float Strength2 = 4;
    [Range(0.0f, 1.0f)] public float PointLightsDirectionality2 = 0.7f;
    [Range(0.0f, 0.5f)] public float Constant2 = 0.1f;
    [Range(0.0f, 0.3f)] public float Scattering2 = 0.05f;
    [Range(0.0f, 100f)] public float SpotExponent2 = 30f;
    [Range(0.0f, 20f)] public float SuppressShadows2 = 0.5f;
    [Range(0.0f, 1.0f)] public float NdotLReduction2 = 0f;

    [Space]
    [Header("Translucency setup 3")]
    [ColorUsageAttribute(false)]
    public Color TranslucencyColor3 = new Color(1, 1, 1, 1);
    [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
    public float Strength3 = 4;
    [Range(0.0f, 1.0f)] public float PointLightsDirectionality3 = 0.7f;
    [Range(0.0f, 0.5f)] public float Constant3 = 0.1f;
    [Range(0.0f, 0.3f)] public float Scattering3 = 0.05f;
    [Range(0.0f, 100f)] public float SpotExponent3 = 30f;
    [Range(0.0f, 20f)] public float SuppressShadows3 = 0.5f;
    [Range(0.0f, 1.0f)] public float NdotLReduction3 = 0f;

    [Space]
    [Header("Translucency setup 4")]
    [ColorUsageAttribute(false)]
    public Color TranslucencyColor4 = new Color(1, 1, 1, 1);
    [Tooltip("You can control strength per light using its color alpha (first enable in UBER config file)")]
    public float Strength4 = 4;
    [Range(0.0f, 1.0f)] public float PointLightsDirectionality4 = 0.7f;
    [Range(0.0f, 0.5f)] public float Constant4 = 0.1f;
    [Range(0.0f, 0.3f)] public float Scattering4 = 0.05f;
    [Range(0.0f, 100f)] public float SpotExponent4 = 30f;
    [Range(0.0f, 20f)] public float SuppressShadows4 = 0.5f;
    [Range(0.0f, 1.0f)] public float NdotLReduction4 = 0f;

    /////////////////////////////////////////////////////////////////////////////////////////////

    private Camera mycam;
	private CommandBuffer combufPreLight;
	private CommandBuffer combufPostLight;

    private Material CopyPropsMat;

    private bool UBERPresenceChecked;
    private bool UBERPresent;
    [HideInInspector] public Texture2D TranslucencyPropsTex;

    private HashSet<Camera> sceneCamsWithBuffer = new HashSet<Camera>();

    // @TODO
    // wszystkie parametry translucency indexuj (nie tylko kolor), dekodowanie mozesz przeprowadzic poprzez lookuptex (malutka textura ARGBFloat przechowujca wszystkie parametry dla 4 setupow) - to powinno dziaac b. szybko (tekstura 4x2 powinna wystarczyc)
    void Start()
    {
        SetupTranslucencyValues();
    }

    public void OnValidate()
    {
        SetupTranslucencyValues();
    }

    public void SetupTranslucencyValues()
    {
        if (TranslucencyPropsTex==null)
        {
            TranslucencyPropsTex = new Texture2D(4, 3, TextureFormat.RGBAFloat, false, true);
            TranslucencyPropsTex.anisoLevel = 0;
            TranslucencyPropsTex.filterMode = FilterMode.Point;
            TranslucencyPropsTex.wrapMode = TextureWrapMode.Clamp;
            TranslucencyPropsTex.hideFlags = HideFlags.HideAndDontSave;
        }
        Shader.SetGlobalTexture("_UBERTranslucencySetup", TranslucencyPropsTex);

        
        byte[] rawTextData = new byte[4 * 3 * 4 * 4]; // 4 setups x 3 RGBA floats
        EncodeRGBAFloatTo16Bytes(TranslucencyColor1.r, TranslucencyColor1.g, TranslucencyColor1.b, Strength1, rawTextData, 0, 0);
        EncodeRGBAFloatTo16Bytes(PointLightsDirectionality1, Constant1, Scattering1, SpotExponent1, rawTextData,  0, 1);
        EncodeRGBAFloatTo16Bytes(SuppressShadows1, NdotLReduction1, 1, 1, rawTextData, 0, 2);

        EncodeRGBAFloatTo16Bytes(TranslucencyColor2.r, TranslucencyColor2.g, TranslucencyColor2.b, Strength2, rawTextData, 1, 0);
        EncodeRGBAFloatTo16Bytes(PointLightsDirectionality2, Constant2, Scattering2, SpotExponent2, rawTextData, 1, 1);
        EncodeRGBAFloatTo16Bytes(SuppressShadows2, NdotLReduction2, 1, 1, rawTextData, 1, 2);

        EncodeRGBAFloatTo16Bytes(TranslucencyColor3.r, TranslucencyColor3.g, TranslucencyColor3.b, Strength3, rawTextData, 2, 0);
        EncodeRGBAFloatTo16Bytes(PointLightsDirectionality3, Constant3, Scattering3, SpotExponent3, rawTextData, 2, 1);
        EncodeRGBAFloatTo16Bytes(SuppressShadows3, NdotLReduction3, 1, 1, rawTextData, 2, 2);

        EncodeRGBAFloatTo16Bytes(TranslucencyColor4.r, TranslucencyColor4.g, TranslucencyColor4.b, Strength4, rawTextData, 3, 0);
        EncodeRGBAFloatTo16Bytes(PointLightsDirectionality4, Constant4, Scattering4, SpotExponent4, rawTextData, 3, 1);
        EncodeRGBAFloatTo16Bytes(SuppressShadows4, NdotLReduction4, 1, 1, rawTextData, 3, 2);

        TranslucencyPropsTex.LoadRawTextureData(rawTextData);
        TranslucencyPropsTex.Apply();
    }

    private void EncodeRGBAFloatTo16Bytes(float r, float g, float b, float a, byte[] rawTexdata, int idx_u, int idx_v)
    {
        const int capacity=4; // 4 setups
        int idx = idx_v * capacity * 16 + idx_u * 16;

        UBER_RGBA_ByteArray barray = new UBER_RGBA_ByteArray();
        barray.R = r;
        barray.G = g;
        barray.B = b;
        barray.A = a;
        rawTexdata[idx++] = barray.Byte0;
        rawTexdata[idx++] = barray.Byte1;
        rawTexdata[idx++] = barray.Byte2;
        rawTexdata[idx++] = barray.Byte3;
        rawTexdata[idx++] = barray.Byte4;
        rawTexdata[idx++] = barray.Byte5;
        rawTexdata[idx++] = barray.Byte6;
        rawTexdata[idx++] = barray.Byte7;
        rawTexdata[idx++] = barray.Byte8;
        rawTexdata[idx++] = barray.Byte9;
        rawTexdata[idx++] = barray.Byte10;
        rawTexdata[idx++] = barray.Byte11;
        rawTexdata[idx++] = barray.Byte12;
        rawTexdata[idx++] = barray.Byte13;
        rawTexdata[idx++] = barray.Byte14;
        rawTexdata[idx++] = barray.Byte15;
    }

public void OnEnable()
    {
        SetupTranslucencyValues();

        if (NotifyDecals()) return; // decals installed and used - don't handle command buffers

        if (mycam == null)
        {
            mycam = GetComponent<Camera>();
            if (mycam == null) return;
        }
        Initialize();
        Camera.onPreRender += SetupCam;
    }

    public void OnDisable()
    {
        NotifyDecals();
        Cleanup();
    }
    public void OnDestroy()
    {
        NotifyDecals();
        Cleanup();
    }

    private bool NotifyDecals()
    {
        System.Type decalsType = System.Type.GetType("UBERDecalSystem.DecalManager");
        if (decalsType != null)
        {
            bool DecalsEnabled = false;
            DecalsEnabled = GameObject.FindObjectOfType(decalsType) != null && (GameObject.FindObjectOfType(decalsType) is MonoBehaviour) && (GameObject.FindObjectOfType(decalsType) as MonoBehaviour).enabled;
            if (DecalsEnabled)
            {
                (GameObject.FindObjectOfType(decalsType) as MonoBehaviour).Invoke("OnDisable", 0);
                (GameObject.FindObjectOfType(decalsType) as MonoBehaviour).Invoke("OnEnable", 0);
                return true;
            }
        }
        return false;
    }

    private void Cleanup()
    {
        if (TranslucencyPropsTex)
        {
            Object.DestroyImmediate(TranslucencyPropsTex);
            TranslucencyPropsTex = null;
        }
        if (combufPreLight != null)
        {
            if (mycam)
            {
                mycam.RemoveCommandBuffer(CameraEvent.BeforeReflections, combufPreLight);
                mycam.RemoveCommandBuffer(CameraEvent.AfterLighting, combufPostLight);
            }
            foreach (Camera cam in sceneCamsWithBuffer)
            {
                if (cam)
                {
                    cam.RemoveCommandBuffer(CameraEvent.BeforeReflections, combufPreLight);
                    cam.RemoveCommandBuffer(CameraEvent.AfterLighting, combufPostLight);
                }
            }
        }
        sceneCamsWithBuffer.Clear();
        Camera.onPreRender -= SetupCam;
    }

    private void SetupCam(Camera cam)
    {
        bool isSceneCam = false;
#if UNITY_EDITOR
        if (Camera.main && Camera.main == mycam)
        {
            // scene cameras handled by component attached to Camera.main only (to not double it)
            isSceneCam = (SceneView.lastActiveSceneView && cam == SceneView.lastActiveSceneView.camera);
        }
#endif
        if (cam==mycam || isSceneCam)
        {
            RefreshComBufs(cam, isSceneCam);
        }
    }

	public void RefreshComBufs(Camera cam, bool isSceneCam) {
		if (cam && combufPreLight!=null && combufPostLight!=null) {
            CommandBuffer[] combufsPreLight = cam.GetCommandBuffers(CameraEvent.BeforeReflections);
            bool found = false;
            foreach (CommandBuffer cbuf in combufsPreLight)
            {
                // instance comparison below DOESN'T work !!! Well, weird isn't it ???
                //if (cbuf == combufPreLight)
                if (cbuf.name == combufPreLight.name)
                {
                    // got it already in command buffers
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                cam.AddCommandBuffer(CameraEvent.BeforeReflections, combufPreLight);
                cam.AddCommandBuffer(CameraEvent.AfterLighting, combufPostLight);
                if (isSceneCam)
                {
                    sceneCamsWithBuffer.Add(cam);
                }
            }
		}
	}

	public void Initialize() {
		if (combufPreLight == null) {
			int propsBufferID = Shader.PropertyToID("_UBERPropsBuffer");

            // prepare material
            if (CopyPropsMat == null)
            {
                if (CopyPropsMat != null)
                {
                    DestroyImmediate(CopyPropsMat);
                }
                CopyPropsMat = new Material(Shader.Find("Hidden/UBER_CopyPropsTexture"));
                CopyPropsMat.hideFlags = HideFlags.DontSave;
            }

            // take a copy of emission buffer.a where UBER stores its props (translucency, self-shadowing, wetness)
            combufPreLight = new CommandBuffer();
			combufPreLight.name="UBERPropsPrelight";
            combufPreLight.GetTemporaryRT(propsBufferID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RHalf);
            combufPreLight.Blit(BuiltinRenderTextureType.CameraTarget, propsBufferID, CopyPropsMat);
            
			// release temp buffer
			combufPostLight = new CommandBuffer();
			combufPostLight.name="UBERPropsPostlight";
            combufPostLight.ReleaseTemporaryRT (propsBufferID);
		}
    }
}
