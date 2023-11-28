import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
export type Vec2 = [number, number];
export type Vec2WithLen = [number, number, number];
export declare function length(vec: Vec2): number;
export declare function length(x: number, y: number): number;
export declare function add(a: Vec2, b: Vec2, out: Vec2): void;
export declare function sub(a: Vec2, b: Vec2, out: Vec2): void;
export declare function zero(a: Vec2): void;
export declare function scale(a: number, b: Vec2, out: Vec2): void;
export declare function copy(a: Vec2, out: Vec2): void;
interface IBounds {
    left: number;
    top: number;
    right: number;
    bottom: number;
    width: number;
    height: number;
}
export interface IFullBounds {
    styles: CSSStyleDeclaration;
    margin: IBounds;
    border: IBounds;
    padding: IBounds;
    interior: IBounds;
}
type BoundsCache = Map<Element, IFullBounds>;
export declare class ForceDirectedNode<T> extends GraphNode<T> {
    private readonly content;
    readonly pinner: HTMLButtonElement;
    readonly element: HTMLElement;
    readonly mouseOffset: Vec2;
    readonly position: Vec2;
    readonly dynamicForce: Vec2;
    private hw;
    private hh;
    private _pinned;
    bounds: IFullBounds;
    depth: number;
    get pinned(): boolean;
    set pinned(v: boolean);
    get moving(): boolean;
    set moving(v: boolean);
    constructor(value: T, elementClass: string, content: string | HTMLElement);
    setContent(content: string | HTMLElement): void;
    setMouseOffset(mousePoint: Vec2): void;
    computeBounds(boundsCache: BoundsCache): void;
    updatePosition(cx: number, cy: number, maxDepth: number): void;
    moveTo(mousePoint: Vec2): void;
    resetForce(): void;
    canDrawArrow(maxDepth: number): boolean;
    gravitate(gravity: number): void;
    attractRepel(n2: ForceDirectedNode<T>, attract: number, attractFunc: (connected: boolean, len: number) => number, repel: number, repelFunc: (connected: boolean, len: number) => number, getWeightMod: (connected: boolean, dist: number, a: T, b: T) => number): void;
    apply(t: number, w: number, h: number): void;
}
export {};
//# sourceMappingURL=ForceDirectedNode.d.ts.map