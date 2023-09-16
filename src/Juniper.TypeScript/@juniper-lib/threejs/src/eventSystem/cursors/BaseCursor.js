export class BaseCursor {
    constructor() {
        this._visible = true;
        this._style = "default";
    }
    get style() {
        return this._style;
    }
    set style(v) {
        this._style = v;
    }
    get visible() {
        return this._visible;
    }
    set visible(v) {
        this._visible = v;
    }
}
//# sourceMappingURL=BaseCursor.js.map