import { singleton } from "@juniper-lib/util";
import { registerFactory, TypedHTMLInputElement } from "@juniper-lib/dom";
import { formatMoney, parseMoney } from "./dollarFormatting";
export class InputCurrencyElement extends TypedHTMLInputElement {
    get valueAsNumber() { return parseMoney(this.value); }
    set valueAsNumber(v) { this.value = formatMoney(v); }
    get type() { return "currency"; }
    set type(_v) { super.type = "text"; }
    #prepped = false;
    connectedCallback() {
        super.connectedCallback();
        if (!this.#prepped) {
            this.element.style.textAlign = "right";
            const fixCaret = () => {
                let { selectionStart, selectionEnd, selectionDirection } = this;
                selectionStart = Math.max(1, selectionStart);
                selectionEnd = Math.max(1, selectionEnd);
                this.setSelectionRange(selectionStart, selectionEnd, selectionDirection);
            };
            const format = () => {
                const s = this.value;
                const p = Math.floor(100 * parseMoney(s) || 0);
                const f = formatMoney(p / 100);
                let { selectionStart, selectionEnd, selectionDirection } = this;
                let d = f.length - s.length;
                const inCents = selectionStart + d >= f.length - 2;
                const atEnd = selectionStart === f.length;
                if (atEnd) {
                    if (d === 0) {
                        d -= 1;
                    }
                }
                else if (inCents) {
                    d = 0;
                }
                selectionStart += d;
                selectionEnd += d;
                this.value = f;
                this.setSelectionRange(selectionStart, selectionEnd, selectionDirection);
                fixCaret();
            };
            this.addEventListener("blur", format);
            this.addEventListener("focus", format);
            this.addEventListener("mouseup", fixCaret);
            this.addEventListener("keyup", format);
        }
    }
    static install() {
        return singleton("Juniper::Widgets::InputCurrencyElement", () => registerFactory("input-currency", InputCurrencyElement));
    }
}
export function InputCurrency(...rest) {
    return InputCurrencyElement.install()(...rest);
}
//# sourceMappingURL=InputCurrencyElement.js.map