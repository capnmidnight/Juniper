export interface IResponse<T = void> {
    status: number;
    path: string;
    headers: ReadonlyMap<string, string>;
    date: Date;
    fileName: string;
    contentType: string;
    contentLength: number;
    content?: T;
}

export class InternalResponse<T = void> implements IResponse<T> {
    public readonly status: number;
    public readonly path: string;
    public readonly headers: ReadonlyMap<string, string>;
    public readonly date: Date;
    public readonly fileName: string;
    public readonly contentType: string;
    public readonly contentLength: number;

    private readonly _content: T;

    constructor(response: IResponse<T>) {
        this.status = response.status;
        this.path = response.path;
        this.headers = response.headers;
        this.date = response.date;
        this.fileName = response.fileName;
        this.contentType = response.contentType;
        this.contentLength = response.contentLength;
        this._content = response.content;
    }

    get content(): T {
        if (this.status >= 400) {
            console.warn("Not a valid content response");
        }

        return this._content;
    }
}