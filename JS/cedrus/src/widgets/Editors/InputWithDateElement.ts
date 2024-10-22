import { singleton } from "@juniper-lib/util";
import { ElementChild, InputDate, registerFactory, SingletonStyleBlob, SlotTag, TemplateInstance } from "@juniper-lib/dom";
import { NameAttr } from "@juniper-lib/widgets";
import { TimeSeries } from "../../models";

export class InputWithDateElement<T> extends HTMLElement {
    constructor() {
        super();
    }

    #dateElement: HTMLInputElement = null;
    #inputElement: HTMLInputElement = null;
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

            shadow.append(
                style.cloneNode(true),
                template,
                this.#inputElement
            );
        }
    }

    get item(): TimeSeries<T> {
        return {
            date: this.#dateElement.valueAsDate,
            value: this.#inputElement.value as T
        };
    }

    set item(v: TimeSeries<T>) {
        this.#dateElement.valueAsDate = v.date;
        this.#inputElement.value = v.value as any;
    }

    static install() {
        return singleton("Juniper::Cedrus::InputWithDataElement", () => registerFactory("input-with-date", InputWithDateElement));
    }
}

export function InputWithDate<T>(...rest: ElementChild[]) {
    return InputWithDateElement.install()(...rest) as InputWithDateElement<T>;
}