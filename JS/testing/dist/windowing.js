import { isNumber, URLBuilder } from "@juniper-lib/util";
import { getUserNumber } from "./userNumber";
const windows = [];
if (!IS_WORKER) {
    // Closes all the windows.
    window.addEventListener("unload", () => {
        for (const w of windows) {
            w.close();
        }
    });
}
export function openWindow(url, xOrWidth, yOrHeight, width, height) {
    if (IS_WORKER) {
        throw new Error("Cannot open a window from a Worker.");
    }
    let opts = undefined;
    if (isNumber(width) && isNumber(height)) {
        opts = `left=${xOrWidth},top=${yOrHeight},width=${width},height=${height}`;
    }
    else if (isNumber(xOrWidth) && isNumber(yOrHeight)) {
        opts = `width=${xOrWidth},height=${yOrHeight}`;
    }
    const w = globalThis.open(url, "_blank", opts);
    if (w) {
        windows.push(w);
    }
}
/**
 * Opens a new window with a query string parameter that can be used to differentiate different test instances.
 **/
export function openSideTest() {
    if (IS_WORKER) {
        throw new Error("Cannot open a window from a Worker.");
    }
    const loc = new URLBuilder(location.href)
        .query("testUserNumber", (getUserNumber() + windows.length + 1).toString())
        .toString();
    openWindow(loc, globalThis.screenLeft + globalThis.outerWidth, 0, globalThis.innerWidth, globalThis.innerHeight);
}
//# sourceMappingURL=windowing.js.map