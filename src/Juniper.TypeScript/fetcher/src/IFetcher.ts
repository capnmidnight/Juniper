import { CanvasImageTypes } from "juniper-dom";
import type { MediaType } from "juniper-mediatypes";
import { IProgress } from "juniper-tslib";

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
    get(path: string, handler?: string): IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherGetResult;
    post(path: string, handler?: string): IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    head(path: string, handler?: string): IFetcherHeadHeadersAndTimeoutAndWithCredentials & IFetcherHeadResult;
}

export interface IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndTimeoutAndWithCredentials & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndProgressAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndProgressAndTimeout & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgressAndTimeout {
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndTimeout & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndProgress & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgressAndWithCredentials {
    header(name: string, value: string): IFetcherGetHeadersAndProgressAndWithCredentials & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndProgress & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherGetHeadersAndTimeoutAndWithCredentials & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndProgress {
    header(name: string, value: string): IFetcherGetHeadersAndProgress & IFetcherGetResult;
    progress(prog: IProgress): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndTimeout {
    header(name: string, value: string): IFetcherGetHeadersAndTimeout & IFetcherGetResult;
    timeout(value: number): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeadersAndWithCredentials {
    header(name: string, value: string): IFetcherGetHeadersAndWithCredentials & IFetcherGetResult;
    withCredentials(): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherGetHeaders {
    header(name: string, value: string): IFetcherGetHeaders & IFetcherGetResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndBodyAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndBodyAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndTimeout {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndTimeout & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBodyAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBodyAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeoutAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeoutAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndBody {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndBody & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndBody & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndTimeout {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndTimeout & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndTimeout {
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndTimeout & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgressAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndProgressAndWithCredentials & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndProgress & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBodyAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndBodyAndWithCredentials & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndBody & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndTimeoutAndWithCredentials & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndProgress {
    header(name: string, value: string): IFetcherPostHeadersAndProgress & IFetcherPostResult;
    progress(prog: IProgress): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndBody {
    header(name: string, value: string): IFetcherPostHeadersAndBody & IFetcherPostResult;
    body<T>(body: T, contentType?: string | MediaType): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndTimeout {
    header(name: string, value: string): IFetcherPostHeadersAndTimeout & IFetcherPostResult;
    timeout(value: number): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeadersAndWithCredentials {
    header(name: string, value: string): IFetcherPostHeadersAndWithCredentials & IFetcherPostResult;
    withCredentials(): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherPostHeaders {
    header(name: string, value: string): IFetcherPostHeaders & IFetcherPostResult;
}

export interface IFetcherHeadHeadersAndTimeoutAndWithCredentials {
    header(name: string, value: string): IFetcherHeadHeadersAndTimeoutAndWithCredentials & IFetcherHeadResult;
    timeout(value: number): IFetcherHeadHeadersAndWithCredentials & IFetcherHeadResult;
    withCredentials(): IFetcherHeadHeadersAndTimeout & IFetcherHeadResult;
}

export interface IFetcherHeadHeadersAndTimeout {
    header(name: string, value: string): IFetcherHeadHeadersAndTimeout & IFetcherHeadResult;
    timeout(value: number): IFetcherHeadHeaders & IFetcherHeadResult;
}

export interface IFetcherHeadHeadersAndWithCredentials {
    header(name: string, value: string): IFetcherHeadHeadersAndWithCredentials & IFetcherHeadResult;
    withCredentials(): IFetcherHeadHeaders & IFetcherHeadResult;
}

export interface IFetcherHeadHeaders {
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
    canvasImage(acceptType?: string | MediaType): Promise<IResponse<CanvasImageTypes>>;

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