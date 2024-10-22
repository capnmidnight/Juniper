import { PriorityMap } from "@juniper-lib/collections";
import { CanvasTypes, CssColorValue } from "@juniper-lib/dom";
import { AssetImage } from "@juniper-lib/fetcher";
import { InstancedMesh, Object3D, PlaneGeometry, ShaderMaterial } from "three";
import { ErsatzObject } from "../../objects";
import { InstancedMeshButton } from "./InstancedMeshButton";
export declare class InstancedButtonFactory implements ErsatzObject {
    private readonly imagePaths;
    private readonly padding;
    readonly buttonFillColor: CssColorValue;
    readonly labelFillColor: CssColorValue;
    readonly content3d: Object3D;
    private readonly uvDescrips;
    private readonly ready;
    private canvas;
    private _enabledInstances;
    get enabledInstances(): InstancedMesh<PlaneGeometry, ShaderMaterial>;
    private _disabledInstances;
    get disabledInstances(): InstancedMesh<PlaneGeometry, ShaderMaterial>;
    private readonly buttons;
    readonly assets: AssetImage[];
    private readonly assetSets;
    constructor(imagePaths: PriorityMap<string, string, string>, padding: number, buttonFillColor: CssColorValue, labelFillColor: CssColorValue, debug: boolean);
    getCanvas(): Promise<CanvasTypes>;
    getInstancedMeshButton(setName: string, iconName: string, size: number): Promise<InstancedMeshButton>;
    deleteButton(btn: InstancedMeshButton): void;
    getImageSrc(setName: string, iconName: string): string;
}
//# sourceMappingURL=index.d.ts.map