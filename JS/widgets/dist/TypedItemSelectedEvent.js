import { isArray, isDefined } from "@juniper-lib/util";
import { TypedEvent } from "@juniper-lib/events";
export class TypedItemSelectedEvent extends TypedEvent {
    #item;
    get item() { return this.#item; }
    #items;
    get items() { return this.#items; }
    constructor(itemOrItems) {
        super("itemselected", { bubbles: true });
        if (isArray(itemOrItems)) {
            this.#items = itemOrItems;
            this.#item = itemOrItems[0];
        }
        else if (isDefined(itemOrItems)) {
            this.#item = itemOrItems;
            this.#items = [itemOrItems];
        }
        else {
            this.#item = null;
            this.#items = [];
        }
    }
}
//# sourceMappingURL=TypedItemSelectedEvent.js.map