import { ClassName, Em, RawElementChild } from "@juniper-lib/dom";

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

export function formatMoney(inMoney: number) {
    return USDollar.format(inMoney);
}

export function moneyToFinancialHTML(inMoney: number | string) {
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

export function moneyToFinancialHTMLDOM(inMoney: number | string): RawElementChild {
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

export function parseMoney(str: string) {
    const trimmed = str.replaceAll(/[\$,]/g, "");
    const value = parseFloat(trimmed);
    if (Number.isNaN(value)) {
        return null;
    }
    return value;
}
