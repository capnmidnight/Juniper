import { Task } from "@juniper-lib/tslib/events/Task";

export function blobToDataURL(blob: Blob): Promise<string> {
    const task = new Task<string>();
    const reader = new FileReader();
    reader.onload = _e => task.resolve(reader.result as string);
    reader.onerror = _e => task.reject(reader.error);
    reader.onabort = _e => task.reject(new Error("Read aborted"));
    reader.readAsDataURL(blob);
    return task;
}