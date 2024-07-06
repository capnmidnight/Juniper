namespace Juniper
{
    public partial class MediaType
    {
        public static readonly Image Image_Aces = new("aces");
        public static readonly Image Image_Avci = new("avci");
        public static readonly Image Image_Avcs = new("avcs");
        public static readonly Image Image_Bmp = new("bmp", "bmp");
        public static readonly Image Image_Cgm = new("cgm", "cgm");
        public static readonly Image Image_Dicom_Rle = new("dicom-rle");
        public static readonly Image Image_Emf = new("emf");
        public static readonly Image Image_Example = new("example");
        public static readonly Image Image_EXR = new("x-exr", "exr");
        public static readonly Image Image_Fits = new("fits");
        public static readonly Image Image_G3fax = new("g3fax", "g3");
        public static readonly Image Image_Gif = new("gif", "gif");
        public static readonly Image Image_Heic = new("heic");
        public static readonly Image Image_Heic_Sequence = new("heic-sequence");
        public static readonly Image Image_Heif = new("heif");
        public static readonly Image Image_Heif_Sequence = new("heif-sequence");
        public static readonly Image Image_Hej2k = new("hej2k");
        public static readonly Image Image_Hsj2 = new("hsj2");
        public static readonly Image Image_Ief = new("ief", "ief");
        public static readonly Image Image_Jls = new("jls");
        public static readonly Image Image_Jp2 = new("jp2");
        public static readonly Image Image_Jpeg = new("jpeg", "jpeg", "jpg", "jpe");
        public static readonly Image Image_Jph = new("jph");
        public static readonly Image Image_Jphc = new("jphc");
        public static readonly Image Image_Jpm = new("jpm");
        public static readonly Image Image_Jpx = new("jpx");
        public static readonly Image Image_Jxr = new("jxr");
        public static readonly Image Image_JxrA = new("jxra");
        public static readonly Image Image_JxrS = new("jxrs");
        public static readonly Image Image_Jxs = new("jxs");
        public static readonly Image Image_Jxsc = new("jxsc");
        public static readonly Image Image_Jxsi = new("jxsi");
        public static readonly Image Image_Jxss = new("jxss");
        public static readonly Image Image_Ktx = new("ktx", "ktx");
        public static readonly Image Image_Naplps = new("naplps");
        public static readonly Image Image_Png = new("png", "png");
        public static readonly Image Image_PrsBtif = new("prs.btif", "btif");
        public static readonly Image Image_PrsPti = new("prs.pti");
        public static readonly Image Image_Pwg_Raster = new("pwg-raster");
        public static readonly Image Image_Raw = new("x-raw", "raw");
        public static readonly Image Image_Sgi = new("sgi", "sgi");
        public static readonly Image Image_SvgXml = new("svg+xml", "svg", "svgz");
        public static readonly Image Image_T38 = new("t38");
        public static readonly Image Image_Tiff = new("tiff", "tiff", "tif");
        public static readonly Image Image_Tiff_Fx = new("tiff-fx");
        public static readonly Image Image_VendorAdobePhotoshop = new("vnd.adobe.photoshop", "psd");
        public static readonly Image Image_VendorAirzipAcceleratorAzv = new("vnd.airzip.accelerator.azv");
        public static readonly Image Image_VendorCnsInf2 = new("vnd.cns.inf2");
        public static readonly Image Image_VendorDeceGraphic = new("vnd.dece.graphic", "uvi", "uvvi", "uvg", "uvvg");
        public static readonly Image Image_VendorDjvu = new("vnd.djvu", "djvu", "djv");
        public static readonly Image Image_VendorDvbSubtitle = new("vnd.dvb.subtitle", "sub");
        public static readonly Image Image_VendorDwg = new("vnd.dwg", "dwg");
        public static readonly Image Image_VendorDxf = new("vnd.dxf", "dxf");
        public static readonly Image Image_VendorFastbidsheet = new("vnd.fastbidsheet", "fbs");
        public static readonly Image Image_VendorFpx = new("vnd.fpx", "fpx");
        public static readonly Image Image_VendorFst = new("vnd.fst", "fst");
        public static readonly Image Image_VendorFujixeroxEdmics_Mmr = new("vnd.fujixerox.edmics-mmr", "mmr");
        public static readonly Image Image_VendorFujixeroxEdmics_Rlc = new("vnd.fujixerox.edmics-rlc", "rlc");
        public static readonly Image Image_VendorGlobalgraphicsPgb = new("vnd.globalgraphics.pgb");
        public static readonly Image Image_VendorMicrosoftIcon = new("vnd.microsoft.icon", "ico");
        public static readonly Image Image_VendorMix = new("vnd.mix");
        public static readonly Image Image_VendorMozillaApng = new("vnd.mozilla.apng");
        public static readonly Image Image_VendorMs_Modi = new("vnd.ms-modi", "mdi");
        public static readonly Image Image_VendorMs_Photo = new("vnd.ms-photo", "wdp");
        public static readonly Image Image_VendorNet_Fpx = new("vnd.net-fpx", "npx");
        public static readonly Image Image_VendorRadiance = new("vnd.radiance");
        public static readonly Image Image_VendorSealedmediaSoftsealGif = new("vnd.sealedmedia.softseal.gif");
        public static readonly Image Image_VendorSealedmediaSoftsealJpg = new("vnd.sealedmedia.softseal.jpg");
        public static readonly Image Image_VendorSealedPng = new("vnd.sealed.png");
        public static readonly Image Image_VendorSvf = new("vnd.svf");
        public static readonly Image Image_VendorTencentTap = new("vnd.tencent.tap");
        public static readonly Image Image_VendorValveSourceTexture = new("vnd.valve.source.texture");
        public static readonly Image Image_VendorWapWbmp = new("vnd.wap.wbmp", "wbmp");
        public static readonly Image Image_VendorXiff = new("vnd.xiff", "xif");
        public static readonly Image Image_VendorZbrushPcx = new("vnd.zbrush.pcx");
        public static readonly Image Image_Webp = new("webp", "webp");
        public static readonly Image Image_Wmf = new("wmf");
        public static readonly Image Image_X_3ds = new("x-3ds", "3ds");
        public static readonly Image Image_X_Cmu_Raster = new("x-cmu-raster", "ras");
        public static readonly Image Image_X_Cmx = new("x-cmx", "cmx");

        [System.Obsolete("DEPRECATED in favor of image/emf")]
        public static readonly Image Image_X_Emf = new("x-emf");

        public static readonly Image Image_X_Freehand = new("x-freehand", "fh", "fhc", "fh4", "fh5", "fh7");
        public static readonly Image Image_X_Icon = new("x-icon", "ico");
        public static readonly Image Image_X_Mrsid_Image = new("x-mrsid-image", "sid");
        public static readonly Image Image_X_Pcx = new("x-pcx", "pcx");
        public static readonly Image Image_X_Pict = new("x-pict", "pic", "pct");
        public static readonly Image Image_X_Portable_Anymap = new("x-portable-anymap", "pnm");
        public static readonly Image Image_X_Portable_Bitmap = new("x-portable-bitmap", "pbm");
        public static readonly Image Image_X_Portable_Graymap = new("x-portable-graymap", "pgm");
        public static readonly Image Image_X_Portable_Pixmap = new("x-portable-pixmap", "ppm");
        public static readonly Image Image_X_Rgb = new("x-rgb", "rgb");
        public static readonly Image Image_X_Tga = new("x-tga", "tga");

        [System.Obsolete("DEPRECATED in favor of image/wmf")]
        public static readonly Image Image_X_Wmf = new("x-wmf");

        public static readonly Image Image_X_Xbitmap = new("x-xbitmap", "xbm");
        public static readonly Image Image_X_Xpixmap = new("x-xpixmap", "xpm");
        public static readonly Image Image_X_Xwindowdump = new("x-xwindowdump", "xwd");
    }
}
