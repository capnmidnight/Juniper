import { TestCase } from "@juniper-lib/testing/dist/tdd/TestCase";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { once } from "@juniper-lib/events/dist/once";
class Rig extends TypedEventTarget {
    triggerBubbles() {
        const evt = new TypedEvent("test", { bubbles: true });
        this.dispatchEvent(evt);
    }
    triggerNoBubbles() {
        const evt = new TypedEvent("test", { bubbles: true });
        evt.stopPropagation();
        this.dispatchEvent(evt);
    }
}
export class TypedEventTargetTests extends TestCase {
    setup() {
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
        evt.stopPropagation();
        child.triggerNoBubbles();
        await this.rejects(task);
    }
}
//# sourceMappingURL=TypedEventTarget.js.map