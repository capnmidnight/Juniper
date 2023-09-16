import { Src, Title_attr } from "@juniper-lib/dom/dist/attrs";
import { ButtonPrimary, Img } from "@juniper-lib/dom/dist/tags";
import { obj, objGraph, objectSetVisible } from "../objects";
import { Widget } from "./widgets";
export class ButtonImageWidget extends Widget {
    constructor(buttons, setName, iconName) {
        const t = Title_attr(`${setName} ${iconName}`);
        super(ButtonPrimary(t, Img(t, Src(buttons.getImageSrc(setName, iconName)))), obj(`${name}-button`), "inline-block");
        this.mesh = null;
        this.load(buttons, setName, iconName);
    }
    async load(buttons, setName, iconName) {
        this.mesh = await buttons.getMeshButton(setName, iconName, 0.2);
        this.mesh.disabled = this.disabled;
        objGraph(this, this.mesh);
        this.mesh.object.visible = this.visible;
        this.mesh.addEventListener("click", () => {
            this.element.click();
        });
    }
    get disabled() {
        return this.element.disabled;
    }
    set disabled(v) {
        this.element.disabled = v;
        if (this.mesh) {
            this.mesh.disabled = v;
        }
    }
    get visible() {
        return super.visible;
    }
    set visible(v) {
        super.visible = v;
        if (this.mesh) {
            objectSetVisible(this.mesh, v);
        }
    }
}
//# sourceMappingURL=ButtonImageWidget.js.map