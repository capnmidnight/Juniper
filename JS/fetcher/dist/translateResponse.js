import { isDefined } from "@juniper-lib/util";
export async function translateResponse(response, translate) {
    const { status, requestPath, responsePath, content, contentType, contentLength, fileName, headers, date, errorMessage, errorObject } = response;
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
//# sourceMappingURL=translateResponse.js.map