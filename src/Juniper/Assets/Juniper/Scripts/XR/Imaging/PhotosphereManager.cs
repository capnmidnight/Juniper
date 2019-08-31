using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Imaging.Unity;

using UnityEngine;

namespace Juniper.Imaging
{
    public class PhotosphereManager : MonoBehaviour
    {
        private static Vector2[] MakeTestAngles(float fov, bool allAngles)
        {
            var list = new List<Vector2>();

            for (var ay = -180f; ay < 180; ay += fov)
            {
                for (var ax = -90f; ax <= 90f; ax += fov)
                {
                    if (Mathf.Abs(ax) != 90 || ay == 0 || allAngles)
                    {
                        list.Add(new Vector2(ay, ax));
                    }
                }
            }

            return list
                .Distinct()
                .OrderBy(x => x.magnitude)
                .ToArray();
        }

        internal int[] FOVs;
        internal Vector2[][] fovTestAngles;
        internal int[] lodLevelRequirements;


        private readonly Dictionary<string, Photosphere> photospheres = new Dictionary<string, Photosphere>();

        private Photosphere curSphere;

        public event CubemapImageNeeded CubemapNeeded;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> PhotosphereComplete;

        public event Action<Photosphere> PhotosphereReady;

        public UnityTextureCodec codec;
        private UnityTextureCodec lastCodec;

        public void SetDetailLevels(float[] fovs)
        {
            FOVs = new int[fovs.Length];
            fovTestAngles = new Vector2[fovs.Length][];
            var requiredAngles = new Vector2[fovs.Length][];
            for (int f = 0; f < fovs.Length; ++f)
            {
                FOVs[f] = (int)fovs[f];
                fovTestAngles[f] = MakeTestAngles(fovs[f], true);
                requiredAngles[f] = MakeTestAngles(fovs[f], false);
            }

            lodLevelRequirements = requiredAngles
                .Select(a => a.Length)
                .ToArray();
        }

        public int Count { get { return photospheres.Count; } }

        public T GetPhotosphere<T>(string key) where T : Photosphere
        {
            if (curSphere == null || curSphere.Key != key)
            {
                if(curSphere != null)
                {
                    curSphere.enabled = false;
                }

                if (!photospheres.ContainsKey(key))
                {
                    var photo = CreatePhotosphere<Photosphere>(key);
                    photospheres.Add(key, photo);
                }

                curSphere = photospheres[key];
            }

            return (T)curSphere;
        }

        public void Awake()
        {
            var existing = GetComponentsInChildren<Photosphere>();
            foreach(var photo in existing)
            {
                photo.Ready += Photo_Ready;
                photo.Complete += Photo_Complete;
                photo.CubemapNeeded += Photo_CubemapNeeded;
                photo.ImageNeeded += Photo_ImageNeeded;
                photo.enabled = false;
                photo.OnDisable();
                photospheres.Add(photo.Key, photo);
            }
        }

        public T CreatePhotosphere<T>(string key)
            where T : Photosphere
        {
            var photoGo = new GameObject(key);
            photoGo.Deactivate();
            photoGo.transform.SetParent(transform, true);
            var photo = photoGo.Ensure<T>().Value;
            photo.Key = key;
            photo.CubemapNeeded += Photo_CubemapNeeded;
            photo.ImageNeeded += Photo_ImageNeeded;
            photo.Complete += Photo_Complete;
            photo.Ready += Photo_Ready;
            photo.codec = codec;
            return photo;
        }

        private string Photo_CubemapNeeded(Photosphere source)
        {
            return CubemapNeeded?.Invoke(source);
        }

        private Task<Stream> Photo_ImageNeeded(Photosphere source, int lodLevel, int heading, int pitch)
        {
            return ImageNeeded?.Invoke(source, lodLevel, heading, pitch);
        }

        private void Photo_Ready(Photosphere obj)
        {
            PhotosphereReady?.Invoke(obj);
        }

        private void Photo_Complete(Photosphere obj)
        {
            obj.CubemapNeeded -= Photo_CubemapNeeded;
            obj.ImageNeeded -= Photo_ImageNeeded;
            PhotosphereComplete?.Invoke(obj);
        }

        public void Update()
        {
            if(codec != lastCodec)
            {
                lastCodec = codec;
                foreach (var photo in photospheres.Values)
                {
                    photo.codec = codec;
                }
            }
        }
    }
}