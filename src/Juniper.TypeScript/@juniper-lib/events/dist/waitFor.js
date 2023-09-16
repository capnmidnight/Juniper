import { Task } from "./Task";
export function waitFor(test) {
    const task = new Task();
    const handle = setInterval(() => {
        if (test()) {
            clearInterval(handle);
            task.resolve();
        }
    }, 100);
    return task;
}
//# sourceMappingURL=waitFor.js.map