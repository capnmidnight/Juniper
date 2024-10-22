import { IResponse, isBoolean, isDefined, isFunction } from "@juniper-lib/util";
import { Application_Javascript, Application_Json, MediaType, Text_Css } from "@juniper-lib/mediatypes";
import { IProgress } from "@juniper-lib/progress";
import { IFetcher, IFetcherBodiedResult } from "./IFetcher";
import { unwrapResponse } from "./unwrapResponse";

export function isAsset(obj: any): obj is BaseAsset {
    return isDefined(obj)
        && isFunction(obj.then)
        && isFunction(obj.catch)
        && isFunction(obj.finally)
        && isFunction(obj.fetch)
        && isFunction(obj.getSize);
}

export abstract class BaseAsset<ResultT = any, ErrorT = unknown> implements Promise<ResultT> {

    readonly #promise: Promise<ResultT>;

    #result: ResultT = null;
    get result(): ResultT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this.#result;
    }

    #error: ErrorT = null;
    get error(): ErrorT { return this.#error; }

    #started = false;
    get started(): boolean {
        return this.#started;
    }

    #finished = false;
    get finished(): boolean {
        return this.#finished;
    }

    #resolve: (value: ResultT) => void = null;
    #reject: (reason: ErrorT) => void = null;

    constructor(public readonly path: string, public readonly type: string | MediaType) {
        this.#promise = new Promise((resolve, reject) => {
            this.#resolve = (value: ResultT) => {
                this.#result = value;
                this.#finished = true;
                resolve(value);
            };

            this.#reject = (reason: ErrorT) => {
                this.#error = reason;
                this.#finished = true;
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
            this.#resolve(result);
        }
        catch (err) {
            this.#reject(err as ErrorT);
        }
    }

    protected abstract getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT>;

    get [Symbol.toStringTag](): string {
        return this.#promise.toString();
    }

    then<TResult1 = ResultT, TResult2 = never>(onfulfilled?: (value: ResultT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.#promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultT | TResult> {
        return this.#promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<ResultT> {
        return this.#promise.finally(onfinally);
    }
}

export class AssetWorker<ErrorT = unknown> extends BaseAsset<Worker, ErrorT> {

    readonly #workerType: WorkerType;

    constructor(path: string, workerType: WorkerType = "module") {
        super(path, Application_Javascript);
        this.#workerType = workerType;
    }

    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<Worker> {
        return fetcher
            .get(this.path)
            .progress(prog)
            .worker(this.#workerType)
            .then(unwrapResponse);
    }
}

export class AssetCustom<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {

    readonly #getter: (fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) => Promise<ResultT>

    constructor(path: string, type: string | MediaType, getter: (fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) => Promise<ResultT>) {
        super(path, type);
        this.#getter = getter;
    }

    getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        return this.#getter(fetcher, this.path, this.type, prog);
    }
}

export abstract class BaseFetchedAsset<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {

    readonly #useCache: boolean;

    constructor(path: string, type: string | MediaType, useCache: boolean);
    constructor(path: string, type: string | MediaType);
    constructor(path: string, useCache: boolean);
    constructor(path: string);
    constructor(path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean);
    constructor(path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean) {
        let type: string | MediaType;
        if (isBoolean(typeOrUseCache)) {
            useCache = typeOrUseCache;
        }
        else {
            type = typeOrUseCache;
        }
        super(path, type);

        this.#useCache = !!useCache;
    }

    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        return this.#getRequest(fetcher, prog)
            .then(unwrapResponse);
    }

    #getRequest(fetcher: IFetcher, prog?: IProgress): Promise<IResponse<ResultT>> {
        const request = fetcher
            .get(this.path)
            .useCache(this.#useCache)
            .progress(prog);
        return this.getResponse(request);
    }

    protected abstract getResponse(request: IFetcherBodiedResult): Promise<IResponse<ResultT>>;
}

export class AssetAudioBuffer<ErrorT = unknown> extends BaseFetchedAsset<AudioBuffer, ErrorT> {

    readonly #context: BaseAudioContext;

    constructor(context: BaseAudioContext, path: string);
    constructor(context: BaseAudioContext, path: string, useCache: boolean);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType, useCache: boolean);
    constructor(context: BaseAudioContext, path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean) {
        super(path, typeOrUseCache, useCache);

        this.#context = context;
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<AudioBuffer>> {
        return request.audioBuffer(this.#context, this.type);
    }
}

export class AssetFile<ErrorT = unknown> extends BaseFetchedAsset<string, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>> {
        return request.file(this.type);
    }
}

export class AssetImage<ErrorT = unknown> extends BaseFetchedAsset<HTMLImageElement, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<HTMLImageElement>> {
        return request.image(this.type);
    }
}

export class AssetObject<T, ErrorT = unknown> extends BaseFetchedAsset<T, ErrorT> {
    constructor(path: string) {
        super(path, Application_Json);
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<T>> {
        return request.object(this.type);
    }
}

export class AssetText<ErrorT = unknown> extends BaseFetchedAsset<string, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>> {
        return request.text(this.type);
    }
}

export class AssetStyleSheet<ErrorT = unknown> extends BaseFetchedAsset<void, ErrorT> {
    constructor(path: string, useCache?: boolean) {
        super(path, Text_Css, useCache);
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse> {
        return request.style();
    }
}

export class AssetScript<ErrorT = unknown> extends BaseFetchedAsset<void, ErrorT> {
    readonly #test: () => boolean = null;
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

        this.#test = test;
    }

    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse> {
        return request.script(this.#test);
    }
}