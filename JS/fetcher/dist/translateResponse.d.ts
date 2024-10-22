import { IResponse } from "./IResponse";
export declare function translateResponse<T>(response: IResponse<T>): Promise<IResponse>;
export declare function translateResponse<T, U>(response: IResponse<T>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>>;
//# sourceMappingURL=translateResponse.d.ts.map