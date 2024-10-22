import { isObject } from "@juniper-lib/util";

export interface ErsatzNode<T extends Node> {
    content: T;
}

export type Nodes<T extends Node = Node> = T | ErsatzNode<T>;

export function isErsatzNode<T extends Node>(obj: unknown): obj is ErsatzNode<T> {
    return isObject(obj)
        && "content" in obj
        && (obj.content instanceof Element
            || obj.content instanceof DocumentFragment);
}
export function isNodes<T extends Node>(obj: unknown): obj is T | ErsatzNode<T> {
    return obj instanceof Node
        || isErsatzNode(obj);
}

export function resolveNode<T extends Node>(obj: T | ErsatzNode<T>) {
    if (isErsatzNode(obj)) {
        return obj.content;
    }
    return obj;
}