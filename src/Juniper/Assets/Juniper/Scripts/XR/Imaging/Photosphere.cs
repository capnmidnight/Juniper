using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Display;
using Juniper.Progress;
using Juniper.Serialization;

using UnityEngine;

namespace Juniper.Imaging
{
    public delegate Task<ImageData> PhotosphereImageNeeded(Photosphere source, int fov, int heading, int pitch);

    public class Photosphere : MonoBehaviour
    {
        private readonly Dictionary<int, Transform> detailContainerCache = new Dictionary<int, Transform>();
        private readonly Dictionary<int, Dictionary<int, Transform>> detailSliceContainerCache = new Dictionary<int, Dictionary<int, Transform>>();
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, Transform>>> detailSliceFrameContainerCache = new Dictionary<int, Dictionary<int, Dictionary<int, Transform>>>();

        private int[] lodLevelRequirements;

        private bool wasComplete;
        private bool wasReady;

        public event PhotosphereImageNeeded ImageNeeded;

        public event Action<Photosphere> Complete;

        public event Action<Photosphere> Ready;

        public void SetDetailRequirements(int[] lodLevels)
        {
            lodLevelRequirements = lodLevels;
        }

        public bool IsDetailLevelComplete(int lodLevel)
        {
            if (lodLevelRequirements == null
                || !detailSliceFrameContainerCache.ContainsKey(lodLevel))
            {
                return false;
            }
            else
            {
                var frameCount = 0;
                foreach (var sliceFrameContainer in detailSliceFrameContainerCache[lodLevel])
                {
                    frameCount += sliceFrameContainer.Value.Count;
                }

                return frameCount == lodLevelRequirements[lodLevel];
            }
        }

        public bool IsComplete
        {
            get
            {
                if (detailSliceFrameContainerCache.Count == 0)
                {
                    return false;
                }
                else
                {
                    foreach (var lodLevel in detailSliceFrameContainerCache.Keys)
                    {
                        if (!IsDetailLevelComplete(lodLevel))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        public bool HasDetailLevel(int lodLevel, int heading, int pitch)
        {
            return detailContainerCache.ContainsKey(lodLevel)
                && detailSliceContainerCache[lodLevel].ContainsKey(heading)
                && detailSliceFrameContainerCache[lodLevel][heading].ContainsKey(pitch);
        }

        public bool DetailLevelNeeded(int lodLevel, int heading, int pitch)
        {
            return !HasDetailLevel(lodLevel, heading, pitch);
        }

        public IEnumerator CreateDetailLevel(int lodLevel, int heading, int pitch, float radius, float scale)
        {
            if (!detailContainerCache.ContainsKey(lodLevel))
            {
                var detail = new GameObject(lodLevel.ToString()).transform;
                detail.SetParent(transform, false);
                detailContainerCache[lodLevel] = detail;
                detailSliceContainerCache[lodLevel] = new Dictionary<int, Transform>();
                detailSliceFrameContainerCache[lodLevel] = new Dictionary<int, Dictionary<int, Transform>>();
            }

            var detailContainer = detailContainerCache[lodLevel];
            var sliceContainerCache = detailSliceContainerCache[lodLevel];
            var sliceFrameContainerCache = detailSliceFrameContainerCache[lodLevel];

            if (!sliceContainerCache.ContainsKey(heading))
            {
                var slice = new GameObject(heading.ToString()).transform;
                slice.SetParent(detailContainer, false);
                sliceContainerCache[heading] = slice;
                sliceFrameContainerCache[heading] = new Dictionary<int, Transform>();
            }

            var sliceContainer = sliceContainerCache[heading];
            var frameContainerCache = sliceFrameContainerCache[heading];

            if (!frameContainerCache.ContainsKey(pitch))
            {
                var frame = new GameObject(pitch.ToString()).transform;
                frame.rotation = Quaternion.Euler(-pitch, heading, 0);
                frame.position = frame.rotation * (radius * Vector3.forward);
                frame.SetParent(sliceContainer, false);
                frameContainerCache[pitch] = frame;
            }

            var frameContainer = frameContainerCache[pitch];

            var imageTask = ImageNeeded?.Invoke(this, lodLevel, heading, pitch);

            while (imageTask.IsRunning())
            {
                yield return null;
            }

            if (imageTask.IsCompleted)
            {
                var image = imageTask.Result;
                if (image != null)
                {
                    var texture = image.ToTexture();
                    var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    var renderer = frame.GetComponent<MeshRenderer>();
                    var material = new Material(Shader.Find("Unlit/Texture"));
                    material.SetTexture("_MainTex", texture);
                    renderer.SetMaterial(material);

                    frame.layer = LayerMask.NameToLayer("Photospheres");
                    frame.transform.SetParent(frameContainer, false);
                    frame.transform.localScale = scale * Vector3.one;
                }
            }

            if (!wasComplete)
            {
                if (IsComplete)
                {
                    wasComplete = true;
                    Complete?.Invoke(this);
                }
                else if (!wasReady && IsDetailLevelComplete(0))
                {
                    wasReady = true;
                    Ready?.Invoke(this);
                }
            }
        }

#if UNITY_EDITOR
        private bool cubemapLock;

        public void CaptureCubemap(IImageDecoder<ImageData> encoder)
        {
            if (!cubemapLock)
            {
                cubemapLock = true;
                StartCoroutine(CaptureCubemapCoroutine(encoder));
            }
        }

        private IEnumerator CaptureCubemapCoroutine(IImageDecoder<ImageData> encoder)
        {
            var fileName = Path.Combine("Assets", "StreamingAssets", $"{name}.jpeg");
            if (!File.Exists(fileName))
            {
                using (var prog = new UnityEditorProgressDialog("Saving cubemap " + name))
                {
                    var subProgs = prog.Split(
                        "Rendering cubemap",
                        "Copying cubemap faces",
                        "Concatenating faces",
                        "Saving image");

                    subProgs[0].Report(0);
                    const int dim = 2048;
                    var cubemap = new Cubemap(dim, TextureFormat.RGB24, false);
                    cubemap.Apply();

                    var curMask = DisplayManager.MainCamera.cullingMask;
                    DisplayManager.MainCamera.cullingMask = LayerMask.GetMask("Photosphere");

                    var curRotation = DisplayManager.MainCamera.transform.rotation;
                    DisplayManager.MainCamera.transform.rotation = Quaternion.identity;

                    DisplayManager.MainCamera.RenderToCubemap(cubemap, 63);

                    DisplayManager.MainCamera.cullingMask = curMask;
                    DisplayManager.MainCamera.transform.rotation = curRotation;
                    subProgs[0].Report(1);

                    var faces = new[]
                    {
                        CubemapFace.NegativeY,
                        CubemapFace.NegativeX,
                        CubemapFace.PositiveZ,
                        CubemapFace.PositiveX,
                        CubemapFace.NegativeZ,
                        CubemapFace.PositiveY
                    };

                    var images = new ImageData[faces.Length];

                    for (var f = 0; f < faces.Length; ++f)
                    {
                        subProgs[1].Report(f, faces.Length);
                        try
                        {
                            var pixels = cubemap.GetPixels(faces[f]);
                            var buf = new byte[pixels.Length * 3];
                            for (var y = 0; y < cubemap.height; ++y)
                            {
                                for (var x = 0; x < cubemap.width; ++x)
                                {
                                    var bufI = y * cubemap.width + x;
                                    var pixI = (cubemap.height - 1 - y) * cubemap.width + x;
                                    buf[bufI * 3 + 0] = (byte)(255 * pixels[pixI].r);
                                    buf[bufI * 3 + 1] = (byte)(255 * pixels[pixI].g);
                                    buf[bufI * 3 + 2] = (byte)(255 * pixels[pixI].b);
                                }
                            }

                            images[f] = new ImageData(DataSource.None, cubemap.width, cubemap.height, 3, ImageFormat.None, buf);
                        }
                        catch
                        {
                            cubemapLock = false;
                            throw;
                        }
                        subProgs[1].Report(f + 1, faces.Length);
                        yield return null;
                    }

                    var saveTask = Task.Run(() =>
                    {
                        try
                        {
                            var img = encoder.Concatenate(ImageData.CubeCross(images), subProgs[2]);
                            encoder.Save(fileName, img, subProgs[3]);
                        }
                        catch
                        {
                            cubemapLock = false;
                            throw;
                        }
                    });

                    while (saveTask.IsRunning())
                    {
                        yield return null;
                    }

                    if (saveTask.IsCompleted)
                    {
                        Debug.Log("Cubemap saved");
                    }
                    else if (saveTask.IsFaulted)
                    {
                        Debug.Log("Cubemap save error");
                    }
                    else
                    {
                        Debug.Log("Cubemap canceled");
                    }
                }
            }

            cubemapLock = false;
        }

#endif
    }
}