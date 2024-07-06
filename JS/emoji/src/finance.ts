import { EmojiGroup } from "./EmojiGroup";
import { moneyBag, currencyExchange, heavyDollarSign, creditCard, yenBanknote, dollarBanknote, euroBanknote, poundBanknote, moneyWithWings, chartIncreasingWithYen } from ".";


export const finance = /*@__PURE__*/ (function () {
    return new EmojiGroup(
        "Finance", "Finance",
        moneyBag,
        currencyExchange,
        heavyDollarSign,
        creditCard,
        yenBanknote,
        dollarBanknote,
        euroBanknote,
        poundBanknote,
        moneyWithWings,
        //coin,
        chartIncreasingWithYen);
})();