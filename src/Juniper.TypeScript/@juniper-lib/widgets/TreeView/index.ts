import { className, tabIndex } from "@juniper-lib/dom/attrs";
import { CssProp } from "@juniper-lib/dom/css";
import {
    isModifierless,
    onClick,
    onContextMenu,
    onDragEnd,
    onDragOver,
    onDragStart,
    onDrop,
    onKeyDown
} from "@juniper-lib/dom/evts";
import {
    ButtonPrimarySmall, Div,
    elementApply,
    elementClearChildren,
    elementGetIndexInParent,
    elementInsertBefore,
    elementReplace,
    ErsatzElement
} from "@juniper-lib/dom/tags";
import { arrayClear } from "@juniper-lib/tslib/collections/arrayClear";
import { arrayRemove } from "@juniper-lib/tslib/collections/arrayRemove";
import { buildTree, TreeNode } from "@juniper-lib/tslib/collections/TreeNode";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import "./styles";
import { TreeViewNode, TreeViewNodeContextMenuEvent, TreeViewNodeEvents, TreeViewNodeSelectedEvent } from "./TreeViewNode";


export interface TreeViewOptions<T> {
    getLabel: (value: T) => string;
    getKey: (value: T) => any;
    getParentKey: (value: T) => any;
    getOrder?: (value: T) => number;
    getDescription: (value: T) => string;
    canReorder: (node: T) => boolean;
    getChildDescription: (node: TreeNode<T>) => string;
    canAddChildren: (node: TreeNode<T>) => boolean;
    canParent: (parent: TreeNode<T>, child: TreeNode<T>) => boolean;
    replaceElement?: HTMLElement;
}

class TreeViewNodeEvent<EventT extends string, DataT> extends TypedEvent<EventT> {
    constructor(type: EventT, public readonly node: TreeNode<DataT>) {
        super(type);
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
}

export class TreeView<T>
    extends TypedEventBase<TreeViewNodeEvents<T> & TreeViewEvents<T>>
    implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly expandButton: HTMLButtonElement;
    private readonly collapseButton: HTMLButtonElement;
    private readonly children: HTMLElement;
    private readonly options: TreeViewOptions<T>;
    private readonly canChangeOrder: boolean;

    private readonly elements = new Array<TreeViewNode<T>>();
    private readonly nodes2Elements = new Map<TreeNode<T>, TreeViewNode<T>>();
    private readonly htmlElements2Nodes = new Map<HTMLElement, TreeNode<T>>();
    private readonly htmlElements2Elements = new Map<HTMLElement, TreeViewNode<T>>();

    private _rootNode: TreeNode<T> = null;
    private locked = false;
    private _disabled = false;

    constructor(
        options?: TreeViewOptions<T>,
        ...styleProps: CssProp[]) {
        super();

        this.createElement = this.createElement.bind(this);

        this.options = Object.assign<Partial<TreeViewOptions<T>>, TreeViewOptions<T>>({
            getOrder: null,
            replaceElement: null
        }, options);

        this.canChangeOrder = isFunction(this.options.getOrder);

        this.element = Div(
            className("tree-view"),
            ...styleProps,
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
                        await rootElement._launchMenu(evt, new TreeViewNodeContextMenuEvent<T>(rootElement));
                    }
                }),

                onClick((evt) => {
                    if (!this.disabled && !evt.defaultPrevented) {
                        for (const element of this.elements) {
                            if (element.selected) {
                                this.selectedNode = null;
                                this.dispatchEvent(new TreeViewNodeSelectedEvent<T>(null));
                                return;
                            }
                        }
                    }
                }),

                onKeyDown((evt) => {
                    if (isModifierless(evt)) {
                        const sel = this.selectedElement;
                        if (sel) {
                            if (evt.key === "ArrowUp") {
                                const index = elementGetIndexInParent(sel);
                                if (index > 0) {
                                    const nextHTMLElement = sel.element.parentElement.children[index - 1] as HTMLElement;
                                    const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                    nextElement.select();
                                }
                                else if (sel.node.isChild && sel.node.parent.isChild) {
                                    const nextNode = sel.node.parent;
                                    const nextElement = this.nodes2Elements.get(nextNode);
                                    nextElement.select();
                                }
                            }
                            else if (evt.key === "ArrowDown") {
                                const index = elementGetIndexInParent(sel);
                                if (index < sel.element.parentElement.childElementCount - 1) {
                                    const nextHTMLElement = sel.element.parentElement.children[index + 1] as HTMLElement;
                                    const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                    nextElement.select();
                                }
                                else if (sel.node.isChild) {
                                    const parentNode = sel.node.parent;
                                    const parentElement = this.nodes2Elements.get(parentNode);
                                    const parentIndex = elementGetIndexInParent(parentElement);
                                    const nextHTMLElement = parentElement.element.parentElement.children[parentIndex + 1] as HTMLElement;
                                    if (nextHTMLElement) {
                                        const nextElement = this.htmlElements2Elements.get(nextHTMLElement);
                                        nextElement.select();
                                    }
                                }
                            }
                            else if (evt.key === "ArrowRight") {
                                if (sel.node.hasChildren) {
                                    if (sel.isOpen) {
                                        const nextHTMLElem = sel.children.children[0] as HTMLElement;
                                        if (nextHTMLElem) {
                                            const elem = this.htmlElements2Elements.get(nextHTMLElem);
                                            elem.select();
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
                                    parentElem.select();
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
                                elem.select();
                            }
                        }
                    }
                }),

                this.children = Div(
                    className("tree-view-children")
                )
            )
        );

        if (this.canChangeOrder) {

            let draggedElement: TreeViewNode<T> = null;
            let dropElement: TreeViewNode<T> = null;
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

        this.addEventListener("select", (evt) => {
            for (const elem of this.elements) {
                elem._selected = elem.node === evt.node;
            }
        });

        if (this.options.replaceElement) {
            elementReplace(this.options.replaceElement, this);
        }
    }

    private canReparent(parent: TreeViewNode<T>, child: TreeViewNode<T>, target: HTMLElement) {
        return isDefined(parent)
            && isDefined(child)
            && (parent.canAddChildren
                && this.options.canParent(parent.node, child.node)
                || target === parent.upper
                || target === parent.lower)
            && !child.node.contains(parent.node)
            && child.node.parent !== parent.node;
    }

    get disabled(): boolean {
        return this._disabled;
    }

    set disabled(v: boolean) {
        this.expandButton.disabled
            = this.collapseButton.disabled
            = this._disabled
            = v;

        for (const element of this.elements) {
            element.refresh();
        }
    }

    get values(): T[] {
        return this.rootNode.children
            .flatMap(f => Array.from(f.breadthFirst()))
            .map(n => n.value);
    }

    set values(arr: readonly T[]) {
        const rootNode = buildTree(
            arr,
            this.options.getKey,
            this.options.getParentKey,
            this.options.getOrder);

        this.locked = true;

        for (const element of this.elements) {
            element.removeBubbler(this);
        }

        elementClearChildren(this.children);
        arrayClear(this.elements);
        this.nodes2Elements.clear();
        this.htmlElements2Nodes.clear();
        this.htmlElements2Elements.clear();

        this._rootNode = rootNode;
        if (this.rootNode) {
            for (const node of this.rootNode.breadthFirst()) {
                this.createElement(node);
            }

            const rootElement = this.nodes2Elements.get(rootNode);
            elementApply(this.children, rootElement);
        }

        this.locked = false;

        for (const elem of this.elements) {
            if (elem.canAddChildren) {
                this.reorderChildren(elem);
            }
        }

        this.nodes2Elements.get(this.rootNode).refresh();
    }

    get rootNode() {
        return this._rootNode;
    }

    get selectedValue(): T {
        const node = this.selectedNode;
        if (node) {
            return node.value;
        }

        return null;
    }

    findNode(data: T): TreeNode<T> {
        return this.rootNode.find(data);
    }

    set selectedValue(v: T) {
        if (v !== this.selectedValue) {
            this.selectedNode = this.rootNode.find(v);
        }
    }

    get selectedNode(): TreeNode<T> {
        const elem = this.selectedElement;
        if (elem) {
            return elem.node;
        }

        return null;
    }

    set selectedNode(v: TreeNode<T>) {
        if (v !== this.selectedNode) {
            this.selectedElement = this.nodes2Elements.get(v);
        }
    }

    private get selectedElement(): TreeViewNode<T> {
        for (const elem of this.elements) {
            if (elem.selected) {
                return elem;
            }
        }

        return null;
    }

    private set selectedElement(e: TreeViewNode<T>) {
        if (isDefined(e) && e.node.isChild) {
            e.select();
        }
        else {
            this.dispatchEvent(new TreeViewNodeSelectedEvent<T>(null));
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

    private reorderChildren(parentElement: TreeViewNode<T>) {
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

    private createElement(node: TreeNode<T>): TreeViewNode<T> {
        const element = new TreeViewNode(
            this,
            node,
            this.options.getLabel,
            this.options.getDescription,
            this.options.canReorder,
            this.options.getChildDescription,
            this.options.canAddChildren,
            this.createElement);

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
                    let nextNode: TreeNode<T> = null;
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

    addValue(value: T): TreeViewNode<T> {
        const parentID = this.options.getParentKey(value);
        const parentNode = isNullOrUndefined(parentID)
            ? this.rootNode
            : this.rootNode.search(n =>
                n.isChild
                && this.options.getKey(n.value) === parentID);

        const parentElement = this.nodes2Elements.get(parentNode);
        return parentElement.add(value);
    }

    updateNode(node: TreeNode<T>) {
        const element = this.nodes2Elements.get(node);
        if (element) {
            element.refresh();
        }
    }

    reparentNode(node: TreeNode<T>, newParentNode: TreeNode<T>, delta: number) {
        const curParent = node.parent;
        const curElement = this.nodes2Elements.get(node);
        const curParentElement = this.nodes2Elements.get(curParent);

        let nextParentElement = this.nodes2Elements.get(newParentNode);
        let nextSiblingElement: TreeViewNode<T> = null;

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

    removeValue(value: T) {
        this.removeNode(this.rootNode.find(value));
    }

    removeNode(node: TreeNode<T>) {
        const element = this.nodes2Elements.get(node);
        if (element.selected) {
            this.dispatchEvent(new TreeViewNodeSelectedEvent<T>(null));
        }

        const parentElement = this.nodes2Elements.get(node.parent);

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
            if (element.node.isChild && element.canAddChildren) {
                element.isOpen = false;
            }
        }
    }

    expandAll(maxDepth: number = null) {
        for (const element of this.elements) {
            if (element.node.isChild
                && element.canAddChildren
                && (isNullOrUndefined(maxDepth)
                    || element.node.depth <= maxDepth)) {
                element.isOpen = true;
            }
        }
    }
}