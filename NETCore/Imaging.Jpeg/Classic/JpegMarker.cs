namespace BitMiracle.LibJpeg.Classic;

/// <summary>
/// JPEG marker codes.
/// </summary>
/// <seealso href="81c88818-a5d7-4550-9ce5-024a768f7b1e.htm" target="_self">Special markers</seealso>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "<Pending>")]
public enum JpegMarker
{
    TEM = 0x01,
    SOF0 = 0xc0,
    SOF1 = 0xc1,
    SOF2 = 0xc2,
    SOF3 = 0xc3,
    DHT = 0xc4,
    SOF5 = 0xc5,
    SOF6 = 0xc6,
    SOF7 = 0xc7,
    JPG = 0xc8,
    SOF9 = 0xc9,
    SOF10 = 0xca,
    SOF11 = 0xcb,
    DAC = 0xcc,
    SOF13 = 0xcd,
    SOF14 = 0xce,
    SOF15 = 0xcf,
    RST0 = 0xd0,
    RST1 = 0xd1,
    RST2 = 0xd2,
    RST3 = 0xd3,
    RST4 = 0xd4,
    RST5 = 0xd5,
    RST6 = 0xd6,
    RST7 = 0xd7,
    SOI = 0xd8,
    EOI = 0xd9,
    SOS = 0xda,
    DQT = 0xdb,
    DNL = 0xdc,
    DRI = 0xdd,
    DHP = 0xde,
    EXP = 0xdf,
    APP0 = 0xe0,
    APP1 = 0xe1,
    APP2 = 0xe2,
    APP3 = 0xe3,
    APP4 = 0xe4,
    APP5 = 0xe5,
    APP6 = 0xe6,
    APP7 = 0xe7,
    APP8 = 0xe8,
    APP9 = 0xe9,
    APP10 = 0xea,
    APP11 = 0xeb,
    APP12 = 0xec,
    APP13 = 0xed,
    APP14 = 0xee,
    APP15 = 0xef,
    JPG0 = 0xf0,
    JPG8 = 0xf8,
    JPG13 = 0xfd,
    COM = 0xfe,
    ERROR = 0x100
}