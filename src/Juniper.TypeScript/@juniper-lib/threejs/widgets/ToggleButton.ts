import { ButtonPrimary, HtmlRender, elementSetDisplay, Img } from "@juniper-lib/dom/tags";
import { all } from "@juniper-lib/events/all";
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
            ButtonPrimary(),
            obj(`${setName}-button`),
            "inline-block"
        );

        HtmlRender(this, this.btnImage = Img());
        this.load();
    }

    private async load() {
        const [enter, exit] = await all(
            this.buttons.getMeshButton(this.setName, this.activeName, 0.2),
            this.buttons.getMeshButton(this.setName, this.inactiveName, 0.2)
        );

        objGraph(this.object,
            this.enterButton = enter,
            this.exitButton = exit);

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
