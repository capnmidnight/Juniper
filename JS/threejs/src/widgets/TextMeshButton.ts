import type { TextImageOptions } from "@juniper-lib/graphics2d";
import { TextImage } from "@juniper-lib/graphics2d";
import { stringRandom } from "@juniper-lib/util";
import { isDefined } from "@juniper-lib/util";
import { FrontSide } from "three";
import { scaleOnHover } from "../animation/scaleOnHover";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { RayTarget } from "../eventSystem/RayTarget";
import { obj, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";

export class TextMeshButton extends RayTarget {
    readonly image: TextImage;
    readonly enabledImage: TextMesh;
    readonly disabledImage: TextMesh;

    constructor(protected readonly env: BaseEnvironment,
        name: string,
        value: string,
        textImageOptions?: Partial<TextImageOptions>) {
        super(obj(name));

        if (isDefined(value)) {
            textImageOptions = Object.assign({
                textFillColor: "white",
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

            objGraph(this, this.enabledImage, this.disabledImage);
        }

        this.addMesh(this.enabledImage.mesh);
        this.addMesh(this.disabledImage.mesh);
        this.clickable = true;

        if (isDefined(value)) {
            scaleOnHover(this, true);
        }
    }

    private createImage(id: string, opacity: number) {
        const image = new TextMesh(
            this.env,
            `text-${id}`,
            "none",
            this.image,
            {
                side: FrontSide,
                opacity
            });

        return image;
    }

    override get disabled(): boolean {
        return super.disabled;
    }

    override set disabled(v: boolean) {
        super.disabled = v;
        this.enabledImage.visible = !v;
        this.disabledImage.visible = v;
    }
}
