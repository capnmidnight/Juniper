import type { MediaType } from "juniper-mediatypes";
import { IProgress } from "juniper-tslib";
import { IResponse } from "./IResponse";

export interface IFetcherBasic {
    query(name: string, value: string): this;
    header(name: string, value: string): this;
}

export interface IFetcher {
    head(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendTimeoutCredentialsCache & IFetcherBodilessResult;
    options(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendTimeoutCredentialsCache & IFetcherBodilessResult;
    get(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsCacheGetBody & IFetcherBodiedResult;
    post(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    put(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    patch(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    delete(path: string | URL, base?: string | URL): IFetcherBasic & IFetcherSendProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
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

export interface IFetcherSendProgressBodyTimeoutCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyTimeoutCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressBodyCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressBodyTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendProgressBodyTimeoutGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyTimeoutGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressBodyGetBody & IFetcherResult;
}

export interface IFetcherSendProgressBodyCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressBodyGetBody & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutCredentialsCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCredentialsCacheGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCredentialsCacheGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressTimeoutCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressTimeoutCredentialsGetBody & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendBodyTimeoutCredentialsGetBody {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendBodyCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendBodyTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendProgressBodyGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendBodyGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutCacheGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendProgressTimeoutGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherResult;
}

export interface IFetcherSendBodyTimeoutGetBody {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherBasic & IFetcherSendBodyGetBody & IFetcherResult;
}

export interface IFetcherSendProgressCredentialsCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCredentialsCacheGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressCredentialsGetBody & IFetcherResult;
}

export interface IFetcherSendProgressCredentialsGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherResult;
}

export interface IFetcherSendBodyCredentialsGetBody {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendBodyGetBody & IFetcherResult;
}

export interface IFetcherSendTimeoutCredentialsCacheGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsCacheGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeoutCredentialsGetBody & IFetcherResult;
}

export interface IFetcherSendTimeoutCredentialsGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendProgressCacheGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendProgressGetBody & IFetcherResult;
}

export interface IFetcherSendProgressGetBody {
    progress(prog: IProgress): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendBodyGetBody {
    body<T>(body: T, contentType?: string | MediaType): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendTimeoutCacheGetBody {
    timeout(value: number): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendTimeoutGetBody {
    timeout(value: number): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendCredentialsCacheGetBody {
    withCredentials(): IFetcherBasic & IFetcherSendCacheGetBody & IFetcherResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendCredentialsGetBody & IFetcherResult;
}

export interface IFetcherSendCredentialsGetBody {
    withCredentials(): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendCacheGetBody {
    useCache(enabled?: boolean): IFetcherBasic & IFetcherResult;
}

export interface IFetcherSendTimeoutCredentialsCache {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentialsCache & IFetcherBodilessResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeoutCache & IFetcherBodilessResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeoutCredentials & IFetcherBodilessResult;
}

export interface IFetcherSendTimeoutCredentials {
    timeout(value: number): IFetcherBasic & IFetcherSendCredentials & IFetcherBodilessResult;
    withCredentials(): IFetcherBasic & IFetcherSendTimeout & IFetcherBodilessResult;
}

export interface IFetcherSendTimeoutCache {
    timeout(value: number): IFetcherBasic & IFetcherSendCache & IFetcherBodilessResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendTimeout & IFetcherBodilessResult;
}

export interface IFetcherSendTimeout {
    timeout(value: number): IFetcherBasic & IFetcherBodilessResult;
}

export interface IFetcherSendCredentialsCache {
    withCredentials(): IFetcherBasic & IFetcherSendCache & IFetcherBodilessResult;
    useCache(enabled?: boolean): IFetcherBasic & IFetcherSendCredentials & IFetcherBodilessResult;
}

export interface IFetcherSendCredentials {
    withCredentials(): IFetcherBasic & IFetcherBasic & IFetcherBodilessResult;
}

export interface IFetcherSendCache {
    useCache(enabled?: boolean): IFetcherBasic & IFetcherBasic & IFetcherBodilessResult;
}

export interface IFetcherBodilessResult {
    exec(): Promise<IResponse<void>>;
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