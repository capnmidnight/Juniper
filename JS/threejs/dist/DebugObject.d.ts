import { ColorRepresentation, Object3D } from "three";
export declare class DebugObject extends Object3D {
    private color?;
    private center;
    private xp;
    private yp;
    private zn;
    constructor(color?: ColorRepresentation);
    copy(source: this, recursive?: boolean): this;
    get size(): number;
    set size(v: number);
}
//# sourceMappingURL=DebugObject.d.ts.map