import type { IProgress } from "@juniper/progress";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IResponse } from "./IResponse";


export interface IFetchingServiceImpl {
    sendNothingGetNothing(request: IRequest): Promise<IResponse<void>>;
    sendNothingGetSomething<T>(xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendSomethingGetSomething<T>(xhrType: XMLHttpRequestResponseType, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>>;
}
