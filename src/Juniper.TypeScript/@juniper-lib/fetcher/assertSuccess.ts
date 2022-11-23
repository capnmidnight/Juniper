import { IResponse } from "./IResponse";

export function assertSuccess<T>(response: IResponse<T>): IResponse<T> {
    if (response.status >= 400) {
        throw new Error("Resource could not be retrieved: " + response.path);
    }
    return response;
}