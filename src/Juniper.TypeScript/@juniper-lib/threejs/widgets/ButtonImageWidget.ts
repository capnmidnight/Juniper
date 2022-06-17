import { title } from "@juniper-lib/dom/attrs";
import { ButtonPrimary, elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { ButtonFactory } from "../ButtonFactory";
import { MeshButton } from "../MeshButton";
import { obj } from "../objects";
import type { Widget } from "./widgets";

export class ButtonImageWidget implements Widget, EventTarget {

    readonly element: HTMLButtonElement;
    readonly object: THREE.Object3D;
    private mesh: MeshButton = null;

    constructor(buttons: ButtonFactory, setName: string, iconName: string) {
        this.element = ButtonPrimary(
            title(iconName),
            buttons.getImageElement(setName, iconName));

        this.object = obj(`${name}-button`);
        this.load(buttons, setName, iconName);
    }

    private async load(buttons: ButtonFactory, setName: string, iconName: string) {
        const { geometry, enabledMaterial, disabledMaterial } = await buttons.getGeometryAndMaterials(setName, iconName);
        this.mesh = new MeshButton(iconName, geometry, enabledMaterial, disabledMaterial, 0.2);
        this.object.add(this.mesh);
        this.mesh.visible = this.visible;
        this.mesh.target.addEventListener("click", () => {
            this.element.click();
        });
    }

    get name() {
        return this.object.name;
    }

    addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.element.addEventListener(type, listener, options);
    }

    dispatchEvent(event: Event): boolean {
        return this.element.dispatchEvent(event);
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void {
        this.element.removeEventListener(type, callback, options);
    }

    click() {
        this.element.click();
    }

    get visible() {
        return elementIsDisplayed(this);
    }

    set visible(visible) {
        elementSetDisplay(this, visible, "inline-block");
        if (this.mesh) {
            this.mesh.visible = visible;
        }
    }
}
