import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
import type { Application } from "./Application";
import type { Environment } from "./Environment";
declare class ApplicationLoaderEvent<T extends string> extends TypedEvent<T> {
    readonly appName: string;
    constructor(type: T, appName: string);
}
export declare class ApplicationLoaderAppLoadingEvent extends ApplicationLoaderEvent<"apploading"> {
    readonly appLoadParams: Map<string, unknown>;
    constructor(appName: string, appLoadParams: Map<string, unknown>);
}
export declare class ApplicationLoaderAppLoadedEvent extends ApplicationLoaderEvent<"apploaded"> {
    readonly app: Application;
    constructor(appName: string, app: Application);
}
export declare class ApplicationLoaderAppShownEvent extends ApplicationLoaderEvent<"appshown"> {
    readonly app: Application;
    constructor(appName: string, app: Application);
}
type LoadApp<T extends Application> = (name: string, params: Map<string, unknown>, prog?: IProgress) => Promise<T>;
declare class ApplicationLoadRequest<T extends Application> {
    private readonly loader;
    private readonly name;
    private readonly params;
    constructor(loader: LoadApp<T>, name: string);
    param(name: string, value: unknown): this;
    load(prog?: IProgress): Promise<T>;
}
type ApplicationLoaderEvents = {
    apploading: ApplicationLoaderAppLoadingEvent;
    apploaded: ApplicationLoaderAppLoadedEvent;
    appshown: ApplicationLoaderAppShownEvent;
};
export declare class ApplicationLoader extends TypedEventTarget<ApplicationLoaderEvents> implements Iterable<Application> {
    private readonly env;
    private readonly getAppUrl;
    private readonly loadedModules;
    private readonly loadingApps;
    private readonly currentApps;
    cacheBustString: string;
    constructor(env: Environment, getAppUrl: (name: string) => string);
    [Symbol.iterator](): IterableIterator<Application>;
    isLoaded(name: string): boolean;
    get<T extends Application>(name: string): T;
    waitFor<T extends Application>(name: string): Promise<T>;
    private loadAppConstructor;
    app<T extends Application>(name: string): ApplicationLoadRequest<T>;
    private loadApp;
    private loadAppInstance;
    private unloadApp;
}
export {};
//# sourceMappingURL=ApplicationLoader.d.ts.map