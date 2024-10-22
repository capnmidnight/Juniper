import { HTTPMethods } from "./HTTPMethods";
export interface IRequest {
    method: HTTPMethods;
    path: string;
    timeout: number;
    withCredentials: boolean;
    useCache: boolean;
    headers: Map<string, string>;
    retryCount: number;
}
export interface IRequestWithBody extends IRequest {
    body: any;
}
//# sourceMappingURL=IRequest.d.ts.map