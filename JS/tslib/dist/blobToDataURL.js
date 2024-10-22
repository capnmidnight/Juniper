export function blobToDataString(blob) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = () => reject(reader.error);
        reader.onabort = () => reject(new Error("Read aborted"));
        reader.readAsDataURL(blob);
    });
}
export async function blobToDataURL(blob) {
    return new URL(await blobToDataString(blob));
}
//# sourceMappingURL=blobToDataURL.js.map