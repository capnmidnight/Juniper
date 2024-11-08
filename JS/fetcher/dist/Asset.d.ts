import { IResponse } from "@juniper-lib/util";
import { MediaType } from "@juniper-lib/mediatypes";
import { IProgress } from "@juniper-lib/progress";
import { IFetcher, IFetcherBodiedResult } from "./IFetcher";
export declare function isAsset(obj: any): obj is BaseAsset;
export declare abstract class BaseAsset<ResultT = any, ErrorT = unknown> implements Promise<ResultT> {
    #private;
    readonly path: string;
    readonly type: string | MediaType;
    get result(): ResultT;
    get error(): ErrorT;
    get started(): boolean;
    get finished(): boolean;
    constructor(path: string, type: string | MediaType);
    getSize(fetcher: IFetcher): Promise<[this, number]>;
    fetch(fetcher: IFetcher, prog?: IProgress): Promise<void>;
    protected abstract getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT>;
    get [Symbol.toStringTag](): string;
    then<TResult1 = ResultT, TResult2 = never>(onfulfilled?: (value: ResultT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2>;
    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultT | TResult>;
    finally(onfinally?: () => void): Promise<ResultT>;
}
export declare class AssetWorker<ErrorT = unknown> extends BaseAsset<Worker, ErrorT> {
    #private;
    constructor(path: string, workerType?: WorkerType);
    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<Worker>;
}
export declare class AssetCustom<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {
    #private;
    constructor(path: string, type: string | MediaType, getter: (fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) => Promise<ResultT>);
    getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT>;
}
export declare abstract class BaseFetchedAsset<ResultT, ErrorT = unknown> extends BaseAsset<ResultT, ErrorT> {
    #private;
    constructor(path: string, type: string | MediaType, useCache: boolean);
    constructor(path: string, type: string | MediaType);
    constructor(path: string, useCache: boolean);
    constructor(path: string);
    constructor(path: string, typeOrUseCache?: string | MediaType | boolean, useCache?: boolean);
    protected getResult(fetcher: IFetcher, prog?: IProgress): Promise<ResultT>;
    protected abstract getResponse(request: IFetcherBodiedResult): Promise<IResponse<ResultT>>;
}
export declare class AssetAudioBuffer<ErrorT = unknown> extends BaseFetchedAsset<AudioBuffer, ErrorT> {
    #private;
    constructor(context: BaseAudioContext, path: string);
    constructor(context: BaseAudioContext, path: string, useCache: boolean);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType);
    constructor(context: BaseAudioContext, path: string, type: string | MediaType, useCache: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<AudioBuffer>>;
}
export declare class AssetFile<ErrorT = unknown> extends BaseFetchedAsset<string, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>>;
}
export declare class AssetImage<ErrorT = unknown> extends BaseFetchedAsset<HTMLImageElement, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<HTMLImageElement>>;
}
export declare class AssetObject<T, ErrorT = unknown> extends BaseFetchedAsset<T, ErrorT> {
    constructor(path: string);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<T>>;
}
export declare class AssetText<ErrorT = unknown> extends BaseFetchedAsset<string, ErrorT> {
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<string>>;
}
export declare class AssetStyleSheet<ErrorT = unknown> extends BaseFetchedAsset<void, ErrorT> {
    constructor(path: string, useCache?: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse>;
}
export declare class AssetScript<ErrorT = unknown> extends BaseFetchedAsset<void, ErrorT> {
    #private;
    constructor(path: string);
    constructor(path: string, useCache?: boolean);
    constructor(path: string, test: () => boolean);
    constructor(path: string, test: () => boolean, useCache: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse>;
}
//# sourceMappingURL=Asset.d.ts.map