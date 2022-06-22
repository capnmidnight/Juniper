import { IFetcher } from "@juniper-lib/fetcher";
import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { TextImage } from "@juniper-lib/graphics2d/TextImage";
import { isDefined, stringRandom } from "@juniper-lib/tslib";
import { scaleOnHover } from "../animation/scaleOnHover";
import { assureRayTarget, RayTarget } from "../eventSystem/RayTarget";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { TextMesh } from "./TextMesh";

export class TextMeshButton extends THREE.Object3D {
    private _disabled = false;

    readonly target: RayTarget;

    readonly image: TextImage;
    readonly enabledImage: TextMesh;
    readonly disabledImage: TextMesh;

    constructor(protected readonly fetcher: IFetcher,
        protected readonly env: IWebXRLayerManager,
        name: string,
        value: string,
        textImageOptions?: Partial<TextImageOptions>) {
        super();

        if (isDefined(value)) {
            this.name = name;

            textImageOptions = Object.assign({
                textFillColor: "#ffffff",
                fontFamily: "Segoe UI Emoji",
                fontSize: 20,
                minHeight: 0.25,
                maxHeight: 0.25
            }, textImageOptions, {
                value
            });

            this.image = new TextImage(textImageOptions);
            const id = stringRandom(16);

            this.enabledImage = this.createImage(`${id}-enabled`, 1);
            this.disabledImage = this.createImage(`${id}-disabled`, 0.5);
            this.disabledImage.visible = false;

            this.add(this.enabledImage, this.disabledImage);
        }

        this.target = assureRayTarget(this);
        this.target.addMesh(this.enabledImage.mesh);
        this.target.clickable = true;

        if (isDefined(value)) {
            scaleOnHover(this, true);
        }
    }

    private createImage(id: string, opacity: number) {
        const image = new TextMesh(
            this.env,
            `text-${id}`,
            this.image,
            {
                side: THREE.FrontSide,
                opacity
            });

        return image;
    }

    get disabled(): boolean {
        return this._disabled;
    }

    set disabled(v: boolean) {
        if (v !== this.disabled) {
            this._disabled = v;
            this.enabledImage.visible = !v;
            this.disabledImage.visible = v;
        }
    }
}
