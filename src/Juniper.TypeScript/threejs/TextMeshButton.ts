import type { TextImageOptions } from "juniper-2d/TextImage";
import { IFetcher } from "juniper-fetcher";
import { isDefined } from "juniper-tslib";
import { scaleOnHover } from "./animation/scaleOnHover";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { PlaneCollider } from "./Plane";
import { TextMeshLabel } from "./TextMeshLabel";

export class TextMeshButton extends TextMeshLabel {

    collider: PlaneCollider = null;
    isClickable = true;

    constructor(fetcher: IFetcher, env: IWebXRLayerManager, name: string, value: string, textImageOptions?: Partial<TextImageOptions>) {
        super(fetcher, env, name, value, textImageOptions);

        if (isDefined(value)) {
            this.image.addEventListener("redrawn", () => {
                this.collider.scale.x = this.image.width;
                this.collider.scale.y = this.image.height;
            });

            this.collider = new PlaneCollider(this.image.width, this.image.height);
            this.collider.name = `collider-${this.name}`;
            this.add(this.collider);

            scaleOnHover(this);
        }
    }
}
