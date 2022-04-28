export function makeBlobURL(obj: Blob | MediaSource): URL {
    return new URL(URL.createObjectURL(obj));
}