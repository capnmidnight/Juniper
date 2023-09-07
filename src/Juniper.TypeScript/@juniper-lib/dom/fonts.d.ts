import type { IProgress } from "@juniper-lib/progress/IProgress";
export interface FontDescription {
    fontSize: number;
    fontFamily: string;
    fontStyle?: string;
    fontVariant?: string;
    fontWeight?: string;
}
export declare function makeFont(style: FontDescription): string;
export declare function loadFont(font: string | FontDescription, testString?: string | null, prog?: IProgress): Promise<void>;
//# sourceMappingURL=fonts.d.ts.map