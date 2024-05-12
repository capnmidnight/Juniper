import { Task } from "./Task";

export function waitFor(test: () => boolean): Promise<void> {
    const task = new Task<void>();
    const handle = setInterval(() => {
        if (test()) {
            clearInterval(handle);
            task.resolve();
        }
    }, 100);
    return task;
}
