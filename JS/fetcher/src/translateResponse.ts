import { IResponse, isDefined } from "@juniper-lib/util";

export async function translateResponse<T>(response: IResponse<T>): Promise<IResponse>;
export async function translateResponse<T, U>(response: IResponse<T>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>>;
export async function translateResponse<T, U>(response: IResponse<T>, translate?: (v: T) => U | Promise<U>): Promise<IResponse<U>> {
    const {
        status,
        requestPath,
        responsePath,
        content,
        contentType,
        contentLength,
        fileName,
        headers,
        date,
        errorMessage,
        errorObject
    } = response;

    return {
        status,
        requestPath,
        responsePath,
        content: isDefined(translate)
            ? await translate(content)
            : undefined,
        contentType,
        contentLength,
        fileName,
        headers,
        date,
        errorMessage,
        errorObject
    };
}