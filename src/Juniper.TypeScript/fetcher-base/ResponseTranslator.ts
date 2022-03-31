import { IResponse } from "./IResponse";

export class ResponseTranslator {
    protected async translateResponse<T, U>(responseTask: Promise<IResponse<T>>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>> {
        const {
            status,
            path,
            content,
            contentType,
            contentLength,
            fileName,
            headers,
            date
        } = await responseTask;

        return {
            status,
            path,
            content: await translate(content),
            contentType,
            contentLength,
            fileName,
            headers,
            date
        };
    }
}
