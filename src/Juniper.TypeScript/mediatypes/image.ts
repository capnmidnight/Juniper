import { deprecate, specialize } from "./util";

const image = specialize("image");

export const anyImage = image("*");
export const Image_Aces = image("aces");
export const Image_Apng = image("apng", "apng");
export const Image_Avci = image("avci");
export const Image_Avcs = image("avcs");
export const Image_Avif = image("avif", "avif");
export const Image_Bmp = image("bmp", "bmp");
export const Image_Cgm = image("cgm", "cgm");
export const Image_Dicom_Rle = image("dicom-rle");
export const Image_Emf = image("emf");
export const Image_Example = image("example");
export const Image_EXR = image("x-exr", "exr");
export const Image_Fits = image("fits");
export const Image_G3fax = image("g3fax", "g3");
export const Image_Gif = image("gif", "gif");
export const Image_Heic = image("heic");
export const Image_Heic_Sequence = image("heic-sequence");
export const Image_Heif = image("heif");
export const Image_Heif_Sequence = image("heif-sequence");
export const Image_Hej2k = image("hej2k");
export const Image_Hsj2 = image("hsj2");
export const Image_Ief = image("ief", "ief");
export const Image_Jls = image("jls");
export const Image_Jp2 = image("jp2");
export const Image_Jpeg = image("jpeg", "jpeg", "jpg", "jpe");
export const Image_Jph = image("jph");
export const Image_Jphc = image("jphc");
export const Image_Jpm = image("jpm");
export const Image_Jpx = image("jpx");
export const Image_Jxr = image("jxr");
export const Image_JxrA = image("jxra");
export const Image_JxrS = image("jxrs");
export const Image_Jxs = image("jxs");
export const Image_Jxsc = image("jxsc");
export const Image_Jxsi = image("jxsi");
export const Image_Jxss = image("jxss");
export const Image_Ktx = image("ktx", "ktx");
export const Image_Naplps = image("naplps");
export const Image_Pjpeg = image("pjpeg");
export const Image_Png = image("png", "png");
export const Image_PrsBtif = image("prs.btif", "btif");
export const Image_PrsPti = image("prs.pti");
export const Image_Pwg_Raster = image("pwg-raster");
export const Image_Raw = image("x-raw", "raw");
export const Image_Sgi = image("sgi", "sgi");
export const Image_SvgXml = image("svg+xml", "svg", "svgz");
export const Image_T38 = image("t38");
export const Image_Tiff = image("tiff", "tiff", "tif");
export const Image_Tiff_Fx = image("tiff-fx");
export const Image_Vendor_AdobePhotoshop = image("vnd.adobe.photoshop", "psd");
export const Image_Vendor_AirzipAcceleratorAzv = image("vnd.airzip.accelerator.azv");
export const Image_Vendor_CnsInf2 = image("vnd.cns.inf2");
export const Image_Vendor_DeceGraphic = image("vnd.dece.graphic", "uvi", "uvvi", "uvg", "uvvg");
export const Image_Vendor_Djvu = image("vnd.djvu", "djvu", "djv");
export const Image_Vendor_DvbSubtitle = image("vnd.dvb.subtitle", "sub");
export const Image_Vendor_Dwg = image("vnd.dwg", "dwg");
export const Image_Vendor_Dxf = image("vnd.dxf", "dxf");
export const Image_Vendor_Fastbidsheet = image("vnd.fastbidsheet", "fbs");
export const Image_Vendor_Fpx = image("vnd.fpx", "fpx");
export const Image_Vendor_Fst = image("vnd.fst", "fst");
export const Image_Vendor_FujixeroxEdmics_Mmr = image("vnd.fujixerox.edmics-mmr", "mmr");
export const Image_Vendor_FujixeroxEdmics_Rlc = image("vnd.fujixerox.edmics-rlc", "rlc");
export const Image_Vendor_GlobalgraphicsPgb = image("vnd.globalgraphics.pgb");
export const Image_Vendor_MicrosoftIcon = image("vnd.microsoft.icon");
export const Image_Vendor_Mix = image("vnd.mix");
export const Image_Vendor_MozillaApng = image("vnd.mozilla.apng");
export const Image_Vendor_Ms_Dds = image("vnd.ms-dds", "dds");
export const Image_Vendor_Ms_Modi = image("vnd.ms-modi", "mdi");
export const Image_Vendor_Ms_Photo = image("vnd.ms-photo", "wdp");
export const Image_Vendor_Net_Fpx = image("vnd.net-fpx", "npx");
export const Image_Vendor_Radiance = image("vnd.radiance");
export const Image_Vendor_SealedmediaSoftsealGif = image("vnd.sealedmedia.softseal.gif");
export const Image_Vendor_SealedmediaSoftsealJpg = image("vnd.sealedmedia.softseal.jpg");
export const Image_Vendor_SealedPng = image("vnd.sealed.png");
export const Image_Vendor_Svf = image("vnd.svf");
export const Image_Vendor_TencentTap = image("vnd.tencent.tap");
export const Image_Vendor_ValveSourceTexture = image("vnd.valve.source.texture");
export const Image_Vendor_WapWbmp = image("vnd.wap.wbmp", "wbmp");
export const Image_Vendor_Xiff = image("vnd.xiff", "xif");
export const Image_Vendor_ZbrushPcx = image("vnd.zbrush.pcx");
export const Image_Webp = image("webp", "webp");
export const Image_Wmf = image("wmf");
export const Image_X_3ds = image("x-3ds", "3ds");
export const Image_X_Cmu_Raster = image("x-cmu-raster", "ras");
export const Image_X_Cmx = image("x-cmx", "cmx");
export const Image_X_Emf = deprecate(image("x-emf"), "in favor of image/emf");
export const Image_X_Freehand = image("x-freehand", "fh", "fhc", "fh4", "fh5", "fh7");
export const Image_X_Icon = image("x-icon", "ico");
export const Image_X_Mrsid_Image = image("x-mrsid-image", "sid");
export const Image_X_Ms_Bmp = image("x-ms-bmp");
export const Image_X_Pcx = image("x-pcx", "pcx");
export const Image_X_Pict = image("x-pict", "pic", "pct");
export const Image_X_Portable_Anymap = image("x-portable-anymap", "pnm");
export const Image_X_Portable_Bitmap = image("x-portable-bitmap", "pbm");
export const Image_X_Portable_Graymap = image("x-portable-graymap", "pgm");
export const Image_X_Portable_Pixmap = image("x-portable-pixmap", "ppm");
export const Image_X_Rgb = image("x-rgb", "rgb");
export const Image_X_Tga = image("x-tga", "tga");
export const Image_X_Wmf = deprecate(image("x-wmf"), "in favor of image/wmf");
export const Image_X_Xbitmap = image("x-xbitmap", "xbm");
export const Image_X_Xcf = image("x-xcf");
export const Image_X_Xpixmap = image("x-xpixmap", "xpm");
export const Image_X_Xwindowdump = image("x-xwindowdump", "xwd");
export const allImage = [
    Image_Aces,
    Image_Apng,
    Image_Avci,
    Image_Avcs,
    Image_Avif,
    Image_Bmp,
    Image_Cgm,
    Image_Dicom_Rle,
    Image_Emf,
    Image_Example,
    Image_EXR,
    Image_Fits,
    Image_G3fax,
    Image_Gif,
    Image_Heic,
    Image_Heic_Sequence,
    Image_Heif,
    Image_Heif_Sequence,
    Image_Hej2k,
    Image_Hsj2,
    Image_Ief,
    Image_Jls,
    Image_Jp2,
    Image_Jpeg,
    Image_Jph,
    Image_Jphc,
    Image_Jpm,
    Image_Jpx,
    Image_Jxr,
    Image_JxrA,
    Image_JxrS,
    Image_Jxs,
    Image_Jxsc,
    Image_Jxsi,
    Image_Jxss,
    Image_Ktx,
    Image_Naplps,
    Image_Pjpeg,
    Image_Png,
    Image_PrsBtif,
    Image_PrsPti,
    Image_Pwg_Raster,
    Image_Raw,
    Image_Sgi,
    Image_SvgXml,
    Image_T38,
    Image_Tiff,
    Image_Tiff_Fx,
    Image_Vendor_AdobePhotoshop,
    Image_Vendor_AirzipAcceleratorAzv,
    Image_Vendor_CnsInf2,
    Image_Vendor_DeceGraphic,
    Image_Vendor_Djvu,
    Image_Vendor_DvbSubtitle,
    Image_Vendor_Dwg,
    Image_Vendor_Dxf,
    Image_Vendor_Fastbidsheet,
    Image_Vendor_Fpx,
    Image_Vendor_Fst,
    Image_Vendor_FujixeroxEdmics_Mmr,
    Image_Vendor_FujixeroxEdmics_Rlc,
    Image_Vendor_GlobalgraphicsPgb,
    Image_Vendor_MicrosoftIcon,
    Image_Vendor_Mix,
    Image_Vendor_MozillaApng,
    Image_Vendor_Ms_Dds,
    Image_Vendor_Ms_Modi,
    Image_Vendor_Ms_Photo,
    Image_Vendor_Net_Fpx,
    Image_Vendor_Radiance,
    Image_Vendor_SealedmediaSoftsealGif,
    Image_Vendor_SealedmediaSoftsealJpg,
    Image_Vendor_SealedPng,
    Image_Vendor_Svf,
    Image_Vendor_TencentTap,
    Image_Vendor_ValveSourceTexture,
    Image_Vendor_WapWbmp,
    Image_Vendor_Xiff,
    Image_Vendor_ZbrushPcx,
    Image_Webp,
    Image_Wmf,
    Image_X_3ds,
    Image_X_Cmu_Raster,
    Image_X_Cmx,
    Image_X_Freehand,
    Image_X_Icon,
    Image_X_Mrsid_Image,
    Image_X_Ms_Bmp,
    Image_X_Pcx,
    Image_X_Pict,
    Image_X_Portable_Anymap,
    Image_X_Portable_Bitmap,
    Image_X_Portable_Graymap,
    Image_X_Portable_Pixmap,
    Image_X_Rgb,
    Image_X_Tga,
    Image_X_Xbitmap,
    Image_X_Xcf,
    Image_X_Xpixmap,
    Image_X_Xwindowdump
];