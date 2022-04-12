import { isNumber, isWorker, URLBuilder } from "juniper-tslib";
import { getUserNumber } from "./userNumber";

const windows: Window[] = [];

if (!isWorker) {
    // Closes all the windows.
    window.addEventListener("unload", () => {
        for (const w of windows) {
            w.close();
        }
    });
}

/**
 * Opens a window that will be closed when the window that opened it is closed.
 * @param href - the location to load in the window
 * @param x - the screen position horizontal component
 * @param y - the screen position vertical component
 * @param width - the screen size horizontal component
 * @param height - the screen size vertical component
 */
export function openWindow(href: string | URL): void;
export function openWindow(href: string | URL, width: number, height: number): void;
export function openWindow(href: string | URL, x: number, y: number, width: number, height: number): void;
export function openWindow(url: string | URL, xOrWidth?: number, yOrHeight?: number, width?: number, height?: number): void {
    if (isWorker) {
        throw new Error("Cannot open a window from a Worker.");
    }

    let opts: string = undefined;
    if (isNumber(width) && isNumber(height)) {
        opts = `left=${xOrWidth},top=${yOrHeight},width=${width},height=${height}`;
    }
    else if (isNumber(xOrWidth) && isNumber(yOrHeight)) {
        opts = `width=${xOrWidth},height=${yOrHeight}`;
    }

    const w = window.open(url, "_blank", opts);

    if (w) {
        windows.push(w);
    }
}

/**
 * Opens a new window with a query string parameter that can be used to differentiate different test instances.
 **/
export function openSideTest() {
    if (isWorker) {
        throw new Error("Cannot open a window from a Worker.");
    }

    const loc = new URLBuilder(location.href)
        .query("testUserNumber", (getUserNumber() + windows.length + 1).toString())
        .toString();
    openWindow(
        loc,
        window.screenLeft + window.outerWidth,
        0,
        window.innerWidth,
        window.innerHeight);
}