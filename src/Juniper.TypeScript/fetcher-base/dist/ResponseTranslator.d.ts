import { IResponse } from "./IFetcher";
export declare class ResponseTranslator {
    protected translateResponse<T, U>(responseTask: Promise<IResponse<T>>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>>;
}
