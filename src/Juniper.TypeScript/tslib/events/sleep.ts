import { Task } from "../Promises";

export function sleep(milliseconds: number): Promise<void> {
    const task = new Task();
    setTimeout(task.resolve, milliseconds);
    return task;
}
