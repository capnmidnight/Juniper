import type { CanvasTypes } from "@juniper-lib/dom/canvas";
import type { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import type { PriorityMap } from "@juniper-lib/tslib/collections/PriorityMap";
import type { Environment, EnvironmentOptions } from "./Environment";

export interface EnvironmentConstructor {
    new(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: PriorityMap<string, string, string>,
        buttonFillColor: CSSColorValue,
        defaultAvatarHeight: number,
        enableFullResolution: boolean,
        options?: Partial<EnvironmentOptions>): Environment;
}

export interface EnvironmentModule {
    default: EnvironmentConstructor;
}
