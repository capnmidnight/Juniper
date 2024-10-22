import { ClassName, Em } from "@juniper-lib/dom";
const USDollar = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    currencySign: "standard"
});
const USDollarAcct = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    currencySign: "accounting"
});
export function formatMoney(inMoney) {
    return USDollar.format(inMoney);
}
export function moneyToFinancialHTML(inMoney) {
    if (typeof inMoney === "string") {
        inMoney = parseFloat(inMoney);
    }
    if (inMoney < 0) {
        return '<em class="negative-dollars">' + USDollarAcct.format(inMoney) + '</em>';
    }
    else {
        return USDollarAcct.format(inMoney);
    }
}
export function moneyToFinancialHTMLDOM(inMoney) {
    if (typeof inMoney === "string") {
        inMoney = parseFloat(inMoney);
    }
    if (inMoney < 0) {
        return Em(ClassName("negative-dollars"), USDollarAcct.format(inMoney));
    }
    else {
        return USDollarAcct.format(inMoney);
    }
}
export function parseMoney(str) {
    const trimmed = str.replaceAll(/[\$,]/g, "");
    const value = parseFloat(trimmed);
    if (Number.isNaN(value)) {
        return null;
    }
    return value;
}
//# sourceMappingURL=dollarFormatting.js.map