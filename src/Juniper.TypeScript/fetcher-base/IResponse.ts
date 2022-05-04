export interface IBodilessResponse {
    status: number;
    path: string;
    date: Date;
    headers: Map<string, string>;
    contentType: string;
    contentLength: number;
    fileName: string;
}

export interface IResponse<T> extends IBodilessResponse {
    content: T;
}
