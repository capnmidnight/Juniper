import { TreeNode } from "@juniper-lib/collections/dist/TreeNode";
import { ClassList } from "@juniper-lib/dom/dist/attrs";
import { onClick, onContextMenu, onDblClick } from "@juniper-lib/dom/dist/evts";
import { ButtonSmall, Div, Span, StyleBlob, elementSetDisplay, elementSetText, elementSetTitle } from "@juniper-lib/dom/dist/tags";
import { blackDiamondCentered, blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered, plus } from "@juniper-lib/emoji";
import { EventTargetMixin } from "@juniper-lib/events/dist/EventTarget";
import { Task } from "@juniper-lib/events/dist/Task";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/dist/TypedEventTarget";
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

export class TreeViewNodeClickedEvent<T> extends TypedEvent<"click"> {
    constructor(public readonly node: TreeNode<T>) {
        super("click");
    }
}

export class TreeViewNodeSelectedEvent<T> extends TypedEvent<"select"> {
    constructor(public readonly node: TreeNode<T>) {
        super("select");
    }
}

export abstract class TreeViewNodeEvent<EventT extends string, ValueT> extends TypedEvent<EventT>{
    private readonly _finished = new Task();
    public get finished(): Promise<void> {
        return this._finished;
    }

    constructor(type: EventT, public readonly parent: TreeViewNodeElement<ValueT>) {
        super(type);
    }

    complete() {
        this._finished.resolve();
    }
}

export class TreeViewNodeAddEvent<T> extends TreeViewNodeEvent<"add", T> {
    constructor(parent: TreeViewNodeElement<T>) {
        super("add", parent);
    }
}

export class TreeViewNodeContextMenuEvent<T> extends TreeViewNodeEvent<"contextmenu", T> {
    constructor(parent: TreeViewNodeElement<T>) {
        super("contextmenu", parent);
    }
}

export type TreeViewNodeEvents<T> = {
    click: TreeViewNodeClickedEvent<T>;
    select: TreeViewNodeSelectedEvent<T>;
    add: TreeViewNodeAddEvent<T>;
    contextmenu: TreeViewNodeContextMenuEvent<T>;
}

export class TreeViewNodeElement<T>
    extends HTMLElement
    implements ITypedEventTarget<TreeViewNodeEvents<T>> {


    static create<T>(
        node: TreeNode<T>,
        defaultLabel: string,
        getLabel: (value: T) => string,
        getDescription: (value: T) => string,
        canChangeOrder: (value: T) => boolean,
        getChildDescription: (node: T) => string,
        canHaveChildren: (node: TreeNode<T>) => boolean,
        createElement: (node: TreeNode<T>) => TreeViewNodeElement<T>) {

        const element = document.createElement("tree-view-node") as TreeViewNodeElement<T>;

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

    private readonly eventTarget: EventTargetMixin;
    private readonly root: HTMLElement;
    readonly childTreeNodes: HTMLElement;

    private readonly infoView: HTMLDivElement;
    private readonly subView: HTMLDivElement;

    private readonly collapser: HTMLButtonElement;
    private readonly labeler: HTMLSpanElement;
    private readonly adder: HTMLButtonElement;

    readonly upper: HTMLDivElement;
    readonly lower: HTMLDivElement;

    readonly refresh: () => void;

    private _node: TreeNode<T>;
    get node() { return this._node; }
    private set node(v) { this._node = v; }

    private defaultLabel: string;
    private _getLabel: (value: T) => string;
    private _getDescription: (value: T) => string;
    private _canChangeOrder: (value: T) => boolean;
    private _getChildDescription: (node: T) => string;
    private _canHaveChildren: (node: TreeNode<T>) => boolean;
    private createElement: (node: TreeNode<T>) => TreeViewNodeElement<T>;

    constructor() {

        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );

        this.refresh = debounce(() => this.onRefresh());

        const onEnabledClick = (act: (evt: Event) => void) => onClick((evt: Event) => {
            if (!this.disabled) {
                evt.preventDefault();
                evt.stopPropagation();
                act(evt);
            }
        });

        this.root = Div(
            ClassList("tree-view-node"),

            this.upper = Div(ClassList("drag-buffer", "top")),

            this.infoView = Div(
                ClassList("tree-view-node-label"),

                onEnabledClick(() => {
                    if (!this.selected) {
                        this._select(true);
                    }
                }),

                onDblClick((evt) => {
                    if (!this.disabled && this.canHaveChildren) {
                        evt.preventDefault();
                        evt.stopPropagation();
                        this.open = !this.open;
                    }
                }),

                this.collapser = ButtonSmall(
                    ClassList("tree-view-node-collapser"),
                    onClick((evt) => {
                        if (this.canHaveChildren) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            this.open = !this.open;
                        }
                        else {
                            this._select(true);
                        }
                    })
                ),

                this.labeler = Span(
                    onContextMenu(async (evt) => {
                        if (!this.disabled) {
                            await this._launchMenu(evt, new TreeViewNodeContextMenuEvent(this));
                        }
                    })
                )
            ),

            this.subView = Div(
                ClassList("tree-view-node-children"),
                this.childTreeNodes = Div(),

                this.adder = ButtonSmall(
                    ClassList("tree-view-node-adder"),
                    onEnabledClick(async (evt: Event) => {
                        if (this.canHaveChildren) {
                            await this._launchMenu(evt, new TreeViewNodeAddEvent(this));
                        }
                    }),
                    plus.emojiStyle
                )
            ),

            this.lower = Div(ClassList("drag-buffer", "bottom"))
        );
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

    override addEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>): void {
        this.eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<TreeViewNodeEvents<T>>): void {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<TreeViewNodeEvents<T>>): void {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof TreeViewNodeEvents<T>>(type?: EventTypeT): void {
        this.eventTarget.clearEventListeners(type as string);
    }

    async _launchMenu(parentEvt: Event, evt: TreeViewNodeEvent<string, T>): Promise<void> {
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

    private onRefresh(): void {
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
        elementSetText(this.collapser,
            canOpenClose
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

    get specialSelectMode(): boolean {
        return this.adder.classList.contains("disabled");
    }

    set specialSelectMode(v: boolean) {
        if (v !== this.specialSelectMode) {
            this.adder.classList.toggle("disabled");
            this.refresh();
        }
    }

    get canHaveChildren(): boolean {
        return this._canHaveChildren(this.node);
    }

    private get label(): string {
        return isDefined(this.node.value) ? this._getLabel(this.node.value) : this.defaultLabel;
    }

    private get description(): string {
        return isDefined(this.node.value) ? this._getDescription(this.node.value) : null;
    }

    private get childDescription(): string {
        return isDefined(this.node.value) ? this._getChildDescription(this.node.value) : null;
    }

    private get canChangeOrder(): boolean {
        return isDefined(this.node.value) && this._canChangeOrder(this.node.value);
    }

    private get collapserTitle(): string {
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

    private get adderTitle(): string {
        return "Add " + this.childDescription;
    }

    get disabled(): boolean {
        return this.hasAttribute("disabled");
    }

    set disabled(v: boolean) {
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

    get filtered(): boolean {
        return this.hasAttribute("filtered");
    }

    set _filtered(v: boolean) {
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

    get selected(): boolean {
        return this.hasAttribute("selected");
    }

    set _selected(v: boolean) {
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

    get highlighted(): number {
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

    set highlighted(v: number) {
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

    get open(): boolean {
        return this.hasAttribute("open");
    }

    set open(v: boolean) {
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

    add(value: T): void {
        this.open = true;
        const node = new TreeNode(value);
        this.node.connectTo(node);
        this.createElement(node);
        this.refresh();
    }

    _select(bubbles: boolean): void {
        this.dispatchEvent(new TreeViewNodeClickedEvent(this.node));

        if (!this.selected) {
            const selectEvt = new TreeViewNodeSelectedEvent(this.node);
            if (!bubbles) {
                selectEvt.stopPropagation();
            }
            this.dispatchEvent(selectEvt);
        }
    }

    override scrollIntoView(arg?: boolean | ScrollIntoViewOptions) {
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