import type { CanvasTypes } from "juniper-dom/canvas";
import type { IFetcher } from "juniper-fetcher";
import type { PriorityMap } from "juniper-tslib";
import type { Environment, EnvironmentOptions } from "./Environment";

export interface EnvironmentConstructor {
    new(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: PriorityMap<string, string, string>,
        defaultAvatarHeight: number,
        enableFullResolution: boolean,
        options?: Partial<EnvironmentOptions>): Environment;
}

export interface EnvironmentModule {
    default: EnvironmentConstructor;
}
