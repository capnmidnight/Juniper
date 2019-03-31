// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;

public class InstallSimulatorApk
{
    private static void GeneralSettings()
    {
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "2.0.0";
        PlayerSettings.companyName = "HTC Corp.";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
    }

    [UnityEditor.MenuItem("WaveVR/Simulator/Install Simulator Apk")]
    static void InstallSimulator()
    {
        UninstallSimulatorInner();
        InstallSimulatorInner();
    }

    [UnityEditor.MenuItem("WaveVR/Simulator/Start Simulator")]
    static void StartSimulator()
    {
        StartSimulatorInner();
    }

    public static void UninstallSimulatorInner()
    {
        try
        {
            Process myProcess = new Process();
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
            myProcess.StartInfo.Arguments = "/c adb uninstall com.htc.vr.simulator.server";
            //myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            int ExitCode = myProcess.ExitCode;
            UnityEngine.Debug.Log("Uninstall Simulator apk is done : " + ExitCode);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public static void InstallSimulatorInner()
    {
        try
        {
            Process myProcess = new Process();
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
            myProcess.StartInfo.Arguments = "/c adb install -r -d Assets/Plugins/WaveVR_SimulatorServer.apk";
            //myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            int ExitCode = myProcess.ExitCode;
            UnityEngine.Debug.Log("Install Simulator apk is done : " + ExitCode);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public static void StartSimulatorInner()
    {
        try
        {
            Process myProcess = new Process();
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
            myProcess.StartInfo.Arguments = "/c adb shell am start com.htc.vr.simulator.server/.MainActivity ";
            //myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            int ExitCode = myProcess.ExitCode;
            UnityEngine.Debug.Log("Start Simulator is done : " + ExitCode);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }
}
