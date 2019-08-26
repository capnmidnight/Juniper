namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Audio : MediaType
        {
            public Audio(string value, string[] extensions = null) : base("audio/" + value, extensions) {}

            public static readonly Audio Aac = new Audio("aac");
            public static readonly Audio Ac3 = new Audio("ac3");
            public static readonly Audio Adpcm = new Audio("adpcm", new string[] {"adp"});
            public static readonly Audio AMR = new Audio("amr");
            public static readonly Audio AMR_WB = new Audio("amr-wb");
            public static readonly Audio Amr_WbPlus = new Audio("amr-wb+");
            public static readonly Audio Aptx = new Audio("aptx");
            public static readonly Audio Asc = new Audio("asc");
            public static readonly Audio ATRAC_ADVANCED_LOSSLESS = new Audio("atrac-advanced-lossless");
            public static readonly Audio ATRAC_X = new Audio("atrac-x");
            public static readonly Audio ATRAC3 = new Audio("atrac3");
            public static readonly Audio Basic = new Audio("basic", new string[] {"au", "snd"});
            public static readonly Audio BV16 = new Audio("bv16");
            public static readonly Audio BV32 = new Audio("bv32");
            public static readonly Audio Clearmode = new Audio("clearmode");
            public static readonly Audio CN = new Audio("cn");
            public static readonly Audio DAT12 = new Audio("dat12");
            public static readonly Audio Dls = new Audio("dls");
            public static readonly Audio Dsr_Es201108 = new Audio("dsr-es201108");
            public static readonly Audio Dsr_Es202050 = new Audio("dsr-es202050");
            public static readonly Audio Dsr_Es202211 = new Audio("dsr-es202211");
            public static readonly Audio Dsr_Es202212 = new Audio("dsr-es202212");
            public static readonly Audio DV = new Audio("dv");
            public static readonly Audio DVI4 = new Audio("dvi4");
            public static readonly Audio Eac3 = new Audio("eac3");
            public static readonly Audio Encaprtp = new Audio("encaprtp");
            public static readonly Audio EVRC = new Audio("evrc");
            public static readonly Audio EVRC_QCP = new Audio("evrc-qcp");
            public static readonly Audio EVRC0 = new Audio("evrc0");
            public static readonly Audio EVRC1 = new Audio("evrc1");
            public static readonly Audio EVRCB = new Audio("evrcb");
            public static readonly Audio EVRCB0 = new Audio("evrcb0");
            public static readonly Audio EVRCB1 = new Audio("evrcb1");
            public static readonly Audio EVRCNW = new Audio("evrcnw");
            public static readonly Audio EVRCNW0 = new Audio("evrcnw0");
            public static readonly Audio EVRCNW1 = new Audio("evrcnw1");
            public static readonly Audio EVRCWB = new Audio("evrcwb");
            public static readonly Audio EVRCWB0 = new Audio("evrcwb0");
            public static readonly Audio EVRCWB1 = new Audio("evrcwb1");
            public static readonly Audio EVS = new Audio("evs");
            public static readonly Audio Example = new Audio("example");
            public static readonly Audio Flexfec = new Audio("flexfec");
            public static readonly Audio Fwdred = new Audio("fwdred");
            public static readonly Audio G711_0 = new Audio("g711-0");
            public static readonly Audio G719 = new Audio("g719");
            public static readonly Audio G722 = new Audio("g722");
            public static readonly Audio G7221 = new Audio("g7221");
            public static readonly Audio G723 = new Audio("g723");
            public static readonly Audio G726_16 = new Audio("g726-16");
            public static readonly Audio G726_24 = new Audio("g726-24");
            public static readonly Audio G726_32 = new Audio("g726-32");
            public static readonly Audio G726_40 = new Audio("g726-40");
            public static readonly Audio G728 = new Audio("g728");
            public static readonly Audio G729 = new Audio("g729");
            public static readonly Audio G7291 = new Audio("g7291");
            public static readonly Audio G729D = new Audio("g729d");
            public static readonly Audio G729E = new Audio("g729e");
            public static readonly Audio GSM = new Audio("gsm");
            public static readonly Audio GSM_EFR = new Audio("gsm-efr");
            public static readonly Audio GSM_HR_08 = new Audio("gsm-hr-08");
            public static readonly Audio ILBC = new Audio("ilbc");
            public static readonly Audio Ip_Mr_v25 = new Audio("ip-mr_v2.5");
            public static readonly Audio Isac = new Audio("isac");
            public static readonly Audio L16 = new Audio("l16");
            public static readonly Audio L20 = new Audio("l20");
            public static readonly Audio L24 = new Audio("l24");
            public static readonly Audio L8 = new Audio("l8");
            public static readonly Audio LPC = new Audio("lpc");
            public static readonly Audio MELP = new Audio("melp");
            public static readonly Audio MELP1200 = new Audio("melp1200");
            public static readonly Audio MELP2400 = new Audio("melp2400");
            public static readonly Audio MELP600 = new Audio("melp600");
            public static readonly Audio Midi = new Audio("midi", new string[] {"mid", "midi", "kar", "rmi"});
            public static readonly Audio Mobile_Xmf = new Audio("mobile-xmf");
            public static readonly Audio Mp4 = new Audio("mp4", new string[] {"m4a", "mp4a"});
            public static readonly Audio MP4A_LATM = new Audio("mp4a-latm");
            public static readonly Audio MPA = new Audio("mpa");
            public static readonly Audio Mpa_Robust = new Audio("mpa-robust");
            public static readonly Audio Mpeg = new Audio("mpeg", new string[] {"mpga", "mp2", "mp2a", "mp3", "m2a", "m3a"});
            public static readonly Audio Mpeg4_Generic = new Audio("mpeg4-generic");
            public static readonly Audio Musepack = new Audio("musepack");
            public static readonly Audio Ogg = new Audio("ogg", new string[] {"oga", "ogg", "spx"});
            public static readonly Audio Opus = new Audio("opus");
            public static readonly Audio Parityfec = new Audio("parityfec");
            public static readonly Audio PCMA = new Audio("pcma");
            public static readonly Audio PCMA_WB = new Audio("pcma-wb");
            public static readonly Audio PCMU = new Audio("pcmu");
            public static readonly Audio PCMU_WB = new Audio("pcmu-wb");
            public static readonly Audio PrsSid = new Audio("prs.sid");
            public static readonly Audio Qcelp = new Audio("qcelp");
            public static readonly Audio Raptorfec = new Audio("raptorfec");
            public static readonly Audio RED = new Audio("red");
            public static readonly Audio Rtp_Enc_Aescm128 = new Audio("rtp-enc-aescm128");
            public static readonly Audio Rtp_Midi = new Audio("rtp-midi");
            public static readonly Audio Rtploopback = new Audio("rtploopback");
            public static readonly Audio Rtx = new Audio("rtx");
            public static readonly Audio S3m = new Audio("s3m", new string[] {"s3m"});
            public static readonly Audio Silk = new Audio("silk", new string[] {"sil"});
            public static readonly Audio SMV = new Audio("smv");
            public static readonly Audio SMV_QCP = new Audio("smv-qcp");
            public static readonly Audio SMV0 = new Audio("smv0");
            public static readonly Audio Sp_Midi = new Audio("sp-midi");
            public static readonly Audio Speex = new Audio("speex");
            public static readonly Audio T140c = new Audio("t140c");
            public static readonly Audio T38 = new Audio("t38");
            public static readonly Audio Telephone_Event = new Audio("telephone-event");
            public static readonly Audio TETRA_ACELP = new Audio("tetra_acelp");
            public static readonly Audio Tone = new Audio("tone");
            public static readonly Audio UEMCLIP = new Audio("uemclip");
            public static readonly Audio Ulpfec = new Audio("ulpfec");
            public static readonly Audio Usac = new Audio("usac");
            public static readonly Audio VDVI = new Audio("vdvi");
            public static readonly Audio Vendor1d_Interleaved_Parityfec = new Audio("1d-interleaved-parityfec");
            public static readonly Audio Vendor32kadpcm = new Audio("32kadpcm");
            public static readonly Audio Vendor3gpp = new Audio("3gpp");
            public static readonly Audio Vendor3gpp2 = new Audio("3gpp2");
            public static readonly Audio Vendor3gppIufp = new Audio("vnd.3gpp.iufp");
            public static readonly Audio Vendor4SB = new Audio("vnd.4sb");
            public static readonly Audio VendorAudiokoz = new Audio("vnd.audiokoz");
            public static readonly Audio VendorCELP = new Audio("vnd.celp");
            public static readonly Audio VendorCiscoNse = new Audio("vnd.cisco.nse");
            public static readonly Audio VendorCmlesRadio_Events = new Audio("vnd.cmles.radio-events");
            public static readonly Audio VendorCnsAnp1 = new Audio("vnd.cns.anp1");
            public static readonly Audio VendorCnsInf1 = new Audio("vnd.cns.inf1");
            public static readonly Audio VendorDeceAudio = new Audio("vnd.dece.audio", new string[] {"uva", "uvva"});
            public static readonly Audio VendorDigital_Winds = new Audio("vnd.digital-winds", new string[] {"eol"});
            public static readonly Audio VendorDlnaAdts = new Audio("vnd.dlna.adts");
            public static readonly Audio VendorDolbyHeaac1 = new Audio("vnd.dolby.heaac.1");
            public static readonly Audio VendorDolbyHeaac2 = new Audio("vnd.dolby.heaac.2");
            public static readonly Audio VendorDolbyMlp = new Audio("vnd.dolby.mlp");
            public static readonly Audio VendorDolbyMps = new Audio("vnd.dolby.mps");
            public static readonly Audio VendorDolbyPl2 = new Audio("vnd.dolby.pl2");
            public static readonly Audio VendorDolbyPl2x = new Audio("vnd.dolby.pl2x");
            public static readonly Audio VendorDolbyPl2z = new Audio("vnd.dolby.pl2z");
            public static readonly Audio VendorDolbyPulse1 = new Audio("vnd.dolby.pulse.1");
            public static readonly Audio VendorDra = new Audio("vnd.dra", new string[] {"dra"});
            public static readonly Audio VendorDts = new Audio("vnd.dts", new string[] {"dts"});
            public static readonly Audio VendorDtsHd = new Audio("vnd.dts.hd", new string[] {"dtshd"});
            public static readonly Audio VendorDtsUhd = new Audio("vnd.dts.uhd");
            public static readonly Audio VendorDvbFile = new Audio("vnd.dvb.file");
            public static readonly Audio VendorEveradPlj = new Audio("vnd.everad.plj");
            public static readonly Audio VendorHnsAudio = new Audio("vnd.hns.audio");
            public static readonly Audio VendorLucentVoice = new Audio("vnd.lucent.voice", new string[] {"lvp"});
            public static readonly Audio VendorMs_PlayreadyMediaPya = new Audio("vnd.ms-playready.media.pya", new string[] {"pya"});
            public static readonly Audio VendorNokiaMobile_Xmf = new Audio("vnd.nokia.mobile-xmf");
            public static readonly Audio VendorNortelVbk = new Audio("vnd.nortel.vbk");
            public static readonly Audio VendorNueraEcelp4800 = new Audio("vnd.nuera.ecelp4800", new string[] {"ecelp4800"});
            public static readonly Audio VendorNueraEcelp7470 = new Audio("vnd.nuera.ecelp7470", new string[] {"ecelp7470"});
            public static readonly Audio VendorNueraEcelp9600 = new Audio("vnd.nuera.ecelp9600", new string[] {"ecelp9600"});
            public static readonly Audio VendorOctelSbc = new Audio("vnd.octel.sbc");
            public static readonly Audio VendorPresonusMultitrack = new Audio("vnd.presonus.multitrack");

            [System.Obsolete("DEPRECATED in favor of audio/qcelp")]
            public static readonly Audio VendorQcelp = new Audio("vnd.qcelp");

            public static readonly Audio VendorRhetorex32kadpcm = new Audio("vnd.rhetorex.32kadpcm");
            public static readonly Audio VendorRip = new Audio("vnd.rip", new string[] {"rip"});
            public static readonly Audio VendorSealedmediaSoftsealMpeg = new Audio("vnd.sealedmedia.softseal.mpeg");
            public static readonly Audio VendorVmxCvsd = new Audio("vnd.vmx.cvsd");
            public static readonly Audio VMR_WB = new Audio("vmr-wb");
            public static readonly Audio Vorbis = new Audio("vorbis");
            public static readonly Audio Vorbis_Config = new Audio("vorbis-config");
            public static readonly Audio Webm = new Audio("webm", new string[] {"weba"});
            public static readonly Audio X_Aac = new Audio("x-aac", new string[] {"aac"});
            public static readonly Audio X_Aiff = new Audio("x-aiff", new string[] {"aif", "aiff", "aifc"});
            public static readonly Audio X_Caf = new Audio("x-caf", new string[] {"caf"});
            public static readonly Audio X_Flac = new Audio("x-flac", new string[] {"flac"});
            public static readonly Audio X_Matroska = new Audio("x-matroska", new string[] {"mka"});
            public static readonly Audio X_Mpegurl = new Audio("x-mpegurl", new string[] {"m3u"});
            public static readonly Audio X_Ms_Wax = new Audio("x-ms-wax", new string[] {"wax"});
            public static readonly Audio X_Ms_Wma = new Audio("x-ms-wma", new string[] {"wma"});
            public static readonly Audio X_Pn_Realaudio = new Audio("x-pn-realaudio", new string[] {"ram", "ra"});
            public static readonly Audio X_Pn_Realaudio_Plugin = new Audio("x-pn-realaudio-plugin", new string[] {"rmp"});
            public static readonly Audio X_Tta = new Audio("x-tta");
            public static readonly Audio X_Wav = new Audio("x-wav", new string[] {"wav"});
            public static readonly Audio Xm = new Audio("xm", new string[] {"xm"});

            public static readonly new Audio[] Values = {
                Aac,
                Ac3,
                Adpcm,
                AMR,
                AMR_WB,
                Amr_WbPlus,
                Aptx,
                Asc,
                ATRAC_ADVANCED_LOSSLESS,
                ATRAC_X,
                ATRAC3,
                Basic,
                BV16,
                BV32,
                Clearmode,
                CN,
                DAT12,
                Dls,
                Dsr_Es201108,
                Dsr_Es202050,
                Dsr_Es202211,
                Dsr_Es202212,
                DV,
                DVI4,
                Eac3,
                Encaprtp,
                EVRC,
                EVRC_QCP,
                EVRC0,
                EVRC1,
                EVRCB,
                EVRCB0,
                EVRCB1,
                EVRCNW,
                EVRCNW0,
                EVRCNW1,
                EVRCWB,
                EVRCWB0,
                EVRCWB1,
                EVS,
                Example,
                Flexfec,
                Fwdred,
                G711_0,
                G719,
                G722,
                G7221,
                G723,
                G726_16,
                G726_24,
                G726_32,
                G726_40,
                G728,
                G729,
                G7291,
                G729D,
                G729E,
                GSM,
                GSM_EFR,
                GSM_HR_08,
                ILBC,
                Ip_Mr_v25,
                Isac,
                L16,
                L20,
                L24,
                L8,
                LPC,
                MELP,
                MELP1200,
                MELP2400,
                MELP600,
                Midi,
                Mobile_Xmf,
                Mp4,
                MP4A_LATM,
                MPA,
                Mpa_Robust,
                Mpeg,
                Mpeg4_Generic,
                Musepack,
                Ogg,
                Opus,
                Parityfec,
                PCMA,
                PCMA_WB,
                PCMU,
                PCMU_WB,
                PrsSid,
                Qcelp,
                Raptorfec,
                RED,
                Rtp_Enc_Aescm128,
                Rtp_Midi,
                Rtploopback,
                Rtx,
                S3m,
                Silk,
                SMV,
                SMV_QCP,
                SMV0,
                Sp_Midi,
                Speex,
                T140c,
                T38,
                Telephone_Event,
                TETRA_ACELP,
                Tone,
                UEMCLIP,
                Ulpfec,
                Usac,
                VDVI,
                Vendor1d_Interleaved_Parityfec,
                Vendor32kadpcm,
                Vendor3gpp,
                Vendor3gpp2,
                Vendor3gppIufp,
                Vendor4SB,
                VendorAudiokoz,
                VendorCELP,
                VendorCiscoNse,
                VendorCmlesRadio_Events,
                VendorCnsAnp1,
                VendorCnsInf1,
                VendorDeceAudio,
                VendorDigital_Winds,
                VendorDlnaAdts,
                VendorDolbyHeaac1,
                VendorDolbyHeaac2,
                VendorDolbyMlp,
                VendorDolbyMps,
                VendorDolbyPl2,
                VendorDolbyPl2x,
                VendorDolbyPl2z,
                VendorDolbyPulse1,
                VendorDra,
                VendorDts,
                VendorDtsHd,
                VendorDtsUhd,
                VendorDvbFile,
                VendorEveradPlj,
                VendorHnsAudio,
                VendorLucentVoice,
                VendorMs_PlayreadyMediaPya,
                VendorNokiaMobile_Xmf,
                VendorNortelVbk,
                VendorNueraEcelp4800,
                VendorNueraEcelp7470,
                VendorNueraEcelp9600,
                VendorOctelSbc,
                VendorPresonusMultitrack,
                VendorRhetorex32kadpcm,
                VendorRip,
                VendorSealedmediaSoftsealMpeg,
                VendorVmxCvsd,
                VMR_WB,
                Vorbis,
                Vorbis_Config,
                Webm,
                X_Aac,
                X_Aiff,
                X_Caf,
                X_Flac,
                X_Matroska,
                X_Mpegurl,
                X_Ms_Wax,
                X_Ms_Wma,
                X_Pn_Realaudio,
                X_Pn_Realaudio_Plugin,
                X_Tta,
                X_Wav,
                Xm,
            };
        }
    }
}
