import { Application_Javascript, Application_Json, Text_Css } from "@juniper-lib/mediatypes";
import { isBoolean, isDefined, isFunction } from "@juniper-lib/tslib/typeChecks";
import { unwrapResponse } from "./unwrapResponse";
export function isAsset(obj) {
    return isDefined(obj)
        && isFunction(obj.then)
        && isFunction(obj.catch)
        && isFunction(obj.finally)
        && isFunction(obj.fetch)
        && isFunction(obj.getSize);
}
export class BaseAsset {
    get result() {
        if (isDefined(this.error)) {
            throw this.error;
        }
        return this._result;
    }
    get error() {
        return this._error;
    }
    get started() {
        return this._started;
    }
    get finished() {
        return this._finished;
    }
    constructor(path, type) {
        this.path = path;
        this.type = type;
        this._result = null;
        this._error = null;
        this._started = false;
        this._finished = false;
        this.resolve = null;
        this.reject = null;
        this.promise = new Promise((resolve, reject) => {
            this.resolve = (value) => {
                this._result = value;
                this._finished = true;
                resolve(value);
            };
            this.reject = (reason) => {
                this._error = reason;
                this._finished = true;
                reject(reason);
            };
        });
    }
    async getSize(fetcher) {
        try {
            const { contentLength } = await fetcher
                .head(this.path)
                .accept(this.type)
                .exec();
            return [this, contentLength || 1];
        }
        catch (exp) {
            console.warn(exp);
            return [this, 1];
        }
    }
    async fetch(fetcher, prog) {
        try {
            const result = await this.getResult(fetcher, prog);
            this.resolve(result);
        }
        catch (err) {
            this.reject(err);
        }
    }
    get [Symbol.toStringTag]() {
        return this.promise.toString();
    }
    then(onfulfilled, onrejected) {
        return this.promise.then(onfulfilled, onrejected);
    }
    catch(onrejected) {
        return this.promise.catch(onrejected);
    }
    finally(onfinally) {
        return this.promise.finally(onfinally);
    }
}
export class AssetWorker extends BaseAsset {
    constructor(path, workerType = "module") {
        super(path, Application_Javascript);
        this.workerType = workerType;
    }
    getResult(fetcher, prog) {
        return fetcher
            .get(this.path)
            .progress(prog)
            .worker(this.workerType)
            .then(unwrapResponse);
    }
}
export class AssetCustom extends BaseAsset {
    constructor(path, type, getter) {
        super(path, type);
        this.getter = getter;
    }
    getResult(fetcher, prog) {
        return this.getter(fetcher, this.path, this.type, prog);
    }
}
export class BaseFetchedAsset extends BaseAsset {
    constructor(path, typeOrUseCache, useCache) {
        let type;
        if (isBoolean(typeOrUseCache)) {
            useCache = typeOrUseCache;
        }
        else {
            type = typeOrUseCache;
        }
        super(path, type);
        this.useCache = !!useCache;
    }
    getResult(fetcher, prog) {
        return this.getRequest(fetcher, prog)
            .then(unwrapResponse);
    }
    getRequest(fetcher, prog) {
        const request = fetcher
            .get(this.path)
            .useCache(this.useCache)
            .progress(prog);
        return this.getResponse(request);
    }
}
export class AssetAudioBuffer extends BaseFetchedAsset {
    constructor(context, path, typeOrUseCache, useCache) {
        if (isBoolean(typeOrUseCache)) {
            super(path, typeOrUseCache);
        }
        else {
            super(path, typeOrUseCache, useCache);
        }
        this.context = context;
    }
    getResponse(request) {
        return request.audioBuffer(this.context, this.type);
    }
}
export class AssetFile extends BaseFetchedAsset {
    getResponse(request) {
        return request.file(this.type);
    }
}
export class AssetImage extends BaseFetchedAsset {
    getResponse(request) {
        return request.image(this.type);
    }
}
export class AssetObject extends BaseFetchedAsset {
    constructor(path) {
        super(path, Application_Json);
    }
    getResponse(request) {
        return request.object(this.type);
    }
}
export class AssetText extends BaseFetchedAsset {
    getResponse(request) {
        return request.text(this.type);
    }
}
export class AssetStyleSheet extends BaseFetchedAsset {
    constructor(path, useCache) {
        super(path, Text_Css, useCache);
    }
    getResponse(request) {
        return request.style();
    }
}
export class AssetScript extends BaseFetchedAsset {
    constructor(path, testOrUseCache, useCache) {
        let test = undefined;
        if (isBoolean(testOrUseCache)) {
            useCache = testOrUseCache;
            testOrUseCache = undefined;
        }
        if (isFunction(testOrUseCache)) {
            test = testOrUseCache;
        }
        super(path, Application_Javascript, useCache);
        this.test = null;
        this.test = test;
    }
    getResponse(request) {
        return request.script(this.test);
    }
}
//# sourceMappingURL=Asset.js.map