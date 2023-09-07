/**
 * Unicode-standardized pictograms.
 **/
export class Emoji {
    /**
     * Creates a new Unicode-standardized pictograms.
     * @param value - a Unicode sequence.
     * @param desc - an English text description of the pictogram.
     * @param props - an optional set of properties to store with the emoji.
     */
    constructor(value, desc, props = null) {
        this.value = value;
        this.desc = desc;
        this.value = value;
        this.desc = desc;
        this.props = props || {};
    }
    /**
     * Determines of the provided Emoji or EmojiGroup is a subset of
     * this emoji.
     */
    contains(e) {
        if (e instanceof Emoji) {
            return this.contains(e.value);
        }
        else {
            return this.value.indexOf(e) >= 0;
        }
    }
    changeStyle(expected, wanted) {
        let value = this.value;
        if (value.endsWith(expected)) {
            value = value.substring(0, value.length - expected.length);
        }
        value += wanted;
        return value;
    }
    get textStyle() {
        return this.changeStyle("\uFE0F", "\uFE0E");
    }
    get emojiStyle() {
        return this.changeStyle("\uFE0E", "\uFE0F");
    }
}
//# sourceMappingURL=Emoji.js.map