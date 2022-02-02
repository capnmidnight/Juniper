using System;

using Juniper.Sound;
using Juniper.Speech.Azure.CognitiveServices;

namespace Juniper
{
    public class GenerateSpeechEventArgs : EventArgs
    {
        public Voice Voice { get; }
        public AudioFormat Format { get; }
        public string Text { get; }
        public float RateChange { get; }
        public float PitchChange { get; }
        public string FileName { get; }
        public bool DecodeRequired { get; }

        public GenerateSpeechEventArgs(Voice voice, AudioFormat format, string text, float rateChange, float pitchChange, string fileName, bool decodeRequired)
        {
            Voice = voice;
            Format = format;
            Text = text;
            RateChange = rateChange;
            PitchChange = pitchChange;
            FileName = fileName;
            DecodeRequired = decodeRequired;
        }
    }
}
