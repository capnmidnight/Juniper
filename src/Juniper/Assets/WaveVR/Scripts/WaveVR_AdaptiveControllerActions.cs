using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System;

[System.Serializable]
public class MeshObject
{
    public string MeshName;
    public bool hasEffect;
    public GameObject gameObject;
    public Vector3 originPosition;
    public Material originMat;
    public Material effectMat;
}

public class WaveVR_AdaptiveControllerActions : MonoBehaviour {
    private static string LOG_TAG = "WaveVR_AdaptiveControllerActions";
    public bool enableButtonEffect = true;
    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public bool useSystemConfig = true;
    public Color buttonEffectColor = new Color(0, 179, 227, 255);

    private void PrintDebugLog(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(LOG_TAG + ": device: " + device + ", " + msg);
#endif
        Log.d(LOG_TAG, "device: " + device + ", " + msg);
    }

    private void PrintInfoLog(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(LOG_TAG + ": device: " + device + ", " + msg);
#endif
        Log.i(LOG_TAG, "device: " + device + ", " + msg);
    }

    private static readonly WVR_InputId[] pressIds = new WVR_InputId[] {
            WVR_InputId.WVR_InputId_Alias1_System,
            WVR_InputId.WVR_InputId_Alias1_Menu,
            WVR_InputId.WVR_InputId_Alias1_Grip,
            WVR_InputId.WVR_InputId_Alias1_DPad_Left,
            WVR_InputId.WVR_InputId_Alias1_DPad_Up,
            WVR_InputId.WVR_InputId_Alias1_DPad_Right,
            WVR_InputId.WVR_InputId_Alias1_DPad_Down,
            WVR_InputId.WVR_InputId_Alias1_Volume_Up,
            WVR_InputId.WVR_InputId_Alias1_Volume_Down,
            WVR_InputId.WVR_InputId_Alias1_Digital_Trigger,
            WVR_InputId.WVR_InputId_Alias1_Touchpad,
            WVR_InputId.WVR_InputId_Alias1_Trigger,
            WVR_InputId.WVR_InputId_Alias1_Volume_Up,
            WVR_InputId.WVR_InputId_Alias1_Volume_Down
    };

    private static readonly string[] PressEffectNames = new string[] {
        "__CM__HomeButton", // WVR_InputId_Alias1_System
        "__CM__AppButton", // WVR_InputId_Alias1_Menu
        "__CM__Grip", // WVR_InputId_Alias1_Grip
        "__CM__DPad_Left", // DPad_Left
        "__CM__DPad_Up", // DPad_Up
        "__CM__DPad_Right", // DPad_Right
        "__CM__DPad_Down", // DPad_Down
        "__CM__VolumeUp", // VolumeUpKey
        "__CM__VolumeDown", // VolumeDownKey
        "__CM__DigitalTriggerKey", // DigitalTriggerKey
        "__CM__TouchPad", // TouchPad_Press
        "__CM__TriggerKey", // TriggerKey
        "__CM__VolumeKey", // Volume
        "__CM__VolumeKey" // Volume
    };

    private MeshObject[] pressObjectArrays = new MeshObject[pressIds.Length];

    private static readonly WVR_InputId[] touchIds = new WVR_InputId[] {
            WVR_InputId.WVR_InputId_Alias1_Touchpad
    };

    private static readonly string[] TouchEffectNames = new string[] {
        "__CM__TouchPad_Touch" // TouchPad_Touch
    };

    private MeshObject[] touchObjectArrays = new MeshObject[touchIds.Length];

    private GameObject touchpad = null;
    private Mesh touchpadMesh = null;
    private Mesh toucheffectMesh = null;
    private bool currentIsLeftHandMode = false;

    void onAdaptiveControllerModelReady(params object[] args)
    {
        WVR_DeviceType device = (WVR_DeviceType)args[0];

        if (device == this.device)
            CollectEffectObjects();
    }

    void OnEnable()
    {
        resetButtonState();
        //WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.ADAPTIVE_CONTROLLER_READY, onAdaptiveControllerModelReady);

}

    void OnDisable()
    {
        //WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.ADAPTIVE_CONTROLLER_READY, onAdaptiveControllerModelReady);
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
#endif
        if (!pauseStatus) // resume
        {
            PrintInfoLog("Pause(" + pauseStatus + ") and reset button state");
            resetButtonState();
        }
    }

    void resetButtonState()
    {
        PrintDebugLog("reset button state");
        if (!enableButtonEffect) return;

        for (int i=0; i < pressObjectArrays.Length; i++)
        {
            if (pressObjectArrays[i] == null) continue;
            if (pressObjectArrays[i].hasEffect)
            {
                if (pressObjectArrays[i].gameObject != null && pressObjectArrays[i].originMat != null && pressObjectArrays[i].effectMat != null)
                {
                    pressObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = pressObjectArrays[i].originMat;
                }
            }
        }

        for (int i = 0; i < touchObjectArrays.Length; i++)
        {
            if (touchObjectArrays[i] == null) continue;
            if (touchObjectArrays[i].hasEffect)
            {
                if (touchObjectArrays[i].gameObject != null && touchObjectArrays[i].originMat != null && touchObjectArrays[i].effectMat != null)
                {
                    touchObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = touchObjectArrays[i].originMat;
                    touchObjectArrays[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        resetButtonState();
    }

    // Update is called once per frame
    void Update () {
        if (!enableButtonEffect)
            return;

        if (currentIsLeftHandMode != WaveVR_Controller.IsLeftHanded)
        {
            currentIsLeftHandMode = WaveVR_Controller.IsLeftHanded;
            PrintInfoLog("Controller role is changed to " + (currentIsLeftHandMode ? "Left" : "Right") + " and reset button state");
            resetButtonState();
        }

        for (int i=0; i<pressIds.Length; i++)
        {
            if (pressObjectArrays[i] == null) continue;
            if (WaveVR_Controller.Input(device).GetPressDown(pressIds[i]))
            {
                PrintInfoLog(pressIds[i] + " press down");
                if (pressObjectArrays[i].hasEffect)
                {
                    if (pressObjectArrays[i].gameObject != null && pressObjectArrays[i].originMat != null && pressObjectArrays[i].effectMat != null)
                    {
                        pressObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = pressObjectArrays[i].effectMat;
                    }
                }
            }

            if (WaveVR_Controller.Input(device).GetPress(pressIds[i]))
            {
                if (pressObjectArrays[i].hasEffect)
                {
                    if (pressObjectArrays[i].gameObject != null && pressObjectArrays[i].originMat != null && pressObjectArrays[i].effectMat != null)
                    {
                        pressObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = pressObjectArrays[i].effectMat;
                    }
                }
            }

            if (WaveVR_Controller.Input(device).GetPressUp(pressIds[i]))
            {
                PrintInfoLog(pressIds[i] + " press up");
                if (pressObjectArrays[i].hasEffect)
                {
                    if (pressObjectArrays[i].gameObject != null && pressObjectArrays[i].originMat != null && pressObjectArrays[i].effectMat != null)
                    {
                        pressObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = pressObjectArrays[i].originMat;
                    }
                }
            }
        }

        for (int i = 0; i < touchIds.Length; i++)
        {
            if (touchObjectArrays[i] == null) continue;
            if (WaveVR_Controller.Input(device).GetTouchDown(touchIds[i]))
            {
                PrintInfoLog(touchIds[i] + " touch down");
                if (touchObjectArrays[i].hasEffect)
                {
                    if (touchObjectArrays[i].gameObject != null && touchObjectArrays[i].originMat != null && touchObjectArrays[i].effectMat != null)
                    {
                        touchObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = touchObjectArrays[i].effectMat;
                        touchObjectArrays[i].gameObject.SetActive(true);
                    }
                }
            }

            if (WaveVR_Controller.Input(device).GetTouch(touchIds[i]))
            {
                if (touchObjectArrays[i].hasEffect && touchObjectArrays[i].MeshName == "__CM__TouchPad_Touch")
                {
                    if (touchObjectArrays[i].gameObject != null && touchObjectArrays[i].originMat != null && touchObjectArrays[i].effectMat != null)
                    {
                        var axis = WaveVR_Controller.Input(this.device).GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);

                        float xangle = axis.x * (touchpadMesh.bounds.size.x * touchpad.transform.localScale.x - toucheffectMesh.bounds.size.x * touchObjectArrays[i].gameObject.transform.localScale.x) / 2;
                        float yangle = axis.y * (touchpadMesh.bounds.size.z * touchpad.transform.localScale.z - toucheffectMesh.bounds.size.z * touchObjectArrays[i].gameObject.transform.localScale.z) / 2;

                        var height = touchpadMesh.bounds.size.y * touchpad.transform.localScale.y;
                        Log.gpl.d(LOG_TAG, "WVR_InputId_Alias1_Touchpad axis x: " + axis.x + " axis.y: " + axis.y + ", xangle: " + xangle + ", yangle: " + yangle + ", height: " + height);

#if DEBUG
                        Log.gpl.d(LOG_TAG, "TouchEffectMesh.bounds.size: " + toucheffectMesh.bounds.size.x + ", " + toucheffectMesh.bounds.size.y + ", " + toucheffectMesh.bounds.size.z);
                        Log.gpl.d(LOG_TAG, "TouchEffectMesh.scale: " + touchObjectArrays[i].gameObject.transform.localScale.x + ", " + touchObjectArrays[i].gameObject.transform.localScale.y + ", " + touchObjectArrays[i].gameObject.transform.localScale.z);
                        Log.gpl.d(LOG_TAG, "TouchpadMesh.bounds.size: " + touchpadMesh.bounds.size.x + ", " + touchpadMesh.bounds.size.y + ", " + touchpadMesh.bounds.size.z);
                        Log.gpl.d(LOG_TAG, "TouchpadMesh. scale: " + touchObjectArrays[i].gameObject.transform.localScale.x + ", " + touchObjectArrays[i].gameObject.transform.localScale.y + ", " + touchObjectArrays[i].gameObject.transform.localScale.z);
                        Log.gpl.d(LOG_TAG, "TouchEffect.originPosition: " + touchObjectArrays[i].originPosition.x + ", " + touchObjectArrays[i].originPosition.y + ", " + touchObjectArrays[i].originPosition.z);
#endif
                        Vector3 translateVec = Vector3.zero;
                        translateVec = new Vector3(xangle, height, yangle);
                        touchObjectArrays[i].gameObject.transform.localPosition = touchObjectArrays[i].originPosition + translateVec;
                    }
                }
            }

            if (WaveVR_Controller.Input(device).GetTouchUp(touchIds[i]))
            {
                PrintInfoLog(touchIds[i] + " touch up");
                if (touchObjectArrays[i].hasEffect)
                {
                    if (touchObjectArrays[i].gameObject != null && touchObjectArrays[i].originMat != null && touchObjectArrays[i].effectMat != null)
                    {
                        touchObjectArrays[i].gameObject.GetComponent<MeshRenderer>().material = touchObjectArrays[i].originMat;
                        touchObjectArrays[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private Material effectMat;
    private Material touchMat;

    private void CollectEffectObjects()
    {
        effectMat = new Material(Shader.Find("Unlit/Color"));
        touchMat = new Material(Shader.Find("Unlit/Texture"));
        if (useSystemConfig)
        {
            PrintInfoLog("use system config in controller model!");
            ReadJsonValues();
        }
        else
        {
            Log.w(LOG_TAG, "use custom config in controller model!");
        }

        var ch = this.transform.childCount;
        PrintDebugLog("childCount: " + ch);
        effectMat.color = buttonEffectColor;

        for (var j = 0; j < PressEffectNames.Length; j++)
        {
            pressObjectArrays[j] = new MeshObject();
            pressObjectArrays[j].MeshName = PressEffectNames[j];
            pressObjectArrays[j].hasEffect = false;
            pressObjectArrays[j].gameObject = null;
            pressObjectArrays[j].originPosition = new Vector3(0, 0, 0);
            pressObjectArrays[j].originMat = null;
            pressObjectArrays[j].effectMat = null;

            PrintDebugLog("mesh name: " + PressEffectNames[j]);
            for (int i = 0; i < ch; i++)
            {
                GameObject CM = this.transform.GetChild(i).gameObject;
                string[] t = CM.name.Split("."[0]);
                var childname = t[0];

                if (pressObjectArrays[j].MeshName == childname)
                {
                    PrintInfoLog(childname + " is found! ");
                    pressObjectArrays[j].gameObject = CM;
                    pressObjectArrays[j].originPosition = CM.transform.localPosition;
                    pressObjectArrays[j].originMat = CM.GetComponent<MeshRenderer>().material;
                    pressObjectArrays[j].effectMat = effectMat;
                    pressObjectArrays[j].hasEffect = true;

                    if (childname == "__CM__TouchPad")
                    {
                        touchpad = pressObjectArrays[j].gameObject;
                        touchpadMesh = touchpad.GetComponent<MeshFilter>().mesh;
                        if (touchpadMesh != null)
                        {
                            PrintInfoLog("touchpad is found! ");
                        }
                    }
                    break;
                }
            }
        }

        for (var j = 0; j < TouchEffectNames.Length; j++)
        {
            touchObjectArrays[j] = new MeshObject();
            touchObjectArrays[j].MeshName = TouchEffectNames[j];
            touchObjectArrays[j].hasEffect = false;
            touchObjectArrays[j].gameObject = null;
            touchObjectArrays[j].originPosition = new Vector3(0, 0, 0);
            touchObjectArrays[j].originMat = null;
            touchObjectArrays[j].effectMat = null;

            PrintDebugLog("mesh name: " + TouchEffectNames[j]);
            for (int i = 0; i < ch; i++)
            {
                GameObject CM = this.transform.GetChild(i).gameObject;
                string[] t = CM.name.Split("."[0]);
                var childname = t[0];

                if (touchObjectArrays[j].MeshName == childname)
                {
                    PrintInfoLog(childname + " is found! ");
                    touchObjectArrays[j].gameObject = CM;
                    touchObjectArrays[j].originPosition = CM.transform.localPosition;
                    touchObjectArrays[j].originMat = CM.GetComponent<MeshRenderer>().material;
                    touchObjectArrays[j].effectMat = effectMat;
                    touchObjectArrays[j].hasEffect = true;

                    if (childname == "__CM__TouchPad_Touch")
                    {
                        toucheffectMesh = touchObjectArrays[j].gameObject.GetComponent<MeshFilter>().mesh;
                        if (toucheffectMesh != null)
                        {
                            PrintInfoLog("toucheffectMesh is found! ");
                        }
                    }

                    break;
                }
            }
        }

        resetButtonState();
    }

    private Color StringToColor(string color_string)
    {
        float _color_r = (float)Convert.ToInt32(color_string.Substring(1, 2), 16);
        float _color_g = (float)Convert.ToInt32(color_string.Substring(3, 2), 16);
        float _color_b = (float)Convert.ToInt32(color_string.Substring(5, 2), 16);
        float _color_a = (float)Convert.ToInt32(color_string.Substring(7, 2), 16);

        return new Color(_color_r, _color_g, _color_b, _color_a);
    }

    private Texture2D GetTexture2D(string texture_path)
    {
        if (System.IO.File.Exists(texture_path))
        {
            var _bytes = System.IO.File.ReadAllBytes(texture_path);
            var _texture = new Texture2D(1, 1);
            _texture.LoadImage(_bytes);
            return _texture;
        }
        return null;
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);

            }
        }
        tex.Apply();
    }

    private void ReadJsonValues()
    {
        string json_values = WaveVR_Utils.OEMConfig.getControllerConfig();
        if (!json_values.Equals(""))
        {
            SimpleJSON.JSONNode jsNodes = SimpleJSON.JSONNode.Parse(json_values);
            string node_value = "";

            node_value = jsNodes["model"]["touchpad_dot_use_texture"].Value;
            if (node_value.ToLower().Equals("false"))
            {
                PrintDebugLog("touchpad_dot_use_texture = false, create texture");

                // effect color.
                node_value = jsNodes["model"]["touchpad_dot_color"].Value;
                if (!node_value.Equals(""))
                {
                    PrintInfoLog("touchpad_dot_color: " + node_value);
                    buttonEffectColor = StringToColor(node_value);

                    var texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                    Color o = Color.clear;
                    o.r = 1f;
                    o.g = 1f;
                    o.b = 1f;
                    o.a = 0f;
                    for (int i = 0; i < 256; i++)
                    {
                        for (int j = 0; j < 256; j++)
                        {
                            texture.SetPixel(i, j, o);
                        }
                    }
                    texture.Apply();

                    Circle(texture, 128, 128, 100, buttonEffectColor);

                    touchMat.mainTexture = texture;
                }
            }
            else
            {
                PrintDebugLog("touchpad_dot_use_texture = true");
                node_value = jsNodes["model"]["touchpad_dot_texture_name"].Value;
                if (!node_value.Equals(""))
                {
                    if (System.IO.File.Exists(node_value))
                    {
                        var _bytes = System.IO.File.ReadAllBytes(node_value);
                        var _texture = new Texture2D(1, 1);
                        _texture.LoadImage(_bytes);

                            PrintInfoLog("touchpad_dot_texture_name: " + node_value);
                            touchMat.mainTexture = _texture;
                            touchMat.color = buttonEffectColor;

                    }
                }
            }

            // Battery
            node_value = jsNodes["battery"]["battery_level_count"].Value;
            if (!node_value.Equals(""))
            {
                // TODO: TBD for battery levels
                /*
                batteryLevels = Convert.ToInt32(node_value, 10);

                if (batteryLevels > 0)
                {
                    bool updateBatteryTextures = true;
                    string texName = "";
                    string minPercentStr = "";
                    string maxPercentStr = "";
                    for (int i = 0; i < batteryLevels; i++)
                    {
                        texName = jsNodes["battery"]["battery_levels"][i]["level_texture_name"].Value;
                        minPercentStr = jsNodes["battery"]["battery_levels"][i]["level_min_value"].Value;
                        maxPercentStr = jsNodes["battery"]["battery_levels"][i]["level_max_value"].Value;
                        if (!texName.Equals("") && !minPercentStr.Equals("") && !maxPercentStr.Equals(""))
                        {
                            Texture _tex = GetTexture2D(texName);
                            if (_tex == null)
                            {
                                updateBatteryTextures = false;
                                break;
                            }
                        }
                        else
                        {
                            updateBatteryTextures = false;
                            break;
                        }
                    }

                    if (updateBatteryTextures)
                    {
                        Log.d(LOG_TAG, "updateBatteryTextures, battery_level_count: " + batteryLevels);
                        batteryPercentages.Clear();

                        for (int i = 0; i < batteryLevels; i++)
                        {
                            texName = jsNodes["battery"]["battery_levels"][i]["level_texture_name"].Value;
                            minPercentStr = jsNodes["battery"]["battery_levels"][i]["level_min_value"].Value;
                            maxPercentStr = jsNodes["battery"]["battery_levels"][i]["level_max_value"].Value;
                            if (!texName.Equals("") && !minPercentStr.Equals("") && !maxPercentStr.Equals(""))
                            {
                                Texture _tex = GetTexture2D(texName);
                                if (_tex != null)
                                {
                                    BatteryPercentage tmpBP = new BatteryPercentage();
                                    tmpBP.texture = _tex;
                                    tmpBP.minBatteryPercentage = float.Parse(minPercentStr);
                                    tmpBP.maxBatteryPercentage = float.Parse(maxPercentStr);

                                    Log.d(LOG_TAG, "updateBatteryTextures, level: " + i + ", min = " + minPercentStr + ", max = " + maxPercentStr + ", texName = " + texName);
                                    batteryPercentages.Add(tmpBP);
                                }
                            }
                        }
                    }
                }*/
            }
        }
    }
}
