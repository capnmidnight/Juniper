using System;
using System.Linq;
using System.Windows.Forms;

using Juniper.Audio;
using Juniper.Azure.CognitiveServices;

namespace Juniper.Speech
{
    public partial class SpeechGen : Form
    {
        private readonly SaveFileDialog saveFile;

        public event EventHandler<GenerateSpeechEventArgs> GenerateSpeech;

        public SpeechGen()
        {
            InitializeComponent();

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            saveFile = new SaveFileDialog
            {
                Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav|All files (*.*)|*.*",
                FilterIndex = 1,
                InitialDirectory = userProfile,
                OverwritePrompt = true
            };

            Disposed += SpeechGen_Disposed;
        }

        private void SpeechGen_Disposed(object sender, EventArgs e)
        {
            saveFile.Dispose();
        }

        private Voice[] voices;

        private string[] regions;
        private string[] genders;
        private string[] voiceNames;

        public void SetError(Exception exception)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Exception>(SetError), exception);
            }
            else
            {
                var head = exception;
                while (head != null)
                {
                    textBox.Text += head.Message;
                    textBox.Text += Environment.NewLine;
                    textBox.Text += head.StackTrace;

                    head = head.InnerException;
                }
            }
        }

        public Voice[] Voices
        {
            get
            {
                return voices;
            }
            set
            {
                voices = value;
                regions = (from voice in Voices
                           orderby voice.Locale
                           select voice.Locale)
                        .Distinct()
                        .ToArray();

                regionSelection.DataSource = regions;
            }
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
                Invoke(new Action(Save));
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
