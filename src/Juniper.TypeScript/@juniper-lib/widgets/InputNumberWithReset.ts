import { Attr, className } from "@juniper-lib/dom/attrs";
import { display, gridAutoFlow, rule } from "@juniper-lib/dom/css";
import { onClick, onInput } from "@juniper-lib/dom/evts";
import { ButtonSmallSecondary, Div, elementApply, elementSetClass, elementSetText, ErsatzElement, InputNumber, Style } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib";

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

    constructor(onInputEvt: () => void, onReset: () => void, ...rest: Attr[]);
    constructor(onInputEvt: () => void, resetButton: HTMLButtonElement, ...rest: Attr[]);
    constructor(inputCallback: () => void, onResetOrResetButton: (() => void) | HTMLButtonElement, ...rest: Attr[]) {
        super();

        if (onResetOrResetButton instanceof HTMLButtonElement) {
            this.resetButton = onResetOrResetButton;
            elementSetText(this.resetButton, "Reset");
            elementSetClass(this.resetButton, false, "btn-danger");
            elementSetClass(this.resetButton, true, "btn-secondary");
        }
        else {
            this.resetButton = ButtonSmallSecondary(
                "Reset",
                onClick(onResetOrResetButton)
            );
        }

        const fireEvt = (evt: Event) => () => this.dispatchEvent(evt);

        this.element = Div(
            className("input-number-with-reset"),
            this.numberInput = InputNumber(
                ...rest,
                onInput(inputCallback),
                onInput(fireEvt(new InputEvent("input")))
            ),
            elementApply(
                this.resetButton,
                onClick(fireEvt(new TypedEvent("reset")))
            )
        );
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
        this.numberInput.disabled
            = this.resetButton.disabled
            = v;
    }
}