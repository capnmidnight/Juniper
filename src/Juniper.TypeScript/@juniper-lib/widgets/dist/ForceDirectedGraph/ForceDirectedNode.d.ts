import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
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
    readonly name: string;
    private readonly content;
    readonly pinner: HTMLButtonElement;
    readonly element: HTMLElement;
    readonly mouseOffset: [number, number];
    readonly position: [number, number];
    readonly dynamicForce: [number, number];
    readonly staticForce: [number, number];
    private hw;
    private hh;
    private _pinned;
    bounds: IFullBounds;
    depth: number;
    get pinned(): boolean;
    set pinned(v: boolean);
    constructor(value: T, name: string, content: string | HTMLElement);
    setContent(content: string | HTMLElement): void;
    setMouseOffset(mousePoint: [number, number]): void;
    computeBounds(boundsCache: BoundsCache): void;
    updatePosition(maxDepth: number): void;
    moveTo(mousePoint: number[]): void;
    resetForce(): void;
    canDrawArrow(maxDepth: number): boolean;
    updateForce(w: number, h: number, wallForce: number): void;
    applyForce(n2: ForceDirectedNode<T>, attract: number, attractFunc: (connected: boolean, len: number) => number, repel: number, repelFunc: (connected: boolean, len: number) => number, getWeightMod: (a: T, b: T, connected: boolean) => number): void;
    limit(t: number, w: number, h: number): void;
}
export {};
//# sourceMappingURL=ForceDirectedNode.d.ts.map