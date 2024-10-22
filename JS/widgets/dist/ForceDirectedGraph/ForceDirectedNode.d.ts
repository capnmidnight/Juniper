import { GraphNode } from "@juniper-lib/collections";
import { Vec2 } from "gl-matrix";
export declare class ForceDirectedNode<T> extends GraphNode<T> {
    #private;
    readonly pinner: HTMLButtonElement;
    readonly element: HTMLElement;
    readonly mouseOffset: Vec2;
    readonly position: Vec2;
    readonly size: Vec2;
    readonly halfSize: Vec2;
    readonly dynamicForce: Vec2;
    depth: number;
    hidden: boolean;
    get pinned(): boolean;
    set pinned(v: boolean);
    get grabbed(): boolean;
    set grabbed(v: boolean);
    get moving(): boolean;
    set moving(v: boolean);
    constructor(value: T, elementClass: string, content: string | HTMLElement);
    setContent(content: string | HTMLElement): void;
    setMouseOffset(mousePoint: Vec2): void;
    computeBounds(sz: number): void;
    applyForces(nodes: Iterable<ForceDirectedNode<T>>, running: boolean, performLayout: boolean, displayDepth: number, centeringGravity: number, mousePoint: Vec2, attract: number, repel: number, attractFunc: (connected: boolean, len: number) => number, repelFunc: (connected: boolean, len: number) => number): void;
    updatePosition(center: Vec2, maxDepth: number): void;
    moveTo(mousePoint: Vec2): void;
    canDrawArrow(maxDepth: number): boolean;
    updateWeights(getWeightMod: (connected: boolean, a: T, b: T) => number, nodes: Iterable<this>): void;
    attractRepel(n2: ForceDirectedNode<T>, attract: number, attractFunc: (connected: boolean, len: number) => number, repel: number, repelFunc: (connected: boolean, len: number) => number): void;
    apply(t: number): void;
}
//# sourceMappingURL=ForceDirectedNode.d.ts.map