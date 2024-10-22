import { BufferGeometry, Material, Mesh } from "three";
import { RayTarget } from "../eventSystem/RayTarget";
export declare class MeshButton extends RayTarget {
    protected readonly enabledMaterial: Material;
    protected readonly disabledMaterial: Material;
    protected readonly mesh: Mesh;
    constructor(name: string, geometry: BufferGeometry, enabledMaterial: Material, disabledMaterial: Material, size: number);
    get size(): number;
    set size(v: number);
    get disabled(): boolean;
    set disabled(v: boolean);
}
//# sourceMappingURL=MeshButton.d.ts.map