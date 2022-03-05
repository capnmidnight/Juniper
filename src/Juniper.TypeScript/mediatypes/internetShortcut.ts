import { isNullOrUndefined, isString } from "juniper-tslib";
import { Application_X_Url } from "./application";

export function makeInternetShortcut(url: string | URL) {
    if (isNullOrUndefined(url)) {
        throw new Error("Cannot create a shortcut from a null URL");
    }

    if (!isString(url)) {
        url = url.href;
    }

    return new Blob([
        `[InternetShortcut]\n`,
        `URL=${url}\n`
    ], {
        type: Application_X_Url.value
    });
}

export function parseInternetShortcut(shortcut: string) {
    const match = shortcut.match(/\[InternetShortcut\]\r?\nURL=(.+)\r?\n/);

    if (!match) {
        throw new Error("Could not parse link: " + shortcut);
    }

    return match[1];
}