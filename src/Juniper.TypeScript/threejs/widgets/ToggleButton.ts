import { className } from "juniper-dom/attrs";
import { buttonSetEnabled } from "juniper-dom/buttonSetEnabled";
import { Button, elementSetDisplay, Img } from "juniper-dom/tags";
import { ButtonFactory } from "../ButtonFactory";
import { MeshButton } from "../MeshButton";
import { obj, objectSetEnabled, objectSetVisible, objGraph } from "../objects";
import type { Widget } from "./widgets";


export class ToggleButton implements Widget, EventTarget {

    readonly element: HTMLButtonElement;
    readonly object: THREE.Object3D;
    private enterButton: MeshButton = null;
    private exitButton: MeshButton = null;

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
        this.element = Button(
            className("btn btn-primary"),
            this.btnImage = Img());

        this.object = obj(`${this.setName}-button`);
        this.load();
    }

    private async load() {
        const [activate, deactivate] = await Promise.all([
            this.buttons.getGeometryAndMaterials(this.setName, this.activeName),
            this.buttons.getGeometryAndMaterials(this.setName, this.inactiveName)
        ]);

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

        buttonSetEnabled(this, this.available && this.visible && this.enabled, "primary");
        elementSetDisplay(this, this.available && this.visible, "inline-block");

        if (this.enterButton && this.exitButton) {
            objectSetEnabled(this, this.available && this.visible && this.enabled);
            const visible = objectSetVisible(this.object, this.available && this.visible);
            objectSetVisible(this.enterButton, visible && !this.active);
            objectSetVisible(this.exitButton, visible && this.active);
        }
    }
}