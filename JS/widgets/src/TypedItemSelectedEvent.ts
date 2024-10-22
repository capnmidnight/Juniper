import { isArray, isDefined } from "@juniper-lib/util";
import { TypedEvent } from "@juniper-lib/events";

export class TypedItemSelectedEvent<ItemT, TargetT extends EventTarget = EventTarget> extends TypedEvent<"itemselected", TargetT> {
    #item: ItemT;
    get item() { return this.#item; }

    #items: ItemT[];
    get items() { return this.#items; }

    constructor(itemOrItems: ItemT | ItemT[]) {
        super("itemselected", { bubbles: true });
        if (isArray(itemOrItems)) {
            this.#items = itemOrItems;
            this.#item = itemOrItems[0];
        }
        else if(isDefined(itemOrItems)) {
            this.#item = itemOrItems;
            this.#items = [itemOrItems];
        }
        else{
            this.#item = null;
            this.#items = [];
        }
    }
}
