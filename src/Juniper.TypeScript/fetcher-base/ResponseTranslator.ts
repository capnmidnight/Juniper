import { IResponse } from "./IResponse";

export async function translateResponse<T, U>(response: IResponse<T>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>> {
    const {
        status,
        path,
        content,
        contentType,
        contentLength,
        fileName,
        headers,
        date
    } = response;

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