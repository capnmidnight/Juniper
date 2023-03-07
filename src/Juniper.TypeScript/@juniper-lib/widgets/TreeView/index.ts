import { Attr, autoComplete, className, placeHolder, tabIndex } from "@juniper-lib/dom/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/css";
import {
    isModifierless,
    onClick,
    onContextMenu,
    onDragEnd,
    onDragOver,
    onDragStart,
    onDrop,
    onInput,
    onKeyDown
} from "@juniper-lib/dom/evts";
import {
    ButtonPrimarySmall, ButtonSecondarySmall, Div,
    elementApply,
    elementClearChildren,
    elementGetIndexInParent,
    elementInsertBefore,
    elementReplace,
    elementSetDisplay,
    ErsatzElement,
    InputText
} from "@juniper-lib/dom/tags";
import { arrayClear, arrayRemove } from "@juniper-lib/tslib/collections/arrays";
import { buildTree, TreeNode } from "@juniper-lib/tslib/collections/TreeNode";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { alwaysFalse, identity } from "@juniper-lib/tslib/identity";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { TreeViewNode, TreeViewNodeContextMenuEvent, TreeViewNodeEvents, TreeViewNodeSelectedEvent } from "./TreeViewNode";

import { PropertyDef, PropertyList } from "../PropertyList";
import { SelectList } from "../SelectList";
import "./styles.css";


export interface TreeViewOptions<ValueT, FilterTypeT extends string = never> {
    defaultLabel?: string;
    getLabel: (value: ValueT) => string;
    getParent: (value: ValueT) => ValueT;
    getOrder?: (value: ValueT) => number;
    getDescription: (value: ValueT) => string;
    showNameFilter?: boolean;
    typeFilters?: {
        getTypes: () => FilterTypeT[];
        getTypeLabel: (type: FilterTypeT) => string;
        getTypeFor: (value: ValueT) => FilterTypeT;
    };
    canReorder?: (value: ValueT) => boolean;
    getChildDescription: (value: ValueT) => string;
    canHaveChildren: (node: TreeNode<ValueT>) => boolean;
    canParent?: (parent: TreeNode<ValueT>, child: TreeNode<ValueT>) => boolean;
    replaceElement?: HTMLElement;
    additionalProperties?: PropertyDef[]
}

class TreeViewNodeEvent<EventT extends string, DataT> extends TypedEvent<EventT> {
    constructor(type: EventT, public readonly node: TreeNode<DataT>) {
        super(type);
    }
}

export class TreeViewNodeDeleteEvent<DataT> extends TreeViewNodeEvent<"delete", DataT> {
    constructor(node: TreeNode<DataT>) {
        super("delete", node);
    }
}

export class TreeViewNodeMovedEvent<DataT> extends TreeViewNodeEvent<"moved", DataT> {
    constructor(node: TreeNode<DataT>, public readonly newIndex: number) {
        super("moved", node);
    }
}

export class TreeViewNodeReparentedEvent<DataT> extends TreeViewNodeEvent<"reparented", DataT> {
    constructor(node: TreeNode<DataT>, public readonly newParent: TreeNode<DataT>) {
        super("reparented", node);
    }
}

interface TreeViewEvents<T> {
    moved: TreeViewNodeMovedEvent<T>;
    reparented: TreeViewNodeReparentedEvent<T>;
    delete: TreeViewNodeDeleteEvent<T>;
}

export class TreeView<ValueT, FilterTypeT extends string = never>
    extends TypedEventBase<TreeViewNodeEvents<ValueT> & TreeViewEvents<ValueT>>
    implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly expandButton: HTMLButtonElement;
    private readonly collapseButton: HTMLButtonElement;
    private readonly filters: PropertyList;
    private readonly filterTypeInput: SelectList<FilterTypeT> = null;
    private readonly filterNameInput: HTMLInputElement;

    private readonly children: HTMLElement;
    private readonly options: TreeViewOptions<ValueT, FilterTypeT>;
    private readonly canChangeOrder: boolean;

    private readonly elements = new Array<TreeViewNode<ValueT>>();
    private readonly nodes2Elements = new Map<TreeNode<ValueT>, TreeViewNode<ValueT>>();
    private readonly htmlElements2Nodes = new Map<HTMLElement, TreeNode<ValueT>>();
    private readonly htmlElements2Elements = new Map<HTMLElement, TreeViewNode<ValueT>>();

    private _rootNode: TreeNode<ValueT> = null;
    private locked = false;
    private _disabled = false;

    private typeFilter: FilterTypeT = null;
    private nameFilter: string = null;

    constructor(
        options?: TreeViewOptions<ValueT, FilterTypeT>,
        ...styleProps: (CssElementStyleProp | Attr<"id">)[]) {
        super();

        this.createElement = this.createElement.bind(this);

        this.options = Object.assign<Partial<TreeViewOptions<ValueT, FilterTypeT>>, TreeViewOptions<ValueT, FilterTypeT>>({
            defaultLabel: null,
            getOrder: null,
            replaceElement: null,
            canReorder: alwaysFalse,
            additionalProperties: []
        }, options);

        if (isNullOrUndefined(this.options.canParent)) {
            this.options.canParent = this.options.canHaveChildren;
        }

        this.canChangeOrder = isFunction(this.options.getOrder);

        this.element = Div(
            className("tree-view"),
            ...styleProps,
            this.filters = new PropertyList(
                className("tree-view-controls"),
                ...this.options.additionalProperties
            ),
            Div(
                className("tree-view-controls"),

                this.collapseButton = ButtonPrimarySmall(
                    onClick(() => this.collapseAll()),
                    "Collapse all"
                ),

                this.expandButton = ButtonPrimarySmall(
                    onClick(() => this.expandAll()),
                    "Expand all"
                )
            ),
            Div(
                className("tree-view-inner"),
                tabIndex(0),

                onContextMenu(async (evt) => {
                    if (!this.disabled) {
                        const rootElement = this.nodes2Elements.get(this.rootNode);
                        await rootElement._launchMenu(evt, new TreeViewNodeContextMenuEvent<ValueT>(rootElement));
                    }
                }),

                onClick((evt) => {
                    if (!this.disabled && !evt.defaultPrevented) {
                        for (const element of this.elements) {
                            if (element.selected) {
                                this.selectedNode = null;
                                this.dispatchEvent(new TreeViewNodeSelectedEvent<ValueT>(null));
                                return;
                            }
                        }
                    }
                }),

                onKeyDown((evt) => {
                    if (isModifierless(evt)) {
                        const sel = this.selectedElement;
                        if (sel) {
                            if (evt.key === "Delete") {
                                this.dispatchEvent(new TreeViewNodeDeleteEvent<ValueT>(sel.node))
                            }
                            else if (evt.key === "ArrowUp") {
                                const index = elementGetIndexInParent(sel);
                                if (index > 0) {
                                    const nextHTMLElement = sel.element.parentElement.children[index - 1] as HTMLElement;
                                    const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                    nextElement._select(true);
                                }
                                else if (sel.node.isChild && sel.node.parent.isChild) {
                                    const nextNode = sel.node.parent;
                                    const nextElement = this.nodes2Elements.get(nextNode);
                                    nextElement._select(true);
                                }
                            }
                            else if (evt.key === "ArrowDown") {
                                const index = elementGetIndexInParent(sel);
                                if (index < sel.element.parentElement.childElementCount - 1) {
                                    const nextHTMLElement = sel.element.parentElement.children[index + 1] as HTMLElement;
                                    const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                    nextElement._select(true);
                                }
                                else if (sel.node.isChild) {
                                    const parentNode = sel.node.parent;
                                    const parentElement = this.nodes2Elements.get(parentNode);
                                    const parentIndex = elementGetIndexInParent(parentElement);
                                    const nextHTMLElement = parentElement.element.parentElement.children[parentIndex + 1] as HTMLElement;
                                    if (nextHTMLElement) {
                                        const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                        nextElement._select(true);
                                    }
                                }
                            }
                            else if (evt.key === "ArrowRight") {
                                if (sel.node.hasChildren) {
                                    if (sel.isOpen) {
                                        const nextHTMLElem = sel.children.children[0] as HTMLElement;
                                        if (nextHTMLElem) {
                                            const elem = this.htmlElements2Elements.get(nextHTMLElem);
                                            elem._select(true);
                                        }
                                    }
                                    else {
                                        sel.isOpen = true;
                                    }
                                }
                            }
                            else if (evt.key === "ArrowLeft") {
                                const sel = this.selectedElement;
                                if (sel.isOpen) {
                                    sel.isOpen = false;
                                }
                                else if (sel.node.isChild && sel.node.parent.isChild) {
                                    const parentElem = this.nodes2Elements.get(sel.node.parent);
                                    parentElem._select(true);
                                }
                            }
                        }
                        else if (this.children.children.length > 0) {
                            const rootElem = this.nodes2Elements.get(this.rootNode);
                            let htmlElem: HTMLElement = null;
                            if (evt.key === "ArrowUp") {
                                htmlElem = rootElem.children.children[rootElem.children.children.length - 1] as HTMLElement;
                            }
                            else if (evt.key === "ArrowDown") {
                                htmlElem = rootElem.children.children[0] as HTMLElement;
                            }

                            if (htmlElem) {
                                const elem = this.htmlElements2Elements.get(htmlElem);
                                elem._select(true);
                            }
                        }
                    }
                }),

                this.children = Div(
                    className("tree-view-children")
                )
            )
        );

        if (this.options.showNameFilter || isDefined(this.options.typeFilters)) {
            if (this.options.showNameFilter) {
                this.filters.append(
                    ["Name", this.filterNameInput = InputText(
                        placeHolder("Filter by name"),
                        autoComplete(false),
                        onInput(() => {
                            this.nameFilter = this.filterNameInput.value.toLocaleLowerCase();
                            if (this.nameFilter.length === 0) {
                                this.nameFilter = null;
                            }
                            this.refreshFilter();
                        })
                    )]
                );
            }

            if (isDefined(this.options.typeFilters)) {
                this.filters.append(
                    ["Type", this.filterTypeInput = new SelectList(
                        identity,
                        this.options.typeFilters.getTypeLabel,
                        this.options.typeFilters.getTypeLabel,
                        "Filter by type"
                    )]
                );

                this.filterTypeInput.values = this.options.typeFilters.getTypes();
                this.filterTypeInput.addEventListener("itemselected", (evt) => {
                    this.typeFilter = evt.item;
                    this.refreshFilter();
                });
            }

            this.filters.append(
                ButtonSecondarySmall(
                    "Clear filter",
                    onClick(() => this.clearFilter())
                )
            )
        }

        if (this.canChangeOrder) {

            let draggedElement: TreeViewNode<ValueT> = null;
            let dropElement: TreeViewNode<ValueT> = null;
            let hoverTimer: number = null;
            let delta = 0;
            let lastTarget: HTMLElement = null;

            const clearTarget = () => {
                if (hoverTimer) {
                    clearTimeout(hoverTimer);
                }

                lastTarget = null;

                if (dropElement) {
                    dropElement.highlighted = null;
                    dropElement = null;
                }

                delta = 0;
            }

            elementApply(this.children,

                onDragStart((evt) => {
                    clearTarget();
                    draggedElement = this.findElement(evt.target);
                    draggedElement.element.style.opacity = "0.5";
                }),

                onDragOver((evt) => {
                    const target = evt.target as HTMLElement;
                    const targetChanged = target !== lastTarget;
                    const elem = this.findElement(target);

                    if (targetChanged) {
                        clearTarget();
                        lastTarget = target;
                    }

                    if (elem && this.canReparent(elem, draggedElement, target)) {
                        dropElement = elem;

                        if (target === elem.upper) {
                            delta = -1;
                        }
                        else if (target === elem.lower) {
                            delta = 1;
                        }
                        else {
                            delta = 0;
                        }

                        dropElement.highlighted = delta;

                        if (targetChanged && delta === 0 && !elem.isOpen) {
                            hoverTimer = setTimeout(() => elem.isOpen = true, 1000) as unknown as number;
                        }

                        evt.preventDefault();
                    }
                }),

                onDrop((evt) => {
                    if (this.canReparent(dropElement, draggedElement, lastTarget)) {
                        evt.preventDefault();
                        this.reparentNode(draggedElement.node, dropElement.node, delta);
                    }

                    clearTarget();
                }),

                onDragEnd(() => {
                    if (draggedElement) {
                        draggedElement.element.style.opacity = "1";
                    }

                    clearTarget();
                })
            );
        }

        if (this.options.replaceElement) {
            elementReplace(this.options.replaceElement, this);
        }
    }

    clearFilter() {
        if (isDefined(this.filterNameInput)) {
            this.filterNameInput.value = "";
            this.nameFilter = null;
        }

        if (isDefined(this.filterTypeInput)) {
            this.filterTypeInput.selectedValue = null;
            this.typeFilter = null;
        }

        this.refreshFilter();
    }

    private refreshFilter() {
        const included = new Set<TreeNode<ValueT>>();
        for (const node of this.rootNode.depthFirst()) {
            const nameMatch = isNullOrUndefined(this.nameFilter)
                || this.options.getLabel(node.value).toLocaleLowerCase().indexOf(this.nameFilter) >= 0;
            const typeMatch = isNullOrUndefined(this.typeFilter)
                || this.options.typeFilters.getTypeFor(node.value) === this.typeFilter;
            const isMatch = nameMatch && typeMatch;
            const show = isMatch || included.has(node);
            const elem = this.nodes2Elements.get(node);
            elementSetDisplay(elem, show, "block");
            elem._filtered = included.has(node)  && !isMatch;
            elem.isOpen = true;

            if (show && isDefined(node.parent)) {
                included.add(node.parent);
            }
        }

        if (isDefined(this.selectedElement)) {
            this.selectedElement.scrollIntoView();
        }
    }

    private canReparent(parent: TreeViewNode<ValueT>, child: TreeViewNode<ValueT>, target: HTMLElement) {
        return isDefined(parent)
            && isDefined(child)
            && (parent.canHaveChildren
                && this.options.canParent(parent.node, child.node)
                || target === parent.upper
                || target === parent.lower)
            && !child.node.contains(parent.node)
            && child.node.parent !== parent.node;
    }

    get enabled(): boolean {
        return !this.disabled;
    }

    set enabled(v: boolean) {
        this.disabled = !v;
    }

    get disabled(): boolean {
        return this._disabled;
    }

    set disabled(v: boolean) {
        this.expandButton.disabled
            = this.collapseButton.disabled
            = this.filters.disabled
            = this._disabled
            = v;

        for (const element of this.elements) {
            element.disabled = v;
        }
    }

    async withLock(action: () => Promise<void>): Promise<void> {
        const isEnabled = this.enabled;
        if (this.enabled) {
            try {
                await action();
            }
            finally {
                this.enabled = isEnabled;
            }
        }
    }

    findAll(predicate: (value: ValueT) => boolean): TreeNode<ValueT>[] {
        return Array.from(this.rootNode.searchAll((node) => predicate(node.value)));
    }

    enableOnlyValues(values: readonly ValueT[]): void {
        for (const element of this.elements) {
            element.disabled = values.indexOf(element.node.value) === -1;
            element.specialSelectMode = true;
        }
    }

    enableAllElements() {
        for (const element of this.elements) {
            element.enabled = true;
            element.specialSelectMode = false;
        }
    }

    get values(): ValueT[] {
        return this.rootNode.children
            .flatMap(f => Array.from(f.breadthFirst()))
            .map(n => n.value);
    }

    set values(arr: readonly ValueT[]) {
        this.rootNode = buildTree(
            arr,
            this.options.getParent,
            this.options.getOrder);
    }

    get rootNode() {
        return this._rootNode;
    }

    set rootNode(v) {
        if (v !== this.rootNode) {
            this.locked = true;

            for (const element of this.elements) {
                element.removeScope(this);
                element.removeBubbler(this);
            }

            elementClearChildren(this.children);
            arrayClear(this.elements);
            this.nodes2Elements.clear();
            this.htmlElements2Nodes.clear();
            this.htmlElements2Elements.clear();

            this._rootNode = v;

            if (this.rootNode) {
                for (const node of this.rootNode.breadthFirst()) {
                    this.createElement(node);
                }

                const rootElement = this.nodes2Elements.get(this.rootNode);
                elementApply(this.children, rootElement);
            }

            this.locked = false;

            for (const elem of this.elements) {
                if (elem.canHaveChildren) {
                    this.reorderChildren(elem);
                }
            }

            this.nodes2Elements.get(this.rootNode).refresh();
        }
    }

    get selectedValue(): ValueT {
        const node = this.selectedNode;
        if (node) {
            return node.value;
        }

        return null;
    }

    findNode(data: ValueT): TreeNode<ValueT> {
        return this.rootNode.find(data);
    }

    set selectedValue(v: ValueT) {
        if (v !== this.selectedValue) {
            this.selectedNode = this.rootNode.find(v);
        }
    }

    get selectedNode(): TreeNode<ValueT> {
        const elem = this.selectedElement;
        if (elem) {
            return elem.node;
        }

        return null;
    }

    set selectedNode(v: TreeNode<ValueT>) {
        if (v !== this.selectedNode) {
            this.selectedElement = this.nodes2Elements.get(v);
        }
    }

    private get selectedElement(): TreeViewNode<ValueT> {
        for (const elem of this.elements) {
            if (elem.selected) {
                return elem;
            }
        }

        return null;
    }

    private set selectedElement(e: TreeViewNode<ValueT>) {
        if (isDefined(e)) {
            e._select(false);
            let here = e;
            while (isDefined(here)) {
                here.isOpen = true;
                here = this.nodes2Elements.get(here.node.parent);
            }
            e.scrollIntoView();
        }
        else {
            e = this.selectedElement
            if (isDefined(e)) {
                e._selected = false;
            }
            this.dispatchEvent(new TreeViewNodeSelectedEvent<ValueT>(null));
        }
    }

    private findElement(target: EventTarget) {
        let here = target as HTMLElement;
        while (here !== null) {
            const node = this.htmlElements2Elements.get(here);
            if (isDefined(node)) {
                return node;
            }

            here = here.parentElement;

            if (here === this.children) {
                return null;
            }
        }

        return null;
    }

    private reorderChildren(parentElement: TreeViewNode<ValueT>) {
        if (!this.locked && this.canChangeOrder) {
            const numChildren = parentElement.children.children.length;
            for (let i = 0; i < numChildren; ++i) {
                const htmlElem = parentElement.children.children[i] as HTMLElement;
                const node = this.htmlElements2Nodes.get(htmlElem);
                const elem = this.nodes2Elements.get(node);
                const index = this.options.getOrder(node.value);
                if (index !== i) {
                    this.dispatchEvent(new TreeViewNodeMovedEvent(node, i));
                }
                elem.refresh();
            }

            parentElement.refresh();
        }
    }

    private createElement(node: TreeNode<ValueT>): TreeViewNode<ValueT> {
        const element = new TreeViewNode(
            node,
            this.options.defaultLabel,
            this.options.getLabel,
            this.options.getDescription,
            this.options.canReorder,
            this.options.getChildDescription,
            this.options.canHaveChildren,
            this.createElement);

        element.addScopedEventListener(this, "select", (evt) => {
            for (const elem of this.elements) {
                elem._selected = elem.node === evt.node;
            }
        });

        element.addBubbler(this);

        this.elements.push(element);
        this.nodes2Elements.set(node, element);
        this.htmlElements2Nodes.set(element.element, node);
        this.htmlElements2Elements.set(element.element, element);

        const parentNode = node.parent;
        if (parentNode) {
            const parentElement = this.nodes2Elements.get(parentNode);
            if (parentElement) {
                if (!this.canChangeOrder) {
                    elementApply(parentElement.children, element);
                }
                else {
                    const index = this.options.getOrder(node.value);
                    let nextNodeIndex = Number.MAX_SAFE_INTEGER;
                    let nextNode: TreeNode<ValueT> = null;
                    for (const sibling of parentNode.children) {
                        if (sibling !== node) {
                            const sibIndex = this.options.getOrder(sibling.value);
                            if (sibIndex > index && sibIndex < nextNodeIndex) {
                                nextNode = sibling;
                                nextNodeIndex = sibIndex;
                            }
                        }
                    }

                    const nextElement = this.nodes2Elements.has(nextNode)
                        && this.nodes2Elements.get(nextNode).element
                        || null;

                    elementInsertBefore(parentElement.children, element, nextElement);

                    this.reorderChildren(parentElement);
                }
            }
        }

        return element;
    }

    addValue(value: ValueT) {
        const parent = this.options.getParent(value);
        const parentNode = isNullOrUndefined(parent)
            ? this.rootNode
            : this.rootNode.find(parent);

        const parentElement = this.nodes2Elements.get(parentNode);
        parentElement.add(value);
    }

    updateNode(node: TreeNode<ValueT>) {
        const element = this.nodes2Elements.get(node);
        if (element) {
            element.refresh();
        }
    }

    reparentNode(node: TreeNode<ValueT>, newParentNode: TreeNode<ValueT>, delta: number) {
        const curParent = node.parent;
        const curElement = this.nodes2Elements.get(node);
        const curParentElement = this.nodes2Elements.get(curParent);

        let nextParentElement = this.nodes2Elements.get(newParentNode);
        let nextSiblingElement: TreeViewNode<ValueT> = null;

        if (delta !== 0) {
            nextSiblingElement = nextParentElement;
            newParentNode = newParentNode.parent;
            nextParentElement = this.nodes2Elements.get(newParentNode);

            if (delta === 1) {
                const index = elementGetIndexInParent(nextSiblingElement) + 1;
                const nextSiblingHTMLElement = nextParentElement.children.children[index] as HTMLElement;
                nextSiblingElement = this.htmlElements2Elements.get(nextSiblingHTMLElement);
            }
        }
        else if (this.canChangeOrder) {
            const curOrder = this.options.getOrder(node.value);
            for (let i = 0; i < newParentNode.children.length && nextSiblingElement === null; ++i) {
                const nextSiblingNode = newParentNode.children[i];
                const nextOrder = this.options.getOrder(nextSiblingNode.value);
                if (nextOrder > curOrder) {
                    nextSiblingElement = this.nodes2Elements.get(nextSiblingNode);
                }
            }
        }

        elementInsertBefore(nextParentElement.children, curElement, nextSiblingElement);

        if (nextParentElement !== curParentElement) {
            newParentNode.connectTo(node);
            this.reorderChildren(curParentElement);
            this.reorderChildren(nextParentElement);
            this.dispatchEvent(new TreeViewNodeReparentedEvent(node, newParentNode));
        }
        else {
            this.reorderChildren(curParentElement);
        }

        nextParentElement.isOpen = true;
    }

    removeValue(value: ValueT) {
        this.removeNode(this.rootNode.find(value));
    }

    removeNode(node: TreeNode<ValueT>) {
        const element = this.nodes2Elements.get(node);
        if (element.selected) {
            this.dispatchEvent(new TreeViewNodeSelectedEvent<ValueT>(null));
        }

        const parentElement = this.nodes2Elements.get(node.parent);

        element.removeScope(this);
        element.removeBubbler(this);
        element.element.remove();
        node.removeFromParent();

        arrayRemove(this.elements, element);
        this.nodes2Elements.delete(node);
        this.htmlElements2Nodes.delete(element.element);
        this.htmlElements2Elements.delete(element.element);

        this.reorderChildren(parentElement);
    }

    collapseAll() {
        for (const element of this.elements) {
            if (element.node.isChild
                && element.canHaveChildren) {
                element.isOpen = false;
            }
        }
    }

    expandAll(maxDepth: number = null) {
        for (const element of this.elements) {
            if (element.node.isChild
                && element.canHaveChildren
                && (isNullOrUndefined(maxDepth)
                    || element.node.depth <= maxDepth)) {
                element.isOpen = true;
            }
        }
    }
}