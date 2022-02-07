import { specialize } from "./util";

const video = specialize("video");

export const anyVideo = video("*");
export const Video_BMPEG = video("bmpeg");
export const Video_BT656 = video("bt656");
export const Video_CelB = video("celb");
export const Video_DV = video("dv");
export const Video_Encaprtp = video("encaprtp");
export const Video_Example = video("example");
export const Video_Flexfec = video("flexfec");
export const Video_H261 = video("h261", "h261");
export const Video_H263 = video("h263", "h263");
export const Video_H263_1998 = video("h263-1998");
export const Video_H263_2000 = video("h263-2000");
export const Video_H264 = video("h264", "h264");
export const Video_H264_RCDO = video("h264-rcdo");
export const Video_H264_SVC = video("h264-svc");
export const Video_H265 = video("h265");
export const Video_IsoSegment = video("iso.segment");
export const Video_JPEG = video("jpeg", "jpgv");
export const Video_Jpeg2000 = video("jpeg2000");
export const Video_Jpm = video("jpm", "jpm", "jpgm");
export const Video_Mj2 = video("mj2", "mj2", "mjp2");
export const Video_MP1S = video("mp1s");
export const Video_MP2P = video("mp2p");
export const Video_MP2T = video("mp2t");
export const Video_Mp4 = video("mp4", "mp4", "mp4v", "mpg4");
export const Video_MP4V_ES = video("mp4v-es");
export const Video_Mpeg = video("mpeg", "mpeg", "mpg", "mpe", "m1v", "m2v");
export const Video_Mpeg4_Generic = video("mpeg4-generic");
export const Video_MPV = video("mpv");
export const Video_Nv = video("nv");
export const Video_Ogg = video("ogg", "ogv");
export const Video_Parityfec = video("parityfec");
export const Video_Pointer = video("pointer");
export const Video_Quicktime = video("quicktime", "qt", "mov");
export const Video_Raptorfec = video("raptorfec");
export const Video_Raw = video("raw");
export const Video_Rtp_Enc_Aescm128 = video("rtp-enc-aescm128");
export const Video_Rtploopback = video("rtploopback");
export const Video_Rtx = video("rtx");
export const Video_Smpte291 = video("smpte291");
export const Video_SMPTE292M = video("smpte292m");
export const Video_Ulpfec = video("ulpfec");
export const Video_Vc1 = video("vc1");
export const Video_Vc2 = video("vc2");
export const Video_Vendor_1d_Interleaved_Parityfec = video("1d-interleaved-parityfec");
export const Video_Vendor_3gpp = video("3gpp", "3gp");
export const Video_Vendor_3gpp_Tt = video("3gpp-tt");
export const Video_Vendor_3gpp2 = video("3gpp2", "3g2");
export const Video_Vendor_CCTV = video("vnd.cctv");
export const Video_Vendor_DeceHd = video("vnd.dece.hd", "uvh", "uvvh");
export const Video_Vendor_DeceMobile = video("vnd.dece.mobile", "uvm", "uvvm");
export const Video_Vendor_DeceMp4 = video("vnd.dece.mp4");
export const Video_Vendor_DecePd = video("vnd.dece.pd", "uvp", "uvvp");
export const Video_Vendor_DeceSd = video("vnd.dece.sd", "uvs", "uvvs");
export const Video_Vendor_DeceVideo = video("vnd.dece.video", "uvv", "uvvv");
export const Video_Vendor_DirectvMpeg = video("vnd.directv.mpeg");
export const Video_Vendor_DirectvMpeg_Tts = video("vnd.directv.mpeg-tts");
export const Video_Vendor_DlnaMpeg_Tts = video("vnd.dlna.mpeg-tts");
export const Video_Vendor_DvbFile = video("vnd.dvb.file", "dvb");
export const Video_Vendor_Fvt = video("vnd.fvt", "fvt");
export const Video_Vendor_HnsVideo = video("vnd.hns.video");
export const Video_Vendor_Iptvforum1dparityfec_1010 = video("vnd.iptvforum.1dparityfec-1010");
export const Video_Vendor_Iptvforum1dparityfec_2005 = video("vnd.iptvforum.1dparityfec-2005");
export const Video_Vendor_Iptvforum2dparityfec_1010 = video("vnd.iptvforum.2dparityfec-1010");
export const Video_Vendor_Iptvforum2dparityfec_2005 = video("vnd.iptvforum.2dparityfec-2005");
export const Video_Vendor_IptvforumTtsavc = video("vnd.iptvforum.ttsavc");
export const Video_Vendor_IptvforumTtsmpeg2 = video("vnd.iptvforum.ttsmpeg2");
export const Video_Vendor_MotorolaVideo = video("vnd.motorola.video");
export const Video_Vendor_MotorolaVideop = video("vnd.motorola.videop");
export const Video_Vendor_Mpegurl = video("vnd.mpegurl", "mxu", "m4u");
export const Video_Vendor_Ms_PlayreadyMediaPyv = video("vnd.ms-playready.media.pyv", "pyv");
export const Video_Vendor_NokiaInterleaved_Multimedia = video("vnd.nokia.interleaved-multimedia");
export const Video_Vendor_NokiaMp4vr = video("vnd.nokia.mp4vr");
export const Video_Vendor_NokiaVideovoip = video("vnd.nokia.videovoip");
export const Video_Vendor_Objectvideo = video("vnd.objectvideo");
export const Video_Vendor_RadgamettoolsBink = video("vnd.radgamettools.bink");
export const Video_Vendor_RadgamettoolsSmacker = video("vnd.radgamettools.smacker");
export const Video_Vendor_SealedmediaSoftsealMov = video("vnd.sealedmedia.softseal.mov");
export const Video_Vendor_SealedMpeg1 = video("vnd.sealed.mpeg1");
export const Video_Vendor_SealedMpeg4 = video("vnd.sealed.mpeg4");
export const Video_Vendor_SealedSwf = video("vnd.sealed.swf");
export const Video_Vendor_UvvuMp4 = video("vnd.uvvu.mp4", "uvu", "uvvu");
export const Video_Vendor_Vivo = video("vnd.vivo", "viv");
export const Video_Vendor_YoutubeYt = video("vnd.youtube.yt");
export const Video_VP8 = video("vp8");
export const Video_Webm = video("webm", "webm");
export const Video_X_F4v = video("x-f4v", "f4v");
export const Video_X_Fli = video("x-fli", "fli");
export const Video_X_Flv = video("x-flv", "flv");
export const Video_X_M4v = video("x-m4v", "m4v");
export const Video_X_Matroska = video("x-matroska", "mkv", "mk3d", "mks");
export const Video_X_Mng = video("x-mng", "mng");
export const Video_X_Ms_Asf = video("x-ms-asf", "asf", "asx");
export const Video_X_Ms_Vob = video("x-ms-vob", "vob");
export const Video_X_Ms_Wm = video("x-ms-wm", "wm");
export const Video_X_Ms_Wmv = video("x-ms-wmv", "wmv");
export const Video_X_Ms_Wmx = video("x-ms-wmx", "wmx");
export const Video_X_Ms_Wvx = video("x-ms-wvx", "wvx");
export const Video_X_Msvideo = video("x-msvideo", "avi");
export const Video_X_Sgi_Movie = video("x-sgi-movie", "movie");
export const Video_X_Smv = video("x-smv", "smv");
export const allVideo = [
    Video_BMPEG,
    Video_BT656,
    Video_CelB,
    Video_DV,
    Video_Encaprtp,
    Video_Example,
    Video_Flexfec,
    Video_H261,
    Video_H263,
    Video_H263_1998,
    Video_H263_2000,
    Video_H264,
    Video_H264_RCDO,
    Video_H264_SVC,
    Video_H265,
    Video_IsoSegment,
    Video_JPEG,
    Video_Jpeg2000,
    Video_Jpm,
    Video_Mj2,
    Video_MP1S,
    Video_MP2P,
    Video_MP2T,
    Video_Mp4,
    Video_MP4V_ES,
    Video_Mpeg,
    Video_Mpeg4_Generic,
    Video_MPV,
    Video_Nv,
    Video_Ogg,
    Video_Parityfec,
    Video_Pointer,
    Video_Quicktime,
    Video_Raptorfec,
    Video_Raw,
    Video_Rtp_Enc_Aescm128,
    Video_Rtploopback,
    Video_Rtx,
    Video_Smpte291,
    Video_SMPTE292M,
    Video_Ulpfec,
    Video_Vc1,
    Video_Vc2,
    Video_Vendor_1d_Interleaved_Parityfec,
    Video_Vendor_3gpp,
    Video_Vendor_3gpp_Tt,
    Video_Vendor_3gpp2,
    Video_Vendor_CCTV,
    Video_Vendor_DeceHd,
    Video_Vendor_DeceMobile,
    Video_Vendor_DeceMp4,
    Video_Vendor_DecePd,
    Video_Vendor_DeceSd,
    Video_Vendor_DeceVideo,
    Video_Vendor_DirectvMpeg,
    Video_Vendor_DirectvMpeg_Tts,
    Video_Vendor_DlnaMpeg_Tts,
    Video_Vendor_DvbFile,
    Video_Vendor_Fvt,
    Video_Vendor_HnsVideo,
    Video_Vendor_Iptvforum1dparityfec_1010,
    Video_Vendor_Iptvforum1dparityfec_2005,
    Video_Vendor_Iptvforum2dparityfec_1010,
    Video_Vendor_Iptvforum2dparityfec_2005,
    Video_Vendor_IptvforumTtsavc,
    Video_Vendor_IptvforumTtsmpeg2,
    Video_Vendor_MotorolaVideo,
    Video_Vendor_MotorolaVideop,
    Video_Vendor_Mpegurl,
    Video_Vendor_Ms_PlayreadyMediaPyv,
    Video_Vendor_NokiaInterleaved_Multimedia,
    Video_Vendor_NokiaMp4vr,
    Video_Vendor_NokiaVideovoip,
    Video_Vendor_Objectvideo,
    Video_Vendor_RadgamettoolsBink,
    Video_Vendor_RadgamettoolsSmacker,
    Video_Vendor_SealedmediaSoftsealMov,
    Video_Vendor_SealedMpeg1,
    Video_Vendor_SealedMpeg4,
    Video_Vendor_SealedSwf,
    Video_Vendor_UvvuMp4,
    Video_Vendor_Vivo,
    Video_Vendor_YoutubeYt,
    Video_VP8,
    Video_Webm,
    Video_X_F4v,
    Video_X_Fli,
    Video_X_Flv,
    Video_X_M4v,
    Video_X_Matroska,
    Video_X_Mng,
    Video_X_Ms_Asf,
    Video_X_Ms_Vob,
    Video_X_Ms_Wm,
    Video_X_Ms_Wmv,
    Video_X_Ms_Wmx,
    Video_X_Ms_Wvx,
    Video_X_Msvideo,
    Video_X_Sgi_Movie,
    Video_X_Smv
];
