import { Application_Javascript, Application_Json, MediaType, Text_Css } from "@juniper-lib/mediatypes";
import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { isBoolean, isDefined, isFunction } from "@juniper-lib/tslib/dist/typeChecks";
import { IFetcher, IFetcherBodiedResult } from "./IFetcher";
import { IResponse } from "./IResponse";
import { unwrapResponse } from "./unwrapResponse";

export function isAsset(obj: any): obj is BaseAsset {
    return isDefined(obj)
        && isFunction(obj.then)
        && isFunction(obj.catch)
        && isFunction(obj.finally)
        && isFunction(obj.fetch)
        && isFunction(obj.getSize);
}

export abstract class BaseAsset<ResultT = any> implements Promise<ResultT> {

    private readonly promise: Promise<ResultT>;

    private _result: ResultT = null;
    private _error: unknown = null;
    private _started = false;
    private _finished = false;

    get result(): ResultT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): unknown {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    private resolve: (value: ResultT) => void = null;
    private reject: (reason: unknown) => void = null;

    constructor(public readonly path: string, public readonly type: string | MediaType) {
        this.promise = new Promise((resolve, reject) => {
            this.resolve = (value: ResultT) => {
                this._result = value;
                this._finished = true;
                resolve(value);
            };

            this.reject = (reason: unknown) => {
                this._error = reason;
                this._finished = true;
                reject(reason);
            };
        });
    }

    async getSize(fetcher: IFetcher): Promise<[this, number]> {
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

    async fetch(fetcher: IFetcher, prog?: IProgress) {
        try {
            const result = await this.getResult(fetcher, prog);
            this.resolve(result);
        }
        catch (err) {
            this.reject(err);
        }
    }

    protected abstract getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT>;

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = ResultT, TResult2 = never>(onfulfilled?: (value: ResultT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultT | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<ResultT> {
        return this.promise.finally(onfinally);
    }
}

export class AssetWorker extends BaseAsset<Worker> {

    constructor(path: string, private readonly workerType: WorkerType = "module") {
        super(path, Application_Javascript);
    }

    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<Worker> {
        return fetcher
            .get(this.path)
            .progress(prog)
            .worker(this.workerType)
            .then(unwrapResponse);
    }
}

export class AssetCustom<ResultT> extends BaseAsset<ResultT> {
    constructor(path: string, type: string | MediaType, private readonly getter: (fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) => Promise<ResultT>) {
        super(path, type);
    }

    getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        return this.getter(fetcher, this.path, this.type, prog);
    }
}

export abstract class BaseFetchedAsset<ResultT> extends BaseAsset<ResultT> {

    private readonly useCache: boolean;

    constructor(path: string, type: string | MediaType, useCache: boolean);
    constructor(path: string, type: string | MediaType);
    constructor(path: string, useCache: boolean);
    constructor(path: string);
    constructor(path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean) {
        let type: string | MediaType;
        if (isBoolean(typeOrUseCache)) {
            useCache = typeOrUseCache;
        }
        else {
            type = typeOrUseCache;
        }
        super(path, type);

        this.useCache = !!useCache;
    }

    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        return this.getRequest(fetcher, prog)
            .then(unwrapResponse);
    }

    private getRequest(fetcher: IFetcher, prog?: IProgress): Promise<IResponse<ResultT>> {
        const request = fetcher
            .get(this.path)
            .useCache(this.useCache)
            .progress(prog);
        return this.getResponse(request);
    }

    protected abstract getResponse(request: IFetcherBodiedResult): Promise<IResponse<ResultT>>;
}

export class AssetAudioBuffer extends BaseFetchedAsset<AudioBuffer> {

    private readonly context: BaseAudioContext;

    constructor(context: BaseAudioContext, path: string);
    constructor(context: BaseAudioContext, path: string, useCache: boolean);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType, useCache: boolean);
    constructor(context: BaseAudioContext, path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean) {
        if (isBoolean(typeOrUseCache)) {
            super(path, typeOrUseCache);
        }
        else {
            super(path, typeOrUseCache, useCache);
        }

        this.context = context;
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<AudioBuffer>> {
        return request.audioBuffer(this.context, this.type);
    }
}

export class AssetFile extends BaseFetchedAsset<string> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>> {
        return request.file(this.type);
    }
}

export class AssetImage extends BaseFetchedAsset<HTMLImageElement> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<HTMLImageElement>> {
        return request.image(this.type);
    }
}

export class AssetObject<T> extends BaseFetchedAsset<T> {
    constructor(path: string) {
        super(path, Application_Json);
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<T>> {
        return request.object(this.type);
    }
}

export class AssetText extends BaseFetchedAsset<string> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>> {
        return request.text(this.type);
    }
}

export class AssetStyleSheet extends BaseFetchedAsset<void> {
    constructor(path: string, useCache?: boolean) {
        super(path, Text_Css, useCache);
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse> {
        return request.style();
    }
}

export class AssetScript extends BaseFetchedAsset<void> {
    private readonly test: () => boolean = null;
    constructor(path: string);
    constructor(path: string, useCache?: boolean);
    constructor(path: string, test: () => boolean);
    constructor(path: string, test: () => boolean, useCache: boolean);
    constructor(path: string, testOrUseCache?: boolean | (() => boolean), useCache?: boolean) {
        let test: () => boolean = undefined;

        if (isBoolean(testOrUseCache)) {
            useCache = testOrUseCache;
            testOrUseCache = undefined;
        }

        if (isFunction(testOrUseCache)) {
            test = testOrUseCache;
        }

        super(path, Application_Javascript, useCache);

        this.test = test;
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse> {
        return request.script(this.test);
    }
}