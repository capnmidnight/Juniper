import { className } from "@juniper-lib/dom/attrs";
import { Div, ElementChild, ErsatzElement, IDisableable, isDisableable, isIElementAppliable } from "@juniper-lib/dom/tags";
import { isDefined } from "@juniper-lib/tslib";

export class GroupPanel
    implements ErsatzElement {

    readonly element: HTMLElement;
    private readonly disableable = new Array<IDisableable>();
    private _refreshTimer: number = null;
    private _disabled = false;

    constructor(...rest: ElementChild[]) {

        const elems = rest.filter((v) => !isIElementAppliable(v));

        for (const elem of elems) {
            if (isDisableable(elem)) {
                this.disableable.push(elem);
            }
        }

        this.element = Div(
            className("group-panel"),
            ...rest
        )

        Object.seal(this);

        this.refresh();
    }

    get disabled() {
        return this._disabled;
    }

    set disabled(v) {
        if (v !== this.disabled) {
            this._disabled = v;
            this.refresh();
        }
    }

    get enabled() {
        return !this.disabled;
    }

    set enabled(v) {
        this.disabled = !v;
    }

    private refresh() {
        if (isDefined(this._refreshTimer)) {
            clearTimeout(this._refreshTimer);
            this._refreshTimer = null;
        }

        this._refreshTimer = setTimeout(() => {
            this._refreshTimer = null;
            this.onRefresh();
        });
    }

    protected onRefresh() {
        for (const elem of this.disableable) {
            elem.disabled = this.disabled;
        }
    }
}
