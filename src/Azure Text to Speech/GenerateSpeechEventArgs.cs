using System;
using Juniper.Audio;
using Juniper.Azure.CognitiveServices;

namespace Juniper.Speech
{
    public class GenerateSpeechEventArgs : EventArgs
    {
        public readonly Voice voice;
        public readonly AudioFormat format;
        public readonly string text;
        public readonly float rateChange;
        public readonly float pitchChange;
        public readonly string fileName;

        public GenerateSpeechEventArgs(Voice voice, AudioFormat format, string text, float rateChange, float pitchChange, string fileName)
        {
            this.voice = voice;
            this.format = format;
            this.text = text;
            this.rateChange = rateChange;
            this.pitchChange = pitchChange;
            this.fileName = fileName;
        }
    }
}
