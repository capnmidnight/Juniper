using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

/*
 * Beat Detector script.
 * Add it as a component to the AudioSource that plays your music.
 * Set the Events for OnBeat and OnSpectrum (data for visualization).
 * Fiddle with the sliders (probably stay away from "Other", I don't really understand them yet).
 * Click the "Save settings" button to save a config file to the Resources directory.
 */
[Serializable]
public class OnSpectrumEventHandler : UnityEngine.Events.UnityEvent<float[]>
{

}

[RequireComponent(typeof(AudioSource))]
public class BeatDetector : MonoBehaviour
{
    private AudioSource Source;

    [Header("Events")]
    public UnityEvent OnNewSong;
    public UnityEvent OnBeat;
    public UnityEvent OnMeasure;
    public UnityEvent OnSongEnd;
    public OnSpectrumEventHandler OnSpectrum;

    [Range(1, 32)]
    public int VisualizationBandCount = 16;
    public bool VisualizeOutput;
    private bool wasVisualizeOutput;

    public BeatDetectorConfiguration Configuration;

    [Range(1, 100)]
    public int QueueLength = 10;
    [Range(60, 480)]
    public float TargetBPM = 120;
    public float MeasuredBPM;

    [Range(0, 1)]
    public float LerpBeat = 0.05f;
    private int beat = 0;
    private float lastBeat;
    private float lastTap;
    private readonly List<float> deltas = new List<float>();
    private int updatesSinceLastBeat = 0;
    private float[] frequencyDomainSlice;
    private float[] currentSlice;
    private float[] combinedSlice;
    private float[] visualizationBinnedSpectrum;
    private float[] binnedSpectrum;
    private float[] acVals;
    private float[] onsets;
    private float[] beatScores;
    private bool[] didBeat;
    private int ringBufferIndex = 0;
    private int[] blipDelay;
    private float tempo, lastT, nowT, diff, entries, sum;
    private float[] binnedAmplitude;

    // the spectrum of the previous step

    // Autocorrelation structure
    private readonly int maxlag = 100;

    // (in frames) largest lag to track
    private readonly float decay = 0.997f;
    private float lastBandWidth;

    // smoothing constant for running average

    private float[] delays;
    private float[] outputs;
    private int indx;
    private float[] bpms;
    private float[] rweight;
    private readonly float BPMMidPointWeight = 120f;
    private string lastResourceName;

    private bool CanPlay
    {
        get
        {
            return Source != null && Source.clip != null && Source.isPlaying;
        }
    }

    private void Start()
    {
        Source = GetComponent<AudioSource>();
        lastT = CurrentTime;
    }

    public string resourceName
    {
        get
        {
            if (Source != null && Source.clip != null)
            {
                return BeatDetectorConfiguration.MetadataFile(Source.clip.name);
            }
            else
            {
                return null;
            }
        }
    }

    public void NewSong()
    {
        deltas.Clear();
        lastTap = 0;
        lastBeat = 0;
        beat = 0;
        TotalBeats = 0;
        OnNewSong.Invoke();
        if (Source.clip != null)
        {
            Invoke("SongEnded", Source.clip.length);
        }
    }

    public void SongEnded()
    {
        Source.Stop();
        OnSongEnd.Invoke();
    }

    private void FollowBeat()
    {
        if (CurrentTime > Configuration.DelaySeconds)
        {
            if (Configuration.LockBPM)
            {
                TargetBPM = Configuration.StaticBPM;
            }
            else
            {
                TargetBPM = Mathf.Lerp(TargetBPM, MeasuredBPM, LerpBeat);
            }
            if (CurrentTime >= nextBeat)
            {
                ++beat;
                OnBeat.Invoke();
                if (beat == Configuration.BeatsPerMeasure)
                {
                    OnMeasure.Invoke();
                    beat = 0;
                }
                lastBeat = CurrentTime;
            }
        }
    }

    private void InitArrays()
    {
        if (resourceName != lastResourceName)
        {
            Configuration.LoadAudioMetadata(resourceName);
        }

        if (frequencyDomainSlice == null || frequencyDomainSlice.Length != BufferSize)
        {
            frequencyDomainSlice = new float[BufferSize];
            currentSlice = new float[BufferSize];
            combinedSlice = new float[BufferSize];
        }

        if (visualizationBinnedSpectrum == null || visualizationBinnedSpectrum.Length != VisualizationBandCount)
        {
            visualizationBinnedSpectrum = new float[VisualizationBandCount];
        }

        if (binnedSpectrum == null || binnedSpectrum.Length != Configuration.BandCount)
        {
            binnedSpectrum = new float[Configuration.BandCount];
            binnedAmplitude = new float[Configuration.BandCount];
        }

        if (onsets == null || Configuration.RingBufferSize != onsets.Length)
        {
            onsets = new float[Configuration.RingBufferSize];
            beatScores = new float[Configuration.RingBufferSize];
            didBeat = new bool[Configuration.RingBufferSize];
        }

        if (blipDelay == null || blipDelay.Length != Configuration.BlipDelayLen)
        {
            blipDelay = new int[Configuration.BlipDelayLen];
        }

        var recalcWeights = false;
        if (acVals == null || acVals.Length != maxlag)
        {
            acVals = new float[maxlag];
            delays = new float[maxlag];
            outputs = new float[maxlag];
            bpms = new float[maxlag];
            rweight = new float[maxlag];
            recalcWeights = true;
        }

        if (recalcWeights || BandWidth != lastBandWidth)
        {
            // calculate a log-lag gaussian weighting function, to prefer tempi around 120 bpm
            for (var i = 0; i < maxlag; ++i)
            {
                bpms[i] = BandWidth * 60.0f / i;
                // weighting is Gaussian on log-BPM axis, centered at wmidbpm, SD = woctavewidth octaves
                rweight[i] = Mathf.Exp(-0.5f * Mathf.Pow(
                    Mathf.Log(bpms[i] / BPMMidPointWeight) /
                    (Mathf.Log(2.0f) * BandWidth),
                    2.0f));
            }
        }
        lastBandWidth = BandWidth;
        lastResourceName = resourceName;
    }

    private void Update()
    {
        Progress = 0;
        if (CanPlay)
        {
            InitArrays();
            ringBufferIndex %= Configuration.RingBufferSize;
            Source.GetSpectrumData(frequencyDomainSlice, 0, FFTWindow.BlackmanHarris);
            var previewSpectrum = VisualizeOutput;
            for (var i = 0; i < Configuration.BandPasses.Length; ++i)
            {
                var bandPass = Configuration.BandPasses[i];
                if (VisualizeOutput && !wasVisualizeOutput)
                {
                    bandPass.Visualize = false;
                }

                previewSpectrum &= !bandPass.Visualize;

                if (bandPass.Visualize && !bandPass.wasVisualized)
                {
                    for (var j = 0; j < Configuration.BandPasses.Length; ++j)
                    {
                        if (j != i)
                        {
                            Configuration.BandPasses[j].Visualize = false;
                        }
                    }
                }
            }

            for (var i = 0; i < Configuration.BandPasses.Length; ++i)
            {
                var bandPass = Configuration.BandPasses[i];
                bandPass.wasVisualized = bandPass.Visualize;
            }

            if (!previewSpectrum)
            {
                VisualizeOutput = false;
            }
            wasVisualizeOutput = VisualizeOutput;

            if (previewSpectrum)
            {
                BinSpectrum(true, visualizationBinnedSpectrum);
            }

            Array.Copy(frequencyDomainSlice, currentSlice, BufferSize);
            for (var i = 0; i < BufferSize; ++i)
            {
                combinedSlice[i] = 0;
            }

            for (var i = 0; i < Configuration.BandPasses.Length; ++i)
            {
                var bandPass = Configuration.BandPasses[i];
                Array.Copy(currentSlice, frequencyDomainSlice, BufferSize);
                for (var j = 0; j < BufferSize; ++j)
                {
                    if (bandPass.Low <= j && j <= bandPass.High)
                    {
                        frequencyDomainSlice[j] *= bandPass.Multiplier / bandPass.Divider;
                        combinedSlice[j] = frequencyDomainSlice[j];
                    }
                    else
                    {
                        frequencyDomainSlice[j] = 0;
                    }
                }

                if (bandPass.Visualize)
                {
                    BinSpectrum(true, binnedSpectrum);
                    previewSpectrum = true;
                }
            }

            Array.Copy(combinedSlice, frequencyDomainSlice, BufferSize);
            BinSpectrum(!previewSpectrum, binnedSpectrum);


            // calculate the value of the onset function in this frame
            float onset = 0;
            for (var i = 0; i < Configuration.BandCount; i++)
            {
                var bandDecibels = 0.025f * Mathf.Max(-100.0f, 20.0f * Mathf.Log10(binnedSpectrum[i]) + 160); // dB value of this band
                var deltaBandDecibels = bandDecibels - binnedAmplitude[i]; // dB increment since last frame
                binnedAmplitude[i] = bandDecibels; // record this frame to use next time around
                onset += deltaBandDecibels; // onset function is the sum of dB increments
            }

            onsets[ringBufferIndex] = onset;

            // update autocorrelator and find peak lag = current tempo
            delays[indx] = onset;

            // update running autocorrelator values
            for (var i = 0; i < maxlag; ++i)
            {
                var delix = (indx - i + maxlag) % maxlag;
                outputs[i] += (1 - decay) * (delays[indx] * delays[delix] - outputs[i]);
            }

            indx = (indx + 1) % maxlag;
            // record largest value in (weighted) autocorrelation as it will be the tempo
            var aMax = 0.0f;
            var tempopd = 0;

            for (var i = 0; i < maxlag; ++i)
            {
                var acVal = Mathf.Sqrt(rweight[i] * outputs[i]);
                if (acVal > aMax)
                {
                    aMax = acVal;
                    tempopd = i;
                }
                // store in array backwards, so it displays right-to-left, in line with traces
                acVals[maxlag - 1 - i] = acVal;
            }

            // calculate DP-ish function to update the best-score function
            float maxBeatScore = float.MinValue;
            // consider all possible preceding beat times from 0.5 to 2.0 x current tempo period
            for (var i = tempopd / 2; i < Mathf.Min(Configuration.RingBufferSize, 2 * tempopd); ++i)
            {
                // objective function - this beat's cost + score to last beat + transition penalty
                var score = onset
                    + beatScores[(ringBufferIndex - i + Configuration.RingBufferSize) % Configuration.RingBufferSize]
                    - 100 * Configuration.Sensitivity * Mathf.Pow(Mathf.Log((float)i / tempopd), 2);
                // keep track of the best-scoring predecesor
                if (score > maxBeatScore)
                {
                    maxBeatScore = score;
                }
            }

            beatScores[ringBufferIndex] = maxBeatScore;
            // keep the smallest value in the score fn window as zero, by subtracing the min val
            var minBeatScore = beatScores[0];
            for (var i = 1; i < Configuration.RingBufferSize; ++i)
            {
                if (beatScores[i] < minBeatScore)
                {
                    minBeatScore = beatScores[i];
                }
            }
            for (var i = 0; i < Configuration.RingBufferSize; ++i)
            {
                beatScores[i] -= minBeatScore;
            }

            // find the largest value in the score fn window, to decide if we emit a blip
            maxBeatScore = beatScores[0];
            var maxBeatScoreIndex = 0;
            for (var i = 0; i < Configuration.RingBufferSize; ++i)
            {
                if (beatScores[i] > maxBeatScore)
                {
                    maxBeatScore = beatScores[i];
                    maxBeatScoreIndex = i;
                }
            }

            // dobeat array records where we actally place beats
            didBeat[ringBufferIndex] = false;  // default is no beat this frame
            ++updatesSinceLastBeat;
            // if current value is largest in the array, probably means we're on a beat
            if (maxBeatScoreIndex == ringBufferIndex && updatesSinceLastBeat > tempopd / 4)
            {
                tempo = TapTempo() * 1000;
                // make sure the most recent beat wasn't too recently
                ++TotalBeats;
                if (!Configuration.LockBPM)
                {
                    Configuration.StaticBPM = TotalBeats * 60 / CurrentTime;
                }
                var t = CurrentTime;
                var deltaT = t - lastTap;
                deltas.Add(deltaT);
                if (deltas.Count > QueueLength)
                {
                    deltas.RemoveAt(0);
                }
                var avg = 0f;
                foreach (var d in deltas)
                {
                    avg += d / deltas.Count;
                }
                MeasuredBPM = Configuration.BeatMultiplier * 60f / avg;
                lastTap = t;

                blipDelay[0] = 1;
                // record that we did actually mark a beat this frame
                didBeat[ringBufferIndex] = true;
                // reset counter of frames since last beat
                updatesSinceLastBeat = 0;
            }

            // update column index (for ring buffer)
            ++ringBufferIndex;

            if (Source.clip.length > 0)
            {
                Progress = Source.time / Source.clip.length;
            }
        }

        FollowBeat();
    }

    public int TotalBeats;

    public float Progress;

    private float nextBeat
    {
        get
        {
            return lastBeat + dBeat;
        }
    }

    private float dBeat
    {
        get
        {
            return 60 / TargetBPM;
        }
    }

    private int FrequencyIndex(int freq)
    {
        if (freq < BandWidth / 2)
        {
            return 0;
        }
        else if (freq > (SamplingRate - BandWidth) / 2)
        {
            return BufferSize / 2;
        }
        else
        {
            var fraction = (float)freq / SamplingRate;
            var i = (int)Mathf.Round(BufferSize * fraction);
            return i;
        }
    }

    private void BinSpectrum(bool emitEvent, float[] arr)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            float avg = 0;
            var lowFreq = (int)(SamplingRate / Mathf.Pow(2, arr.Length - i + 1));
            var hiFreq = (int)(SamplingRate / Mathf.Pow(2, arr.Length - i));
            var lowBound = FrequencyIndex(lowFreq);
            var hiBound = FrequencyIndex(hiFreq);
            for (var j = lowBound; j <= hiBound; j++)
            {
                avg += frequencyDomainSlice[j];
            }
            avg /= (hiBound - lowBound + 1);
            arr[i] = avg;
        }

        if (emitEvent && OnSpectrum != null)
        {
            OnSpectrum.Invoke(arr);
        }
    }

    private int SamplingRate
    {
        get
        {
            return Source.clip.frequency;
        }
    }


    private int _bufferSize, _lastBufferMagnitude = -1;

    private int BufferSize
    {
        get
        {
            if (Configuration.BufferMagnitude != _lastBufferMagnitude)
            {
                _bufferSize = (int)Mathf.Pow(2, Configuration.BufferMagnitude);
                _lastBufferMagnitude = Configuration.BufferMagnitude;
            }
            return _bufferSize;
        }
    }

    private float BandWidth
    {
        get
        {
            return (float)SamplingRate / BufferSize;
        }
    }

    private float TapTempo()
    {
        nowT = CurrentTime;
        diff = nowT - lastT;
        lastT = nowT;
        sum += diff;
        entries++;

        return sum / entries;
    }

    public float CurrentTime
    {
        get
        {
            return Source.time;
        }
    }
}
