import { IFetcher, IFetcherBodiedResult } from "@juniper-lib/fetcher";
import { Application_Json, MediaType } from "@juniper-lib/mediatypes";
import { IProgress, isBoolean, isDefined } from "@juniper-lib/tslib";
import { IResponse } from "./IResponse";

export abstract class BaseAsset<ResultT = any, ErrorT = any> implements Promise<ResultT> {

    private readonly promise: Promise<ResultT>;

    private _result: ResultT = null;
    private _error: ErrorT = null;
    private _started = false;
    private _finished = false;

    get result(): ResultT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): ErrorT {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    private resolve: (value: ResultT) => void = null;
    private reject: (reason: ErrorT) => void = null;

    constructor(public readonly path: string, public readonly type: MediaType) {
        this.promise = new Promise((resolve, reject) => {
            this.resolve = (value: ResultT) => {
                this._result = value;
                this._finished = true;
                resolve(value);
            };

            this.reject = (reason: ErrorT) => {
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
        };
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

export class AssetCustom<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {
    constructor(path: string, type: MediaType, private readonly getter: (fetcher: IFetcher, path: string, type: MediaType, prog?: IProgress) => Promise<ResultT>) {
        super(path, type);
    }

    getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        return this.getter(fetcher, this.path, this.type, prog);
    }
}

abstract class BaseFetchedAsset<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {

    private readonly useCache: boolean;

    constructor(path: string, type: MediaType, useCache: boolean);
    constructor(path: string, type: MediaType);
    constructor(path: string, useCache: boolean);
    constructor(path: string);
    constructor(path: string, typeOrUseCache?: MediaType | boolean, useCache?: boolean) {
        let type: MediaType;
        if (isBoolean(typeOrUseCache)) {
            useCache = typeOrUseCache;
        }
        else {
            type = typeOrUseCache;
        }
        super(path, type);

        this.useCache = !!useCache;
    }

    protected async getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT> {
        const response = await this.getRequest(fetcher, prog);
        return response.content;
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

export class AssetAudio<ErrorT = unknown> extends BaseFetchedAsset<HTMLAudioElement, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<HTMLAudioElement>> {
        return request.audio(false, false, this.type);
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

export class AssetVideo<ErrorT = unknown> extends BaseFetchedAsset<HTMLVideoElement, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<HTMLVideoElement>> {
        return request.video(false, false, this.type);
    }
}