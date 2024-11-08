import { Task } from "@juniper-lib/events";
import { TestCase } from "@juniper-lib/testing";

export class TaskTests extends TestCase {
    async test_Basic() {
        const task = new Task<boolean>();
        task.resolve(true);
        const value = await task;
        this.isTrue(value);
    }

    async test_NoDoubleResolve() {
        const task = new Task();
        let counter = 0;
        task.then(() => ++counter);
        task.resolve();
        await task;
        task.resolve();
        await task;
        this.areExact(counter, 1);
    }

    async test_NoDoubleResolveAfterReset() {
        const task = new Task();
        let counter = 0;
        task.then(() => ++counter);
        task.resolve();
        await task;
        task.reset();
        task.resolve();
        await task;
        this.areExact(counter, 1);
    }

    async test_ThenableAfterAfterReset() {
        const task = new Task();
        let counter = 0;
        task.then(() => ++counter);
        task.resolve();
        await task;
        task.reset();
        task.then(() => ++counter);
        task.resolve();
        await task;
        this.areExact(counter, 2);
    }
}