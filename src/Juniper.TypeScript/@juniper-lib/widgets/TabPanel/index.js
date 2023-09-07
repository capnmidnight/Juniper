import { __decorate, __metadata } from "tslib";
import { CustomElement } from "@juniper-lib/dom/CustomElement";
import { ClassList, QueryAll } from "@juniper-lib/dom/attrs";
import { onClick, onEvent } from "@juniper-lib/dom/evts";
import { ButtonSmall, Div, HtmlTag, elementSetClass, elementSetDisplay, isDisableable, resolveElement } from "@juniper-lib/dom/tags";
import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import "./styles.css";
export class TabPanelTabSelectedEvent extends TypedEvent {
    constructor(tabname) {
        super("tabselected");
        this.tabname = tabname;
    }
}
export function TabPanel(...rest) {
    return HtmlTag("tab-panel", ...rest);
}
export function onTabSelected(callback, opts) { return onEvent("tabselected", callback, opts); }
let TabPanelElement = class TabPanelElement extends HTMLElement {
    constructor() {
        super();
        this.buttons = new Map();
        this.curTab = null;
        this._disabled = false;
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
        this.panels = new Map(QueryAll(this, ":scope > [data-tab-name]")
            .map(d => [d.dataset.tabName, d]));
        this.panelNames = Array.from(this.panels.keys());
        this.buttons = new Map(this.panelNames
            .map(name => {
            const evt = new TabPanelTabSelectedEvent(name);
            return [name, ButtonSmall(name, onClick(() => {
                    this.select(name);
                    this.dispatchEvent(evt);
                }))];
        }));
        this.controls = Div(ClassList("tabs"), ...Array.from(this.buttons.values()));
        this.inner = Div(ClassList("panels"));
    }
    connectedCallback() {
        this.append(this.controls, this.inner);
        this.inner.append(...Array.from(this.panels.values()));
        if (this.panelNames.length > 0) {
            this.select(this.panelNames[0]);
        }
    }
    disconnectedCallback() {
        this.controls.remove();
        this.inner.replaceWith(...Array.from(this.panels.values()));
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
    get disabled() {
        return this._disabled;
    }
    set disabled(v) {
        this._disabled = v;
        for (const [name, panel] of this.panels) {
            const button = this.buttons.get(name);
            button.disabled = v || name === this.curTab;
            elementSetClass(panel, v, "disabled");
            if (isDisableable(panel)) {
                panel.disabled = v;
            }
            else {
                const elem = resolveElement(panel);
                if (isDisableable(elem)) {
                    elem.disabled = v;
                }
            }
        }
    }
    isSelected(name) {
        return this.curTab === name;
    }
    select(name) {
        if (this.panels.has(name)) {
            this.curTab = name;
            for (const [name, panel] of this.panels) {
                const visible = name === this.curTab;
                const button = this.buttons.get(name);
                button.disabled = visible || this.disabled;
                elementSetDisplay(panel, visible);
            }
        }
    }
    addEventListener(type, callback, options) {
        this.eventTarget.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        this.eventTarget.removeEventListener(type, callback);
    }
    dispatchEvent(evt) {
        return this.eventTarget.dispatchEvent(evt);
    }
    addBubbler(bubbler) {
        this.eventTarget.addBubbler(bubbler);
    }
    removeBubbler(bubbler) {
        this.eventTarget.removeBubbler(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        this.eventTarget.addScopedEventListener(scope, type, callback, options);
    }
    removeScope(scope) {
        this.eventTarget.removeScope(scope);
    }
    clearEventListeners(type) {
        this.eventTarget.clearEventListeners(type);
    }
};
TabPanelElement = __decorate([
    CustomElement("tab-panel"),
    __metadata("design:paramtypes", [])
], TabPanelElement);
export { TabPanelElement };
//# sourceMappingURL=index.js.map