import { Checked, ClassList, Div, For, ID, InputCheckbox, Label, OnInput, Value } from "@juniper-lib/dom"

export function BootstrapToggleSwitch(id: string, value: string | number, text: string, onInput: (evt: Event) => void, checked: boolean, ...classes: string[]) {
    return Div(
        ClassList("form-check", "form-switch"),
        InputCheckbox(
            ClassList("form-check-input", ...classes),
            ID(id),
            Value(value),
            Checked(checked),
            OnInput(onInput)
        ),
        Label(
            ClassList("form-check-label"),
            For(id),
            text
        )
    );
}