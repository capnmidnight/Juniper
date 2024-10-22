import { Button, elementSetDisplay, elementSetEnabled, Img } from "@juniper-lib/dom";
import { obj, objectSetEnabled, objectSetVisible, objGraph } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import { Widget } from "./widgets";


export class ToggleButton extends Widget<HTMLButtonElement> {

    private enterButton: MeshButton = null;
    private exitButton: MeshButton = null;
    private readonly btnImage: HTMLImageElement;
    private _isAvailable = true;
    private _isEnabled = true;
    private _isActive = false;

    constructor(
        private readonly buttons: ButtonFactory,
        private readonly setName: string,
        private readonly activeName: string,
        private readonly inactiveName: string) {
        super(
            Button(),
            obj(`${setName}-button`),
            "inline-block"
        );

        this.content.append(this.btnImage = Img());
        this.load();
    }

    private async load() {
        const [enter, exit] = await Promise.all([
            this.buttons.getMeshButton(this.setName, this.activeName, 0.2),
            this.buttons.getMeshButton(this.setName, this.inactiveName, 0.2)
        ]);

        objGraph(
            this,
            this.enterButton = enter,
            this.exitButton = exit
        );

        this.enterButton.addEventListener("click", () => this.content.click());

        this.exitButton.addEventListener("click", () => this.content.click());

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

    override get visible(): boolean {
        return super.visible;
    }

    override set visible(v: boolean) {
        super.visible = v;
        this.refreshState();
    }

    private refreshState() {
        const type = this.active
            ? this.inactiveName
            : this.activeName;
        const text = `${type} ${this.setName}`;
        this.content.title
            = this.btnImage.title
            = text;
        this.btnImage.src = this.buttons.getImageSrc(this.setName, type);

        elementSetEnabled(this, this.available && this.visible && this.enabled);
        elementSetDisplay(this.content, this.available && this.visible, "inline-block");

        if (this.enterButton && this.exitButton) {
            objectSetEnabled(this, this.available && this.visible && this.enabled);
            const visible = objectSetVisible(this, this.available && this.visible);
            objectSetVisible(this.enterButton, visible && !this.active);
            objectSetVisible(this.exitButton, visible && this.active);
        }
    }
}
