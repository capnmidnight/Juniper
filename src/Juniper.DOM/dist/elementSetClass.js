import { isDefined } from "juniper-tslib";
export function elementSetClass(elem, enabled, className) {
    const canEnable = isDefined(className);
    const hasEnabled = canEnable && elem.classList.contains(className);
    if (canEnable && hasEnabled !== enabled) {
        if (enabled) {
            elem.classList.add(className);
        }
        else {
            elem.classList.remove(className);
        }
    }
}
