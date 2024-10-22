import { debounce, isBoolean, isDefined, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { TreeNode } from "@juniper-lib/collections";
import { Button, ClassList, Div, elementSetDisplay, OnClick, OnContextMenu, OnDblClick, SpanTag, StyleBlob } from "@juniper-lib/dom";
import { blackDiamondCentered, blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered, plus } from "@juniper-lib/emoji";
import { EventTargetMixin, Task, TypedEvent } from "@juniper-lib/events";
import { registerFactory } from '../../../dom/src/registerFactory';
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
class BaseTreeViewNodeEvent extends TypedEvent {
    constructor(type) {
        super(type, { bubbles: true });
    }
}
export class TreeViewNodeClickedEvent extends BaseTreeViewNodeEvent {
    constructor() {
        super("click");
    }
}
export class TreeViewNodeSelectedEvent extends BaseTreeViewNodeEvent {
    constructor() {
        super("select");
    }
}
export class TreeViewNodeCreateEvent extends BaseTreeViewNodeEvent {
    constructor(node) {
        super("create");
        this.node = node;
    }
}
export class BaseTreeViewNodeActionEvent extends BaseTreeViewNodeEvent {
    get finished() {
        return this._finished;
    }
    constructor(type) {
        super(type);
        this._finished = new Task();
    }
    complete() {
        this._finished.resolve();
    }
}
export class TreeViewNodeAddEvent extends BaseTreeViewNodeActionEvent {
    constructor() {
        super("add");
    }
}
export class TreeViewNodeContextMenuEvent extends BaseTreeViewNodeActionEvent {
    constructor() {
        super("contextmenu");
    }
}
export class TreeViewNodeElement extends HTMLElement {
    static create(node, defaultLabel, getLabel, getDescription, canChangeOrder, getChildDescription, canHaveChildren) {
        const element = TreeViewNode();
        element.node = node;
        element.defaultLabel = defaultLabel;
        element._getLabel = getLabel;
        element._getDescription = getDescription;
        element._canChangeOrder = canChangeOrder;
        element._getChildDescription = getChildDescription;
        element._canHaveChildren = canHaveChildren;
        return element;
    }
    get node() { return this._node; }
    set node(v) { this._node = v; }
    constructor() {
        super();
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
        this.refresh = debounce(() => this.onRefresh());
        const OnEnabledClick = (act) => OnClick((evt) => {
            if (!this.disabled) {
                evt.preventDefault();
                evt.stopPropagation();
                act(evt);
            }
        });
        this.root = Div(ClassList("tree-view-node"), this.upper = Div(ClassList("drag-buffer", "top")), this.infoView = Div(ClassList("tree-view-node-label"), OnEnabledClick(() => {
            if (!this.selected) {
                this._select(true);
            }
        }), OnDblClick((evt) => {
            if (!this.disabled && this.canHaveChildren) {
                evt.preventDefault();
                evt.stopPropagation();
                this.open = !this.open;
            }
        }), this.collapser = Button(ClassList("tree-view-node-collapser"), OnClick((evt) => {
            if (this.canHaveChildren) {
                evt.preventDefault();
                evt.stopPropagation();
                this.open = !this.open;
            }
            else {
                this._select(true);
            }
        })), this.labeler = SpanTag(OnContextMenu(async (evt) => {
            if (!this.disabled) {
                await this._launchMenu(evt, new TreeViewNodeContextMenuEvent());
            }
        }))), this.subView = Div(ClassList("tree-view-node-children"), this.childTreeNodes = Div(), this.adder = Button(ClassList("tree-view-node-adder"), OnEnabledClick(async (evt) => {
            if (this.canHaveChildren) {
                await this._launchMenu(evt, new TreeViewNodeAddEvent());
            }
        }), plus.emojiStyle)), this.lower = Div(ClassList("drag-buffer", "bottom")));
    }
    connectedCallback() {
        const shadowRoot = this.attachShadow({ mode: "closed" });
        shadowRoot.append(styleSheet.cloneNode(), this.root);
        this.infoView.draggable = this.canChangeOrder;
        this.labeler.replaceChildren(this.label);
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
        this.collapser.title = this.collapserTitle;
        this.collapser.replaceChildren(canOpenClose
            ? this.open
                ? blackMediumDownPointingTriangleCentered.emojiStyle
                : blackMediumRightPointingTriangleCentered.emojiStyle
            : blackDiamondCentered.emojiStyle);
        this.collapser.style.opacity = canOpenClose
            ? "1"
            : "0";
        this.labeler.replaceChildren(this.label);
        this.adder.title = this.adderTitle;
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
        this.dispatchEvent(new TreeViewNodeCreateEvent(node));
        this.refresh();
    }
    _select(bubbles) {
        this.dispatchEvent(new TreeViewNodeClickedEvent());
        if (!this.selected) {
            const selectEvt = new TreeViewNodeSelectedEvent();
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
    static install() { return singleton("Juniper::Widgets::TreeViewNodeElement", () => registerFactory("tree-view-node", TreeViewNodeElement)); }
}
export function TreeViewNode(...rest) {
    return TreeViewNodeElement.install()(...rest);
}
//# sourceMappingURL=TreeViewNodeElement.js.map