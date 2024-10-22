type GraphMode = "directed" | "reverse-directed" | "undirected";
export declare class ForceDirectedGraph<T> {
    #private;
    performLayout: boolean;
    get displayDepth(): number;
    set displayDepth(v: number);
    limit: number;
    cooling: boolean;
    attract: number;
    repel: number;
    centeringGravity: number;
    get running(): boolean;
    get scale(): number;
    set scale(v: number);
    get showArrows(): boolean;
    set showArrows(v: boolean);
    constructor(container: HTMLElement, getWeightMod: (connected: boolean, a: T, b: T) => number, makeElementClass: (value: T) => string, makeContent: (value: T) => string | HTMLElement);
    start(): void;
    stop(): void;
    showCycles(show: boolean, strict: boolean): void;
    get values(): T[];
    set values(v: T[]);
    clear(): void;
    connect(connections: [T, T][], mode?: GraphMode): void;
    reset(): void;
    getElement(value: T): HTMLElement;
    select(value: T): void;
    unpinAll(): void;
    fr91(): void;
}
export {};
//# sourceMappingURL=ForceDirectedGraph.d.ts.map