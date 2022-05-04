import type { IProgress } from "@juniper/tslib";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IBodilessResponse, IResponse } from "./IResponse";


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
    sendNothingGetNothing(request: IRequest): Promise<IBodilessResponse>;
    sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendSomethingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>>;
}
