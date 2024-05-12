import { EmojiGroup } from "./EmojiGroup";
import { multiply, plus, minus, divide } from ".";


export const math = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Math", "Math",
        multiply,
        plus,
        minus,
        divide);
})();