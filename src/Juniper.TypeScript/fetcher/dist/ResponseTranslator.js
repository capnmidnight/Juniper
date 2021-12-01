export class ResponseTranslator {
    async translateResponse(responseTask, translate) {
        const { status, content, contentType, contentLength, fileName, headers, date } = await responseTask;
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
