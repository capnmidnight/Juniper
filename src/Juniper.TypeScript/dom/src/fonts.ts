import type { IProgress } from "juniper-tslib";
import { isFunction, isObject, isPromise, isString } from "juniper-tslib";

const DEFAULT_TEST_TEXT = "The quick brown fox jumps over the lazy dog";
const loadedFonts: string[] = [];

export interface FontDescription {
    fontSize: number;
    fontFamily: string;

    fontStyle?: string;
    fontVariant?: string;
    fontWeight?: string;
}

export function makeFont(style: FontDescription): string {
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

    fontParts.push(`${style.fontSize}px`);
    fontParts.push(style.fontFamily);

    return fontParts.join(" ");
}

interface FontFace {
    family: string;
    style: string;
    weight: string;
    stretch: string;
    unicodeRange: string;
    variant: string;
    featureSettings: string;
    variationSettings: string;
    display: string;
    ascentOverride: string;
    descentOverride: string;
    lineGapOverride: string;

    status: string;

    load(): Promise<FontFace>;
    loaded: Promise<FontFace>;
};

interface FontFaceSet extends EventTarget {
    status: string;
    ready: Promise<void>;
    check(font: string, text?: string): boolean;
    load(font: string, text?: string): Promise<FontFace[]>;
}

interface FontLoadingDocument extends Document {
    fonts: FontFaceSet;
}

function isFontLoadingDocument(doc: Document): doc is FontLoadingDocument {
    return isObject((doc as any).fonts)
        && isString((doc as any).fonts.status)
        && isPromise((doc as any).fonts.ready)
        && isFunction((doc as any).fonts.check)
        && isFunction((doc as any).fonts.load);
}

export async function loadFont(font: string | FontDescription, testString: string | null = null, onProgress?: IProgress) {
    if (!isString(font)) {
        font = makeFont(font);
    }

    if (isFontLoadingDocument(document) && loadedFonts.indexOf(font) === -1) {
        testString = testString || DEFAULT_TEST_TEXT;
        if (onProgress) {
            onProgress.report(0, 1, font);
        }
        const fonts = await document.fonts.load(font, testString);
        if (onProgress) {
            onProgress.report(1, 1, font);
        }
        if (fonts.length === 0) {
            console.warn(`Failed to load font "${font}". If this is a system font, just set the object's \`value\` property, instead of calling \`loadFontAndSetText\`.`);
        }
        else {
            loadedFonts.push(font);
        }
    }
}