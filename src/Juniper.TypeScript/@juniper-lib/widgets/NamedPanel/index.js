import { ClassList } from "@juniper-lib/dom/attrs";
import { Div, elementSetDisplay, elementSetText, H2, Span } from "@juniper-lib/dom/tags";
import { debounce } from "@juniper-lib/events/debounce";
import "./style.css";
export class NamedPanel {
    constructor(_title, ...rest) {
        this._title = _title;
        this._open = true;
        this.element = Div(ClassList("named-panel"), this.header = H2(this.titleText = Span(_title)), this.body = Div(ClassList("body"), ...rest));
        this.refresh = debounce(() => this.onRefresh());
        Object.seal(this);
    }
    get title() {
        return this._title;
    }
    set title(v) {
        if (v !== this.title) {
            this._title = v;
            this.refresh();
        }
    }
    get open() {
        return this._open;
    }
    set open(v) {
        if (v !== this.open) {
            this._open = v;
            this.refresh();
        }
    }
    onRefresh() {
        elementSetText(this.titleText, this._title);
        this.header.classList.toggle("closed", !this.open);
        elementSetDisplay(this.body, this.open);
    }
}
//# sourceMappingURL=index.js.map