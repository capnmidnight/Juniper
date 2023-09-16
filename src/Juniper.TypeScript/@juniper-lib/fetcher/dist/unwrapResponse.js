import { assertSuccess } from "./assertSuccess";
export function unwrapResponse(response) {
    const { content } = assertSuccess(response);
    return content;
}
//# sourceMappingURL=unwrapResponse.js.map