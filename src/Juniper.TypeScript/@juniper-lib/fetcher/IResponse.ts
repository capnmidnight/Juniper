export interface IResponse<T = void> {
    status: number;
    path: string;
    headers: ReadonlyMap<string, string>;
    date: Date;
    fileName: string;
    contentType: string;
    contentLength: number;
    content: T;
}

export type ResponseCallback<T> = () => Promise<IResponse<T>>;