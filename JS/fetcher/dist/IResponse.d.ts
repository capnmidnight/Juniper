export interface IResponse<T = void> {
    status: number;
    requestPath: string;
    responsePath: string;
    headers: ReadonlyMap<string, string>;
    date: Date;
    fileName: string;
    contentType: string;
    contentLength: number;
    content: T;
}
export type ResponseCallback<T> = () => Promise<IResponse<T>>;
//# sourceMappingURL=IResponse.d.ts.map