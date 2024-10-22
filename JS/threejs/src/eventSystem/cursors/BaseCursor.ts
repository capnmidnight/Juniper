export abstract class BaseCursor {
    private _visible = true;
    private _style: CssCursorValue = "default";

    get style(): CssCursorValue {
        return this._style;
    }

    set style(v) {
        this._style = v;
    }

    get visible(): boolean {
        return this._visible;
    }

    set visible(v) {
        this._visible = v;
    }
}