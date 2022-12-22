import { Task } from "./events/Task";

export function blobToDataString(blob: Blob): Promise<string> {
    const task = new Task<string>();
    const reader = new FileReader();
    reader.onload = _e => task.resolve(reader.result as string);
    reader.onerror = _e => task.reject(reader.error);
    reader.onabort = _e => task.reject(new Error("Read aborted"));
    reader.readAsDataURL(blob);
    return task;
}

export async function blobToDataURL(blob: Blob): Promise<URL> {
    return new URL(await blobToDataString(blob));
}