import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/EventBase";
import { once } from "@juniper-lib/events/once";


class Rig extends TypedEventBase<{
    test: TypedEvent<"test">;
}>{
    triggerBubbles() {
        const evt = new TypedEvent("test", { bubbles: true });
        this.dispatchEvent(evt);
    }

    triggerNoBubbles() {
        const evt = new TypedEvent("test", { bubbles: true });
        evt.cancelBubble = true;
        this.dispatchEvent(evt);
    }
}

export class TypedEventBaseTests extends TestCase {

    override setup() {
    }

    async test_Bubbles() {
        const parent = new Rig();
        const child = new Rig();
        child.addBubbler(parent);
        const task = once(parent, "test", 10);
        child.triggerBubbles();
        await this.resolves(task);
    }

    async test_CancelBubble() {
        const parent = new Rig();
        const child = new Rig();
        child.addBubbler(parent);
        const task = once(parent, "test", 10);
        const evt = new TypedEvent("test");
        evt.cancelBubble = true;
        child.triggerNoBubbles();
        await this.rejects(task);
    }
}
