import { elementSetEnabled, Src, TitleAttr } from "@juniper-lib/dom";
import { Button, Img } from "@juniper-lib/dom";
import { obj, objGraph, objectSetEnabled, objectSetVisible } from "../objects";
import { Widget } from "./widgets";
export class ButtonImageWidget extends Widget {
    constructor(buttons, setName, iconName) {
        const t = TitleAttr(`${setName} ${iconName}`);
        super(Button(t, Img(t, Src(buttons.getImageSrc(setName, iconName)))), obj(`${name}-button`), "inline-block");
        this.mesh = null;
        this.load(buttons, setName, iconName);
    }
    async load(buttons, setName, iconName) {
        this.mesh = await buttons.getMeshButton(setName, iconName, 0.2);
        this.mesh.disabled = this.disabled;
        objGraph(this, this.mesh);
        this.mesh.content3d.visible = this.visible;
        this.mesh.addEventListener("click", () => {
            this.content.click();
        });
    }
    get disabled() {
        return this.content.disabled;
    }
    set disabled(v) {
        elementSetEnabled(this, !v);
        objectSetEnabled(this.mesh, !v);
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