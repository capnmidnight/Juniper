export interface IResponse<T> {
    status: number;
    content: T;
    contentType: string;
    contentLength: number;
    date: Date;
    fileName: string;
    headers: Map<string, string>;
}
