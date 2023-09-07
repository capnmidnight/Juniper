import { onClick } from "./evts";
import { ButtonPrimary, HtmlRender } from "./tags";
export async function tilReady(root, obj) {
    if (!obj.isReady) {
        const button = ButtonPrimary("Start", onClick(() => button.disabled = true, true));
        HtmlRender(root, button);
        await obj.ready;
        button.remove();
    }
}
//# sourceMappingURL=tilReady.js.map