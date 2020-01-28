namespace Juniper.Console
{
    public static class VirtualKeyState
    {
        public static readonly int VK_LBUTTON = 0x01;
        public static readonly int VK_RBUTTON = 0x02;
        public static readonly int VK_CANCEL = 0x03;
        public static readonly int VK_MBUTTON = 0x04;
        //
        public static readonly int VK_XBUTTON1 = 0x05;
        public static readonly int VK_XBUTTON2 = 0x06;
        //
        public static readonly int VK_BACK = 0x08;
        public static readonly int VK_TAB = 0x09;
        //
        public static readonly int VK_CLEAR = 0x0C;
        public static readonly int VK_RETURN = 0x0D;
        //
        public static readonly int VK_SHIFT = 0x10;
        public static readonly int VK_CONTROL = 0x11;
        public static readonly int VK_MENU = 0x12;
        public static readonly int VK_PAUSE = 0x13;
        public static readonly int VK_CAPITAL = 0x14;
        //
        public static readonly int VK_KANA = 0x15;
        public static readonly int VK_HANGEUL = 0x15;  /* old name - should be here for compatibility */
        public static readonly int VK_HANGUL = 0x15;
        public static readonly int VK_JUNJA = 0x17;
        public static readonly int VK_FINAL = 0x18;
        public static readonly int VK_HANJA = 0x19;
        public static readonly int VK_KANJI = 0x19;
        //
        public static readonly int VK_ESCAPE = 0x1B;
        //
        public static readonly int VK_CONVERT = 0x1C;
        public static readonly int VK_NONCONVERT = 0x1D;
        public static readonly int VK_ACCEPT = 0x1E;
        public static readonly int VK_MODECHANGE = 0x1F;
        //
        public static readonly int VK_SPACE = 0x20;
        public static readonly int VK_PRIOR = 0x21;
        public static readonly int VK_NEXT = 0x22;
        public static readonly int VK_END = 0x23;
        public static readonly int VK_HOME = 0x24;
        public static readonly int VK_LEFT = 0x25;
        public static readonly int VK_UP = 0x26;
        public static readonly int VK_RIGHT = 0x27;
        public static readonly int VK_DOWN = 0x28;
        public static readonly int VK_SELECT = 0x29;
        public static readonly int VK_PRINT = 0x2A;
        public static readonly int VK_EXECUTE = 0x2B;
        public static readonly int VK_SNAPSHOT = 0x2C;
        public static readonly int VK_INSERT = 0x2D;
        public static readonly int VK_DELETE = 0x2E;
        public static readonly int VK_HELP = 0x2F;
        //
        public static readonly int VK_LWIN = 0x5B;
        public static readonly int VK_RWIN = 0x5C;
        public static readonly int VK_APPS = 0x5D;
        //
        public static readonly int VK_SLEEP = 0x5F;
        //
        public static readonly int VK_NUMPAD0 = 0x60;
        public static readonly int VK_NUMPAD1 = 0x61;
        public static readonly int VK_NUMPAD2 = 0x62;
        public static readonly int VK_NUMPAD3 = 0x63;
        public static readonly int VK_NUMPAD4 = 0x64;
        public static readonly int VK_NUMPAD5 = 0x65;
        public static readonly int VK_NUMPAD6 = 0x66;
        public static readonly int VK_NUMPAD7 = 0x67;
        public static readonly int VK_NUMPAD8 = 0x68;
        public static readonly int VK_NUMPAD9 = 0x69;
        public static readonly int VK_MULTIPLY = 0x6A;
        public static readonly int VK_ADD = 0x6B;
        public static readonly int VK_SEPARATOR = 0x6C;
        public static readonly int VK_SUBTRACT = 0x6D;
        public static readonly int VK_DECIMAL = 0x6E;
        public static readonly int VK_DIVIDE = 0x6F;
        public static readonly int VK_F1 = 0x70;
        public static readonly int VK_F2 = 0x71;
        public static readonly int VK_F3 = 0x72;
        public static readonly int VK_F4 = 0x73;
        public static readonly int VK_F5 = 0x74;
        public static readonly int VK_F6 = 0x75;
        public static readonly int VK_F7 = 0x76;
        public static readonly int VK_F8 = 0x77;
        public static readonly int VK_F9 = 0x78;
        public static readonly int VK_F10 = 0x79;
        public static readonly int VK_F11 = 0x7A;
        public static readonly int VK_F12 = 0x7B;
        public static readonly int VK_F13 = 0x7C;
        public static readonly int VK_F14 = 0x7D;
        public static readonly int VK_F15 = 0x7E;
        public static readonly int VK_F16 = 0x7F;
        public static readonly int VK_F17 = 0x80;
        public static readonly int VK_F18 = 0x81;
        public static readonly int VK_F19 = 0x82;
        public static readonly int VK_F20 = 0x83;
        public static readonly int VK_F21 = 0x84;
        public static readonly int VK_F22 = 0x85;
        public static readonly int VK_F23 = 0x86;
        public static readonly int VK_F24 = 0x87;
        //
        public static readonly int VK_NUMLOCK = 0x90;
        public static readonly int VK_SCROLL = 0x91;
        //
        public static readonly int VK_OEM_NEC_EQUAL = 0x92;   // '=' key on numpad
                                                              //
        public static readonly int VK_OEM_FJ_JISHO = 0x92;   // 'Dictionary' key
        public static readonly int VK_OEM_FJ_MASSHOU = 0x93;   // 'Unregister word' key
        public static readonly int VK_OEM_FJ_TOUROKU = 0x94;   // 'Register word' key
        public static readonly int VK_OEM_FJ_LOYA = 0x95;   // 'Left OYAYUBI' key
        public static readonly int VK_OEM_FJ_ROYA = 0x96;   // 'Right OYAYUBI' key
                                                            //
        public static readonly int VK_LSHIFT = 0xA0;
        public static readonly int VK_RSHIFT = 0xA1;
        public static readonly int VK_LCONTROL = 0xA2;
        public static readonly int VK_RCONTROL = 0xA3;
        public static readonly int VK_LMENU = 0xA4;
        public static readonly int VK_RMENU = 0xA5;
        //
        public static readonly int VK_BROWSER_BACK = 0xA6;
        public static readonly int VK_BROWSER_FORWARD = 0xA7;
        public static readonly int VK_BROWSER_REFRESH = 0xA8;
        public static readonly int VK_BROWSER_STOP = 0xA9;
        public static readonly int VK_BROWSER_SEARCH = 0xAA;
        public static readonly int VK_BROWSER_FAVORITES = 0xAB;
        public static readonly int VK_BROWSER_HOME = 0xAC;
        //
        public static readonly int VK_VOLUME_MUTE = 0xAD;
        public static readonly int VK_VOLUME_DOWN = 0xAE;
        public static readonly int VK_VOLUME_UP = 0xAF;
        public static readonly int VK_MEDIA_NEXT_TRACK = 0xB0;
        public static readonly int VK_MEDIA_PREV_TRACK = 0xB1;
        public static readonly int VK_MEDIA_STOP = 0xB2;
        public static readonly int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public static readonly int VK_LAUNCH_MAIL = 0xB4;
        public static readonly int VK_LAUNCH_MEDIA_SELECT = 0xB5;
        public static readonly int VK_LAUNCH_APP1 = 0xB6;
        public static readonly int VK_LAUNCH_APP2 = 0xB7;
        //
        public static readonly int VK_OEM_1 = 0xBA;   // ';:' for US
        public static readonly int VK_OEM_PLUS = 0xBB;   // '+' any country
        public static readonly int VK_OEM_COMMA = 0xBC;   // ',' any country
        public static readonly int VK_OEM_MINUS = 0xBD;   // '-' any country
        public static readonly int VK_OEM_PERIOD = 0xBE;   // '.' any country
        public static readonly int VK_OEM_2 = 0xBF;   // '/?' for US
        public static readonly int VK_OEM_3 = 0xC0;   // '`~' for US
                                                      //
        public static readonly int VK_OEM_4 = 0xDB;  //  '[{' for US
        public static readonly int VK_OEM_5 = 0xDC;  //  '\|' for US
        public static readonly int VK_OEM_6 = 0xDD;  //  ']}' for US
        public static readonly int VK_OEM_7 = 0xDE;  //  ''"' for US
        public static readonly int VK_OEM_8 = 0xDF;
        //
        public static readonly int VK_OEM_AX = 0xE1;  //  'AX' key on Japanese AX kbd
        public static readonly int VK_OEM_102 = 0xE2;  //  "<>" or "\|" on RT 102-key kbd.
        public static readonly int VK_ICO_HELP = 0xE3;  //  Help key on ICO
        public static readonly int VK_ICO_00 = 0xE4;  //  00 key on ICO
                                                      //
        public static readonly int VK_PROCESSKEY = 0xE5;
        //
        public static readonly int VK_ICO_CLEAR = 0xE6;
        //
        public static readonly int VK_PACKET = 0xE7;
        //
        public static readonly int VK_OEM_RESET = 0xE9;
        public static readonly int VK_OEM_JUMP = 0xEA;
        public static readonly int VK_OEM_PA1 = 0xEB;
        public static readonly int VK_OEM_PA2 = 0xEC;
        public static readonly int VK_OEM_PA3 = 0xED;
        public static readonly int VK_OEM_WSCTRL = 0xEE;
        public static readonly int VK_OEM_CUSEL = 0xEF;
        public static readonly int VK_OEM_ATTN = 0xF0;
        public static readonly int VK_OEM_FINISH = 0xF1;
        public static readonly int VK_OEM_COPY = 0xF2;
        public static readonly int VK_OEM_AUTO = 0xF3;
        public static readonly int VK_OEM_ENLW = 0xF4;
        public static readonly int VK_OEM_BACKTAB = 0xF5;
        //
        public static readonly int VK_ATTN = 0xF6;
        public static readonly int VK_CRSEL = 0xF7;
        public static readonly int VK_EXSEL = 0xF8;
        public static readonly int VK_EREOF = 0xF9;
        public static readonly int VK_PLAY = 0xFA;
        public static readonly int VK_ZOOM = 0xFB;
        public static readonly int VK_NONAME = 0xFC;
        public static readonly int VK_PA1 = 0xFD;
        public static readonly int VK_OEM_CLEAR = 0xFE;
    }
}
