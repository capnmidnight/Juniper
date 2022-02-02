namespace Juniper
{
    public partial class MediaType
    {
        public partial class Audio : MediaType
        {
            public static readonly Audio Aac = new("aac", new[] { "aac" });
            public static readonly Audio Ac3 = new("ac3", new[] { "ac3" });
            public static readonly Audio Adpcm = new("adpcm", new[] { "adp" });
            public static readonly Audio AMR = new("amr", new[] { "amr" });
            public static readonly Audio AMR_WB = new("amr-wb");
            public static readonly Audio Amr_WbPlus = new("amr-wb+");
            public static readonly Audio Aptx = new("aptx");
            public static readonly Audio Asc = new("asc");
            public static readonly Audio ATRAC_ADVANCED_LOSSLESS = new("atrac-advanced-lossless");
            public static readonly Audio ATRAC_X = new("atrac-x");
            public static readonly Audio ATRAC3 = new("atrac3");
            public static readonly Audio Basic = new("basic", new[] { "au", "snd" });
            public static readonly Audio BV16 = new("bv16");
            public static readonly Audio BV32 = new("bv32");
            public static readonly Audio Clearmode = new("clearmode");
            public static readonly Audio CN = new("cn");
            public static readonly Audio DAT12 = new("dat12");
            public static readonly Audio Dls = new("dls");
            public static readonly Audio Dsr_Es201108 = new("dsr-es201108");
            public static readonly Audio Dsr_Es202050 = new("dsr-es202050");
            public static readonly Audio Dsr_Es202211 = new("dsr-es202211");
            public static readonly Audio Dsr_Es202212 = new("dsr-es202212");
            public static readonly Audio DV = new("dv");
            public static readonly Audio DVI4 = new("dvi4");
            public static readonly Audio Eac3 = new("eac3");
            public static readonly Audio Encaprtp = new("encaprtp");
            public static readonly Audio EVRC = new("evrc");
            public static readonly Audio EVRC_QCP = new("evrc-qcp");
            public static readonly Audio EVRC0 = new("evrc0");
            public static readonly Audio EVRC1 = new("evrc1");
            public static readonly Audio EVRCB = new("evrcb");
            public static readonly Audio EVRCB0 = new("evrcb0");
            public static readonly Audio EVRCB1 = new("evrcb1");
            public static readonly Audio EVRCNW = new("evrcnw");
            public static readonly Audio EVRCNW0 = new("evrcnw0");
            public static readonly Audio EVRCNW1 = new("evrcnw1");
            public static readonly Audio EVRCWB = new("evrcwb");
            public static readonly Audio EVRCWB0 = new("evrcwb0");
            public static readonly Audio EVRCWB1 = new("evrcwb1");
            public static readonly Audio EVS = new("evs");
            public static readonly Audio Example = new("example");
            public static readonly Audio Flexfec = new("flexfec");
            public static readonly Audio Fwdred = new("fwdred");
            public static readonly Audio G711_0 = new("g711-0");
            public static readonly Audio G719 = new("g719");
            public static readonly Audio G722 = new("g722");
            public static readonly Audio G7221 = new("g7221");
            public static readonly Audio G723 = new("g723");
            public static readonly Audio G726_16 = new("g726-16");
            public static readonly Audio G726_24 = new("g726-24");
            public static readonly Audio G726_32 = new("g726-32");
            public static readonly Audio G726_40 = new("g726-40");
            public static readonly Audio G728 = new("g728");
            public static readonly Audio G729 = new("g729");
            public static readonly Audio G7291 = new("g7291");
            public static readonly Audio G729D = new("g729d");
            public static readonly Audio G729E = new("g729e");
            public static readonly Audio GSM = new("gsm", new[] { "gsm" });
            public static readonly Audio GSM_EFR = new("gsm-efr");
            public static readonly Audio GSM_HR_08 = new("gsm-hr-08");
            public static readonly Audio ILBC = new("ilbc");
            public static readonly Audio Ip_Mr_v25 = new("ip-mr_v2.5");
            public static readonly Audio Isac = new("isac");
            public static readonly Audio L16 = new("l16");
            public static readonly Audio L20 = new("l20");
            public static readonly Audio L24 = new("l24");
            public static readonly Audio L8 = new("l8");
            public static readonly Audio LPC = new("lpc");
            public static readonly Audio MELP = new("melp");
            public static readonly Audio MELP1200 = new("melp1200");
            public static readonly Audio MELP2400 = new("melp2400");
            public static readonly Audio MELP600 = new("melp600");
            public static readonly Audio Midi = new("midi", new[] { "mid", "midi", "kar", "rmi" });
            public static readonly Audio Mobile_Xmf = new("mobile-xmf");
            public static readonly Audio Mp4 = new("mp4", new[] { "m4a", "mp4a" });
            public static readonly Audio MP4A_LATM = new("mp4a-latm");
            public static readonly Audio MPA = new("mpa");
            public static readonly Audio Mpa_Robust = new("mpa-robust");
            public static readonly Audio Mpeg = new("mpeg", new[] { "mp3", "mp2", "mp2a", "mpga", "m2a", "m3a" });
            public static readonly Audio Mpeg4_Generic = new("mpeg4-generic");
            public static readonly Audio Musepack = new("musepack");
            public static readonly Audio Ogg = new("ogg", new[] { "ogg", "oga", "spx" });
            public static readonly Audio OggOpus = new("ogg", new[] { "ogg", "oga", "spx" });
            public static readonly Audio OggVorbis = new("ogg", new[] { "ogg" });
            public static readonly Audio Parityfec = new("parityfec");
            public static readonly Audio PCMA = new("pcma", new[] { "wav" });
            public static readonly Audio PCMA_WB = new("pcma-wb");
            public static readonly Audio PCMU = new("pcmu", new[] { "wav" });
            public static readonly Audio PCMU_WB = new("pcmu-wb");
            public static readonly Audio PrsSid = new("prs.sid");
            public static readonly Audio Qcelp = new("qcelp");
            public static readonly Audio Raptorfec = new("raptorfec");
            public static readonly Audio Raw = new(new[] { "dat" });
            public static readonly Audio RED = new("red");
            public static readonly Audio Rtp_Enc_Aescm128 = new("rtp-enc-aescm128");
            public static readonly Audio Rtp_Midi = new("rtp-midi");
            public static readonly Audio Rtploopback = new("rtploopback");
            public static readonly Audio Rtx = new("rtx");
            public static readonly Audio S3m = new("s3m", new[] { "s3m" });
            public static readonly Audio Silk = new("silk", new[] { "sil" });
            public static readonly Audio SMV = new("smv");
            public static readonly Audio SMV_QCP = new("smv-qcp");
            public static readonly Audio SMV0 = new("smv0");
            public static readonly Audio Sp_Midi = new("sp-midi");
            public static readonly Audio Speex = new("speex");
            public static readonly Audio T140c = new("t140c");
            public static readonly Audio T38 = new("t38");
            public static readonly Audio Telephone_Event = new("telephone-event");
            public static readonly Audio TETRA_ACELP = new("tetra_acelp");
            public static readonly Audio TETRA_ACELP_BB = new("tetra_acelp_bb");
            public static readonly Audio Tone = new("tone");
            public static readonly Audio UEMCLIP = new("uemclip");
            public static readonly Audio Ulpfec = new("ulpfec");
            public static readonly Audio Usac = new("usac");
            public static readonly Audio VDVI = new("vdvi");
            public static readonly Audio Vendor1d_Interleaved_Parityfec = new("1d-interleaved-parityfec");
            public static readonly Audio Vendor32kadpcm = new("32kadpcm");
            public static readonly Audio Vendor3gpp = new("3gpp");
            public static readonly Audio Vendor3gpp2 = new("3gpp2");
            public static readonly Audio Vendor3gppIufp = new("vnd.3gpp.iufp");
            public static readonly Audio Vendor4SB = new("vnd.4sb");
            public static readonly Audio VendorAudiokoz = new("vnd.audiokoz");
            public static readonly Audio VendorCELP = new("vnd.celp");
            public static readonly Audio VendorCiscoNse = new("vnd.cisco.nse");
            public static readonly Audio VendorCmlesRadio_Events = new("vnd.cmles.radio-events");
            public static readonly Audio VendorCnsAnp1 = new("vnd.cns.anp1");
            public static readonly Audio VendorCnsInf1 = new("vnd.cns.inf1");
            public static readonly Audio VendorDeceAudio = new("vnd.dece.audio", new[] { "uva", "uvva" });
            public static readonly Audio VendorDigital_Winds = new("vnd.digital-winds", new[] { "eol" });
            public static readonly Audio VendorDlnaAdts = new("vnd.dlna.adts");
            public static readonly Audio VendorDolbyHeaac1 = new("vnd.dolby.heaac.1");
            public static readonly Audio VendorDolbyHeaac2 = new("vnd.dolby.heaac.2");
            public static readonly Audio VendorDolbyMlp = new("vnd.dolby.mlp");
            public static readonly Audio VendorDolbyMps = new("vnd.dolby.mps");
            public static readonly Audio VendorDolbyPl2 = new("vnd.dolby.pl2");
            public static readonly Audio VendorDolbyPl2x = new("vnd.dolby.pl2x");
            public static readonly Audio VendorDolbyPl2z = new("vnd.dolby.pl2z");
            public static readonly Audio VendorDolbyPulse1 = new("vnd.dolby.pulse.1");
            public static readonly Audio VendorDra = new("vnd.dra", new[] { "dra" });
            public static readonly Audio VendorDts = new("vnd.dts", new[] { "dts" });
            public static readonly Audio VendorDtsHd = new("vnd.dts.hd", new[] { "dtshd" });
            public static readonly Audio VendorDtsUhd = new("vnd.dts.uhd");
            public static readonly Audio VendorDvbFile = new("vnd.dvb.file");
            public static readonly Audio VendorEveradPlj = new("vnd.everad.plj");
            public static readonly Audio VendorHnsAudio = new("vnd.hns.audio");
            public static readonly Audio VendorLucentVoice = new("vnd.lucent.voice", new[] { "lvp" });
            public static readonly Audio VendorMs_PlayreadyMediaPya = new("vnd.ms-playready.media.pya", new[] { "pya" });
            public static readonly Audio VendorNokiaMobile_Xmf = new("vnd.nokia.mobile-xmf");
            public static readonly Audio VendorNortelVbk = new("vnd.nortel.vbk");
            public static readonly Audio VendorNueraEcelp4800 = new("vnd.nuera.ecelp4800", new[] { "ecelp4800" });
            public static readonly Audio VendorNueraEcelp7470 = new("vnd.nuera.ecelp7470", new[] { "ecelp7470" });
            public static readonly Audio VendorNueraEcelp9600 = new("vnd.nuera.ecelp9600", new[] { "ecelp9600" });
            public static readonly Audio VendorOctelSbc = new("vnd.octel.sbc");
            public static readonly Audio VendorPresonusMultitrack = new("vnd.presonus.multitrack");

            [System.Obsolete("DEPRECATED in favor of audio/qcelp")]
            public static readonly Audio VendorQcelp = new("vnd.qcelp");

            public static readonly Audio VendorRhetorex32kadpcm = new("vnd.rhetorex.32kadpcm");
            public static readonly Audio VendorRip = new("vnd.rip", new[] { "rip" });
            public static readonly Audio VendorSealedmediaSoftsealMpeg = new("vnd.sealedmedia.softseal.mpeg");
            public static readonly Audio VendorVmxCvsd = new("vnd.vmx.cvsd");
            public static readonly Audio VendorWave = new("vnd.wave", new[] { "wav" });
            public static readonly Audio VMR_WB = new("vmr-wb");
            public static readonly Audio Vorbis_Config = new("vorbis-config");
            public static readonly Audio Wav = new("wav", new[] { "wav" });
            public static readonly Audio Wave = new("wave", new[] { "wav" });
            public static readonly Audio Webm = new("webm", new[] { "weba" });
            public static readonly Audio WebMOpus = new("webm; codec=opus", new[] { "weba" });
            public static readonly Audio X_Aac = new("x-aac", new[] { "aac" });
            public static readonly Audio X_Aiff = new("x-aiff", new[] { "aif", "aiff", "aifc" });
            public static readonly Audio X_Caf = new("x-caf", new[] { "caf" });
            public static readonly Audio X_Flac = new("x-flac", new[] { "flac" });
            public static readonly Audio X_Matroska = new("x-matroska", new[] { "mka" });
            public static readonly Audio X_Mpegurl = new("x-mpegurl", new[] { "m3u" });
            public static readonly Audio X_Ms_Wax = new("x-ms-wax", new[] { "wax" });
            public static readonly Audio X_Ms_Wma = new("x-ms-wma", new[] { "wma" });
            public static readonly Audio X_Pn_Realaudio = new("x-pn-realaudio", new[] { "ram", "ra" });
            public static readonly Audio X_Pn_Realaudio_Plugin = new("x-pn-realaudio-plugin", new[] { "rmp" });
            public static readonly Audio X_Tta = new("x-tta");
            public static readonly Audio X_Wav = new("x-wav", new[] { "wav" });
            public static readonly Audio Xm = new("xm", new[] { "xm" });

            public static new readonly Audio[] Values = {
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
                OggOpus,
                OggVorbis,
                Parityfec,
                PCMA,
                PCMA_WB,
                PCMU,
                PCMU_WB,
                PrsSid,
                Qcelp,
                Raptorfec,
                Raw,
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
                TETRA_ACELP_BB,
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
                VendorWave,
                VMR_WB,
                Vorbis_Config,
                Wav,
                Wave,
                Webm,
                WebMOpus,
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
                Xm
            };
        }
    }
}
