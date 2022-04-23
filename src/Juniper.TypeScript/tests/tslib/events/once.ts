import { TestCase } from "@juniper/tdd/tdd/TestCase";
import { once, success, TypedEvent, TypedEventBase } from "@juniper/tslib";

class WithValueEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly value: number) {
        super(type);
    }
}

class Rig extends TypedEventBase<{
    good: TypedEvent<"good">;
    withValue: WithValueEvent<"withValue">;
    bad: TypedEvent<"bad">;
}>{
    triggerGood() {
        this.dispatchEvent(new TypedEvent("good"));
    }

    triggerWithValue(value: number) {
        this.dispatchEvent(new WithValueEvent("withValue", value));
    }

    triggerBad() {
        this.dispatchEvent(new TypedEvent("bad"));
    }
}

export class OnceTests extends TestCase {

    private rig: Rig;

    override setup() {
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
        await this.throws(async () => {
            await once(this.rig, "good", timeout);
        });
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
        await this.throws(async () => {
            this.rig.triggerBad();
            await task;
        });
    }
}

export class SuccessTests extends TestCase {

    private rig: Rig;

    override setup() {
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
