import { IResponse } from "./IFetcher";

export class ResponseTranslator {
    protected async translateResponse<T, U>(responseTask: Promise<IResponse<T>>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>> {
        const {
            status, content, contentType, contentLength, fileName, headers, date
        } = await responseTask;

        return {
            status,
            content: await translate(content),
            contentType,
            contentLength,
            fileName,
            headers,
            date
        };
    }
}
