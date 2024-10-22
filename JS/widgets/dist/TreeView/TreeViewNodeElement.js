import { TreeNode } from "@juniper-lib/collections/dist/TreeNode";
import { ClassList } from "@juniper-lib/dom/dist/attrs";
import { onClick, onContextMenu, onDblClick } from "@juniper-lib/dom/dist/evts";
import { ButtonSmall, Div, Span, StyleBlob, elementSetDisplay, elementSetText, elementSetTitle } from "@juniper-lib/dom/dist/tags";
import { blackDiamondCentered, blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered, plus } from "@juniper-lib/emoji";
import { EventTargetMixin } from "@juniper-lib/events/dist/EventTarget";
import { Task } from "@juniper-lib/events/dist/Task";
import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import { debounce } from "@juniper-lib/events/dist/debounce";
import { isBoolean, isDefined, isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
const styleSheet = StyleBlob(`
.tree-view-node-children {
    padding-left: 1.25em;
}

.tree-view-node {
    white-space: pre;
    color: black;
    cursor: default;
}

    .tree-view-node:hover > .tree-view-node-label {
        background-color: rgb(127, 127, 127, 0.125);
    }

.tree-view-node-label[draggable=true] {
    cursor: grab;
}

.tree-view-node.disabled, .tree-view-node.disabled > .tree-view-node-label[draggable=true] {
    cursor: not-allowed;
}

    .tree-view-node.disabled > .tree-view-node-label, .tree-view-node.filtered > .tree-view-node-label {
        opacity: 0.5;
    }

    .tree-view-node.disabled:hover > .tree-view-node-label {
        background-color: unset;
    }

.tree-view-node.selected > .tree-view-node-label {
    background-color: rgb(221, 255, 255);
}

.tree-view-node.selected.disabled > .tree-view-node-label {
    background-color: lightgrey;
}

.tree-view-node .drag-buffer {
    width: 100%;
    height: 0.3em;
}

.tree-view-node.highlighted {
    background-color: rgb(221, 255, 221);
}

.tree-view-node .drag-buffer.highlighted {
    background-color: rgb(200 200 200);
}

.tree-view-node-collapser, .tree-view-node-adder {
    font-family: monospace;
    font-size: 80%;
    color: rgb(119, 119, 119);
    border: none;
    background-color: transparent;
}`);
export class TreeViewNodeClickedEvent extends TypedEvent {
    constructor(node) {
        super("click");
        this.node = node;
    }
}
export class TreeViewNodeSelectedEvent extends TypedEvent {
    constructor(node) {
        super("select");
        this.node = node;
    }
}
export class TreeViewNodeEvent extends TypedEvent {
    get finished() {
        return this._finished;
    }
    constructor(type, parent) {
        super(type);
        this.parent = parent;
        this._finished = new Task();
    }
    complete() {
        this._finished.resolve();
    }
}
export class TreeViewNodeAddEvent extends TreeViewNodeEvent {
    constructor(parent) {
        super("add", parent);
    }
}
export class TreeViewNodeContextMenuEvent extends TreeViewNodeEvent {
    constructor(parent) {
        super("contextmenu", parent);
    }
}
export class TreeViewNodeElement extends HTMLElement {
    static create(node, defaultLabel, getLabel, getDescription, canChangeOrder, getChildDescription, canHaveChildren, createElement) {
        const element = document.createElement("tree-view-node");
        element.node = node;
        element.defaultLabel = defaultLabel;
        element._getLabel = getLabel;
        element._getDescription = getDescription;
        element._canChangeOrder = canChangeOrder;
        element._getChildDescription = getChildDescription;
        element._canHaveChildren = canHaveChildren;
        element.createElement = createElement;
        return element;
    }
    get node() { return this._node; }
    set node(v) { this._node = v; }
    constructor() {
        super();
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
        this.refresh = debounce(() => this.onRefresh());
        const onEnabledClick = (act) => onClick((evt) => {
            if (!this.disabled) {
                evt.preventDefault();
                evt.stopPropagation();
                act(evt);
            }
        });
        this.root = Div(ClassList("tree-view-node"), this.upper = Div(ClassList("drag-buffer", "top")), this.infoView = Div(ClassList("tree-view-node-label"), onEnabledClick(() => {
            if (!this.selected) {
                this._select(true);
            }
        }), onDblClick((evt) => {
            if (!this.disabled && this.canHaveChildren) {
                evt.preventDefault();
                evt.stopPropagation();
                this.open = !this.open;
            }
        }), this.collapser = ButtonSmall(ClassList("tree-view-node-collapser"), onClick((evt) => {
            if (this.canHaveChildren) {
                evt.preventDefault();
                evt.stopPropagation();
                this.open = !this.open;
            }
            else {
                this._select(true);
            }
        })), this.labeler = Span(onContextMenu(async (evt) => {
            if (!this.disabled) {
                await this._launchMenu(evt, new TreeViewNodeContextMenuEvent(this));
            }
        }))), this.subView = Div(ClassList("tree-view-node-children"), this.childTreeNodes = Div(), this.adder = ButtonSmall(ClassList("tree-view-node-adder"), onEnabledClick(async (evt) => {
            if (this.canHaveChildren) {
                await this._launchMenu(evt, new TreeViewNodeAddEvent(this));
            }
        }), plus.emojiStyle)), this.lower = Div(ClassList("drag-buffer", "bottom")));
    }
    connectedCallback() {
        const shadowRoot = this.attachShadow({ mode: "closed" });
        shadowRoot.append(styleSheet.cloneNode(), this.root);
        this.infoView.draggable = this.canChangeOrder;
        elementSetText(this.labeler, this.label);
        this.adder.title = this.adderTitle;
        this.setAttribute("open", "");
        this.open = this.node.isRoot;
        this.refresh();
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
    async _launchMenu(parentEvt, evt) {
        parentEvt.preventDefault();
        parentEvt.stopPropagation();
        this.adder.disabled = true;
        try {
            this.dispatchEvent(evt);
            await evt.finished;
        }
        catch {
            evt.complete();
        }
        finally {
            this.adder.disabled = false;
        }
    }
    onRefresh() {
        if (this.node.isRoot !== (this.adder.parentElement === this.subView)) {
            if (this.node.isRoot) {
                this.subView.append(this.adder);
            }
            else {
                this.infoView.append(this.adder);
            }
        }
        const canOpenClose = this.canHaveChildren
            && this.node.hasChildren
            && this.node.isChild;
        elementSetTitle(this.collapser, this.collapserTitle);
        elementSetText(this.collapser, canOpenClose
            ? this.open
                ? blackMediumDownPointingTriangleCentered.emojiStyle
                : blackMediumRightPointingTriangleCentered.emojiStyle
            : blackDiamondCentered.emojiStyle);
        this.collapser.style.opacity = canOpenClose
            ? "1"
            : "0";
        elementSetText(this.labeler, this.label);
        elementSetTitle(this.adder, this.adderTitle);
        elementSetDisplay(this.adder, this.canHaveChildren, "inline-block");
        elementSetDisplay(this.upper, this.node.isChild && this.canChangeOrder && isNullOrUndefined(this.previousSibling));
        elementSetDisplay(this.lower, this.node.isChild && this.canChangeOrder);
        this.collapser.disabled = this.disabled && !this.specialSelectMode || this.node.isRoot;
        this.adder.disabled = this.disabled || this.specialSelectMode;
    }
    get specialSelectMode() {
        return this.adder.classList.contains("disabled");
    }
    set specialSelectMode(v) {
        if (v !== this.specialSelectMode) {
            this.adder.classList.toggle("disabled");
            this.refresh();
        }
    }
    get canHaveChildren() {
        return this._canHaveChildren(this.node);
    }
    get label() {
        return isDefined(this.node.value) ? this._getLabel(this.node.value) : this.defaultLabel;
    }
    get description() {
        return isDefined(this.node.value) ? this._getDescription(this.node.value) : null;
    }
    get childDescription() {
        return isDefined(this.node.value) ? this._getChildDescription(this.node.value) : null;
    }
    get canChangeOrder() {
        return isDefined(this.node.value) && this._canChangeOrder(this.node.value);
    }
    get collapserTitle() {
        if (!this.canHaveChildren) {
            return "Select " + this.description;
        }
        else if (this.open) {
            return "Collapse " + this.description;
        }
        else {
            return "Expand " + this.description;
        }
    }
    get adderTitle() {
        return "Add " + this.childDescription;
    }
    get disabled() {
        return this.hasAttribute("disabled");
    }
    set disabled(v) {
        if (v !== this.disabled) {
            if (v) {
                this.setAttribute("disabled", "");
            }
            else {
                this.removeAttribute("disabled");
            }
            this.root.classList.toggle("disabled", v);
            this.refresh();
        }
    }
    get filtered() {
        return this.hasAttribute("filtered");
    }
    set _filtered(v) {
        if (v !== this.filtered) {
            if (v) {
                this.setAttribute("filtered", "");
            }
            else {
                this.removeAttribute("filtered");
            }
            this.root.classList.toggle("filtered", v);
        }
    }
    get selected() {
        return this.hasAttribute("selected");
    }
    set _selected(v) {
        if (v !== this.selected) {
            if (v) {
                this.setAttribute("selected", "");
            }
            else {
                this.removeAttribute("selected");
            }
            this.root.classList.toggle("selected", v);
        }
    }
    get highlighted() {
        if (this.root.classList.contains("highlighted")) {
            return 0;
        }
        else if (this.upper.classList.contains("highlighted")) {
            return -1;
        }
        else if (this.lower.classList.contains("highlighted")) {
            return 1;
        }
        else {
            return null;
        }
    }
    set highlighted(v) {
        if (v !== this.highlighted) {
            this.root.classList.remove("highlighted");
            this.upper.classList.remove("highlighted");
            this.lower.classList.remove("highlighted");
            if (v === 0) {
                this.root.classList.add("highlighted");
            }
            else if (v === -1) {
                this.upper.classList.add("highlighted");
            }
            else if (v === 1) {
                this.lower.classList.add("highlighted");
            }
        }
    }
    get open() {
        return this.hasAttribute("open");
    }
    set open(v) {
        if (v !== this.open) {
            if (v) {
                this.setAttribute("open", "");
            }
            else {
                this.removeAttribute("open");
            }
            elementSetDisplay(this.subView, v);
            this.refresh();
        }
    }
    add(value) {
        this.open = true;
        const node = new TreeNode(value);
        this.node.connectTo(node);
        this.createElement(node);
        this.refresh();
    }
    _select(bubbles) {
        this.dispatchEvent(new TreeViewNodeClickedEvent(this.node));
        if (!this.selected) {
            const selectEvt = new TreeViewNodeSelectedEvent(this.node);
            if (!bubbles) {
                selectEvt.stopPropagation();
            }
            this.dispatchEvent(selectEvt);
        }
    }
    scrollIntoView(arg) {
        if (isNullOrUndefined(arg)) {
            arg = {};
        }
        if (!isBoolean(arg)) {
            if (isNullOrUndefined(arg.behavior)) {
                arg.behavior = "smooth";
            }
            if (isNullOrUndefined(arg.block)) {
                arg.block = "center";
            }
            if (isNullOrUndefined(arg.inline)) {
                arg.inline = "nearest";
            }
        }
        super.scrollIntoView({
            behavior: "smooth",
            block: "center",
            inline: "nearest"
        });
    }
}
customElements.define("tree-view-node", TreeViewNodeElement);
//# sourceMappingURL=TreeViewNodeElement.js.map