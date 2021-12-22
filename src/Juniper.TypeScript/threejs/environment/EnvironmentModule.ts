import type { CanvasTypes } from "juniper-dom/canvas";
import type { IFetcher } from "juniper-fetcher";
import type { Environment } from "./Environment";

export interface EnvironmentConstructor {
    new(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: Map<string, Map<string, string>>,
        defaultAvatarHeight: number,
        enableFullResolution: boolean,
        JS_EXT?: string,
        DEBUG?: boolean): Environment;
}

export interface EnvironmentModule {
    default: EnvironmentConstructor;
}
