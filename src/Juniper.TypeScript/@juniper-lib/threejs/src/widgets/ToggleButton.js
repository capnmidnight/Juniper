import { ButtonPrimary, HtmlRender, elementSetDisplay, Img } from "@juniper-lib/dom/tags";
import { all } from "@juniper-lib/events/all";
import { obj, objectSetEnabled, objectSetVisible, objGraph } from "../objects";
import { Widget } from "./widgets";
export class ToggleButton extends Widget {
    constructor(buttons, setName, activeName, inactiveName) {
        super(ButtonPrimary(), obj(`${setName}-button`), "inline-block");
        this.buttons = buttons;
        this.setName = setName;
        this.activeName = activeName;
        this.inactiveName = inactiveName;
        this.enterButton = null;
        this.exitButton = null;
        this._isAvailable = true;
        this._isEnabled = true;
        this._isActive = false;
        HtmlRender(this, this.btnImage = Img());
        this.load();
    }
    async load() {
        const [enter, exit] = await all(this.buttons.getMeshButton(this.setName, this.activeName, 0.2), this.buttons.getMeshButton(this.setName, this.inactiveName, 0.2));
        objGraph(this.object, this.enterButton = enter, this.exitButton = exit);
        this.enterButton.addEventListener("click", () => this.element.click());
        this.exitButton.addEventListener("click", () => this.element.click());
        this.refreshState();
    }
    get mesh() {
        return this.active
            ? this.enterButton
            : this.exitButton;
    }
    get available() {
        return this._isAvailable;
    }
    set available(v) {
        this._isAvailable = v;
        this.refreshState();
    }
    get active() {
        return this._isActive;
    }
    set active(v) {
        this._isActive = v;
        this.refreshState();
    }
    get enabled() {
        return this._isEnabled;
    }
    set enabled(v) {
        this._isEnabled = v;
        this.refreshState();
    }
    get visible() {
        return super.visible;
    }
    set visible(v) {
        super.visible = v;
        this.refreshState();
    }
    refreshState() {
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
//# sourceMappingURL=ToggleButton.js.map