import { BaseCursor } from "./BaseCursor";

export class CursorSystem extends BaseCursor {
    private _hidden = false;

    constructor(public readonly element: HTMLElement) {
        super();

        this.visible = true;
        this.style = "default";

        document.addEventListener("pointerlockchange", () => {
            this._hidden = !!document.pointerLockElement;
            this.refresh();
        });
    }

    override get style() {
        return super.style;
    }

    override set style(v) {
        super.style = v;
        this.refresh();
    }

    override get visible() {
        return super.visible
            && !this._hidden;
    }

    override set visible(v) {
        super.visible = v;
        this.refresh();
    }

    private refresh() {
        this.element.style.cursor = this.visible
            ? this.style
            : "none";
    }
}