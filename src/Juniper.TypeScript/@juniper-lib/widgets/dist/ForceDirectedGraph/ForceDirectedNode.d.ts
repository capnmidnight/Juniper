import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { Vec2 } from "gl-matrix/dist/esm";
export declare class ForceDirectedNode<T> extends GraphNode<T> {
    private readonly content;
    readonly pinner: HTMLButtonElement;
    readonly element: HTMLElement;
    readonly mouseOffset: Vec2;
    readonly position: Vec2;
    readonly size: Vec2;
    readonly halfSize: Vec2;
    readonly dynamicForce: Vec2;
    private _pinned;
    depth: number;
    hidden: boolean;
    get pinned(): boolean;
    set pinned(v: boolean);
    get moving(): boolean;
    set moving(v: boolean);
    constructor(value: T, elementClass: string, content: string | HTMLElement);
    setContent(content: string | HTMLElement): void;
    setMouseOffset(mousePoint: Vec2): void;
    computeBounds(sz: number): void;
    updatePosition(center: Vec2, maxDepth: number): void;
    moveTo(mousePoint: Vec2): void;
    resetForce(): void;
    private isVisible;
    canDrawArrow(maxDepth: number): boolean;
    gravitate(gravity: number): void;
    attractRepel(n2: ForceDirectedNode<T>, attract: number, attractFunc: (connected: boolean, len: number) => number, repel: number, repelFunc: (connected: boolean, len: number) => number, getWeightMod: (connected: boolean, dist: number, a: T, b: T) => number): void;
    apply(t: number): void;
}
//# sourceMappingURL=ForceDirectedNode.d.ts.map