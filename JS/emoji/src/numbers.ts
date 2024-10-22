import { EmojiGroup } from "./EmojiGroup";
import { asterisk, numberSign, keycapAsterisk, keycapNumberSign, keycap10, digitZero, digitOne, digitTwo, digitThree, digitFour, digitFive, digitSix, digitSeven, digitEight, digitNine, keycapDigitZero, keycapDigitOne, keycapDigitTwo, keycapDigitThree, keycapDigitFour, keycapDigitFive, keycapDigitSix, keycapDigitSeven, keycapDigitEight, keycapDigitNine } from ".";

export const keycapDigits = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Keycap Digits", "Keycap Digits",
        keycapDigitZero,
        keycapDigitOne,
        keycapDigitTwo,
        keycapDigitThree,
        keycapDigitFour,
        keycapDigitFive,
        keycapDigitSix,
        keycapDigitSeven,
        keycapDigitEight,
        keycapDigitNine,
        keycap10);
})();

export const numbers = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Numbers", "Numbers",
        digitZero,
        digitOne,
        digitTwo,
        digitThree,
        digitFour,
        digitFive,
        digitSix,
        digitSeven,
        digitEight,
        digitNine,
        asterisk,
        numberSign,
        keycapDigitZero,
        keycapDigitOne,
        keycapDigitTwo,
        keycapDigitThree,
        keycapDigitFour,
        keycapDigitFive,
        keycapDigitSix,
        keycapDigitSeven,
        keycapDigitEight,
        keycapDigitNine,
        keycapAsterisk,
        keycapNumberSign,
        keycap10);
})();
