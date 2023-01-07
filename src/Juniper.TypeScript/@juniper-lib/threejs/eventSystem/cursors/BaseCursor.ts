export abstract class BaseCursor {
    private _visible: boolean = true;
    private _style: CSSCursorValue = "default";

    get style(): CSSCursorValue {
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