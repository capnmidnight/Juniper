import { IResponse } from "./IResponse";
import { assertSuccess } from "./assertSuccess";

export function unwrapResponse<T>(response: IResponse<T>): T {
    const { content } = assertSuccess(response);
    return content;
}
