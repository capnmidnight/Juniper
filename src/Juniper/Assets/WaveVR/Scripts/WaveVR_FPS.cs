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
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class WaveVR_FPS : MonoBehaviour
{
    private Text textField;
    private float fps = 60;

    void Awake()
    {
        textField = GetComponent<Text>();
    }

    void LateUpdate()
    {
        // Avoid crash when timeScale is 0.
        if (Time.deltaTime == 0)
        {
            textField.text = "0fps";
            return;
        }

        string text = "";

        float interp = Time.deltaTime / (0.5f + Time.deltaTime);
        float currentFPS = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, currentFPS, interp);
        text += Mathf.RoundToInt(fps) + "fps";
        textField.text = text;
    }
}