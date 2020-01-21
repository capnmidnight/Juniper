using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.IO;
using Juniper.Sound;
using Juniper.Speech.Azure.CognitiveServices;

namespace Juniper
{
    public static class Program
    {
        private static SpeechGen form;
        private static SoundPlayer player;
        private static TextToSpeechStreamClient client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            // credentials
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(userProfile, "Projects", "DevKeys", "azure-speech.txt");
            var lines = File.ReadAllLines(keyFile);

            client = new TextToSpeechStreamClient(
                lines[1],
                lines[0],
                lines[2],
                new JsonFactory<Voice[]>(),
                AudioFormat.Raw24KHz16BitMonoPCM,
                new CachingStrategy()
                    .AppendLayer(new FileCacheLayer(new DirectoryInfo(Path
                        .Combine(userProfile, "Projects")))));

            using var p = player = new SoundPlayer();
            using var f = form = new SpeechGen
            {
                Voices = await client.GetVoicesAsync()
                    .ConfigureAwait(true)
            };

            form.GenerateSpeech += Form_GenerateSpeech;

            Application.Run(form);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            form.SetError(e.Exception);
        }

        private static void Form_GenerateSpeech(object sender, GenerateSpeechEventArgs e)
        {
            _ = Task.Run(async () =>
              {
                  try
                  {
                      client.OutputFormat = e.Format;
                      var newStream = await client.GetAudioDataStreamAsync(e.Text, e.Voice, e.RateChange, e.PitchChange)
                          .ConfigureAwait(true);
                      if (e.FileName is object)
                      {
                          SaveStream(newStream, e.FileName);
                      }
                      else
                      {
                          PlayStream(newStream);
                      }
                  }
                  catch (Exception exp)
                  {
                      form.SetError(exp);
                  }
              });
        }

        private static void SaveStream(Stream newStream, string fileName)
        {
            StopPlayback();

            using (var outFile = File.OpenWrite(fileName))
            using (newStream)
            {
                newStream.CopyTo(outFile);
            }
        }

        private static void PlayStream(Stream newStream)
        {
            StopPlayback();

            player.Stream = newStream;
            player.Play();
        }

        private static void StopPlayback()
        {
            if (player.Stream is object)
            {
                var stream = player.Stream;
                player.Stop();
                player.Stream = null;
                stream.Dispose();
            }
        }
    }
}
