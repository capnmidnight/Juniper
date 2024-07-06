
export class TypedItemSelectedEvent<T> extends Event {
    constructor(public readonly item: T, public readonly items: T[]) {
        super("itemselected");
    }
}
