using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Units;

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

        private int[] FOVs;

        private Vector2[][] fovTestAngles;
        private int[] lodLevelRequirements;

        private readonly Dictionary<string, Photosphere> photospheres = new Dictionary<string, Photosphere>();

        private Photosphere curSphere;

        public event CubemapImageNeeded CubemapNeeded;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> PhotosphereComplete;

        public event Action<Photosphere> PhotosphereReady;

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

        public Photosphere this[string key]
        {
            get
            {
                if (curSphere?.name != key)
                {
                    if (curSphere != null)
                    {
                        curSphere.Deactivate();
                    }

                    if (!photospheres.ContainsKey(key))
                    {
                        var photoGo = new GameObject(key);
                        photoGo.Deactivate();
                        var photo = photoGo.Ensure<Photosphere>().Value;
                        photo.CubemapNeeded += Photo_CubemapNeeded;
                        photo.ImageNeeded += Photo_ImageNeeded;
                        photo.Complete += Photo_Complete;
                        photo.Ready += Photo_Ready;
                        photo.SetDetailRequirements(FOVs, fovTestAngles, lodLevelRequirements);
                        photospheres.Add(key, photo);
                    }

                    curSphere = photospheres[key];
                    curSphere.Activate();
                }

                return curSphere;
            }
        }

        private Task<ImageData> Photo_CubemapNeeded(Photosphere source)
        {
            return CubemapNeeded?.Invoke(source);
        }

        private Task<ImageData> Photo_ImageNeeded(Photosphere source, int lodLevel, int heading, int pitch)
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
            obj.Ready -= Photo_Ready;
            obj.Complete -= Photo_Complete;
            PhotosphereComplete?.Invoke(obj);
        }
    }
}