import { singleton } from "@juniper-lib/tslib/dist/singleton";
import { isString } from "@juniper-lib/tslib/dist/typeChecks";
import { px } from "./css";
const DEFAULT_TEST_TEXT = "The quick brown fox jumps over the lazy dog";
const loadedFonts = singleton("juniper::loadedFonts", () => []);
export function makeFont(style) {
    const fontParts = [];
    if (style.fontStyle && style.fontStyle !== "normal") {
        fontParts.push(style.fontStyle);
    }
    if (style.fontVariant && style.fontVariant !== "normal") {
        fontParts.push(style.fontVariant);
    }
    if (style.fontWeight && style.fontWeight !== "normal") {
        fontParts.push(style.fontWeight);
    }
    fontParts.push(px(style.fontSize));
    fontParts.push(style.fontFamily);
    return fontParts.join(" ");
}
export async function loadFont(font, testString = null, prog) {
    if (!isString(font)) {
        font = makeFont(font);
    }
    if (loadedFonts.indexOf(font) === -1) {
        testString = testString || DEFAULT_TEST_TEXT;
        if (prog) {
            prog.start(font);
        }
        const fonts = await document.fonts.load(font, testString);
        if (prog) {
            prog.end(font);
        }
        if (fonts.length === 0) {
            console.warn(`Failed to load font "${font}". If this is a system font, just set the object's \`value\` property, instead of calling \`loadFontAndSetText\`.`);
        }
        else {
            loadedFonts.push(font);
        }
    }
}
//# sourceMappingURL=fonts.js.map