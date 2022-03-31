export interface IResponse<T> {
    status: number;
    path: string;
    content: T;
    contentType: string;
    contentLength: number;
    date: Date;
    fileName: string;
    headers: Map<string, string>;
}
