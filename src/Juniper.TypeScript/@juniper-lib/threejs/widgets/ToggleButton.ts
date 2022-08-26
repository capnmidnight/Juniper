import { ButtonPrimary, elementSetDisplay, Img } from "@juniper-lib/dom/tags";
import { all } from "@juniper-lib/tslib/events/all";
import { Object3D } from "three";
import { Pointer3DEvents } from "../eventSystem/devices/Pointer3DEvent";
import { obj, objectSetEnabled, objectSetVisible, objGraph } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import type { Widget } from "./widgets";


export class ToggleButton implements Widget, EventTarget {

    readonly element: HTMLButtonElement;
    readonly object: Object3D;
    private enterButton: MeshButton = null;
    private exitButton: MeshButton = null;

    get name() {
        return this.object.name;
    }

    addEventListener(type: keyof Pointer3DEvents, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.element.addEventListener(type, listener, options);
    }

    dispatchEvent(event: Event): boolean {
        return this.element.dispatchEvent(event);
    }

    removeEventListener(type: keyof Pointer3DEvents, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void {
        this.element.removeEventListener(type, callback, options);
    }

    click() {
        this.element.click();
    }

    private readonly btnImage: HTMLImageElement;
    private _isAvailable = true;
    private _isEnabled = true;
    private _isVisible = true;
    private _isActive = false;

    constructor(
        private readonly buttons: ButtonFactory,
        private readonly setName: string,
        private readonly activeName: string,
        private readonly inactiveName: string) {
        this.element = ButtonPrimary(
            this.btnImage = Img());

        this.object = obj(`${this.setName}-button`);
        this.load();
    }

    private async load() {
        const [activate, deactivate] = await all(
            this.buttons.getGeometryAndMaterials(this.setName, this.activeName),
            this.buttons.getGeometryAndMaterials(this.setName, this.inactiveName)
        );

        objGraph(this.object,
            this.enterButton = new MeshButton(`${this.setName}-activate-button`, activate.geometry, activate.enabledMaterial, activate.disabledMaterial, 0.2),
            this.exitButton = new MeshButton(`${this.setName}-deactivate-button`, deactivate.geometry, deactivate.enabledMaterial, deactivate.disabledMaterial, 0.2));

        this.enterButton.addEventListener("click", () => this.element.click());

        this.exitButton.addEventListener("click", () => this.element.click());

        this.refreshState();
    }

    get mesh() {
        return this.active
            ? this.enterButton
            : this.exitButton;
    }

    get available(): boolean {
        return this._isAvailable;
    }

    set available(v: boolean) {
        this._isAvailable = v;
        this.refreshState();
    }

    get active(): boolean {
        return this._isActive;
    }

    set active(v: boolean) {
        this._isActive = v;
        this.refreshState();
    }

    get enabled(): boolean {
        return this._isEnabled;
    }

    set enabled(v: boolean) {
        this._isEnabled = v;
        this.refreshState();
    }

    get visible(): boolean {
        return this._isVisible;
    }

    set visible(v: boolean) {
        this._isVisible = v;
        this.refreshState();
    }

    private refreshState() {
        const type = this.active
            ? this.inactiveName
            : this.activeName;
        const text = `${type} ${this.setName}`;
        this.element.title
            = this.btnImage.title
            = text;
        this.btnImage.src = this.buttons.getImageSrc(this.setName, type);

        this.element.disabled = !this.available || !this.visible || !this.enabled;
        elementSetDisplay(this, this.available && this.visible, "inline-block");

        if (this.enterButton && this.exitButton) {
            objectSetEnabled(this, this.available && this.visible && this.enabled);
            const visible = objectSetVisible(this.object, this.available && this.visible);
            objectSetVisible(this.enterButton, visible && !this.active);
            objectSetVisible(this.exitButton, visible && this.active);
        }
    }
}
