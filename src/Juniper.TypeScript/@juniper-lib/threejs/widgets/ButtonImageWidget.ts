import { title } from "@juniper-lib/dom/attrs";
import { ButtonPrimary } from "@juniper-lib/dom/tags";
import { obj, objectSetVisible, objGraph } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import { Widget } from "./widgets";

export class ButtonImageWidget extends Widget<HTMLButtonElement> {

    private mesh: MeshButton = null;

    constructor(buttons: ButtonFactory, setName: string, iconName: string) {
        super(
            ButtonPrimary(
                title(iconName),
                buttons.getImageElement(setName, iconName)
            ),
            obj(`${name}-button`),
            "inline-block");
        this.load(buttons, setName, iconName);
    }

    private async load(buttons: ButtonFactory, setName: string, iconName: string) {
        const { geometry, enabledMaterial, disabledMaterial } = await buttons.getGeometryAndMaterials(setName, iconName);
        this.mesh = new MeshButton(iconName, geometry, enabledMaterial, disabledMaterial, 0.2);
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
