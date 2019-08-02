#if UNITY_MODULES_AUDIO

using System;
using System.Collections;
using System.IO;

using Juniper.Data;
using Juniper.Progress;
using Juniper.Unity.Coroutines;

using UnityEngine;

namespace Juniper.Audio
{
    [Serializable]
    public class StreamableAudioClip : StreamableAsset<AudioClip>
    {
        public IEnumerator Load(Action<AudioClip> resolve, Action<Exception> reject, IProgress prog = null)
        {
            var info = new FileInfo(LoadPath);
            var ext = info.Extension.Substring(1).ToLowerInvariant();

            var format = AudioFormat.Unsupported;
            foreach (AudioFormat value in Enum.GetValues(typeof(AudioFormat)))
            {
                if (ext == AudioData.GetExtension(value))
                {
                    format = value;
                }
            }

            if (format == AudioFormat.Unsupported)
            {
                throw new InvalidOperationException($"{ext} is not a recognized audio format ({LoadPath}).");
            }
            else
            {
                var decoder = new Decoder(format);
                string mime = AudioData.GetContentType(format);

                var audioTask = StreamingAssets.GetStream(
                    Application.temporaryCachePath,
                    LoadPath,
                    mime,
                    prog);

                yield return new WaitForTask(audioTask);

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