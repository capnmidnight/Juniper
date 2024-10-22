export interface ErsatzNode<T extends Node> {
    content: T;
}
export type Nodes<T extends Node = Node> = T | ErsatzNode<T>;
export declare function isErsatzNode<T extends Node>(obj: unknown): obj is ErsatzNode<T>;
export declare function isNodes<T extends Node>(obj: unknown): obj is T | ErsatzNode<T>;
export declare function resolveNode<T extends Node>(obj: T | ErsatzNode<T>): T;
//# sourceMappingURL=ErsatzNode.d.ts.map