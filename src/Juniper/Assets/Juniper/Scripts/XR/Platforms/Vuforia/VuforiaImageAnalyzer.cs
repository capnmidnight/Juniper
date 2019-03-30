#if VUFORIA
using Juniper.Data;

using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace Juniper.Unity.Imaging
{
    /// <summary>
    /// Basic scaffolding around performing image analysis on Vuforia's image feed. This typically
    /// only works on Android.
    /// </summary>
    public class VuforiaImageAnalyzer : AbstractBackgroundRepeatingThread
    {
        /// <summary>
        /// A mapping from Vuforia's pixel formats to functions that can retrieve an indexed pixel in
        /// those formats.
        /// </summary>
        static Dictionary<Image.PIXEL_FORMAT, GetPixelFromBuffer> PixelConverters = new Dictionary<Image.PIXEL_FORMAT, GetPixelFromBuffer>
        {
            { Image.PIXEL_FORMAT.GRAYSCALE, ColorExt.GetGrayscalePixel },
            { Image.PIXEL_FORMAT.RGB565, ColorExt.GetRGB565Pixel },
            { Image.PIXEL_FORMAT.RGB888, ColorExt.GetRGB888Pixel },
            { Image.PIXEL_FORMAT.RGBA8888, ColorExt.GetRGB888Pixel },
            { Image.PIXEL_FORMAT.YUV, ColorExt.GetYUVPixel },
            { Image.PIXEL_FORMAT.UNKNOWN_FORMAT, ColorExt.GetRGB888Pixel}
        };

        /// <summary>
        /// A queue of formats to try. Not all formats will work on all systems, so we search through
        /// the queue until we find one that works.
        /// </summary>
        static Queue<Image.PIXEL_FORMAT> attemptFormats = new Queue<Image.PIXEL_FORMAT>(new[]
        {
            Image.PIXEL_FORMAT.RGB888,
            Image.PIXEL_FORMAT.RGBA8888,
            Image.PIXEL_FORMAT.YUV,
            Image.PIXEL_FORMAT.RGB565,
            Image.PIXEL_FORMAT.GRAYSCALE,
            Image.PIXEL_FORMAT.UNKNOWN_FORMAT
        });

        /// <summary>
        /// The current image format. If there are no more image formats left to try, the value will
        /// be null.
        /// </summary>
        /// <value>The image format.</value>
        public static Image.PIXEL_FORMAT? ImageFormat
        {
            get
            {
                if (attemptFormats.Count > 0)
                {
                    return attemptFormats.Peek();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The current pixel retriever.
        /// </summary>
        GetPixelFromBuffer pixelGetter;

        /// <summary>
        /// The image format used in the previous attempt. Useful for
        /// </summary>
        Image.PIXEL_FORMAT? lastFormat;

        /// <summary>
        /// The function passed in to perform image analysis.
        /// </summary>
        public Action<Image, GetPixelFromBuffer> imageAnalyzer;

        /// <summary>
        /// Checks to see if Vuforia is still running and either starts up the analysis thread if
        /// Vuforia is running but the thread isn't, or shuts it down if the thread is running but
        /// Vuforia isn't.
        /// </summary>
        void Update()
        {
            if (imageAnalyzer != null)
            {
                if (VuforiaBehaviour.Instance.enabled && !Running)
                {
                    Begin();
                }
                else if (!VuforiaBehaviour.Instance.enabled && Running)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// attempts to setup the right image format, retrieve images from Vuforia, and pass them on
        /// for processing.
        /// </summary>
        protected override void Loop()
        {
            if (ImageFormat != lastFormat)
            {
                while (ImageFormat != null && !CameraDevice.Instance.SetFrameFormat(ImageFormat.Value, true))
                {
                    attemptFormats.Dequeue();
                }
                lastFormat = ImageFormat;
                pixelGetter = null;
                if (ImageFormat != null && PixelConverters.ContainsKey(ImageFormat.Value))
                {
                    pixelGetter = PixelConverters[ImageFormat.Value];
                }
            }

            if (ImageFormat != null)
            {
                try
                {
                    var image = CameraDevice.Instance.GetCameraImage(ImageFormat.Value);
                    if (image != null)
                    {
                        imageAnalyzer(image, pixelGetter);
                    }
                }
                catch (Exception exp)
                {
                    ScreenDebugger.Print(exp, "VuforiaImageAnalyzer");
                }
            }
        }
    }
}
#endif