import { className, draggable, title } from "@juniper-lib/dom/attrs";
import { buttonSetEnabled } from "@juniper-lib/dom/buttonSetEnabled";
import { onClick, onDblClick } from "@juniper-lib/dom/evts";
import {
    ButtonSmall,
    Div,
    elementIsDisplayed,
    elementSetDisplay,
    elementSetText,
    elementSetTitle,
    ErsatzElement,
    Span
} from "@juniper-lib/dom/tags";
import { plus } from "@juniper-lib/emoji";
import { isFunction, isNullOrUndefined, Task, TreeNode, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { TreeView } from "./TreeView";

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

export class TreeViewNodeAddEvent<T, K> extends TypedEvent<"add"> {
    private readonly _finished = new Task();
    public get finished(): Promise<void> {
        return this._finished;
    }

    constructor(public readonly parent: TreeViewNode<T, K>) {
        super("add");
    }

    complete() {
        this._finished.resolve();
    }
}

export interface TreeViewNodeEvents<T, K> {
    click: TreeViewNodeClickedEvent<T>;
    select: TreeViewNodeSelectedEvent<T>;
    add: TreeViewNodeAddEvent<T, K>;
}

export class TreeViewNode<T, K>
    extends TypedEventBase<TreeViewNodeEvents<T, K>>
    implements ErsatzElement {

    public readonly element: HTMLElement;
    private readonly infoView: HTMLDivElement;
    private readonly subView: HTMLDivElement;
    readonly children: HTMLDivElement;

    private readonly collapser: HTMLButtonElement;
    private readonly label: HTMLSpanElement;
    private readonly adder: HTMLButtonElement;

    private readonly _canChangeOrder: boolean;
    readonly upper: HTMLDivElement;
    readonly lower: HTMLDivElement;
    private wasLeaf = true;

    constructor(
        private readonly treeView: TreeView<T, K>,
        public readonly node: TreeNode<T>,
        private readonly getLabel: (node: TreeNode<T>) => string,
        private readonly getAddable: (v: T) => boolean,
        private readonly _canAddNode: boolean,
        private readonly createElement: (node: TreeNode<T>) => TreeViewNode<T, K>,
        private readonly getIndex: (v: T) => number,
        private readonly getIcon: (node: TreeNode<T>, isOpen: boolean) => string,
        private readonly _getDescription: (value: T) => string,
        private readonly _getChildDescription: (value: T) => string) {

        super();

        this._canChangeOrder = isFunction(this.getIndex);

        const onEnabledClick = (act: () => void) => onClick((evt: Event) => {
            if (!this.disabled && !this.treeView.disabled) {
                evt.preventDefault();
                evt.cancelBubble = true;
                act();
            }
        });

        this.element = Div(
            className("tree-view-node"),
            this.infoView = Div(
                className("tree-view-node-label"),

                draggable(this.canChangeOrder),

                onEnabledClick(() => {
                    if (!this.selected) {
                        this.select();
                    }
                }),

                onDblClick((evt) => {
                    if (!this.disabled && !this.treeView.disabled && !this.node.isLeaf) {
                        evt.preventDefault();
                        evt.cancelBubble = true;
                        this.isOpen = !this.isOpen;
                    }
                }),

                this.collapser = ButtonSmall(
                    title(this.collapserTitle),
                    onEnabledClick(() => {
                        if (this.node.isLeaf) {
                            this.select();
                        }
                        else {
                            this.isOpen = !this.isOpen;
                        }
                    })
                ),

                this.label = Span(this.getLabel(this.node)),

                this.adder = ButtonSmall(
                    title(this.adderTitle),
                    onEnabledClick(async () => {
                        buttonSetEnabled(this.adder, false);
                        const addEvt = new TreeViewNodeAddEvent(this);
                        try {
                            this.dispatchEvent(addEvt);
                            await addEvt.finished;
                        }
                        catch {
                            addEvt.complete();
                        }
                        finally {
                            buttonSetEnabled(this.adder, true);
                        }
                    }),
                    plus.value
                )
            ),

            this.subView = Div(
                className("tree-view-node-children"),
                this.children = Div()
            ),

            this.upper = Div(className("drag-buffer top")),

            this.lower = Div(className("drag-buffer bottom"))
        );

        elementSetDisplay(this.infoView, !this.node.isRoot);
        elementSetDisplay(this.upper, !this.node.isRoot && this.canChangeOrder);
        elementSetDisplay(this.lower, !this.node.isRoot && this.canChangeOrder);

        this.refresh();

        this.isOpen = node.isRoot;
    }

    refresh() {
        if (this.node.isLeaf !== this.wasLeaf) {
            this.wasLeaf = this.node.isLeaf;
            if (this.node.isLeaf) {
                this.infoView.append(this.adder);
            }
            else {
                this.subView.append(this.adder);
            }
        }

        buttonSetEnabled(this.collapser, !this.disabled && !this.treeView.disabled);
        elementSetText(this.collapser, this.getIcon(this.node, this.isOpen));
        elementSetTitle(this.collapser, this.collapserTitle);

        elementSetText(this.label, this.getLabel(this.node));

        elementSetDisplay(this.adder, this.canAddNode() && (this.isOpen || this.node.isLeaf), "inline-block");
        elementSetTitle(this.adder, this.adderTitle)
        buttonSetEnabled(this.adder, !this.disabled && !this.treeView.disabled);
    }

    canAddNode(target: HTMLElement = null) {
        return this._canAddNode && (
            isNullOrUndefined(this.getAddable)
            || this.getAddable(this.node.value)
            || target === this.upper
            || target === this.lower);
    }

    private get collapserTitle(): string {
        return (this.node.isLeaf
            ? "Select "
            : "Expand/collapse ")
            + (this._getDescription
                && this._getDescription(this.node.value)
                || "section");
    }

    private get adderTitle(): string {
        return "Add " + (this._getChildDescription
            && this._getChildDescription(this.node.value)
            || "sub item");
    }

    private get canChangeOrder() {
        return this._canChangeOrder && !this.node.isRoot;
    }

    get disabled(): boolean {
        return this.element.classList.contains("disabled");
    }

    set disabled(v: boolean) {
        if (v !== this.disabled) {
            if (v) {
                this.element.classList.add("disabled");
            }
            else {
                this.element.classList.remove("disabled");
            }

            this.refresh();
        }
    }

    get selected(): boolean {
        return this.element.classList.contains("selected");
    }

    set _selected(v: boolean) {
        if (v !== this.selected) {
            if (v) {
                this.element.classList.add("selected");
            }
            else {
                this.element.classList.remove("selected");
            }
        }
    }

    get highlighted(): number {
        if (this.element.classList.contains("highlighted")) {
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
            this.element.classList.remove("highlighted");
            this.upper.classList.remove("highlighted");
            this.lower.classList.remove("highlighted");

            if (v === 0) {
                this.element.classList.add("highlighted");
            }
            else if (v === -1) {
                this.upper.classList.add("highlighted");
            }
            else if (v === 1) {
                this.lower.classList.add("highlighted");
            }
        }
    }

    get isOpen(): boolean {
        return elementIsDisplayed(this.subView);
    }

    set isOpen(v: boolean) {
        if (v !== this.isOpen) {
            elementSetDisplay(this.subView, v);
            this.refresh();
        }
    }

    add(value: T): void {
        this.isOpen = true;
        const node = new TreeNode(value);
        this.node.connectTo(node);
        const element = this.createElement(node);
        element.select();
        this.refresh();
    }

    select() {
        if (!this.disabled && !this.treeView.disabled) {
            this.dispatchEvent(new TreeViewNodeClickedEvent(this.node));

            if (!this.selected) {
                this.dispatchEvent(new TreeViewNodeSelectedEvent(this.node));
            }
        }
    }
}