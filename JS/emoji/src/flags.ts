import { EmojiGroup } from "./EmojiGroup";
import { crossedFlags, chequeredFlag, whiteFlag, rainbowFlag, transgenderFlag, blackFlag, pirateFlag, flagEngland, flagScotland, flagWales, triangularFlag } from ".";


export const flags = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Flags", "Basic flags",
        crossedFlags,
        chequeredFlag,
        whiteFlag,
        rainbowFlag,
        transgenderFlag,
        blackFlag,
        pirateFlag,
        flagEngland,
        flagScotland,
        flagWales,
        triangularFlag);
})();