import { Task } from "@juniper-lib/tslib/events/Task";

export function blobToDataURL(blob: Blob): Promise<URL> {
    const task = new Task<URL>();
    const reader = new FileReader();
    reader.onload = _e => task.resolve(new URL(reader.result as string));
    reader.onerror = _e => task.reject(reader.error);
    reader.onabort = _e => task.reject(new Error("Read aborted"));
    reader.readAsDataURL(blob);
    return task;
}