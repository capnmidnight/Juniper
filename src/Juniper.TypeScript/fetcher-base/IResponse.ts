export interface IResponse<T = void> {
    status: number;
    path: string;
    headers: Map<string, string>;
    date: Date;
    fileName: string;
    contentType: string;
    contentLength: number;
    content: T;
}