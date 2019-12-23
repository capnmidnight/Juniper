using System;

using Juniper.Audio;
using Juniper.Azure.CognitiveServices;

namespace Juniper.Speech
{
    public class GenerateSpeechEventArgs : EventArgs
    {
        public Voice Voice { get; }

        public AudioFormat Format { get; }

        public string Text { get; }

        public float RateChange { get; }

        public float PitchChange { get; }

        public string FileName { get; }

        public GenerateSpeechEventArgs(Voice voice, AudioFormat format, string text, float rateChange, float pitchChange, string fileName)
        {
            Voice = voice;
            Format = format;
            Text = text;
            RateChange = rateChange;
            PitchChange = pitchChange;
            FileName = fileName;
        }
    }
}
