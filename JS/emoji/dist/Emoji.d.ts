/**
 * Unicode-standardized pictograms.
 **/
export declare class Emoji {
    readonly value: string;
    readonly desc: string;
    readonly props: any;
    /**
     * Creates a new Unicode-standardized pictograms.
     * @param value - a Unicode sequence.
     * @param desc - an English text description of the pictogram.
     * @param props - an optional set of properties to store with the emoji.
     */
    constructor(value: string, desc: string, props?: any);
    /**
     * Determines of the provided Emoji or EmojiGroup is a subset of
     * this emoji.
     */
    contains(e: Emoji | string): boolean;
    private changeStyle;
    get textStyle(): string;
    get emojiStyle(): string;
}
//# sourceMappingURL=Emoji.d.ts.map