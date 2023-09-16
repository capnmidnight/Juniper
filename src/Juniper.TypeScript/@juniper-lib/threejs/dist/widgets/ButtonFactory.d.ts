import { PriorityMap } from "@juniper-lib/collections/dist/PriorityMap";
import { AssetImage } from "@juniper-lib/fetcher/dist/Asset";
import { MeshBasicMaterial, PlaneGeometry } from "three";
import { MeshButton } from "./MeshButton";
export interface ButtonSpec {
    geometry: PlaneGeometry;
    enabledMaterial: MeshBasicMaterial;
    disabledMaterial: MeshBasicMaterial;
}
export declare class ButtonFactory {
    private readonly imagePaths;
    private readonly padding;
    readonly buttonFillColor: CssColorValue;
    readonly labelFillColor: CssColorValue;
    private readonly uvDescrips;
    private readonly geoms;
    private readonly ready;
    private canvas;
    private texture;
    private enabledMaterial;
    private disabledMaterial;
    readonly assets: AssetImage[];
    private readonly assetSets;
    constructor(imagePaths: PriorityMap<string, string, string>, padding: number, buttonFillColor: CssColorValue, labelFillColor: CssColorValue, debug: boolean);
    getImageSrc(setName: string, iconName: string): string;
    getMeshButton(setName: string, iconName: string, size: number): Promise<MeshButton>;
}
//# sourceMappingURL=ButtonFactory.d.ts.map