import { TreeNode } from "@juniper-lib/collections/TreeNode";
import { ClassList, Draggable, Title_attr } from "@juniper-lib/dom/attrs";
import { onClick, onContextMenu, onDblClick } from "@juniper-lib/dom/evts";
import { ButtonSmall, Div, ErsatzElement, Span, elementIsDisplayed, elementSetDisplay, elementSetText, elementSetTitle } from "@juniper-lib/dom/tags";
import { blackDiamondCentered, blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered, plus } from "@juniper-lib/emoji";
import { Task } from "@juniper-lib/events/Task";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/TypedEventBase";
import { debounce } from "@juniper-lib/events/debounce";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

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

    constructor(type: EventT, public readonly parent: TreeViewNode<ValueT>) {
        super(type);
    }

    complete() {
        this._finished.resolve();
    }
}

export class TreeViewNodeAddEvent<T> extends TreeViewNodeEvent<"add", T> {
    constructor(parent: TreeViewNode<T>) {
        super("add", parent);
    }
}

export class TreeViewNodeContextMenuEvent<T> extends TreeViewNodeEvent<"contextmenu", T> {
    constructor(parent: TreeViewNode<T>) {
        super("contextmenu", parent);
    }
}

export type TreeViewNodeEvents<T> = {
    click: TreeViewNodeClickedEvent<T>;
    select: TreeViewNodeSelectedEvent<T>;
    add: TreeViewNodeAddEvent<T>;
    contextmenu: TreeViewNodeContextMenuEvent<T>;
}

export class TreeViewNode<T>
    extends TypedEventBase<TreeViewNodeEvents<T>>
    implements ErsatzElement {

    public readonly element: HTMLElement;
    private readonly infoView: HTMLDivElement;
    private readonly subView: HTMLDivElement;
    readonly children: HTMLDivElement;

    private readonly collapser: HTMLButtonElement;
    private readonly labeler: HTMLSpanElement;
    private readonly adder: HTMLButtonElement;

    readonly upper: HTMLDivElement;
    readonly lower: HTMLDivElement;

    readonly refresh: () => void;

    constructor(
        public readonly node: TreeNode<T>,
        private readonly defaultLabel: string,
        private readonly _getLabel: (value: T) => string,
        private readonly _getDescription: (value: T) => string,
        private readonly _canChangeOrder: (value: T) => boolean,
        private readonly _getChildDescription: (node: T) => string,
        private readonly _canHaveChildren: (node: TreeNode<T>) => boolean,
        private readonly createElement: (node: TreeNode<T>) => TreeViewNode<T>) {

        super();

        this.refresh = debounce(() => this.onRefresh());

        const onEnabledClick = (act: (evt: Event) => void) => onClick((evt: Event) => {
            if (this.enabled) {
                evt.preventDefault();
                evt.stopPropagation();
                act(evt);
            }
        });

        this.element = Div(
            ClassList("tree-view-node"),

            this.upper = Div(ClassList("drag-buffer top")),

            this.infoView = Div(
                ClassList("tree-view-node-label"),

                Draggable(this.canChangeOrder),

                onEnabledClick(() => {
                    if (!this.selected) {
                        this._select(true);
                    }
                }),

                onDblClick((evt) => {
                    if (this.enabled && this.canHaveChildren) {
                        evt.preventDefault();
                        evt.stopPropagation();
                        this.isOpen = !this.isOpen;
                    }
                }),

                this.collapser = ButtonSmall(
                    ClassList("tree-view-node-collapser"),
                    onClick((evt) => {
                        if (this.canHaveChildren) {
                            evt.preventDefault();
                            evt.stopPropagation();
                            this.isOpen = !this.isOpen;
                        }
                        else {
                            this._select(true);
                        }
                    })
                ),

                this.labeler = Span(
                    this.label,
                    onContextMenu(async (evt) => {
                        if (this.enabled) {
                            await this._launchMenu(evt, new TreeViewNodeContextMenuEvent(this));
                        }
                    })
                )
            ),

            this.subView = Div(
                ClassList("tree-view-node-children"),
                this.children = Div(),

                this.adder = ButtonSmall(
                    ClassList("tree-view-node-adder"),
                    Title_attr(this.adderTitle),
                    onEnabledClick(async (evt: Event) => {
                        if (this.canHaveChildren) {
                            await this._launchMenu(evt, new TreeViewNodeAddEvent(this));
                        }
                    }),
                    plus.emojiStyle
                )
            ),

            this.lower = Div(ClassList("drag-buffer bottom"))
        );

        this.refresh();

        this.isOpen = node.isRoot;
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
                ? this.isOpen
                    ? blackMediumDownPointingTriangleCentered.emojiStyle
                    : blackMediumRightPointingTriangleCentered.emojiStyle
                : blackDiamondCentered.emojiStyle);
        this.collapser.style.opacity = canOpenClose
            ? "1"
            : "0";

        elementSetText(this.labeler, this.label);

        elementSetTitle(this.adder, this.adderTitle);
        elementSetDisplay(this.adder, this.canHaveChildren, "inline-block");
        elementSetDisplay(this.upper, this.node.isChild && this.canChangeOrder && isNullOrUndefined(this.element.previousSibling));
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
        else if (this.isOpen) {
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
        return this.element.classList.contains("disabled");
    }

    set disabled(v: boolean) {
        if (v !== this.disabled) {
            this.element.classList.toggle("disabled");
            this.refresh();
        }
    }

    get enabled(): boolean {
        return !this.disabled;
    }

    set enabled(v: boolean) {
        this.disabled = !v;
    }

    get filtered(): boolean {
        return this.element.classList.contains("filtered");
    }

    set _filtered(v: boolean) {
        if (v !== this.filtered) {
            this.element.classList.toggle("filtered");
        }
    }

    get selected(): boolean {
        return this.element.classList.contains("selected");
    }

    set _selected(v: boolean) {
        if (v !== this.selected) {
            this.element.classList.toggle("selected");
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

    scrollIntoView() {
        this.element.scrollIntoView({
            behavior: "smooth",
            block: "center",
            inline: "nearest"
        });
    }
}