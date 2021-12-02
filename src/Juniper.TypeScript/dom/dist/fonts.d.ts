import type { IProgress } from "juniper-tslib";
export interface FontDescription {
    fontSize: number;
    fontFamily: string;
    fontStyle?: string;
    fontVariant?: string;
    fontWeight?: string;
}
export declare function makeFont(style: FontDescription): string;
export declare function loadFont(font: string | FontDescription, testString?: string | null, onProgress?: IProgress): Promise<void>;
