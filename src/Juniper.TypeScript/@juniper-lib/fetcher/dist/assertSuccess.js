export function assertSuccess(response) {
    if (response.status >= 400) {
        throw new Error("Resource could not be retrieved: " + response.requestPath);
    }
    return response;
}
//# sourceMappingURL=assertSuccess.js.map