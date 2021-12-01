import { TestStates } from "./TestStates";
export class TestScore {
    name;
    state;
    messages;
    constructor(name) {
        this.name = name;
        this.state = TestStates.found;
        this.messages = [];
    }
    start() {
        this.state |= TestStates.started;
    }
    success() {
        this.state |= TestStates.succeeded;
    }
    fail(message) {
        this.state |= TestStates.failed;
        this.messages.push(message);
    }
    finish(value) {
        this.state |= TestStates.completed;
        if (!!value) {
            this.messages.push(value);
        }
    }
}
