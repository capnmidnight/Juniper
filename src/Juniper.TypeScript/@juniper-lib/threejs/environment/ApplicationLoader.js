import { unwrapResponse } from "@juniper-lib/fetcher/unwrapResponse";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { Task } from "@juniper-lib/events/Task";
import { progressPopper } from "@juniper-lib/progress/progressPopper";
import { progressSplitWeighted } from "@juniper-lib/progress/progressSplit";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { URLBuilder } from "@juniper-lib/tslib/URLBuilder";
import { dispose } from "@juniper-lib/tslib/using";
class ApplicationLoaderEvent extends TypedEvent {
    constructor(type, appName) {
        super(type);
        this.appName = appName;
    }
}
export class ApplicationLoaderAppLoadingEvent extends ApplicationLoaderEvent {
    constructor(appName, appLoadParams) {
        super("apploading", appName);
        this.appLoadParams = appLoadParams;
    }
}
export class ApplicationLoaderAppLoadedEvent extends ApplicationLoaderEvent {
    constructor(appName, app) {
        super("apploaded", appName);
        this.app = app;
    }
}
export class ApplicationLoaderAppShownEvent extends ApplicationLoaderEvent {
    constructor(appName, app) {
        super("appshown", appName);
        this.app = app;
    }
}
class ApplicationLoadRequest {
    constructor(loader, name) {
        this.loader = loader;
        this.name = name;
        this.params = new Map();
    }
    param(name, value) {
        this.params.set(name, value);
        return this;
    }
    load(prog) {
        return this.loader(this.name, this.params, prog);
    }
}
export class ApplicationLoader extends TypedEventTarget {
    constructor(env, getAppUrl) {
        super();
        this.env = env;
        this.getAppUrl = getAppUrl;
        this.loadedModules = new Map();
        this.loadingApps = new Map();
        this.currentApps = new Map();
        this.cacheBustString = null;
    }
    [Symbol.iterator]() {
        return this.currentApps.values();
    }
    isLoaded(name) {
        return this.currentApps.has(name);
    }
    get(name) {
        return this.currentApps.get(name);
    }
    waitFor(name) {
        if (this.isLoaded(name)) {
            return Promise.resolve(this.get(name));
        }
        const task = new Task();
        this.addScopedEventListener(this, "apploaded", evt => {
            if (evt.appName === name) {
                this.removeScope(this);
                task.resolve(evt.app);
            }
        });
        return task;
    }
    async loadAppConstructor(name, prog) {
        if (!this.loadedModules.has(name)) {
            let url = this.getAppUrl(name);
            if (isDefined(this.cacheBustString)) {
                const uri = new URLBuilder(url, location.href);
                uri.query("v", this.cacheBustString);
                url = uri.toString();
            }
            const task = this.env.fetcher
                .get(url)
                .progress(prog)
                .useCache(!this.env.DEBUG)
                .module()
                .then(unwrapResponse);
            this.loadedModules.set(name, task);
        }
        else if (isDefined(prog)) {
            prog.end();
        }
        const { default: AppConstructor } = await this.loadedModules.get(name);
        return AppConstructor;
    }
    app(name) {
        return new ApplicationLoadRequest(this.loadApp.bind(this), name);
    }
    loadApp(name, paramsOrProg, prog) {
        let params = null;
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
            const appTask = this.loadAppInstance(this.env, name, params, progs.pop(10));
            this.loadingApps.set(name, appTask);
            prog = progs.pop(1);
        }
        return this.loadingApps.get(name);
    }
    async loadAppInstance(env, name, params, prog) {
        if (!this.currentApps.has(name)) {
            const [appLoad, assetLoad] = progressSplitWeighted(prog, [1, 10]);
            const App = await this.loadAppConstructor(name, appLoad);
            const app = new App(env);
            app.addEventListener("quit", () => this.unloadApp(name));
            app.addEventListener("shown", () => this.dispatchEvent(new ApplicationLoaderAppShownEvent(name, app)));
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
        return this.currentApps.get(name);
    }
    unloadApp(name) {
        const app = this.currentApps.get(name);
        setTimeout(() => app.clearEventListeners(), 100);
        dispose(app);
        this.currentApps.delete(name);
        this.loadingApps.delete(name);
    }
}
//# sourceMappingURL=ApplicationLoader.js.map