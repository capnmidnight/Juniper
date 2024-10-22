import { Button, elementSetDisplay, elementSetEnabled, Img } from "@juniper-lib/dom";
import { obj, objectSetEnabled, objectSetVisible, objGraph } from "../objects";
import { Widget } from "./widgets";
export class ToggleButton extends Widget {
    constructor(buttons, setName, activeName, inactiveName) {
        super(Button(), obj(`${setName}-button`), "inline-block");
        this.buttons = buttons;
        this.setName = setName;
        this.activeName = activeName;
        this.inactiveName = inactiveName;
        this.enterButton = null;
        this.exitButton = null;
        this._isAvailable = true;
        this._isEnabled = true;
        this._isActive = false;
        this.content.append(this.btnImage = Img());
        this.load();
    }
    async load() {
        const [enter, exit] = await Promise.all([
            this.buttons.getMeshButton(this.setName, this.activeName, 0.2),
            this.buttons.getMeshButton(this.setName, this.inactiveName, 0.2)
        ]);
        objGraph(this, this.enterButton = enter, this.exitButton = exit);
        this.enterButton.addEventListener("click", () => this.content.click());
        this.exitButton.addEventListener("click", () => this.content.click());
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
//# sourceMappingURL=ToggleButton.js.map