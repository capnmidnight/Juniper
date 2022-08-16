
/**
 * Unicode-standardized pictograms.
 **/
export class Emoji {
    props: any;

    /**
     * Creates a new Unicode-standardized pictograms.
     * @param value - a Unicode sequence.
     * @param desc - an English text description of the pictogram.
     * @param props - an optional set of properties to store with the emoji.
     */
    constructor(public value: string, public desc: string, props: any = null) {
        this.value = value;
        this.desc = desc;
        this.props = props || {};
    }

    /**
     * Determines of the provided Emoji or EmojiGroup is a subset of
     * this emoji.
     */
    contains(e: Emoji | string): boolean {
        if (e instanceof Emoji) {
            return this.contains(e.value);
        }
        else {
            return this.value.indexOf(e) >= 0;
        }
    }

    private changeStyle(expected: string, wanted: string): string {
        let value = this.value;
        if (value.endsWith(expected)) {
            value = value.substring(0, value.length - expected.length);
        }
        value += wanted;
        return value;
    }

    get textStyle(): string {
        return this.changeStyle("\uFE0F", "\uFE0E");
    }

    get emojiStyle(): string {
        return this.changeStyle("\uFE0E", "\uFE0F");
    }
}