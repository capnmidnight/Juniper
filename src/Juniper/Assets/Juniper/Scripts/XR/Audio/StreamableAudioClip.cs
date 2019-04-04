#if UNITY_MODULES_AUDIO

using System;
using System.Collections;
using System.IO;

using Juniper.Data;
using Juniper.Progress;
using Juniper.Unity.Data;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;
using UnityEngine;

namespace Juniper.Unity.Audio
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
                decoder = Juniper.Data.Audio.DecodeMP3;
            }
            else if (ext == ".wav")
            {
                mime = "audio/wav";
                decoder = Juniper.Data.Audio.DecodeWAV;
            }
            else if (ext == ".ogg")
            {
                mime = "audio/ogg";
                decoder = Juniper.Data.Audio.DecodeVorbis;
            }
            else
            {
                throw new InvalidOperationException($"{ext} is not a recognized audio format ({LoadPath}).");
            }

            RawAudio? audio = null;
            StreamingAssets.GetStream(
                Application.temporaryCachePath,
                LoadPath,
                mime,
                stream => audio = decoder(stream),
                reject,
                prog);

            yield return new WaitUntil(() => audio != null);

            try
            {
                var clip = AudioClip.Create(
                    info.Name,
                    (int)audio.Value.samples,
                    audio.Value.channels,
                    audio.Value.frequency,
                    true,
                    data => Juniper.Data.Audio.FillBuffer(audio.Value.stream, data));

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