using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Juniper.IO;
using Juniper.Progress;
using Juniper.Units;

using UnityEngine;

namespace Juniper.Imaging
{
    public delegate Task<Stream> PhotosphereJigImageNeeded(PhotosphereJig source, int fov, int heading, int pitch);

    public class PhotosphereJig : Photosphere
    {
        private static Material material;
        private const int MAX_REQUESTS = 4;

        private readonly Dictionary<int, Transform> detailContainerCache = new Dictionary<int, Transform>();
        private readonly Dictionary<int, Dictionary<int, Transform>> detailSliceContainerCache = new Dictionary<int, Dictionary<int, Transform>>();
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, Transform>>> detailSliceFrameContainerCache = new Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>();

        private PhotosphereManager mgr;
        private Avatar avatar;

        public event PhotosphereJigImageNeeded ImageStreamNeeded;
        public event Action<PhotosphereJig, bool> Complete;

        [Range(0, 1)]
        public float ProgressToComplete;
        private bool wasComplete;

        private TaskFactory mainThread;

        public override void Awake()
        {
            base.Awake();

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            mainThread = new TaskFactory(scheduler);

            mgr = this.FindClosest<PhotosphereManager>();
            Find.Any(out avatar);

            if (material == null)
            {
                material = new Material(Shader.Find("Unlit/Texture"));
            }

            Complete += PhotosphereJig_Complete;
        }

        private void OnComplete(bool captureCubemap)
        {
            wasComplete = true;
            ProgressToComplete = 1;
            Complete?.Invoke(this, captureCubemap);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            wasComplete = false;
            ProgressToComplete = 0;
        }

        private void PhotosphereJig_Complete(Photosphere arg1, bool arg2)
        {
            if (mgr != null
                && mgr.lodLevelRequirements != null
                && mgr.FOVs != null)
            {
                for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                {
                    var lodLevel = mgr.FOVs[f];
                    if (detailContainerCache.ContainsKey(lodLevel))
                    {
                        detailContainerCache[lodLevel].Deactivate();
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (!locked
                && !IsComplete
                && !trySkybox
                && cache != null
                && codec != null
                && mgr != null
                && mgr.lodLevelRequirements != null
                && mgr.lodLevelRequirements.Length > 0)
            {
                var isComplete = false;
                var isReady = IsReady;
                if (mgr.lodLevelRequirements != null)
                {
                    var totalCompleted = 0;
                    var totalNeeded = 0;
                    for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                    {
                        var t = DetailLevelCompleteCount(f);
                        var n = mgr.lodLevelRequirements[f];

                        if (f == 0)
                        {
                            this.Report(t, n);

                            if (t == n)
                            {
                                isReady = true;
                            }
                        }

                        if (t == n)
                        {
                            if (f == 0)
                            {
                                isReady = true;
                            }
                            else
                            {
                                detailContainerCache[mgr.FOVs[f - 1]].Deactivate();
                            }
                        }

                        totalCompleted += t;
                        totalNeeded += n;
                    }

                    ProgressToComplete = totalCompleted / (float)totalNeeded;

                    if (totalCompleted == totalNeeded)
                    {
                        isComplete = true;
                    }
                }

                if (!IsReady && isReady)
                {
                    OnReady();
                }

                if (isComplete)
                {
                    Debug.Log("Cubemap Complete");
                    OnComplete(true);
                }
                else if (!locked)
                {
                    locked = true;
                    this.Run(UpdatePhotosphere().AsCoroutine());
                }
            }
        }

        private async Task UpdatePhotosphere()
        {
            var euler = (Vector2)avatar.Head.rotation.eulerAngles;

            var numRequests = 0;
            for (var f = 0; f < mgr.FOVs.Length && numRequests < MAX_REQUESTS; ++f)
            {
                var fov = mgr.FOVs[f];
                var overlap = mgr.FOVs.Length - f;
                var radius = 10 * overlap + 50;
                var overlapFOV = fov + 2f * overlap;
                var scale = 2 * radius * Mathf.Tan(Degrees.Radians(overlapFOV / 2));

                var testAngles = mgr.fovTestAngles[f];
                for (var a = 0; a < testAngles.Length && numRequests < MAX_REQUESTS; ++a)
                {
                    var testAngle = euler + testAngles[a];
                    var heading = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.y / fov), 360);
                    var unityPitch = (int)Mathf.Repeat(fov * Mathf.Round(testAngle.x / fov), 360);
                    var pitch = -unityPitch;
                    if (unityPitch >= 270)
                    {
                        pitch += 360;
                    }
                    else if (unityPitch > 90)
                    {
                        pitch += 180;
                    }

                    if (90 < unityPitch && unityPitch < 270)
                    {
                        heading = (int)Mathf.Repeat(heading + 180, 360);
                    }

                    if (Mathf.Abs(pitch) == 90)
                    {
                        heading = 0;
                    }

                    var needLodLevel = !detailContainerCache.ContainsKey(fov);
                    var needSlice = needLodLevel || !detailSliceContainerCache[fov].ContainsKey(heading);
                    var needFrame = needSlice || !detailSliceFrameContainerCache[fov][heading].ContainsKey(pitch);
                    if (needLodLevel || needSlice || needFrame)
                    {
                        if (needLodLevel)
                        {
                            await mainThread.StartNew(() =>
                            {
                                var detail = new GameObject(fov.ToString()).transform;
                                detail.SetParent(transform, false);
                                detailContainerCache[fov] = detail;
                                detailSliceContainerCache[fov] = new Dictionary<int, Transform>();
                                detailSliceFrameContainerCache[fov] = new Dictionary<int, Dictionary<int, Transform>>();
                            });
                        }

                        var detailContainer = detailContainerCache[fov];
                        var sliceContainerCache = detailSliceContainerCache[fov];
                        var sliceFrameContainerCache = detailSliceFrameContainerCache[fov];

                        if (needSlice)
                        {
                            await mainThread.StartNew(() =>
                            {
                                var slice = new GameObject(heading.ToString()).transform;
                                slice.SetParent(detailContainer, false);
                                sliceContainerCache[heading] = slice;
                                sliceFrameContainerCache[heading] = new Dictionary<int, Transform>();
                            });
                        }

                        var sliceContainer = sliceContainerCache[heading];
                        var frameContainerCache = sliceFrameContainerCache[heading];

                        if (needFrame)
                        {
                            await mainThread.StartNew(() =>
                            {
                                var frame = new GameObject(pitch.ToString()).transform;
                                frame.rotation = Quaternion.Euler(-pitch, heading, 0);
                                frame.position = frame.rotation * (radius * Vector3.forward);
                                frame.SetParent(sliceContainer, false);
                                frameContainerCache[pitch] = frame;
                            });
                        }

                        var imageStreamTask = ImageStreamNeeded?.Invoke(this, (int)overlapFOV, heading, pitch);
                        if (imageStreamTask != null)
                        {
                            var imageStream = await imageStreamTask;
                            if (imageStream != null)
                            {
                                using (imageStream)
                                {
                                    await mainThread.StartNew(() =>
                                    {
                                        var image = codec.Deserialize(imageStream);
                                        if (image != null)
                                        {
                                            var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                            var renderer = frame.GetComponent<MeshRenderer>();
                                            var properties = new MaterialPropertyBlock();
                                            properties.SetTexture("_MainTex", image);
                                            renderer.SetMaterial(material);
                                            renderer.SetPropertyBlock(properties);
                                            frame.layer = LayerMask.NameToLayer(PHOTOSPHERE_LAYER);
                                            var frameContainer = frameContainerCache[pitch];
                                            frame.transform.SetParent(frameContainer, false);
                                            frame.transform.localScale = scale * Vector3.one;
                                        }
                                    });
                                }
                            }
                        }

                        // For the lowest detail level, we fill out all of the image angles immediately.
                        // For all other detail levels, we break out of testing angles and continue to
                        // next highest detail level.
                        if (f > 0)
                        {
                            break;
                        }
                    }
                }
            }

            locked = false;
        }

        public int DetailLevelCompleteCount(int f)
        {
            if (mgr == null || mgr.lodLevelRequirements == null)
            {
                return 0;
            }

            var lodLevel = mgr.FOVs[f];
            if (!detailSliceFrameContainerCache.ContainsKey(lodLevel))
            {
                return 0;
            }
            else
            {
                var frameCount = 0;
                foreach (var sliceFrameContainer in detailSliceFrameContainerCache[lodLevel])
                {
                    frameCount += sliceFrameContainer.Value.Count;
                }

                return frameCount;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (wasComplete)
                {
                    return true;
                }
                else if (mgr == null || mgr.lodLevelRequirements == null)
                {
                    return false;
                }
                else
                {
                    for (var f = 0; f < mgr.lodLevelRequirements.Length; ++f)
                    {
                        if (DetailLevelCompleteCount(f) != mgr.lodLevelRequirements[f])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
    }
}