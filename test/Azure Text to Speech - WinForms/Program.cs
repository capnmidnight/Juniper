using Juniper.IO;
using Juniper.Sound;
using Juniper.Speech.Azure.CognitiveServices;

using System;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Juniper
{
    public static class Program
    {
        private static SpeechGen form;
        private static SoundPlayer player;
        private static TextToSpeechClient client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var http = new HttpClient(new HttpClientHandler
            {
                UseCookies = false
            });

            // credentials
            var userProfile = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            var projectsDir = userProfile.CD("Projects");
            var keyFile = projectsDir.CD("DevKeys").Touch("azure-speech.txt");
            var lines = File.ReadAllLines(keyFile.FullName);
            client = new TextToSpeechClient(
                http,
                lines[0],
                lines[1],
                lines[2],
                new JsonFactory<Voice[]>(),
                new NAudioAudioDataDecoder(),
                new CachingStrategy
                {
                    new FileCacheLayer(projectsDir)
                });

            using var p = player = new SoundPlayer();
            using var f = form = new SpeechGen();
            form.SetVoices(client.GetVoicesAsync().Result);

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
                      if (e.FileName is not null)
                      {
                          var newStream = await client.GetAudioDataStreamAsync(e.Format, e.Text, e.Voice, e.RateChange, e.PitchChange)
                              .ConfigureAwait(true);
                          SaveStream(newStream, e.FileName);
                      }
                      else if (!e.DecodeRequired)
                      {
                          var newStream = await client.GetAudioDataStreamAsync(e.Format, e.Text, e.Voice, e.RateChange, e.PitchChange)
                              .ConfigureAwait(true);
                          PlayStream(newStream);
                      }
                      else
                      {
                          var newStream = await client.GetWaveAudioAsync(e.Format, e.Text, e.Voice, e.RateChange, e.PitchChange)
                              .ConfigureAwait(true);
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
                outFile.Flush();
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
            if (player.Stream is not null)
            {
                var stream = player.Stream;
                player.Stop();
                player.Stream = null;
                stream.Dispose();
            }
        }
    }
}
