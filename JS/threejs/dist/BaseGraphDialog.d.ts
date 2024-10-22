import { GraphNode } from "@juniper-lib/collections";
import { CssColorValue } from "@juniper-lib/dom";
import { BaseDialogElement } from "@juniper-lib/widgets";
export declare abstract class BaseGraphDialog<T> extends BaseDialogElement<void> {
    private readonly getNodeName;
    private readonly getNodeColor;
    private readonly getWeightMod;
    private readonly t;
    private readonly cooling;
    private readonly attract;
    private readonly repel;
    private readonly hideBare;
    private readonly canvas;
    private readonly g;
    private readonly timer;
    private readonly positions;
    private readonly forces;
    private readonly wasGrabbed;
    private readonly mousePoint;
    private grabbed;
    private graph;
    get w(): number;
    get h(): number;
    constructor(title: string, getNodeName: (value: T) => string, getNodeColor: (value: T) => CssColorValue, getWeightMod: (a: T, b: T, connected: boolean) => number);
    private setMouse;
    private draw;
    private applyForces;
    fr91(): void;
    refreshData(): void;
    protected setGraph(graph: GraphNode<T>[]): void;
}
//# sourceMappingURL=BaseGraphDialog.d.ts.map