import { TypedEvent } from "@juniper-lib/events";
export declare class TypedItemSelectedEvent<ItemT, TargetT extends EventTarget = EventTarget> extends TypedEvent<"itemselected", TargetT> {
    #private;
    get item(): ItemT;
    get items(): ItemT[];
    constructor(itemOrItems: ItemT | ItemT[]);
}
//# sourceMappingURL=TypedItemSelectedEvent.d.ts.map