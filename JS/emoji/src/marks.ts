import { G } from "./EmojiGroup";
import {
    checkMark,
    checkMarkButton,
    circledM,
    copyright,
    crossMark,
    crossMarkButton,
    curlyLoop,
    doubleCurlyLoop,
    doubleExclamationMark,
    exclamationQuestionMark,
    eightSpokedAsterisk,
    exclamationMark,
    information,
    partAlternationMark,
    questionMark,
    registered,
    tradeMark,
    wavyDash,
    whiteExclamationMark,
    whiteQuestionMark
} from ".";


export const marks = /*@__PURE__*/ (function () {
    return G(
        "Marks", "Marks", {
        doubleExclamationMark,
        exclamationQuestionMark,
        information,
        circledM,
        checkMarkButton,
        checkMark,
        eightSpokedAsterisk,
        crossMark,
        crossMarkButton,
        questionMark,
        whiteQuestionMark,
        whiteExclamationMark,
        exclamationMark,
        curlyLoop,
        doubleCurlyLoop,
        wavyDash,
        partAlternationMark,
        tradeMark,
        copyright,
        registered,
    });
})();