import { IResponse } from "@juniper-lib/util";

export function assertSuccess<T>(response: IResponse<T>): IResponse<T> {
    if (response.status >= 400) {
        console.error(response);
        throw new Error(`Resource could not be retrieved: ${response.requestPath}. Reason: ${response.errorMessage}`);
    }
    return response;
}
