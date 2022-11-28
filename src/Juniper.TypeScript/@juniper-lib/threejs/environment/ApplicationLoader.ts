import { unwrapResponse } from "@juniper-lib/fetcher/unwrapResponse";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { Task } from "@juniper-lib/tslib/events/Task";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { progressPopper } from "@juniper-lib/tslib/progress/progressPopper";
import { progressSplitWeighted } from "@juniper-lib/tslib/progress/progressSplit";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { URLBuilder } from "@juniper-lib/tslib/URLBuilder";
import type { Application, ApplicationConstructor, ApplicationModule } from "./Application";
import type { Environment } from "./Environment";

class ApplicationLoaderEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly appName: string) {
        super(type);
    }
}

export class ApplicationLoaderAppLoadingEvent extends ApplicationLoaderEvent<"apploading"> {
    constructor(appName: string, public readonly appLoadParams: Map<string, unknown>) {
        super("apploading", appName);
    }
}

export class ApplicationLoaderAppLoadedEvent extends ApplicationLoaderEvent<"apploaded"> {
    constructor(appName: string, public readonly app: Application) {
        super("apploaded", appName);
    }
}

export class ApplicationLoaderAppShownEvent extends ApplicationLoaderEvent<"appshown"> {
    constructor(appName: string, public readonly app: Application) {
        super("appshown", appName);
    }
}

type LoadApp<T extends Application> = (name: string, params: Map<string, unknown>, prog?: IProgress) => Promise<T>;

class ApplicationLoadRequest<T extends Application> {

    private readonly params = new Map<string, unknown>();

    constructor(private readonly loader: LoadApp<T>, private readonly name: string) {

    }

    param(name: string, value: unknown) {
        this.params.set(name, value);
        return this;
    }

    load(prog?: IProgress) {
        return this.loader(this.name, this.params, prog);
    }
}

interface ApplicationLoaderEvents {
    apploading: ApplicationLoaderAppLoadingEvent;
    apploaded: ApplicationLoaderAppLoadedEvent;
    appshown: ApplicationLoaderAppShownEvent;
}

export class ApplicationLoader
    extends TypedEventBase<ApplicationLoaderEvents>
    implements Iterable<Application> {
    private readonly loadedModules = new Map<string, Promise<ApplicationModule>>();
    private readonly loadingApps = new Map<string, Promise<Application>>();
    private readonly currentApps = new Map<string, Application>();

    cacheBustString: string = null;

    constructor(private readonly env: Environment) {
        super();
    }

    [Symbol.iterator](): IterableIterator<Application> {
        return this.currentApps.values();
    }

    isLoaded(name: string): boolean {
        return this.currentApps.has(name);
    }

    get<T extends Application>(name: string): T {
        return this.currentApps.get(name) as T;
    }

    waitFor<T extends Application>(name: string): Promise<T> {
        if (this.isLoaded(name)) {
            return Promise.resolve(this.get<T>(name));
        }

        const task = new Task<T>();

        this.addScopedEventListener(this, "apploaded", evt => {
            if (evt.appName === name) {
                this.removeScope(this);
                task.resolve(evt.app as any as T);
            }
        });

        return task;
    }

    private async loadAppConstructor(name: string, prog?: IProgress): Promise<ApplicationConstructor> {
        if (!this.loadedModules.has(name)) {
            const JS_EXT = this.env.DEBUG ? ".js" : ".min.js";
            let url = `/js/${name}/index${JS_EXT}`;
            if (isDefined(this.cacheBustString)) {
                const uri = new URLBuilder(url, location.href);
                uri.query("v", this.cacheBustString);
                url = uri.toString();
            }

            const task = this.env.fetcher
                .get(url)
                .progress(prog)
                .useCache(!this.env.DEBUG)
                .module<ApplicationModule>()
                .then(unwrapResponse);

            this.loadedModules.set(name, task);
        }
        else if (isDefined(prog)) {
            prog.end();
        }

        const { default: AppConstructor } = await this.loadedModules.get(name);
        return AppConstructor;
    }

    app<T extends Application>(name: string) {
        return new ApplicationLoadRequest<T>(this.loadApp.bind(this), name);
    }

    private loadApp<T extends Application>(name: string, params: Map<string, unknown>, prog?: IProgress): Promise<T>;
    private loadApp<T extends Application>(name: string, prog?: IProgress): Promise<T>;
    private loadApp<T extends Application>(name: string, paramsOrProg: Map<string, unknown> | IProgress, prog?: IProgress): Promise<T> {
        let params: Map<string, unknown> = null;
        if (paramsOrProg instanceof Map) {
            params = paramsOrProg;
        }
        else {
            prog = paramsOrProg;
        }

        prog = prog || this.env.loadingBar;

        const evt = new ApplicationLoaderAppLoadingEvent(name, params);
        this.dispatchEvent(evt);

        if (!this.loadingApps.has(name)) {
            const progs = progressPopper(prog);
            const appTask = this.loadAppInstance<T>(this.env, name, params, progs.pop(10));
            this.loadingApps.set(name, appTask);
            prog = progs.pop(1);
        }

        return this.loadingApps.get(name) as Promise<T>;
    }

    private async loadAppInstance<T extends Application>(env: Environment, name: string, params: Map<string, unknown>, prog?: IProgress): Promise<T> {
        if (!this.currentApps.has(name)) {
            const [appLoad, assetLoad] = progressSplitWeighted(prog, [1, 10]);
            const App = await this.loadAppConstructor(name, appLoad);
            const app = new App(env);

            app.addEventListener("quit", () =>
                this.unloadApp(name));

            app.addEventListener("shown", () =>
                this.dispatchEvent(new ApplicationLoaderAppShownEvent(name, app)));

            if (isDefined(params)) {
                await app.init(params);
            }

            await app.load(assetLoad);
            this.currentApps.set(name, app);

            this.dispatchEvent(new ApplicationLoaderAppLoadedEvent(name, app));
        }

        if (isDefined(prog)) {
            prog.end();
        }

        return this.currentApps.get(name) as T;
    }

    private unloadApp(name: string) {
        const app = this.currentApps.get(name);
        setTimeout(() =>
            app.clearEventListeners(), 100);
        app.dispose();
        this.currentApps.delete(name);
        this.loadingApps.delete(name);
    }
}