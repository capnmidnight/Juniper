import { IReadyable } from "@juniper-lib/events/dist/IReadyable";
import { onClick } from "./evts";
import { ButtonPrimary, HtmlRender, Elements } from "./tags";


export async function tilReady(root: Elements | string, obj: IReadyable) {
    if (!obj.isReady) {
        const button = ButtonPrimary(
            "Start",
            onClick(() => button.disabled = true, true));
        HtmlRender(root, button);
        await obj.ready;
        button.remove();
    }
}
