import { TextImageOptions } from "@juniper-lib/2d/TextImage";
import { title } from "@juniper-lib/dom/attrs";
import { ButtonPrimary, elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { IFetcher } from "@juniper-lib/fetcher";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { obj } from "../objects";
import { TextMeshButton } from "../TextMeshButton";
import type { Widget } from "./widgets";


export class ButtonTextWidget implements Widget, EventTarget {

    readonly element: HTMLButtonElement;
    readonly object: THREE.Object3D;
    readonly mesh: TextMeshButton;

    constructor(fetcher: IFetcher, protected readonly env: IWebXRLayerManager, name: string, text: string, textButtonStyle: Partial<TextImageOptions>) {
        this.element = ButtonPrimary(
            title(name),
            text);
        this.object = obj(`${name}-button`,
            this.mesh = new TextMeshButton(fetcher, this.env, `${name}-button`, text, textButtonStyle));
        this.mesh.addEventListener("click", () => {
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
        this.mesh.visible = visible;
    }
}
