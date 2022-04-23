import { specialize } from "./util";

const font = specialize("font");

export const Font_Collection = font("collection", "ttc");
export const Font_Otf = font("otf", "otf");
export const Font_Sfnt = font("sfnt");
export const Font_Ttf = font("ttf", "ttf");
export const Font_Woff = font("woff", "woff");
export const Font_Woff2 = font("woff2", "woff2");