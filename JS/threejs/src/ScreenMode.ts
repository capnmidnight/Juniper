export enum ScreenMode {
    None = "None",
    Fullscreen = "Fullscreen",
    VR = "VR",
    AR = "AR",
    Anaglyph = "Anaglyph",
    FullscreenAnaglyph = "FullscreenAnaglyph"
}


export type ScreenModeSelection = Exclude<ScreenMode, ScreenMode.FullscreenAnaglyph>;