namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Image : MediaType
        {
            public static readonly Image Aces = new Image("aces");
            public static readonly Image Avci = new Image("avci");
            public static readonly Image Avcs = new Image("avcs");
            public static readonly Image Bmp = new Image("bmp", new string[] { "bmp" });
            public static readonly Image Cgm = new Image("cgm", new string[] { "cgm" });
            public static readonly Image Dicom_Rle = new Image("dicom-rle");
            public static readonly Image Emf = new Image("emf");
            public static readonly Image Example = new Image("example");
            public static readonly Image EXR = new Image("x-exr", new string[] { "exr" });
            public static readonly Image Fits = new Image("fits");
            public static readonly Image G3fax = new Image("g3fax", new string[] { "g3" });
            public static readonly Image Gif = new Image("gif", new string[] { "gif" });
            public static readonly Image Heic = new Image("heic");
            public static readonly Image Heic_Sequence = new Image("heic-sequence");
            public static readonly Image Heif = new Image("heif");
            public static readonly Image Heif_Sequence = new Image("heif-sequence");
            public static readonly Image Hej2k = new Image("hej2k");
            public static readonly Image Hsj2 = new Image("hsj2");
            public static readonly Image Ief = new Image("ief", new string[] { "ief" });
            public static readonly Image Jls = new Image("jls");
            public static readonly Image Jp2 = new Image("jp2");
            public static readonly Image Jpeg = new Image("jpeg", new string[] { "jpeg", "jpg", "jpe" });
            public static readonly Image Jph = new Image("jph");
            public static readonly Image Jphc = new Image("jphc");
            public static readonly Image Jpm = new Image("jpm");
            public static readonly Image Jpx = new Image("jpx");
            public static readonly Image Jxr = new Image("jxr");
            public static readonly Image JxrA = new Image("jxra");
            public static readonly Image JxrS = new Image("jxrs");
            public static readonly Image Jxs = new Image("jxs");
            public static readonly Image Jxsc = new Image("jxsc");
            public static readonly Image Jxsi = new Image("jxsi");
            public static readonly Image Jxss = new Image("jxss");
            public static readonly Image Ktx = new Image("ktx", new string[] { "ktx" });
            public static readonly Image Naplps = new Image("naplps");
            public static readonly Image Png = new Image("png", new string[] { "png" });
            public static readonly Image PrsBtif = new Image("prs.btif", new string[] { "btif" });
            public static readonly Image PrsPti = new Image("prs.pti");
            public static readonly Image Pwg_Raster = new Image("pwg-raster");
            public static readonly Image Raw = new Image("x-raw", new string[] { "raw" });
            public static readonly Image Sgi = new Image("sgi", new string[] { "sgi" });
            public static readonly Image SvgXml = new Image("svg+xml", new string[] { "svg", "svgz" });
            public static readonly Image T38 = new Image("t38");
            public static readonly Image Tiff = new Image("tiff", new string[] { "tiff", "tif" });
            public static readonly Image Tiff_Fx = new Image("tiff-fx");
            public static readonly Image VendorAdobePhotoshop = new Image("vnd.adobe.photoshop", new string[] { "psd" });
            public static readonly Image VendorAirzipAcceleratorAzv = new Image("vnd.airzip.accelerator.azv");
            public static readonly Image VendorCnsInf2 = new Image("vnd.cns.inf2");
            public static readonly Image VendorDeceGraphic = new Image("vnd.dece.graphic", new string[] { "uvi", "uvvi", "uvg", "uvvg" });
            public static readonly Image VendorDjvu = new Image("vnd.djvu", new string[] { "djvu", "djv" });
            public static readonly Image VendorDvbSubtitle = new Image("vnd.dvb.subtitle", new string[] { "sub" });
            public static readonly Image VendorDwg = new Image("vnd.dwg", new string[] { "dwg" });
            public static readonly Image VendorDxf = new Image("vnd.dxf", new string[] { "dxf" });
            public static readonly Image VendorFastbidsheet = new Image("vnd.fastbidsheet", new string[] { "fbs" });
            public static readonly Image VendorFpx = new Image("vnd.fpx", new string[] { "fpx" });
            public static readonly Image VendorFst = new Image("vnd.fst", new string[] { "fst" });
            public static readonly Image VendorFujixeroxEdmics_Mmr = new Image("vnd.fujixerox.edmics-mmr", new string[] { "mmr" });
            public static readonly Image VendorFujixeroxEdmics_Rlc = new Image("vnd.fujixerox.edmics-rlc", new string[] { "rlc" });
            public static readonly Image VendorGlobalgraphicsPgb = new Image("vnd.globalgraphics.pgb");
            public static readonly Image VendorMicrosoftIcon = new Image("vnd.microsoft.icon");
            public static readonly Image VendorMix = new Image("vnd.mix");
            public static readonly Image VendorMozillaApng = new Image("vnd.mozilla.apng");
            public static readonly Image VendorMs_Modi = new Image("vnd.ms-modi", new string[] { "mdi" });
            public static readonly Image VendorMs_Photo = new Image("vnd.ms-photo", new string[] { "wdp" });
            public static readonly Image VendorNet_Fpx = new Image("vnd.net-fpx", new string[] { "npx" });
            public static readonly Image VendorRadiance = new Image("vnd.radiance");
            public static readonly Image VendorSealedmediaSoftsealGif = new Image("vnd.sealedmedia.softseal.gif");
            public static readonly Image VendorSealedmediaSoftsealJpg = new Image("vnd.sealedmedia.softseal.jpg");
            public static readonly Image VendorSealedPng = new Image("vnd.sealed.png");
            public static readonly Image VendorSvf = new Image("vnd.svf");
            public static readonly Image VendorTencentTap = new Image("vnd.tencent.tap");
            public static readonly Image VendorValveSourceTexture = new Image("vnd.valve.source.texture");
            public static readonly Image VendorWapWbmp = new Image("vnd.wap.wbmp", new string[] { "wbmp" });
            public static readonly Image VendorXiff = new Image("vnd.xiff", new string[] { "xif" });
            public static readonly Image VendorZbrushPcx = new Image("vnd.zbrush.pcx");
            public static readonly Image Webp = new Image("webp", new string[] { "webp" });
            public static readonly Image Wmf = new Image("wmf");
            public static readonly Image X_3ds = new Image("x-3ds", new string[] { "3ds" });
            public static readonly Image X_Cmu_Raster = new Image("x-cmu-raster", new string[] { "ras" });
            public static readonly Image X_Cmx = new Image("x-cmx", new string[] { "cmx" });

            [System.Obsolete("DEPRECATED in favor of image/emf")]
            public static readonly Image X_Emf = new Image("x-emf");

            public static readonly Image X_Freehand = new Image("x-freehand", new string[] { "fh", "fhc", "fh4", "fh5", "fh7" });
            public static readonly Image X_Icon = new Image("x-icon", new string[] { "ico" });
            public static readonly Image X_Mrsid_Image = new Image("x-mrsid-image", new string[] { "sid" });
            public static readonly Image X_Pcx = new Image("x-pcx", new string[] { "pcx" });
            public static readonly Image X_Pict = new Image("x-pict", new string[] { "pic", "pct" });
            public static readonly Image X_Portable_Anymap = new Image("x-portable-anymap", new string[] { "pnm" });
            public static readonly Image X_Portable_Bitmap = new Image("x-portable-bitmap", new string[] { "pbm" });
            public static readonly Image X_Portable_Graymap = new Image("x-portable-graymap", new string[] { "pgm" });
            public static readonly Image X_Portable_Pixmap = new Image("x-portable-pixmap", new string[] { "ppm" });
            public static readonly Image X_Rgb = new Image("x-rgb", new string[] { "rgb" });
            public static readonly Image X_Tga = new Image("x-tga", new string[] { "tga" });

            [System.Obsolete("DEPRECATED in favor of image/wmf")]
            public static readonly Image X_Wmf = new Image("x-wmf");

            public static readonly Image X_Xbitmap = new Image("x-xbitmap", new string[] { "xbm" });
            public static readonly Image X_Xpixmap = new Image("x-xpixmap", new string[] { "xpm" });
            public static readonly Image X_Xwindowdump = new Image("x-xwindowdump", new string[] { "xwd" });

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
