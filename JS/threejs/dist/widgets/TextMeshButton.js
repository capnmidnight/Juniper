import { TextImage } from "@juniper-lib/graphics2d";
import { stringRandom } from "@juniper-lib/util";
import { isDefined } from "@juniper-lib/util";
import { FrontSide } from "three";
import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { obj, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";
export class TextMeshButton extends RayTarget {
    constructor(env, name, value, textImageOptions) {
        super(obj(name));
        this.env = env;
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
    createImage(id, opacity) {
        const image = new TextMesh(this.env, `text-${id}`, "none", this.image, {
            side: FrontSide,
            opacity
        });
        return image;
    }
    get disabled() {
        return super.disabled;
    }
    set disabled(v) {
        super.disabled = v;
        this.enabledImage.visible = !v;
        this.disabledImage.visible = v;
    }
}
//# sourceMappingURL=TextMeshButton.js.map