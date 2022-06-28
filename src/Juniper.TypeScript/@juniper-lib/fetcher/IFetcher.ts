import type { MediaType } from "@juniper-lib/mediatypes";
import type { IProgress } from "@juniper-lib/tslib";
import type { BaseAsset } from "./Asset";
import type { IResponse } from "./IResponse";

export interface IFetcherBasic {
    query(name: string, value: string): this;
    header(name: string, value: string): this;
    accept(type: string | MediaType): this;
}

export interface IFetcher {
    clearCache(): Promise<void>;
    head(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendTimeoutCredentials & IFetcherBodilessResult;
    options(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendTimeoutCredentials & IFetcherBodilessResult;
    get(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsCacheGetBody & IFetcherBodiedResult;
    post(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    put(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    patch(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    delete(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    assets(progress: IProgress, ...assets: BaseAsset[]): Promise<void>;
}

export interface IFetcherSendProgressTimeoutCredentialsCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCredentialsCacheGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCredentialsCacheGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressTimeoutCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressTimeoutCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressTimeoutCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCacheGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressTimeoutGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressCredentialsCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCredentialsCacheGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressCredentialsGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendTimeoutCredentialsCacheGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsCacheGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendTimeoutCredentialsGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendProgressGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherBodiedResult;
}

export interface IFetcherSendTimeoutCacheGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendTimeoutGetBody {
    timeout(value: number): IFetcherBasic & IFetcherBodiedResult;
}

export interface IFetcherSendCredentialsCacheGetBody {
    withCredentials(): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherBodiedResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendCredentialsGetBody {
    withCredentials(): IFetcherBasic & IFetcherBodiedResult;
}

export interface IFetcherSendCacheGetBody {
    useCache(enabled?: boolean): IFetcherBasic & IFetcherBodiedResult;
}

export interface IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyTimeoutCredentialsGetBodyOrExec & IFetcherBodiedResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressBodyCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressBodyTimeoutGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutCredentialsGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressTimeoutGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressBodyTimeoutGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyTimeoutGetBodyOrExec & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressTimeoutGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressBodyGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressBodyCredentialsGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyCredentialsGetBodyOrExec & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressBodyGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendBodyTimeoutCredentialsGetBodyOrExec {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendBodyCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendBodyTimeoutGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressBodyGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyGetBodyOrExec & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendTimeoutCredentialsGetBodyOrExec {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressCredentialsGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendBodyTimeoutGetBodyOrExec {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendTimeoutGetBodyOrExec & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendBodyGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendTimeoutGetBodyOrExec {
    timeout(value: number): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendBodyCredentialsGetBodyOrExec {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendCredentialsGetBodyOrExec & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendBodyGetBodyOrExec & IFetcherResult;
}

export interface IFetcherSendCredentialsGetBodyOrExec {
    withCredentials(): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendProgressGetBodyOrExec {
    progress(prog: IProgress): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendBodyGetBodyOrExec {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendTimeoutCredentials {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentials & IFetcherBodilessResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeout & IFetcherBodilessResult;
}

export interface IFetcherSendTimeout {
    timeout(value: number): IFetcherBasic & IFetcherBodilessResult;
}

export interface IFetcherSendCredentials {
    withCredentials(): IFetcherBasic & IFetcherBasic & IFetcherBodilessResult;
}

export interface IFetcherBodilessResult {
    exec(): Promise<IResponse>;
}

export interface IFetcherBodiedResult {
    blob(acceptType?: string | MediaType): Promise<IResponse<Blob>>;
    buffer(acceptType?: string | MediaType): Promise<IResponse<ArrayBuffer>>;
    file(acceptType?: string | MediaType): Promise<IResponse<string>>;
    text(acceptType?: string | MediaType): Promise<IResponse<string>>;
    object<T>(acceptType?: string | MediaType): Promise<T>;
    xml(acceptType?: string | MediaType): Promise<IResponse<HTMLElement>>;

    image(acceptType?: string | MediaType): Promise<IResponse<HTMLImageElement>>;
    imageBitmap(acceptType?: string | MediaType): Promise<IResponse<ImageBitmap>>;
    offscreenCanvas(acceptType?: string | MediaType): Promise<IResponse<OffscreenCanvas>>;
    htmlCanvas(acceptType?: string | MediaType): Promise<IResponse<HTMLCanvasElement>>;
    canvas(acceptType?: string | MediaType): Promise<IResponse<OffscreenCanvas | HTMLCanvasElement>>;

    audio(autoPlaying: boolean, looping: boolean, acceptType?: string | MediaType): Promise<IResponse<HTMLAudioElement>>;
    audioBuffer(audioCtx: BaseAudioContext, acceptType?: string | MediaType): Promise<IResponse<AudioBuffer>>;

    video(autoPlaying: boolean, looping: boolean, acceptType?: string | MediaType): Promise<IResponse<HTMLVideoElement>>;

    script(test: () => boolean): Promise<void>;
    module<T>(): Promise<T>;
    wasm<T>(imports: Record<string, Record<string, WebAssembly.ImportValue>>): Promise<T>;

    worker(type?: WorkerType): Promise<Worker>;
}

export interface IFetcherResult extends IFetcherBodiedResult, IFetcherBodilessResult {
}