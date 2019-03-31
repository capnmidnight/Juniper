// "WaveVR SDK
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;
using wvr;
using System;
using WaveVR_Log;

/// <summary>
/// Draws a pointer in front of any object that the controller point at.
/// The circle dilates if the object is clickable.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class WaveVR_ControllerPointer : MonoBehaviour {
    private const string LOG_TAG = "WaveVR_ControllerPointer";
    private void PrintDebugLog(string msg){
        #if UNITY_EDITOR
        Debug.Log(LOG_TAG + " " + msg);
        #endif
        Log.d (LOG_TAG, msg);
    }

    public WVR_DeviceType device;
    /// <summary>
    /// Growth speed multiplier for the pointer.
    /// </summary>
    private float pointerGrowthSpeed = 8.0f;
    /// <summary>
    /// Color of the pointer.
    /// </summary>
    public int reticleSegments = 20;
    public bool blink = false;
    public bool useTexture = true;
    public float pointerOuterDiameterMin = 0.25f;          // Current outer diameters of the pointer, before distance multiplication.

    private Color pointerColor = Color.white; // #FFFFFFFF
    private Color colorFactor = Color.white;               // The color variable of the pointer
    private Color borderColor = new Color(120, 120, 120, 255); // #777777FF
    private Color focusColor = new Color(255, 255, 255, 255); // #FFFFFFFF
    private Color focusBorderColor = new Color(120, 120, 120, 255); // #777777FF


    private Material materialComp;
    private Renderer rend;
    private Mesh mesh;
    private float pointerDistanceInMeters = 10.0f;          // Current distance of the pointer (in meters).
    private const float kpointerDistanceMin = 0.2f;         // Minimum distance of the pointer (in meters).
    private const float kpointerDistanceMax = 10.0f;        // Maximum distance of the pointer (in meters).
    private float pointerOuterAngle = 1.2f;                 // Current outer angle of the pointer (in degrees).
    private const float kpointerMinOuterAngle = 1.5f;       // Minimum outer angle of the pointer (in degrees).
    private const float kpointerGrowthAngle = 90f;          // Angle at which to expand the pointer when intersecting with an object (in degrees).
    private float pointerOuterDiameter = 0.0f;              // Current outer diameters of the pointer, before distance multiplication.

    private Texture2D tex = null;
    private float colorFlickerTime = 0.0f;                  // The color flicker time
    public bool ShowPointer = true;                         // true: show pointer, false: remove pointer
    private bool meshIsCreated = false;                     // true: the mesh of reticle is created, false: the mesh of reticle is not ready
    private bool stay = false;
    private string textureName = null;
    private ERaycastMode RaycastMode = ERaycastMode.Mouse;

    #region MonoBehaviour overrides
    void Start ()
    {
        ReadJsonValues ();
        if (this.ShowPointer)
        {
            PrintDebugLog ("Start() show pointer.");
            initialPointer ();
        } else
        {
            PrintDebugLog ("Start() hide pointer.");
            removePointer ();
        }
    }

    void Update()
    {
        if (this.ShowPointer)
        {
            if (!meshIsCreated)
            {
                PrintDebugLog ("Update() show pointer.");
                initialPointer ();
            }
        } else
        {
            if (meshIsCreated)
            {
                PrintDebugLog ("Update() hide pointer.");
                removePointer ();
            }
            return;
        }
        if (RaycastMode == ERaycastMode.Fixed)
            pointerDistanceInMeters = kpointerDistanceMax;
        else if (RaycastMode == ERaycastMode.Mouse)
            pointerDistanceInMeters = kpointerDistanceMin;
        else
            pointerDistanceInMeters = Mathf.Clamp (pointerDistanceInMeters, kpointerDistanceMin, kpointerDistanceMax);

        if (pointerOuterAngle < kpointerMinOuterAngle)
            pointerOuterAngle = kpointerMinOuterAngle;
        float outerHalfAngelRadians = Mathf.Deg2Rad * pointerOuterAngle * 0.5f;
        float outerDiameter = 2.0f * Mathf.Tan (outerHalfAngelRadians);

        if (RaycastMode == ERaycastMode.Fixed)
            pointerOuterDiameter = 0.2f;
        else if (RaycastMode == ERaycastMode.Mouse)
            pointerOuterDiameter = pointerOuterDiameterMin;
        else
            pointerOuterDiameter = Mathf.Lerp (pointerOuterDiameter, outerDiameter, Time.deltaTime * pointerGrowthSpeed);

        if (RaycastMode == ERaycastMode.Fixed)
        {
            materialComp.renderQueue = 1000;
        } else
        {
            materialComp.renderQueue = 5000;
        }

        if (useTexture == false)
        {
            if (blink == true)
            {
                if (Time.unscaledTime - colorFlickerTime >= 0.5f)
                {
                    colorFlickerTime = Time.unscaledTime;
                    if (colorFactor != Color.white)
                    {
                        colorFactor = Color.white;
                    } else
                    {
                        colorFactor = Color.black;
                    }
                }
            } else
            {
                colorFactor = pointerColor;
            }
            //materialComp.SetColor("_borderColor", colorFactor);
            //materialComp.SetColor("_focusColor", colorFactor);
            //materialComp.SetColor("_focusBorderColor", colorFactor);
            materialComp.SetColor ("_Color", colorFactor);
            materialComp.SetTexture ("_MainTex", null);
        } else
        {
            if (textureName != null)
            {
                string dirPath = "Resource/";
                if (textureName != "focused_dot")
                {
                    tex = (Texture2D)Resources.Load (textureName);
                    materialComp.SetTexture ("_MainTex", tex);
                }
            } else
            {
                textureName = "focused_dot";
                tex = (Texture2D)Resources.Load (textureName);
                materialComp.SetTexture ("_MainTex", tex);
            }
        }
        materialComp.SetFloat ("_useTexture", useTexture ? 1.0f : 0.0f);
        materialComp.SetFloat ("_OuterDiameter", (RaycastMode != ERaycastMode.Fixed) ? 0.03f + (pointerDistanceInMeters / kpointerGrowthAngle) : pointerOuterDiameter * pointerDistanceInMeters);
        materialComp.SetFloat ("_DistanceInMeters", pointerDistanceInMeters);
    }
    #endregion

    private void CreateControllerPointer()
    {
	int vertexCount = (reticleSegments + 1) * 2;
	Vector3[] vertices = new Vector3[vertexCount];
	for (int vi = 0, si = 0; si <= reticleSegments; si++)
	{
	    float angle = (float)si / (float)reticleSegments * Mathf.PI * 2.0f;
	    float x = Mathf.Sin(angle);
	    float y = Mathf.Cos(angle);
	    vertices[vi++] = new Vector3(x, y, 0.0f);
	    vertices[vi++] = new Vector3(x, y, 1.0f);
	}

	int indicesCount = (reticleSegments + 1) * 6;
	int[] indices = new int[indicesCount];
	int vert = 0;
	for (int ti = 0, si = 0; si < reticleSegments; si++)
	{
	    indices[ti++] = vert + 1;
	    indices[ti++] = vert;
	    indices[ti++] = vert + 2;
	    indices[ti++] = vert + 1;
	    indices[ti++] = vert + 2;
	    indices[ti++] = vert + 3;

	    vert += 2;
	}

	mesh = new Mesh();
	gameObject.AddComponent<MeshFilter>();
	GetComponent<MeshFilter>().mesh = mesh;
	mesh.vertices = vertices;
	mesh.triangles = indices;
	mesh.RecalculateBounds();
    }

    private void initialPointer()
    {
        if (useTexture == false)
        {
            colorFlickerTime = Time.unscaledTime;
            CreateControllerPointer ();
        }
        rend = GetComponent<Renderer> ();
        rend.enabled = true;
        materialComp = gameObject.GetComponent<Renderer> ().material;
        meshIsCreated = true;
    }

    private void removePointer() {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
        meshIsCreated = false;
    }

    public void OnPointerEnter (Camera camera, GameObject target, Vector3 intersectionPosition, bool isInteractive) {
        SetPointerTarget(intersectionPosition, isInteractive);
    }

    public void OnPointerStay (Camera camera, GameObject target, Vector3 intersectionPosition, bool isInteractive) {
        SetPointerTarget(intersectionPosition, isInteractive);
    }

    public void OnPointerExit (Camera camera, GameObject target) {
        stay = false;
        pointerDistanceInMeters = kpointerDistanceMax;
        pointerOuterAngle = kpointerMinOuterAngle;
    }

    public void setRaycastMode(ERaycastMode mode) {
        RaycastMode = mode;
    }

    public float getPointerCurrentDistance() {
         return pointerDistanceInMeters;
    }

    private void SetPointerTarget (Vector3 target, bool interactive) {
        Vector3 targetLocalPosition = transform.InverseTransformPoint(target);
        pointerDistanceInMeters = Mathf.Clamp(targetLocalPosition.z, kpointerDistanceMin, kpointerDistanceMax);
        if (interactive) {
            pointerOuterAngle = kpointerMinOuterAngle + Mathf.Clamp(Mathf.Abs(targetLocalPosition.z) / kpointerDistanceMax, 0.1f, kpointerDistanceMax) * kpointerGrowthAngle;
        } else {
            pointerOuterAngle = kpointerMinOuterAngle;
        }
    }

    public void SetPointerColor(Color pointer_color)
    {
        pointerColor = pointer_color;
    }

    public void SetColorFlicker(bool switchOn)
    {
	blink = switchOn;
    }

    public bool GetColorFlicker()
    {
	return blink;
    }

    private Color32 StringToColor32(string color_string)
    {
	byte[] _color_r = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(1, 2), 16));
	byte[] _color_g = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(3, 2), 16));
	byte[] _color_b = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(5, 2), 16));
	byte[] _color_a = BitConverter.GetBytes(Convert.ToInt32(color_string.Substring(7, 2), 16));

	return new Color32(_color_r[0], _color_g[0], _color_b[0], _color_a[0]);
    }

    private void ReadJsonValues()
    {
	string json_values = WaveVR_Utils.OEMConfig.getControllerConfig();

	if (!json_values.Equals(""))
	{
	    SimpleJSON.JSONNode jsNodes = SimpleJSON.JSONNode.Parse(json_values);

	    string node_value = "";
	    node_value = jsNodes["pointer"]["diameter"].Value;
	    if (!node_value.Equals(""))
	        pointerOuterDiameterMin = float.Parse(node_value);

	    node_value = jsNodes["pointer"]["distance"].Value;
	    if (!node_value.Equals(""))
	        pointerDistanceInMeters = float.Parse(node_value);

            node_value = jsNodes["pointer"]["use_texture"].Value;
	    if (!node_value.Equals(""))
	        useTexture = bool.Parse(node_value);

	    if (node_value.ToLower().Equals("false"))
	    {
		Log.d(LOG_TAG, "controller_pointer_use_texture = false, create texture");
		if (materialComp != null)
		{
		    // Material of controller pointer.

		    node_value = jsNodes["pointer"]["border_color"].Value;
		    if (!node_value.Equals(""))
			borderColor = StringToColor32(node_value);

		    node_value = jsNodes["pointer"]["focus_color"].Value;
		    if (!node_value.Equals(""))
			focusColor = StringToColor32(node_value);

		    node_value = jsNodes["pointer"]["focus_border_color"].Value;
		    if (!node_value.Equals(""))
			focusBorderColor = StringToColor32(node_value);

		    node_value = jsNodes["pointer"]["color"].Value;
		    if (!node_value.Equals(""))
			pointerColor = StringToColor32(node_value);
		}
	    }
	    else
	    {
		Log.d(LOG_TAG, "controller_pointer_use_texture = true");
		node_value = jsNodes["pointer"]["pointer_texture_name"].Value;
		if (!node_value.Equals(""))
		    textureName = node_value;
	    }
	    node_value = jsNodes["pointer"]["blink"].Value;
	    if (!node_value.Equals(""))
		blink = bool.Parse(node_value);

	    Log.d("WaveVR_Controller_Pointer", "diameter: " + pointerOuterDiameterMin + "use_texture: " + useTexture + "color: " + pointerColor + "pointer_texture_name: " + textureName + ", blink: " + blink);
	}
    }

	//public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
	//{
	//	int x, y, px, nx, py, ny, d;

	//	for (x = 0; x <= r; x++)
	//	{
	//		d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
	//		for (y = 0; y <= d; y++)
	//		{
	//			px = cx + x;
	//			nx = cx - x;
	//			py = cy + y;
	//			ny = cy - y;

	//			tex.SetPixel(px, py, col);
	//			tex.SetPixel(nx, py, col);

	//			tex.SetPixel(px, ny, col);
	//			tex.SetPixel(nx, ny, col);

	//		}
	//	}
	//	tex.Apply();
	//}
}
