import { singleton } from "@juniper-lib/util";
import { InputDate, registerFactory, SingletonStyleBlob, SlotTag, TemplateInstance } from "@juniper-lib/dom";
import { NameAttr } from "@juniper-lib/widgets";
export class InputWithDateElement extends HTMLElement {
    constructor() {
        super();
    }
    #dateElement = null;
    #inputElement = null;
    #setup = false;
    connectedCallback() {
        if (!this.#setup) {
            this.#setup = true;
            const style = SingletonStyleBlob("Juniper::Cedrus::InputWithDate", () => []);
            const template = TemplateInstance("Juniper::Cedrus::InputWithDate", () => [
                InputDate(),
                SlotTag(NameAttr("inputElement"))
            ]);
            const shadow = this.attachShadow({ mode: "open" });
            this.#dateElement = template.querySelector("input[type='date']");
            this.#inputElement = this.querySelector("input");
            if (this.#inputElement) {
                this.#inputElement.slot = "inputElement";
            }
            shadow.append(style.cloneNode(true), template, this.#inputElement);
        }
    }
    get item() {
        return {
            date: this.#dateElement.valueAsDate,
            value: this.#inputElement.value
        };
    }
    set item(v) {
        this.#dateElement.valueAsDate = v.date;
        this.#inputElement.value = v.value;
    }
    static install() {
        return singleton("Juniper::Cedrus::InputWithDataElement", () => registerFactory("input-with-date", InputWithDateElement));
    }
}
export function InputWithDate(...rest) {
    return InputWithDateElement.install()(...rest);
}
//# sourceMappingURL=InputWithDateElement.js.map