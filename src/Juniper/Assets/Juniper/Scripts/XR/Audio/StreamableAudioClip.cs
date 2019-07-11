#if UNITY_MODULES_AUDIO

using System;
using System.Collections;
using System.IO;

using Juniper.Data;
using Juniper.Progress;
using Juniper;

using UnityEngine;
using Juniper.Unity.Coroutines;

namespace Juniper.Audio
{
    [Serializable]
    public class StreamableAudioClip : StreamableAsset<AudioClip>
    {
        public IEnumerator Load(Action<AudioClip> resolve, Action<Exception> reject, IProgress prog = null)
        {
            var info = new FileInfo(LoadPath);
            var ext = info.Extension.ToLowerInvariant();
            string mime = null;
            Func<Stream, RawAudio> decoder;

            if (ext == ".mp3")
            {
                mime = "audio/mpeg";
                decoder = Decoder.DecodeMP3;
            }
            else if (ext == ".wav")
            {
                mime = "audio/wav";
                decoder = Decoder.DecodeWAV;
            }
            else if (ext == ".ogg")
            {
                mime = "audio/ogg";
                decoder = Decoder.DecodeVorbis;
            }
            else
            {
                throw new InvalidOperationException($"{ext} is not a recognized audio format ({LoadPath}).");
            }

            var audioTask = StreamingAssets.GetStream(
                Application.temporaryCachePath,
                LoadPath,
                mime,
                prog);

            yield return new WaitForTask(audioTask);

            var audio = decoder(audioTask.Result.Content);

            try
            {
                var clip = AudioClip.Create(
                    info.Name,
                    (int)audio.samples,
                    audio.channels,
                    audio.frequency,
                    true,
                    data => Decoder.FillBuffer(audio.stream, data));

                clip.LoadAudioData();
                prog?.Report(1);
                resolve(clip);
            }
            catch (Exception exp)
            {
                reject(exp);
            }
        }
    }
}

#endif