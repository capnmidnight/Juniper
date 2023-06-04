import type { IProgress } from "@juniper-lib/progress/IProgress";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IResponse } from "./IResponse";


export interface XMLHttpRequestResponseTypeMap {
    "": void;
    "json": unknown;
    "arraybuffer": ArrayBuffer;
    "blob": Blob;
    "document": Document;
    "text": string;
}

export interface IFetchingServiceImpl {
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendSomethingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>>;
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
}
