// "WaveVR SDK
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

#pragma warning disable 0414

using UnityEngine;
using System.Collections.Generic;
using wvr;
using WaveVR_Log;
using System;
using System.Text;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveVR_Beam: MonoBehaviour
{
    private static string LOG_TAG = "WaveVR_Beam";

    public bool useSystemConfig = true;
    public float startWidth = 0.000625f;    // in x,y axis
    public float endWidth = 0.00125f;       // let the bean seems the same width in far distance.
    public float startOffset = 0.015f;

    public float endOffset = 0.8f;
    private const float endOffsetMin = 0.5f;    // Minimum distance of end offset (in meters).
    private const float endOffsetMax = 9.5f;          // Maximum distance of end offset (in meters).

    public Color32 StartColor = new Color32 (255, 255, 255, 255);
    private Color32 TailColor = new Color32 (255, 255, 255, 255);
    public Color32 EndColor = new Color32 (255, 255, 255, 77);

    private static bool enableBeam = true;

    public int count = 3;
    public bool updateEveryFrame = true;
    public bool makeTail = true; // Offset from 0

    //private bool useTexture = true;
    private string textureName;

    private int maxUVAngle = 30;
    private const float epsilon = 0.001f;
    private bool isBeamEnable = false;
    private ERaycastMode RaycastMode = ERaycastMode.Mouse;

    private void PrintDebugLog(string msg)
    {
        #if UNITY_EDITOR
        Debug.Log(LOG_TAG + " " + msg);
        #endif
        Log.d (LOG_TAG, msg);
    }

    private void Validate()
    {

        if (startWidth < epsilon)
            startWidth = epsilon;

        if (endWidth < epsilon)
            endWidth = epsilon;

        if (startOffset < epsilon)
            startOffset = epsilon;

        if (endOffset < epsilon * 2)
            endOffset = epsilon * 2;

        if (endOffset < startOffset)
            endOffset = startOffset + epsilon;

        if (count < 3)
            count = 3;

        /**
         * The texture pattern should be a radiated image starting 
         * from the texture center.
         * If the mesh's count is too low, the uv map can't keep a 
         * good radiation shap.  Therefore the maxUVAngle should be
         * reduced to avoid the uv area cutting the radiation circle.
        **/
        int uvAngle = 360 / count;
        if (uvAngle > 30)
            maxUVAngle = 30;
        else
            maxUVAngle = uvAngle;
    }

    private int Count = -1, verticesCount = -1, indicesCount = -1;

    public List<Vector3> vertices;
    public List<Vector2> uvs;
    public List<Vector3> normals;
    public List<int> indices;
    public List<Color32> colors;
    public Vector3 position;

    private Mesh emptyMesh;
    private Mesh updateMesh;
    private Material materialComp;
    private MeshFilter mf_beam;

    private bool toUpdateBeam = false;

    void Awake()
    {
        emptyMesh = new Mesh ();
        updateMesh = new Mesh ();
    }

    void OnEnable()
    {
        if (!isBeamEnable)
        {
            if (useSystemConfig)
            {
                PrintDebugLog ("use system config in WaveVR_Beam!");
                ReadJsonValues ();
            }
            else
            {
                PrintDebugLog ("use custom config in WaveVR_Beam!");
            }

            TailColor = StartColor;

            Count = count + 1;
            verticesCount = Count * 2 + (makeTail ? 1 : 0);
            indicesCount = Count * 6 + (makeTail ? count * 3 : 0);

            //uvs = new List<Vector2>(verticesCount);
            //vertices = new List<Vector3> (verticesCount);
            //normals = new List<Vector3>(verticesCount);
            //indices = new List<int>(indicesCount);

            Validate();

            mf_beam = GetComponent<MeshFilter>();
            createMesh();
            mf_beam.mesh = updateMesh;

            PrintDebugLog ("OnEnable() startWidth: " + startWidth
                + ", endWidth: " + endWidth + ", endOffset: " + this.endOffset
                + ", StartColor: " + StartColor.ToString () + ", EndColor: " + EndColor.ToString ());

            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
            isBeamEnable = true;
        }
    }

    void OnDisable()
    {
        isBeamEnable = false;
    }

    public void Update()
    {
        if (false == enableBeam && RaycastMode == ERaycastMode.Beam)
        {
            mf_beam.mesh = emptyMesh;
        } else
        {
            if (!updateEveryFrame && !this.toUpdateBeam)
                return;

            Validate ();
            createMesh ();
            mf_beam.mesh = updateMesh;

            if (this.toUpdateBeam)
                this.toUpdateBeam = !this.toUpdateBeam;
        }
    }

    public void setRaycastMode(ERaycastMode mode) {
        RaycastMode = mode;
    }

    public void SetEndOffset (Vector3 target, bool interactive)
    {
        Vector3 targetLocalPosition = transform.InverseTransformPoint(target);

        if (this.RaycastMode == ERaycastMode.Beam)
        {
            if (targetLocalPosition.z < endOffsetMin)
                enableBeam = false;
            else
            {
                this.endOffset = targetLocalPosition.z - endOffsetMin;
                this.endOffset = Mathf.Clamp (this.endOffset, endOffsetMin, endOffsetMax);
                enableBeam = true;
                this.toUpdateBeam = true;
            }

            PrintDebugLog ("SetEndOffset() targetLocalPosition.z: " + targetLocalPosition.z
                + ", endOffset: " + this.endOffset
                + ", endOffsetMin: " + endOffsetMin
                + ", endOffsetMax: " + endOffsetMax);
        }
    }

    public void ResetEndOffset()
    {
        if (this.RaycastMode == ERaycastMode.Beam)
        {
            this.endOffset = endOffsetMax;
            enableBeam = true;
            this.toUpdateBeam = true;

            PrintDebugLog ("ResetEndOffset() "
                + ", endOffset: " + this.endOffset
                + ", endOffsetMin: " + endOffsetMin
                + ", endOffsetMax: " + endOffsetMax);
        }
    }

    private Matrix4x4 mat44_rot = Matrix4x4.zero;
    private Matrix4x4 mat44_uv = Matrix4x4.zero;
    private Vector3 vec3_vertices_start = Vector3.zero;
    private Vector3 vec3_vertices_end = Vector3.zero;

    private readonly Vector2 vec2_05_05 = new Vector2 (0.5f, 0.5f);
    private readonly Vector3 vec3_0_05_0 = new Vector3 (0, 0.5f, 0);
    private void createMesh()
    {
        updateMesh.Clear ();
        uvs.Clear ();
        vertices.Clear ();
        normals.Clear ();
        indices.Clear ();
        colors.Clear ();

        mat44_rot = Matrix4x4.zero;
        mat44_uv = Matrix4x4.zero;

        for (int i = 0; i < Count; i++)
        {
            int angle = (int) (i * 360.0f / count);
            int UVangle = (int)(i * maxUVAngle / count);
            // make rotation matrix
            mat44_rot.SetTRS(Vector3.zero, Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
            mat44_uv.SetTRS(Vector3.zero, Quaternion.AngleAxis(UVangle, Vector3.forward), Vector3.one);

            // start
            vec3_vertices_start.y = startWidth;
            vec3_vertices_start.z = startOffset;
            vertices.Add (mat44_rot.MultiplyVector (vec3_vertices_start));
            uvs.Add (vec2_05_05);
            colors.Add (StartColor);
            normals.Add (mat44_rot.MultiplyVector (Vector3.up).normalized);

            // end
            vec3_vertices_end.y = endWidth;
            vec3_vertices_end.z = this.endOffset;
            vertices.Add (mat44_rot.MultiplyVector (vec3_vertices_end));
            Vector2 uv = mat44_uv.MultiplyVector (vec3_0_05_0);
            uv.x = uv.x + 0.5f;
            uv.y = uv.y + 0.5f;
            uvs.Add(uv);
            colors.Add (EndColor);
            normals.Add(mat44_rot.MultiplyVector(Vector3.up).normalized);
        }

        for (int i = 0; i < count; i++)
        {
            // bd
            // ac
            int a, b, c, d;
            a = i * 2;
            b = i * 2 + 1;
            c = i * 2 + 2;
            d = i * 2 + 3;

            // first
            indices.Add(a);
            indices.Add(d);
            indices.Add(b);

            // second
            indices.Add(a);
            indices.Add(c);
            indices.Add(d);
        }

        // Make Tail
        if (makeTail)
        {
            vertices.Add (Vector3.zero);
            colors.Add (TailColor);
            uvs.Add (vec2_05_05);
            normals.Add (Vector3.zero);
            int tailIdx = count * 2;
            for (int i = 0; i < count; i++)
            {
                int idx = i * 2;

                indices.Add(tailIdx);
                indices.Add(idx + 2);
                indices.Add(idx);
            }
        }
        updateMesh.vertices = vertices.ToArray();
        //updateMesh.SetUVs(0, uvs);
        //updateMesh.SetUVs(1, uvs);
        updateMesh.colors32  = colors.ToArray ();
        updateMesh.normals = normals.ToArray();
        updateMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        updateMesh.name = "Beam";
    }

    private Color32 StringToColor32(string color_string)
    {
	byte[] _color_r = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(1, 2), 16));
	byte[] _color_g = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(3, 2), 16));
	byte[] _color_b = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(5, 2), 16));
	byte[] _color_a = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(7, 2), 16));

	return new Color32(_color_r[0], _color_g[0], _color_b[0], _color_a[0]);
    }

    private void UpdateStartColor(string color_string)
    {
        byte[] _color_r = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(1, 2), 16));
        byte[] _color_g = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(3, 2), 16));
        byte[] _color_b = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(5, 2), 16));
        byte[] _color_a = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(7, 2), 16));

        this.StartColor.r = _color_r [0];
        this.StartColor.g = _color_g [0];
        this.StartColor.b = _color_b [0];
        this.StartColor.a = _color_a [0];
    }

    private void UpdateEndColor(string color_string)
    {
        byte[] _color_r = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(1, 2), 16));
        byte[] _color_g = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(3, 2), 16));
        byte[] _color_b = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(5, 2), 16));
        byte[] _color_a = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(7, 2), 16));

        this.EndColor.r = _color_r [0];
        this.EndColor.g = _color_g [0];
        this.EndColor.b = _color_b [0];
        this.EndColor.a = _color_a [0];
    }

    private void ReadJsonValues()
    {
        string json_values = WaveVR_Utils.OEMConfig.getControllerConfig ();

        if (!json_values.Equals (""))
        {
            SimpleJSON.JSONNode jsNodes = SimpleJSON.JSONNode.Parse (json_values);

            string node_value = "";
            node_value = jsNodes ["beam"] ["start_width"].Value;
            if (!node_value.Equals (""))
                startWidth = float.Parse (node_value);

            node_value = jsNodes ["beam"] ["end_width"].Value;
            if (!node_value.Equals (""))
                endWidth = float.Parse (node_value);

            node_value = jsNodes ["beam"] ["length"].Value;
            if (!node_value.Equals (""))
                this.endOffset = float.Parse (node_value);

            if (node_value.ToLower ().Equals ("false"))
            {
                Log.d (LOG_TAG, "beam_use_texture = false, create texture");
                node_value = jsNodes ["beam"] ["start_color"].Value;
                if (!node_value.Equals (""))
                {
                    //StartColor = StringToColor32 (node_value);
                    UpdateStartColor (node_value);
                }

                node_value = jsNodes ["beam"] ["end_color"].Value;
                if (!node_value.Equals (""))
                {
                    //EndColor = StringToColor32 (node_value);
                    UpdateEndColor(node_value);
                }
            } else
            {
                Log.d (LOG_TAG, "beam_use_texture = true");
                node_value = jsNodes ["beam"] ["beam_texture_name"].Value;
                //if (!node_value.Equals(""))
                //{
                //	if (System.IO.File.Exists(node_value))
                //	{
                //	    var _bytes = System.IO.File.ReadAllBytes(node_value);
                //	    var _texture = new Texture2D(1, 1);
                //	    _texture.LoadImage(_bytes);

                //	    if (materialComp != null)
                //	    {
                //		Log.d(LOG_TAG, "beam_use_texture: " + node_value);
                //		MeshRenderer _mrdr = gameObject.GetComponentInChildren<MeshRenderer>();
                //		Material _mat = _mrdr.materials[0];
                //		_mat.mainTexture = _texture;
                //	        _mat.color = materialColor;
                //	    }
                //	}
                //}
            }
        }
    }
}
