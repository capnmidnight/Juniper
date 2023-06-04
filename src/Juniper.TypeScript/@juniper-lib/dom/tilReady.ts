import { IReadyable } from "@juniper-lib/events/IReadyable";
import { onClick } from "./evts";
import { ButtonPrimary, elementApply, Elements } from "./tags";


export async function tilReady(root: Elements | string, obj: IReadyable) {
    if (!obj.isReady) {
        const button = ButtonPrimary(
            "Start",
            onClick(() => button.disabled = true, true));
        elementApply(root, button);
        await obj.ready;
        button.remove();
    }
}
