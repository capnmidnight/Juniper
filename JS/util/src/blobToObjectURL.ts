export function blobToObjectURL(obj: Blob | MediaSource): URL {
    return new URL(URL.createObjectURL(obj));
}