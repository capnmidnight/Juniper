import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { InternalResponse, IResponse } from "./IResponse";

export async function translateResponse<T>(response: IResponse<T>): Promise<IResponse>;
export async function translateResponse<T, U>(response: IResponse<T>, translate: (v: T) => U | Promise<U>): Promise<IResponse<U>>;
export async function translateResponse<T, U>(response: IResponse<T>, translate?: (v: T) => U | Promise<U>): Promise<IResponse<U>> {
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

    return new InternalResponse({
        status,
        path,
        content: isDefined(translate)
            ? await translate(content)
            : undefined,
        contentType,
        contentLength,
        fileName,
        headers,
        date
    });
}