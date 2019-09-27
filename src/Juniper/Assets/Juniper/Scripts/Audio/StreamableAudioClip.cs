#if UNITY_MODULES_AUDIO

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Audio.NAudio;
using Juniper.Data;
using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Audio
{
    [Serializable]
    public class StreamableAudioClip : StreamableAsset<AudioClip>
    {
        public IEnumerator Load(Action<AudioClip> resolve, Action<Exception> reject)
        {
            return Load(resolve, reject);
        }

        public IEnumerator Load(Action<AudioClip> resolve, Action<Exception> reject, IProgress prog)
        {
            var info = new FileInfo(LoadPath);
            var ext = info.Extension.Substring(1).ToLowerInvariant();
            var format = MediaType.LookupExtension(ext) as MediaType.Audio;

            if (Array.IndexOf(NAudioAudioDataDecoder.SupportedFormats, format) < 0)
            {
                throw new InvalidOperationException($"{ext} is not a recognized audio format ({LoadPath}).");
            }
            else
            {
                var decoder = new NAudioAudioDataDecoder(format);

                var audioTask = StreamingAssets.GetStream(
                    Application.temporaryCachePath,
                    LoadPath,
                    format,
                    prog);

                yield return audioTask.AsCoroutine();

                var audio = decoder.Deserialize(audioTask.Result.Content);

                try
                {
                    var clip = AudioClip.Create(
                        info.Name,
                        (int)audio.samples,
                        audio.channels,
                        audio.frequency,
                        true,
                        data => AudioData.FillBuffer(audio.stream, data));

                    clip.LoadAudioData();
                    resolve(clip);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    reject(exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}

#endif