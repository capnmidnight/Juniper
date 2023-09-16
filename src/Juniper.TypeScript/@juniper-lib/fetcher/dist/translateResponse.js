import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
export async function translateResponse(response, translate) {
    const { status, requestPath, responsePath, content, contentType, contentLength, fileName, headers, date } = response;
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
        date
    };
}
//# sourceMappingURL=translateResponse.js.map