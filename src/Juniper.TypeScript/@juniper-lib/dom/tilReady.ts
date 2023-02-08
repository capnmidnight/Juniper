import { IReadyable } from "@juniper-lib/tslib/events/IReadyable";
import { onClick } from "./evts";
import { ButtonPrimary, elementApply } from "./tags";


export async function tilReady(root: HTMLElement, obj: IReadyable) {
    if (!obj.isReady) {
        const button = ButtonPrimary(
            "Start",
            onClick(() => button.disabled = true, true));
        elementApply(root, button);
        await obj.ready;
        button.remove();
    }
}
