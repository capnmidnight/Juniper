import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
export declare class ForceDirectedNode<T> extends GraphNode<T> {
    readonly element: HTMLElement;
    readonly position: number[];
    readonly dynamicForce: number[];
    readonly staticForce: number[];
    wasGrabbed: boolean;
    constructor(value: T, element: HTMLElement);
    limit(t: number, w: number, h: number): void;
    updatePosition(): void;
    moveTo(mousePoint: number[]): void;
    updateForce(w: number, h: number): void;
    applyForce(n2: ForceDirectedNode<T>, attract: number, attractFunc: (connected: boolean, len: number) => number, repel: number, repelFunc: (connected: boolean, len: number) => number, getWeightMod: (a: T, b: T, connected: boolean) => number): void;
}
//# sourceMappingURL=ForceDirectedNode.d.ts.map