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
        private const int MAX_REQUESTS = 4;

        private static Vector2[] MakeTestAngles(float fov)
        {
            var list = new List<Vector2>();
            var minX = Mathf.Floor(-180 / fov) * fov;
            var maxX = Mathf.Ceil(180 / fov) * fov;
            var minY = Mathf.Floor(-90 / fov) * fov;
            var maxY = Mathf.Ceil(90 / fov) * fov;
            for (var ax = minX; ax <= maxX; ax += fov)
            {
                for (var ay = minY; ay <= maxY; ay += fov)
                {
                    list.Add(new Vector2(ax, ay));
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
        private Avatar avatar;

        private bool locked;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> PhotosphereComplete;

        public event Action<Photosphere> PhotosphereReady;

        public void Awake()
        {
            avatar = ComponentExt.FindAny<Avatar>();
        }

        public void AddDetailLevels(float[] fovs)
        {
            FOVs = new int[fovs.Length];
            fovTestAngles = new Vector2[fovs.Length][];
            for (int f = 0; f < fovs.Length; ++f)
            {
                FOVs[f] = (int)fovs[f];
                fovTestAngles[f] = MakeTestAngles(fovs[f]);
            }

            lodLevelRequirements = fovTestAngles
                .Select(a => a.Length)
                .ToArray();
        }

        private void CalcRequestParameters(Vector2 testAngle, int f, out int radius, out float scale, out int requestHeading, out int requestPitch)
        {
            var overlap = FOVs.Length - f;
            radius = 5 * overlap + 50;
            if (f == 0)
            {
                overlap = 0;
            }

            var subFOV = FOVs[f];
            var fov = subFOV + 2f * overlap;
            scale = 2 * radius * Mathf.Tan(Degrees.Radians(fov / 2));
            requestHeading = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.y / fov), 360);
            var unityPitch = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.x / fov), 360);
            requestPitch = -unityPitch;
            if (unityPitch >= 270)
            {
                requestPitch += 360;
            }
            else if (unityPitch > 90)
            {
                requestPitch += 180;
            }

            if (90 < unityPitch && unityPitch < 270)
            {
                requestHeading = (int)Mathf.Repeat(requestHeading + 180, 360);
            }
        }

        public int Count { get { return photospheres.Count; } }

        public Photosphere this[string key]
        {
            get
            {
                if (!photospheres.ContainsKey(key))
                {
                    var photo = new GameObject(key).Ensure<Photosphere>().Value;
                    photo.ImageNeeded += OnImageNeeded;
                    photo.Complete += Photo_Complete;
                    photo.Ready += Photo_Ready;
                    photo.SetDetailRequirements(lodLevelRequirements);
                    photospheres.Add(key, photo);
                }

                var sphere = photospheres[key];
                if (!sphere.IsComplete)
                {
                    curSphere = sphere;
                }

                return sphere;
            }
        }

        private void Photo_Ready(Photosphere obj)
        {
            PhotosphereReady?.Invoke(obj);
        }

        private void Photo_Complete(Photosphere obj)
        {
            obj.ImageNeeded -= OnImageNeeded;
            obj.Ready -= Photo_Ready;
            obj.Complete -= Photo_Complete;
            PhotosphereComplete?.Invoke(obj);
        }

        private Task<ImageData> OnImageNeeded(Photosphere source, int lodLevel, int heading, int pitch)
        {
            return ImageNeeded?.Invoke(source, FOVs[lodLevel], heading, pitch);
        }

        public void Update()
        {
            if (curSphere?.IsComplete == false && !locked)
            {
                locked = true;
                StartCoroutine(UpdateSphereCoroutine());
            }
        }

        private IEnumerator UpdateSphereCoroutine()
        {
            var euler = (Vector2)avatar.Head.rotation.eulerAngles;

            var numRequests = 0;
            for (var f = 0; f < FOVs.Length && numRequests < MAX_REQUESTS; ++f)
            {
                var testAngles = fovTestAngles[f];
                for (var a = 0; a < testAngles.Length && numRequests < MAX_REQUESTS; ++a)
                {
                    var testAngle = euler + testAngles[a];
                    CalcRequestParameters(testAngle, f, out var radius, out var scale, out var heading, out var pitch);
                    if (curSphere.DetailLevelNeeded(f, heading, pitch))
                    {
                        yield return curSphere.CreateDetailLevel(f, heading, pitch, radius, scale);
                    }
                }
            }

            locked = false;
        }
    }
}