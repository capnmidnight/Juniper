using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Juniper.Sound;
using Juniper.Speech.Azure.CognitiveServices;

namespace Juniper
{
    public partial class SpeechGen : Form
    {
        public event EventHandler<GenerateSpeechEventArgs> GenerateSpeech;

        public SpeechGen()
        {
            InitializeComponent();

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            saveFile.InitialDirectory = userProfile;
        }

        private Voice[] voices;

        private string[] regions;
        private string[] genders;
        private string[] voiceNames;

        public void SetError(Exception exception)
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action<Exception>(SetError), exception);
            }
            else
            {
                var head = exception;
                while (head is object)
                {
                    textBox.Text += head.Message;
                    textBox.Text += Environment.NewLine;
                    textBox.Text += head.StackTrace;

                    head = head.InnerException;
                }
            }
        }

        public IReadOnlyList<Voice> Voices => voices;

        public void SetVoices(Voice[] value)
        {
            voices = value;
            regions = (from voice in Voices
                       orderby voice.Locale
                       select voice.Locale)
                    .Distinct()
                    .ToArray();

            regionSelection.DataSource = regions;
        }

        private void RegionSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedRegion = (string)regionSelection.SelectedItem;
            genders = (from voice in Voices
                       where voice.Locale == selectedRegion
                       orderby voice.Gender
                       select voice.Gender)
                    .Distinct()
                    .ToArray();

            genderSelection.DataSource = genders;
        }

        private void GenderSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedRegion = (string)regionSelection.SelectedItem;
            var selectedGender = (string)genderSelection.SelectedItem;
            voiceNames = (from voice in Voices
                          where voice.Locale == selectedRegion
                            && voice.Gender == selectedGender
                          orderby voice.ShortName
                          select voice.ShortName)
                            .Distinct()
                            .ToArray();

            voiceNameSelection.DataSource = voiceNames;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Generate(null, AudioFormat.Riff24KHz16BitMonoPCM);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action(Save));
            }
            else
            {
                Save();
            }
        }

        private static readonly AudioFormat[] SUPPORTED_FORMATS =
        {
            AudioFormat.Audio24KHz160KbitrateMonoMP3,
            AudioFormat.Riff24KHz16BitMonoPCM
        };

        private void Save()
        {
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                foreach (var format in SUPPORTED_FORMATS)
                {
                    if (format.ContentType.Matches(saveFile.FileName))
                    {
                        Generate(saveFile.FileName, format);
                        return;
                    }
                }
            }
        }

        private void Generate(string fileName, AudioFormat format)
        {
            var selectedRegion = (string)regionSelection.SelectedItem;
            var selectedGender = (string)genderSelection.SelectedItem;
            var selectedVoiceName = (string)voiceNameSelection.SelectedItem;
            var selectedVoice = (from voice in Voices
                                 where voice.Locale == selectedRegion
                                   && voice.Gender == selectedGender
                                   && voice.ShortName == selectedVoiceName
                                 select voice)
                        .FirstOrDefault();

            GenerateSpeech?.Invoke(this, new GenerateSpeechEventArgs(
                selectedVoice,
                format,
                textBox.Text,
                rateChange.Value / 50f,
                pitchChange.Value / 50f,
                fileName));
        }
    }
}
