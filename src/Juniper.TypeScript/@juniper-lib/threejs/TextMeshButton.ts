import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { IFetcher } from "@juniper-lib/fetcher";
import { isDefined } from "@juniper-lib/tslib";
import { scaleOnHover } from "./animation/scaleOnHover";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { TextMeshLabel } from "./TextMeshLabel";
import { makeRayTarget, RayTarget } from "./eventSystem/RayTarget";

export class TextMeshButton extends TextMeshLabel {

    readonly target: RayTarget;

    constructor(fetcher: IFetcher, env: IWebXRLayerManager, name: string, value: string, textImageOptions?: Partial<TextImageOptions>) {
        super(fetcher, env, name, value, textImageOptions);
        this.target = makeRayTarget(this.enabledImage.mesh, this);
        this.target.clickable = true;

        if (isDefined(value)) {
            scaleOnHover(this);
        }
    }
}
