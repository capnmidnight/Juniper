import { isString } from "juniper-tslib";
import { elementSetClass } from "./elementSetClass";
import { Elements, elementSetText, isDisableable, isErsatzElement } from "./tags";

const types = [
    "danger",
    "dark",
    "info",
    "light",
    "primary",
    "secondary",
    "success",
    "warning"
];

export function buttonSetEnabled(button: Elements, enabled: boolean, btnType?: string, label?: string) {
    if (isErsatzElement(button)) {
        button = button.element;
    }

    if (isString(btnType)) {
        for (const type of types) {
            elementSetClass(button, enabled && type === btnType, `btn-${type}`);
            elementSetClass(button, !enabled && type === btnType, `btn-outline-${type}`);
        }
    }

    if (isDisableable(button)) {
        button.disabled = !enabled;
    }

    if (label) {
        elementSetText(button, label);
    }
}
