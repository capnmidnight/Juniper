import type { MediaType } from "juniper-mediatypes";
import { IProgress } from "juniper-tslib";

export type HTTPMethods = "GET"
    | "HEAD"
    | "POST";
    //| "DELETE"
    //| "OPTIONS"
    //| "PATCH"
    //| "PUT";

export interface IRequest {
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

    head(request: IRequest): Promise<IResponse<void>>;

    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getObject<T>(request: IRequest, progress: IProgress): Promise<T>;
    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;

    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>>;
    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T>;
    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}

export interface IFetcher {
    get(path: string, base?: string): IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherGetResult;
    post(path: string, base?: string): IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    head(path: string, base?: string): IFetcherHeadHeadersAndTimeoutAndWithCredentials & IFetcherHeadResult;
}

export interface IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndTimeoutAndWithCredentials & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndProgressAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndProgressAndTimeout & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgressAndTimeout {
    query(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeout & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeout & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndProgress & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgressAndWithCredentials {
    query(name: string, value: string): IFetcherGetHeadersAndProgressAndWithCredentials & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndWithCredentials & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndProgress & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherGetHeadersAndTimeoutAndWithCredentials & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndTimeoutAndWithCredentials & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgress {
    query(name: string, value: string): IFetcherGetHeadersAndProgress & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndProgress & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndTimeout {
    query(name: string, value: string): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndWithCredentials {
    query(name: string, value: string): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeaders {
    query(name: string, value: string): IFetcherGetHeaders & IFetcherGetResult;
    header(name: string, value: string): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndBodyAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndBodyAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndTimeout {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeout & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeout & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBody {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBody & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndTimeout {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndTimeout {
    query(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgress {
    query(name: string, value: string): IFetcherPostHeadersAndProgress & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndProgress & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBody {
    query(name: string, value: string): IFetcherPostHeadersAndBody & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndBody & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndTimeout {
    query(name: string, value: string): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndWithCredentials {
    query(name: string, value: string): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeaders {
    query(name: string, value: string): IFetcherPostHeaders & IFetcherPostResult;
    header(name: string, value: string): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherHeadHeadersAndTimeoutAndWithCredentials {
    query(name: string, value: string): IFetcherHeadHeadersAndTimeoutAndWithCredentials & IFetcherHeadResult;
    header(name: string, value: string): IFetcherHeadHeadersAndTimeoutAndWithCredentials & IFetcherHeadResult;
    timeout(value: number): IFetcherHeadHeadersAndWithCredentials & IFetcherHeadResult;
    withCredentials(): IFetcherHeadHeadersAndTimeout & IFetcherHeadResult;
}

export interface IFetcherHeadHeadersAndTimeout {
    query(name: string, value: string): IFetcherHeadHeadersAndTimeout & IFetcherHeadResult;
    header(name: string, value: string): IFetcherHeadHeadersAndTimeout & IFetcherHeadResult;
    timeout(value: number): IFetcherHeadHeaders & IFetcherHeadResult;
}

export interface IFetcherHeadHeadersAndWithCredentials {
    query(name: string, value: string): IFetcherHeadHeadersAndWithCredentials & IFetcherHeadResult;
    header(name: string, value: string): IFetcherHeadHeadersAndWithCredentials & IFetcherHeadResult;
    withCredentials(): IFetcherHeadHeaders & IFetcherHeadResult;
}

export interface IFetcherHeadHeaders {
    query(name: string, value: string): IFetcherHeadHeaders & IFetcherHeadResult;
    header(name: string, value: string): IFetcherHeadHeaders & IFetcherHeadResult;
}

export interface IFetcherGetResult {
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

export interface IFetcherHeadResult {
    exec(): Promise<IResponse<void>>;
}

export interface IFetcherPostResult extends IFetcherGetResult, IFetcherHeadResult {
}