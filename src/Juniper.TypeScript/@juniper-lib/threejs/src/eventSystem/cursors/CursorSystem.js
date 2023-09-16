import { BaseCursor } from "./BaseCursor";
export class CursorSystem extends BaseCursor {
    constructor(element) {
        super();
        this.element = element;
        this._hidden = false;
        this.visible = true;
        this.style = "default";
        document.addEventListener("pointerlockchange", () => {
            this._hidden = !!document.pointerLockElement;
            this.refresh();
        });
    }
    get style() {
        return super.style;
    }
    set style(v) {
        super.style = v;
        this.refresh();
    }
    get visible() {
        return super.visible
            && !this._hidden;
    }
    set visible(v) {
        super.visible = v;
        this.refresh();
    }
    refresh() {
        this.element.style.cursor = this.visible
            ? this.style
            : "none";
    }
}
//# sourceMappingURL=CursorSystem.js.map