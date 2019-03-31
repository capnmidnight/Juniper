#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Build;
using UnityEditor;
using System.Xml;

static class CustomBuildProcessor
{
    private static string version_depended_aar = "wavevr_unity_plugin";
    private static string aar5 = Application.dataPath + "/Plugins/Android/" + version_depended_aar + ".aar";
    private static string aar2017 = Application.dataPath + "/Plugins/Android/" + version_depended_aar + "_2017.aar";
    private static string skip = "";
    private static string recovery = "";

    private static string minWaveSDKVersion = "";
    private static string numDoFHmd = "";
    private static string numDoFController = "";
    private static string numController = "";
    private static bool initWaveVRAttributes = false;
    private static bool forceUpdateWaveVRAttributes = false;

    public class WaveVRAttributesWindow : EditorWindow
    {
        [MenuItem("WaveVR/Preference/WaveVR Attributes")]
        static void Init()
        {
            EditorWindow tmp = EditorWindow.focusedWindow;
            // Get existing open window or if none, make a new one:
            WaveVRAttributesWindow window = (WaveVRAttributesWindow)EditorWindow.GetWindow(typeof(WaveVRAttributesWindow), true, "WaveVR Attributes", false);
            window.position = new Rect(Screen.width / 10, Screen.height / 10, 320, 240);
            forceUpdateWaveVRAttributes = true;
            window.Show();
            tmp.Focus();
        }

        public static void ShowWindow()
        {
            GetWindow<WaveVRAttributesWindow>("WaveVR Attributes");
        }

        void OnGUI()
        {
            if (!initWaveVRAttributes || forceUpdateWaveVRAttributes)
                updateMetadata();
            {
#if UNITY_5_6_OR_NEWER
                var packagename = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#else
                var packagename = PlayerSettings.bundleIdentifier;
#endif
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.wordWrap = true;
                {
                    var origin = GUI.color;
                    GUI.color = Color.white;
                    GUILayout.Label ("Extracting from " + packagename + " manifest:", style);
                    GUI.color = origin;
                }

                if (minWaveSDKVersion.Equals(""))
                {
                    var origin = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label ("minWaveSDKVersion is NULL!", EditorStyles.boldLabel);
                    GUI.color = origin;
                }
                else
                    GUILayout.Label ("minWaveSDKVersion is " + minWaveSDKVersion);
                if (numDoFHmd.Equals(""))
                {
                    var origin = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label ("NumDoFHmd is NULL!", EditorStyles.boldLabel);
                    GUI.color = origin;
                }
                else
                    GUILayout.Label ("NumDoFHmd is " + numDoFHmd);
                if (numDoFController.Equals(""))
                {
                    var origin = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label ("NumDoFController is NULL!", EditorStyles.boldLabel);
                    GUI.color = origin;
                }
                else
                    GUILayout.Label ("NumDoFController is " + numDoFController);
                if (numController.Equals(""))
                {
                    var origin = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label ("NumController is NULL!", EditorStyles.boldLabel);
                    GUI.color = origin;
                }
                else
                    GUILayout.Label ("NumController is " + numController);
                {
                    var origin = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label ("Please ensure that these metadata in your manifest matches the capabilities of your title. This metadata will affect how VIVEPORT store distributes and displays your title.", style);
                    GUI.color = origin;
                }

                if (GUILayout.Button("Guide Document for More Details"))
                {
                    Application.OpenURL("https://hub.vive.com/storage/app/doc/en-us/ConfigureAppCapabilities.html");
                }
                if (GUILayout.Button("Close"))
                {
                    this.Close();
                }
            }
        }
    }

    static void updateMetadata() {
        XmlDocument doc = new XmlDocument();
        doc.Load("Assets/Plugins/Android/AndroidManifest.xml");
        XmlNodeList metadataNodeList = doc.SelectNodes("/manifest/application/meta-data");
        if (metadataNodeList != null)
        {
            foreach (XmlNode metadataNode in metadataNodeList)
            {
                string name = metadataNode.Attributes["android:name"].Value;
                string value = metadataNode.Attributes["android:value"].Value;
                if (name.Equals("minWaveSDKVersion"))
                {
                    minWaveSDKVersion = string.Copy(value);
                }
                if (name.Equals("com.htc.vr.content.NumDoFHmd"))
                {
                    numDoFHmd = string.Copy(value);
                }
                if (name.Equals("com.htc.vr.content.NumDoFController"))
                {
                    numDoFController = string.Copy(value);
                }
                if (name.Equals("com.htc.vr.content.NumController"))
                {
                    numController = string.Copy(value);
                }
            }
        }
        initWaveVRAttributes = true;
        forceUpdateWaveVRAttributes = false;
    }

    private class CustomPreprocessor : IPreprocessBuild
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                if (Application.unityVersion.StartsWith("2017.") || Application.unityVersion.StartsWith("2018."))
                {
                    if (!File.Exists(aar2017) && File.Exists(aar2017+".skip"))
                    {
                        try
                        {
                            File.Move(aar2017+".skip", aar2017);
                            AssetDatabase.Refresh();
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Caught " + e.ToString() + " while moving \"" + aar2017 + ".skip" + "\" to \"" + aar2017 + "\"");
                        }
                    }
                    skip = aar5;
                }
                else
                {
                    if (!File.Exists(aar5) && File.Exists(aar5 + ".skip"))
                    {
                        try
                        {
                            File.Move(aar5 + ".skip", aar5);
                            AssetDatabase.Refresh();
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Caught " + e.ToString() + " while moving \"" + aar5 + ".skip" + "\" to \"" + aar5 + "\"");
                        }
                    }
                    skip = aar2017;
                }
                recovery = skip + ".skip";
                if (File.Exists(skip))
                {
                    //Debug.Log("Skip " + skip);
                    try
                    {
                        File.Move(skip, recovery);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Caught " + e.ToString() + " while moving \"" + skip + "\" to \"" + recovery +"\"");
                    }
                }
                EditorWindow tmp = EditorWindow.focusedWindow;
                WaveVRAttributesWindow window = (WaveVRAttributesWindow)EditorWindow.GetWindow(typeof(WaveVRAttributesWindow), true, "WaveVR Attributes", false);
                forceUpdateWaveVRAttributes = true;
                window.Show();
                tmp.Focus();
            }
        }
    }

    private class CustomPostprocessor : IPostprocessBuild
    {
        public int callbackOrder { get { return 0; } }
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                if (File.Exists(recovery) && !File.Exists(skip))
                {
                    //Debug.Log("Recover " + skip);
                    try
                    {
                        File.Move(recovery, skip);
                        AssetDatabase.Refresh();
                        skip = "";
                        recovery = "";
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Caught " + e.ToString() + " while moving \"" + recovery + "\" to \"" + skip + "\"");
                    }
                }
            }
        }
    }
}
#endif
