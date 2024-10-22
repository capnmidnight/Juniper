export function assertSuccess(response) {
    if (response.status >= 400) {
        console.error(response);
        throw new Error(`Resource could not be retrieved: ${response.requestPath}. Reason: ${response.errorMessage}`);
    }
    return response;
}
//# sourceMappingURL=assertSuccess.js.map