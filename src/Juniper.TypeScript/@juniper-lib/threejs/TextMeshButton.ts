import { IFetcher } from "@juniper-lib/fetcher";
import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { isDefined } from "@juniper-lib/tslib";
import { scaleOnHover } from "./animation/scaleOnHover";
import { assureRayTarget, RayTarget } from "./eventSystem/RayTarget";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { TextMeshLabel } from "./TextMeshLabel";

export class TextMeshButton extends TextMeshLabel {

    readonly target: RayTarget;

    constructor(fetcher: IFetcher, env: IWebXRLayerManager, name: string, value: string, textImageOptions?: Partial<TextImageOptions>) {
        super(fetcher, env, name, value, textImageOptions);
        this.target = assureRayTarget(this);
        this.target.addMesh(this.enabledImage.mesh);
        this.target.clickable = true;

        if (isDefined(value)) {
            scaleOnHover(this, true);
        }
    }
}
