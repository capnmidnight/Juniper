using System;
using System.IO;
using UnityEngine;

[Serializable]
public class BeatDetectorConfiguration
{
    public static string BaseMusicDirectory
    {
        get
        {
            return PathX.Combine(Application.dataPath, "StreamingAssets", "Music");
        }
    }

    public static string SongFile(string name)
    {
        return PathX.Combine(BaseMusicDirectory, name);
    }

    public static string MetadataFile(string name)
    {
        return PathX.Combine(BaseMusicDirectory, "Metadata", name + ".json");
    }

    [Header("FFT")]
    [Range(6, 13)]
    public int BufferMagnitude = 10;
    [Range(1, 100)]
    public int BandCount = 24;

    public BandPass[] BandPasses;

    [Header("Other")]
    [Range(10, 200)]
    public int RingBufferSize = 120;
    [Range(1, 32)]
    public int BlipDelayLen = 16;
    [Range(0, 1)]
    public float Sensitivity = 0.1f;

    [Range(1, 5)]
    public int BeatMultiplier = 1;
    [Range(1, 10)]
    public int BeatsPerMeasure = 4;
    public float DelaySeconds = 3;
    public float StaticBPM;
    public bool LockBPM;

    public void Copy(BeatDetectorConfiguration toCopy)
    {
        var t = typeof(BeatDetectorConfiguration);
        var fields = t.GetFields();
        foreach(var field in fields)
        {
            field.SetValue(this, field.GetValue(toCopy));
        }
    }

    public void LoadAudioMetadata(string resourceName)
    {
        if(File.Exists(resourceName))
        {
            var text = File.ReadAllText(resourceName);
            var config = JsonUtility.FromJson<BeatDetectorConfiguration>(text);
            foreach(var pass in config.BandPasses)
            {
                pass.Visualize = false;
            }
            this.Copy(config);
        }
        else
        {
            Debug.LogFormat("Couldn't find resource {0}", resourceName);
        }
    }
}