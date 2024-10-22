import { success, once } from "@juniper-lib/util";
import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";
export class Confirmation extends TypedEventTarget {
    constructor() {
        super(...arguments);
        this.confirm = this.dispatchEvent.bind(this, new TypedEvent("confirm"));
        this.cancel = this.dispatchEvent.bind(this, new TypedEvent("cancel"));
    }
    confirmed() { return success(once(this, "confirm", "cancel")); }
    cancelled() { return success(once(this, "cancel", "confirm")); }
}
//# sourceMappingURL=Confirmation.js.map