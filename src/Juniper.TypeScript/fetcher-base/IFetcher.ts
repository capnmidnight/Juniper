import type { MediaType } from "juniper-mediatypes";
import { IProgress } from "juniper-tslib";

export type HTTPMethods = "HEAD"
    | "OPTIONS"
    | "GET"
    | "POST"
    | "PUT"
    | "PATCH"
    | "DELETE";

export interface IRequest {
    method: HTTPMethods;
    path: string;
    timeout: number;
    withCredentials: boolean;
    headers: Map<string, string>;
}

export interface IRequestWithBody extends IRequest {
    body: any;
}

export interface IResponse<T> {
    status: number;
    content: T;
    contentType: string;
    contentLength: number;
    date: Date;
    fileName: string;
    headers: Map<string, string>;
}

export interface IFetchingService {

    setRequestVerificationToken(value: string): void;

    sendNothingGetNothing(request: IRequest): Promise<IResponse<void>>;

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<T>;
    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>>;

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T>;
    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}

export interface IFetcher {
    head(path: string | URL, base?: string | URL): IFetcherSendHeadersTimeoutCredentials & IFetcherBodilessResult;
    options(path: string | URL, base?: string | URL): IFetcherSendHeadersTimeoutCredentials & IFetcherBodilessResult;
    get(path: string | URL, base?: string | URL): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherBodiedResult;
    post(path: string | URL, base?: string | URL): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    put(path: string | URL, base?: string | URL): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    patch(path: string | URL, base?: string | URL): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    delete(path: string | URL, base?: string | URL): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherBodiedResult;
    progress(prog: IProgress): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersProgressTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherBodiedResult;
    progress(prog: IProgress): IFetcherSendHeadersTimeoutGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherSendHeadersProgressGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersProgressCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherBodiedResult;
    progress(prog: IProgress): IFetcherSendHeadersCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherSendHeadersProgressGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherSendHeadersCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherSendHeadersTimeoutGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersProgressGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersProgressGetBody & IFetcherBodiedResult;
    progress(prog: IProgress): IFetcherSendHeadersGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersTimeoutGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersTimeoutGetBody & IFetcherBodiedResult;
    timeout(value: number): IFetcherSendHeadersGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersCredentialsGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersCredentialsGetBody & IFetcherBodiedResult;
    withCredentials(): IFetcherSendHeadersGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersGetBody {
    query(name: string, value: string): IFetcherSendHeadersGetBody & IFetcherBodiedResult;
    header(name: string, value: string): IFetcherSendHeadersGetBody & IFetcherBodiedResult;
}

export interface IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressBodyTimeoutCredentialsGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersBodyTimeoutCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersProgressBodyCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersProgressBodyTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressBodyTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressBodyTimeoutGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressBodyTimeoutGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersBodyTimeoutGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersProgressBodyGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressBodyCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressBodyCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressBodyCredentialsGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersBodyCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersProgressBodyGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressTimeoutCredentialsGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersBodyTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersBodyTimeoutCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersBodyTimeoutCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersBodyCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersBodyTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressBodyGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressBodyGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressBodyGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersBodyGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersProgressGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressTimeoutGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersProgressGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersBodyTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersBodyTimeoutGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersBodyTimeoutGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersBodyGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressCredentialsGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersProgressGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersBodyCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersBodyCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersBodyCredentialsGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersBodyGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersTimeoutCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersTimeoutCredentialsGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersTimeoutGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersProgressGetBody {
    query(name: string, value: string): IFetcherSendHeadersProgressGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersProgressGetBody & IFetcherResult;
    progress(prog: IProgress): IFetcherSendHeadersGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersBodyGetBody {
    query(name: string, value: string): IFetcherSendHeadersBodyGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersBodyGetBody & IFetcherResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherSendHeadersGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersTimeoutGetBody {
    query(name: string, value: string): IFetcherSendHeadersTimeoutGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersTimeoutGetBody & IFetcherResult;
    timeout(value: number): IFetcherSendHeadersGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersCredentialsGetBody {
    query(name: string, value: string): IFetcherSendHeadersCredentialsGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersCredentialsGetBody & IFetcherResult;
    withCredentials(): IFetcherSendHeadersGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersGetBody {
    query(name: string, value: string): IFetcherSendHeadersGetBody & IFetcherResult;
    header(name: string, value: string): IFetcherSendHeadersGetBody & IFetcherResult;
}

export interface IFetcherSendHeadersTimeoutCredentials {
    query(name: string, value: string): IFetcherSendHeadersTimeoutCredentials & IFetcherBodilessResult;
    header(name: string, value: string): IFetcherSendHeadersTimeoutCredentials & IFetcherBodilessResult;
    timeout(value: number): IFetcherSendHeadersCredentials & IFetcherBodilessResult;
    withCredentials(): IFetcherSendHeadersTimeout & IFetcherBodilessResult;
}

export interface IFetcherSendHeadersTimeout {
    query(name: string, value: string): IFetcherSendHeadersTimeout & IFetcherBodilessResult;
    header(name: string, value: string): IFetcherSendHeadersTimeout & IFetcherBodilessResult;
    timeout(value: number): IFetcherSendHeaders & IFetcherBodilessResult;
}

export interface IFetcherSendHeadersCredentials {
    query(name: string, value: string): IFetcherSendHeadersCredentials & IFetcherBodilessResult;
    header(name: string, value: string): IFetcherSendHeadersCredentials & IFetcherBodilessResult;
    withCredentials(): IFetcherSendHeaders & IFetcherBodilessResult;
}

export interface IFetcherSendHeaders {
    query(name: string, value: string): IFetcherSendHeaders & IFetcherBodilessResult;
    header(name: string, value: string): IFetcherSendHeaders & IFetcherBodilessResult;
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