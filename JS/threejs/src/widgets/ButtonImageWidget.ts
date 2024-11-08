import { elementSetEnabled, Src, TitleAttr } from "@juniper-lib/dom";
import { Button, Img } from "@juniper-lib/dom";
import { obj, objGraph, objectSetEnabled, objectSetVisible } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import { Widget } from "./widgets";

export class ButtonImageWidget extends Widget<HTMLButtonElement> {

    private mesh: MeshButton = null;

    constructor(buttons: ButtonFactory, setName: string, iconName: string) {
        const t = TitleAttr(`${setName} ${iconName}`);
        super(
            Button(
                t,
                Img(t, Src(buttons.getImageSrc(setName, iconName)))
            ),
            obj(`${name}-button`),
            "inline-block");
        this.load(buttons, setName, iconName);
    }

    private async load(buttons: ButtonFactory, setName: string, iconName: string) {
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

    override get visible(): boolean {
        return super.visible;
    }

    override set visible(v: boolean) {
        super.visible = v;
        if (this.mesh) {
            objectSetVisible(this.mesh, v);
        }
    }
}
