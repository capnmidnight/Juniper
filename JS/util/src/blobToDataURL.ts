export function blobToDataString(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result as string);
        reader.onerror = () => reject(reader.error);
        reader.onabort = () => reject(new Error("Read aborted"));
        reader.readAsDataURL(blob);
    });
}

export async function blobToDataURL(blob: Blob): Promise<URL> {
    return new URL(await blobToDataString(blob));
}