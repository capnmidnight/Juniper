namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Image : MediaType
        {
            public static readonly Image Aces = new("aces");
            public static readonly Image Avci = new("avci");
            public static readonly Image Avcs = new("avcs");
            public static readonly Image Bmp = new("bmp", new string[] { "bmp" });
            public static readonly Image Cgm = new("cgm", new string[] { "cgm" });
            public static readonly Image Dicom_Rle = new("dicom-rle");
            public static readonly Image Emf = new("emf");
            public static readonly Image Example = new("example");
            public static readonly Image EXR = new("x-exr", new string[] { "exr" });
            public static readonly Image Fits = new("fits");
            public static readonly Image G3fax = new("g3fax", new string[] { "g3" });
            public static readonly Image Gif = new("gif", new string[] { "gif" });
            public static readonly Image Heic = new("heic");
            public static readonly Image Heic_Sequence = new("heic-sequence");
            public static readonly Image Heif = new("heif");
            public static readonly Image Heif_Sequence = new("heif-sequence");
            public static readonly Image Hej2k = new("hej2k");
            public static readonly Image Hsj2 = new("hsj2");
            public static readonly Image Ief = new("ief", new string[] { "ief" });
            public static readonly Image Jls = new("jls");
            public static readonly Image Jp2 = new("jp2");
            public static readonly Image Jpeg = new("jpeg", new string[] { "jpeg", "jpg", "jpe" });
            public static readonly Image Jph = new("jph");
            public static readonly Image Jphc = new("jphc");
            public static readonly Image Jpm = new("jpm");
            public static readonly Image Jpx = new("jpx");
            public static readonly Image Jxr = new("jxr");
            public static readonly Image JxrA = new("jxra");
            public static readonly Image JxrS = new("jxrs");
            public static readonly Image Jxs = new("jxs");
            public static readonly Image Jxsc = new("jxsc");
            public static readonly Image Jxsi = new("jxsi");
            public static readonly Image Jxss = new("jxss");
            public static readonly Image Ktx = new("ktx", new string[] { "ktx" });
            public static readonly Image Naplps = new("naplps");
            public static readonly Image Png = new("png", new string[] { "png" });
            public static readonly Image PrsBtif = new("prs.btif", new string[] { "btif" });
            public static readonly Image PrsPti = new("prs.pti");
            public static readonly Image Pwg_Raster = new("pwg-raster");
            public static readonly Image Raw = new("x-raw", new string[] { "raw" });
            public static readonly Image Sgi = new("sgi", new string[] { "sgi" });
            public static readonly Image SvgXml = new("svg+xml", new string[] { "svg", "svgz" });
            public static readonly Image T38 = new("t38");
            public static readonly Image Tiff = new("tiff", new string[] { "tiff", "tif" });
            public static readonly Image Tiff_Fx = new("tiff-fx");
            public static readonly Image VendorAdobePhotoshop = new("vnd.adobe.photoshop", new string[] { "psd" });
            public static readonly Image VendorAirzipAcceleratorAzv = new("vnd.airzip.accelerator.azv");
            public static readonly Image VendorCnsInf2 = new("vnd.cns.inf2");
            public static readonly Image VendorDeceGraphic = new("vnd.dece.graphic", new string[] { "uvi", "uvvi", "uvg", "uvvg" });
            public static readonly Image VendorDjvu = new("vnd.djvu", new string[] { "djvu", "djv" });
            public static readonly Image VendorDvbSubtitle = new("vnd.dvb.subtitle", new string[] { "sub" });
            public static readonly Image VendorDwg = new("vnd.dwg", new string[] { "dwg" });
            public static readonly Image VendorDxf = new("vnd.dxf", new string[] { "dxf" });
            public static readonly Image VendorFastbidsheet = new("vnd.fastbidsheet", new string[] { "fbs" });
            public static readonly Image VendorFpx = new("vnd.fpx", new string[] { "fpx" });
            public static readonly Image VendorFst = new("vnd.fst", new string[] { "fst" });
            public static readonly Image VendorFujixeroxEdmics_Mmr = new("vnd.fujixerox.edmics-mmr", new string[] { "mmr" });
            public static readonly Image VendorFujixeroxEdmics_Rlc = new("vnd.fujixerox.edmics-rlc", new string[] { "rlc" });
            public static readonly Image VendorGlobalgraphicsPgb = new("vnd.globalgraphics.pgb");
            public static readonly Image VendorMicrosoftIcon = new("vnd.microsoft.icon");
            public static readonly Image VendorMix = new("vnd.mix");
            public static readonly Image VendorMozillaApng = new("vnd.mozilla.apng");
            public static readonly Image VendorMs_Modi = new("vnd.ms-modi", new string[] { "mdi" });
            public static readonly Image VendorMs_Photo = new("vnd.ms-photo", new string[] { "wdp" });
            public static readonly Image VendorNet_Fpx = new("vnd.net-fpx", new string[] { "npx" });
            public static readonly Image VendorRadiance = new("vnd.radiance");
            public static readonly Image VendorSealedmediaSoftsealGif = new("vnd.sealedmedia.softseal.gif");
            public static readonly Image VendorSealedmediaSoftsealJpg = new("vnd.sealedmedia.softseal.jpg");
            public static readonly Image VendorSealedPng = new("vnd.sealed.png");
            public static readonly Image VendorSvf = new("vnd.svf");
            public static readonly Image VendorTencentTap = new("vnd.tencent.tap");
            public static readonly Image VendorValveSourceTexture = new("vnd.valve.source.texture");
            public static readonly Image VendorWapWbmp = new("vnd.wap.wbmp", new string[] { "wbmp" });
            public static readonly Image VendorXiff = new("vnd.xiff", new string[] { "xif" });
            public static readonly Image VendorZbrushPcx = new("vnd.zbrush.pcx");
            public static readonly Image Webp = new("webp", new string[] { "webp" });
            public static readonly Image Wmf = new("wmf");
            public static readonly Image X_3ds = new("x-3ds", new string[] { "3ds" });
            public static readonly Image X_Cmu_Raster = new("x-cmu-raster", new string[] { "ras" });
            public static readonly Image X_Cmx = new("x-cmx", new string[] { "cmx" });

            [System.Obsolete("DEPRECATED in favor of image/emf")]
            public static readonly Image X_Emf = new("x-emf");

            public static readonly Image X_Freehand = new("x-freehand", new string[] { "fh", "fhc", "fh4", "fh5", "fh7" });
            public static readonly Image X_Icon = new("x-icon", new string[] { "ico" });
            public static readonly Image X_Mrsid_Image = new("x-mrsid-image", new string[] { "sid" });
            public static readonly Image X_Pcx = new("x-pcx", new string[] { "pcx" });
            public static readonly Image X_Pict = new("x-pict", new string[] { "pic", "pct" });
            public static readonly Image X_Portable_Anymap = new("x-portable-anymap", new string[] { "pnm" });
            public static readonly Image X_Portable_Bitmap = new("x-portable-bitmap", new string[] { "pbm" });
            public static readonly Image X_Portable_Graymap = new("x-portable-graymap", new string[] { "pgm" });
            public static readonly Image X_Portable_Pixmap = new("x-portable-pixmap", new string[] { "ppm" });
            public static readonly Image X_Rgb = new("x-rgb", new string[] { "rgb" });
            public static readonly Image X_Tga = new("x-tga", new string[] { "tga" });

            [System.Obsolete("DEPRECATED in favor of image/wmf")]
            public static readonly Image X_Wmf = new("x-wmf");

            public static readonly Image X_Xbitmap = new("x-xbitmap", new string[] { "xbm" });
            public static readonly Image X_Xpixmap = new("x-xpixmap", new string[] { "xpm" });
            public static readonly Image X_Xwindowdump = new("x-xwindowdump", new string[] { "xwd" });

            public static new readonly Image[] Values = {
                Aces,
                Avci,
                Avcs,
                Bmp,
                Cgm,
                Dicom_Rle,
                Emf,
                Example,
                EXR,
                Fits,
                G3fax,
                Gif,
                Heic,
                Heic_Sequence,
                Heif,
                Heif_Sequence,
                Hej2k,
                Hsj2,
                Ief,
                Jls,
                Jp2,
                Jpeg,
                Jph,
                Jphc,
                Jpm,
                Jpx,
                Jxr,
                JxrA,
                JxrS,
                Jxs,
                Jxsc,
                Jxsi,
                Jxss,
                Ktx,
                Naplps,
                Png,
                PrsBtif,
                PrsPti,
                Pwg_Raster,
                Raw,
                Sgi,
                SvgXml,
                T38,
                Tiff,
                Tiff_Fx,
                VendorAdobePhotoshop,
                VendorAirzipAcceleratorAzv,
                VendorCnsInf2,
                VendorDeceGraphic,
                VendorDjvu,
                VendorDvbSubtitle,
                VendorDwg,
                VendorDxf,
                VendorFastbidsheet,
                VendorFpx,
                VendorFst,
                VendorFujixeroxEdmics_Mmr,
                VendorFujixeroxEdmics_Rlc,
                VendorGlobalgraphicsPgb,
                VendorMicrosoftIcon,
                VendorMix,
                VendorMozillaApng,
                VendorMs_Modi,
                VendorMs_Photo,
                VendorNet_Fpx,
                VendorRadiance,
                VendorSealedmediaSoftsealGif,
                VendorSealedmediaSoftsealJpg,
                VendorSealedPng,
                VendorSvf,
                VendorTencentTap,
                VendorValveSourceTexture,
                VendorWapWbmp,
                VendorXiff,
                VendorZbrushPcx,
                Webp,
                Wmf,
                X_3ds,
                X_Cmu_Raster,
                X_Cmx,
                X_Freehand,
                X_Icon,
                X_Mrsid_Image,
                X_Pcx,
                X_Pict,
                X_Portable_Anymap,
                X_Portable_Bitmap,
                X_Portable_Graymap,
                X_Portable_Pixmap,
                X_Rgb,
                X_Tga,
                X_Xbitmap,
                X_Xpixmap,
                X_Xwindowdump
            };
        }
    }
}
