import { className, draggable, title } from "@juniper-lib/dom/attrs";
import { onClick, onContextMenu, onDblClick } from "@juniper-lib/dom/evts";
import {
    buttonSetEnabled, ButtonSmall,
    Div,
    elementIsDisplayed,
    elementSetDisplay,
    elementSetText,
    elementSetTitle,
    ErsatzElement,
    Span
} from "@juniper-lib/dom/tags";
import { blackDiamondCentered, blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered, plus } from "@juniper-lib/emoji";
import { Task, TreeNode, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
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

    readonly upper: HTMLDivElement;
    readonly lower: HTMLDivElement;

    constructor(
        private readonly treeView: TreeView<T, K>,
        public readonly node: TreeNode<T>,
        private readonly getLabel: (node: TreeNode<T>) => string,
        private readonly getDescription: (value: T) => string,
        private readonly getChildDescription: (value: T) => string,
        private readonly _canAddChildren: (value: T) => boolean,
        private readonly _canChangeOrder: (value: T) => boolean,
        private readonly createElement: (node: TreeNode<T>) => TreeViewNode<T, K>) {

        super();

        const onEnabledClick = (act: (evt: Event) => void) => onClick((evt: Event) => {
            if (!this.disabled && !this.treeView.disabled) {
                evt.preventDefault();
                evt.cancelBubble = true;
                act(evt);
            }
        });

        const addItem = async (evt: Event) => {
            if (this.canAddChildren) {
                evt.preventDefault();
                evt.cancelBubble = true;
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
            }
        };

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
                    if (!this.disabled && !this.treeView.disabled && this.canAddChildren) {
                        evt.preventDefault();
                        evt.cancelBubble = true;
                        this.isOpen = !this.isOpen;
                    }
                }),

                this.collapser = ButtonSmall(
                    className("tree-view-node-collapser"),
                    onEnabledClick(() => {
                        if (this.canAddChildren) {
                            this.isOpen = !this.isOpen;
                        }
                        else {
                            this.select();
                        }
                    })
                ),

                this.label = Span(
                    this.getLabel(this.node),
                    onContextMenu((evt) => {
                        if (this.enabled) {
                            addItem(evt);
                        }
                    })
                )
            ),

            this.subView = Div(
                className("tree-view-node-children"),
                this.children = Div(),

                this.adder = ButtonSmall(
                    className("tree-view-node-adder"),
                    title(this.adderTitle),
                    onEnabledClick(addItem),
                    plus.value
                )
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
        const enabled = !this.disabled && !this.treeView.disabled;

        buttonSetEnabled(this.collapser, enabled);
        elementSetText(this.collapser, this.canAddChildren
            ? this.isOpen
                ? blackMediumDownPointingTriangleCentered.value
                : blackMediumRightPointingTriangleCentered.value
            : blackDiamondCentered.value);
        elementSetTitle(this.collapser, this.collapserTitle);

        elementSetText(this.label, this.getLabel(this.node));

        elementSetDisplay(this.adder, this.canAddChildren && (this.isOpen || this.node.isLeaf), "inline-block");
        elementSetTitle(this.adder, this.adderTitle)
        buttonSetEnabled(this.adder, enabled);

        this.element.style.opacity = enabled ? "1" : "0.67";
    }



    get canAddChildren() {
        return this._canAddChildren(this.node.value)
    }

    private get collapserTitle(): string {
        return (!this.canAddChildren
            ? "Select "
            : this.isOpen
                ? "Collapse "
                : "Expand ")
            + this.getDescription(this.node.value);
    }

    private get adderTitle(): string {
        return "Add " + this.getChildDescription(this.node.value);
    }

    private get canChangeOrder() {
        return !this.node.isRoot && this._canChangeOrder(this.node.value);
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