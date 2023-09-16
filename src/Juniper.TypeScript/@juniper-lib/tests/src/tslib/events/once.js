import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { once, success } from "@juniper-lib/events/once";
class WithValueEvent extends TypedEvent {
    constructor(type, value) {
        super(type);
        this.value = value;
    }
}
class Rig extends TypedEventTarget {
    triggerGood() {
        this.dispatchEvent(new TypedEvent("good"));
    }
    triggerWithValue(value) {
        this.dispatchEvent(new WithValueEvent("withValue", value));
    }
    triggerBad() {
        this.dispatchEvent(new TypedEvent("bad"));
    }
}
export class OnceTests extends TestCase {
    setup() {
        this.rig = new Rig();
    }
    async test_Good() {
        const task = once(this.rig, "good");
        await this.doesNotThrow(async () => {
            this.rig.triggerGood();
            const evt = await task;
            this.areExact(evt.type, "good");
        });
    }
    async test_WithValue() {
        const task = once(this.rig, "withValue");
        const value = 13;
        await this.doesNotThrow(async () => {
            this.rig.triggerWithValue(value);
            const evt = await task;
            this.areExact(evt.type, "withValue");
            this.areExact(evt instanceof WithValueEvent && evt.value, value);
        });
    }
    async test_Timeout() {
        const timeout = 250;
        await this.rejects(once(this.rig, "good", timeout));
    }
    async test_NoTimeout() {
        const timeout = 250;
        const task = once(this.rig, "good", timeout);
        await this.doesNotThrow(async () => {
            this.rig.triggerGood();
            const evt = await task;
            this.areExact(evt.type, "good");
        });
    }
    async test_Bad() {
        const task = once(this.rig, "good", "bad");
        this.rig.triggerBad();
        await this.rejects(task);
    }
}
export class SuccessTests extends TestCase {
    setup() {
        this.rig = new Rig();
    }
    async test_Good() {
        const task = once(this.rig, "good");
        await this.doesNotThrow(async () => {
            this.rig.triggerGood();
            this.isTrue(await success(task));
        });
    }
    async test_WithValue() {
        const task = once(this.rig, "withValue");
        const value = 13;
        await this.doesNotThrow(async () => {
            this.rig.triggerWithValue(value);
            this.isTrue(await success(task));
            this.areExact(task.result.type, "withValue");
            this.areExact(task.result instanceof WithValueEvent && task.result.value, value);
        });
    }
    async test_Timeout() {
        const timeout = 250;
        const task = once(this.rig, "good", timeout);
        await this.doesNotThrow(async () => {
            this.isFalse(await success(task));
        });
    }
    async test_NoTimeout() {
        const timeout = 250;
        const task = once(this.rig, "good", timeout);
        await this.doesNotThrow(async () => {
            this.rig.triggerGood();
            this.isTrue(await success(task));
            this.areExact(task.result.type, "good");
        });
    }
    async test_Bad() {
        const task = once(this.rig, "good", "bad");
        await this.doesNotThrow(async () => {
            this.rig.triggerBad();
            this.isFalse(await success(task));
        });
    }
}
//# sourceMappingURL=once.js.map