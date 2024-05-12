import { ElementChild, InputText, OnBlur, OnFocus, StyleAttr } from "@juniper-lib/dom";
import { formatMoney, parseMoney } from "./dollarFormatting";



export function InputCurrency(...rest: ElementChild[]) {
    function format() {
        let { selectionStart, selectionEnd, selectionDirection } = input;
        const s = input.value;
        const p = parseMoney(s) ?? 0;
        const f = formatMoney(p);
        const d = f.length - s.length;
        selectionStart += d;
        selectionEnd += d;
        input.value = f;
        input.setSelectionRange(selectionStart, selectionEnd, selectionDirection);
    }

    const input = InputText(
        StyleAttr({ "text-align": "right" }),
        OnBlur(format),
        OnFocus(format),
        ...rest);

    Object.defineProperty(input, "valueAsNumber", {
        get() { return parseMoney(input.value); },
        set(v) { input.value = formatMoney(v); }
    });

    format();

    return input;
}