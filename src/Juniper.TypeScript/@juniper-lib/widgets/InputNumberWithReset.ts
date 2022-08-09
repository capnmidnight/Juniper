import { className } from "@juniper-lib/dom/attrs";
import { display, gridAutoFlow, rule } from "@juniper-lib/dom/css";
import { HtmlEvt, onClick, onInput } from "@juniper-lib/dom/evts";
import { buttonSetEnabled, ButtonSmallSecondary, Div, elementApply, ElementChild, ErsatzElement, InputNumber, Style } from "@juniper-lib/dom/tags";
import { arrayRemoveAt, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";

Style(
    rule(".input-number-with-reset",
        display("grid"),
        gridAutoFlow("column"))
);

interface InputNumberWithResetEvents {
    input: InputEvent;
    reset: TypedEvent<"reset">;
}

export class InputNumberWithReset extends TypedEventBase<InputNumberWithResetEvents>
    implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly numberInput: HTMLInputElement;
    private readonly resetButton: HTMLButtonElement;

    constructor(...rest: ElementChild[]) {
        super();

        const fireEvt = (evt: Event) => () => this.dispatchEvent(evt);

        let inputEvt: HtmlEvt<InputEvent> = null;
        let resetEvt: HtmlEvt<TypedEvent<"reset">> = null;

        for (let i = rest.length - 1; i >= 0; --i) {
            const here = rest[i];
            if (here instanceof HtmlEvt) {
                if (here.name === "input") {
                    inputEvt = here;
                    arrayRemoveAt(rest, i);
                }
                else if (here.name === "reset") {
                    resetEvt = here;
                    arrayRemoveAt(rest, i);
                }
            }
        }

        this.element = Div(
            className("input-number-with-reset"),
            this.numberInput = InputNumber(
                ...rest,
                onInput(fireEvt(new InputEvent("input")))
            ),
            this.resetButton = ButtonSmallSecondary(
                "Reset",
                onClick(fireEvt(new TypedEvent("reset")))
            )
        );

        if (inputEvt) {
            this.addEventListener("input", inputEvt.callback);
        }

        if (resetEvt) {
            this.addEventListener("reset", resetEvt.callback);
        }

        elementApply(this, inputEvt, resetEvt);
    }

    get value(): string {
        return this.numberInput.value;
    }

    set value(v: string) {
        this.numberInput.value = v;
    }

    get valueAsNumber(): number {
        return this.numberInput.valueAsNumber;
    }

    set valueAsNumber(v: number) {
        this.numberInput.valueAsNumber = v;
    }

    get disabled(): boolean {
        return this.numberInput.disabled;
    }

    set disabled(v: boolean) {
        this.numberInput.disabled = v;
        buttonSetEnabled(this.resetButton, !v, "secondary");
    }
}