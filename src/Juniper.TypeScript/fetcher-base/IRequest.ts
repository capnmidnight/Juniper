import { HTTPMethods } from "./HTTPMethods";

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
