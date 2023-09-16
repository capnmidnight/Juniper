import { TestStates } from "./TestStates";

export class TestScore {
    state: TestStates;
    messages: string[];

    constructor(public readonly name: string) {
        this.state = TestStates.found;
        this.messages = [];
    }

    start() {
        this.state |= TestStates.started;
    }

    success() {
        this.state |= TestStates.succeeded;
    }

    fail(message: string) {
        this.state |= TestStates.failed;
        this.messages.push(message);
    }

    finish(value: string) {
        this.state |= TestStates.completed;
        if (value) {
            this.messages.push(value);
        }
    }
}
