import { RayTarget } from "../eventSystem/RayTarget";
import { Image2D } from "../widgets/Image2D";
export declare class MenuItem extends RayTarget {
    readonly front: Image2D;
    readonly back: Image2D;
    startX: number;
    constructor(width: number, height: number, name: string, front: Image2D, back: Image2D);
    get disabled(): boolean;
    set disabled(v: boolean);
    get clickable(): boolean;
    set clickable(v: boolean);
    private updateHover;
    get width(): number;
    get height(): number;
}
//# sourceMappingURL=MenuItem.d.ts.map