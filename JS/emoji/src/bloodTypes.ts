import { EmojiGroup } from "./EmojiGroup";
import { aButtonBloodType, bButtonBloodType, oButtonBloodType, aBButtonBloodType } from ".";


export const bloodTypes = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Blood Types", "Blood types",
        aButtonBloodType,
        bButtonBloodType,
        oButtonBloodType,
        aBButtonBloodType);
})();