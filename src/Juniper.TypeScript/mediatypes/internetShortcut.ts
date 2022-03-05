import { Application_X_Url } from "./application";

export function makeInternetShortcut(path: string) {
    return new Blob([
        `[InternetShortcut]\n`,
        `URL=${path}\n`
    ], {
        type: Application_X_Url.value
    });
}

export function resolveInternetShortcut(shortcut: string) {
    const match = shortcut.match(/\[InternetShortcut\]\r?\nURL=(.+)\r?\n/);

    if (!match) {
        throw new Error("Could not parse link: " + shortcut);
    }

    return match[1];
}