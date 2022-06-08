import { className, tabIndex } from "@juniper-lib/dom/attrs";
import {
    backgroundColor,
    border,
    bottom,
    color,
    CssProp,
    cursor,
    height,
    opacity,
    overflowWrap,
    overflowX,
    overflowY,
    padding,
    paddingLeft,
    position,
    rule,
    styles,
    top,
    whiteSpace,
    width
} from "@juniper-lib/dom/css";
import {
    onClick,
    onDragEnd,
    onDragOver,
    onDragStart,
    onDrop,
    onKeyDown
} from "@juniper-lib/dom/evts";
import { isModifierless } from "@juniper-lib/dom/isModifierless";
import {
    Div,
    elementApply,
    elementClearChildren,
    elementGetIndexInParent,
    elementInsertBefore,
    elementReplace,
    ErsatzElement,
    Style
} from "@juniper-lib/dom/tags";
import { fileFolder, label, openFileFolder } from "@juniper-lib/emoji";
import { arrayClear, arrayRemove, buildTree, isDefined, isFunction, TreeNode, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { TreeViewNode, TreeViewNodeEvents, TreeViewNodeSelectedEvent } from "./TreeViewNode";

Style(
    rule(".tree-view",
        border("inset 2px"),
        backgroundColor("white"),
        whiteSpace("nowrap"),
        overflowWrap("normal"),
        overflowX("hidden"),
        overflowY("scroll"),
        height("100%")
    ),

    rule(".tree-view .btn-small",
        padding("0", "0.25em")
    ),

    rule(".tree-view-children",
        height("0")
    ),

    rule(".tree-view-node > .tree-view-node-children",
        paddingLeft("1.5em")
    ),

    rule(".tree-view-children > .tree-view-node > .tree-view-node-children",
        padding(0)
    ),

    rule(".tree-view-node",
        position("relative"),
        whiteSpace("pre"),
        color("black"),
        padding("0", "2px", "1px"),
        cursor("default")
    ),

    rule(".tree-view-node:hover > .tree-view-node-label",
        backgroundColor("rgba(128,128,128,0.125)")
    ),

    rule(".tree-view-node.disabled",
        opacity(0.5),
        cursor("not-allowed")
    ),

    rule(".tree-view-node.disabled:hover > .tree-view-node-label",
        backgroundColor("unset")
    ),

    rule(".tree-view-node.selected",
        backgroundColor("#dff")
    ),

    rule(".tree-view-node.selected.disabled",
        backgroundColor("#ddd")
    ),

    rule(".tree-view-node .drag-buffer",
        position("absolute"),
        width("100%"),
        height("0.25em")
    ),

    rule(".tree-view-node .drag-buffer.top",
        top("-0.125em")),

    rule(".tree-view-node .drag-buffer.bottom",
        bottom("-0.125em")),

    rule(".tree-view-node.highlighted",
        backgroundColor("#dfd")
    ),

    rule(".tree-view-node .drag-buffer.highlighted",
        backgroundColor("#000")
    )
);

export interface TreeViewOptions<T, K> {
    canAdd(value: T): boolean;
    orderBy(value: T): number;
    getIcon(node: TreeNode<T>, isOpen: boolean): string;
    getDescription(value: T): string;
    getChildDescription(value: T): string;
    getKey(v: T): K;
    getParentKey(v: T): K;
    getOrder(v: T): number
    replaceElement: HTMLElement;
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

export class TreeView<T, K>
    extends TypedEventBase<TreeViewNodeEvents<T, K> & TreeViewEvents<T>>
    implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly children: HTMLElement;
    private readonly options: TreeViewOptions<T, K>;
    private readonly canChangeOrder: boolean;

    private readonly elements = new Array<TreeViewNode<T, K>>();
    private readonly nodes2Elements = new Map<TreeNode<T>, TreeViewNode<T, K>>();
    private readonly htmlElements2Nodes = new Map<HTMLElement, TreeNode<T>>();
    private readonly htmlElements2Elements = new Map<HTMLElement, TreeViewNode<T, K>>();

    private rootNode: TreeNode<T> = null;
    private locked = false;

    constructor(
        private readonly getLabel: (node: TreeNode<T>) => string,
        options?: Partial<TreeViewOptions<T, K>>,
        ...styleProps: CssProp[]) {
        super();

        this.options = Object.assign<TreeViewOptions<T, K>, Partial<TreeViewOptions<T, K>>>({
            canAdd: null,
            orderBy: null,
            getKey: null,
            getParentKey: null,
            getOrder: null,
            getDescription: null,
            getChildDescription: null,
            getIcon: (node: TreeNode<T>, isOpen: boolean) => {
                if (node.isLeaf) {
                    return label.value;
                }
                else {
                    return isOpen ? openFileFolder.value : fileFolder.value;
                }
            },
            replaceElement: null
        }, options);

        this.canChangeOrder = isFunction(this.options.orderBy);

        this.element = Div(
            className("tree-view"),
            styles(...styleProps),
            tabIndex(0),
            onClick((evt) => {
                if (!this.disabled && !evt.defaultPrevented) {
                    for (const element of this.elements) {
                        if (element.selected) {
                            this.dispatchEvent(new TreeViewNodeSelectedEvent(null));
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
                            else if (!sel.node.isRoot && !sel.node.parent.isRoot) {
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
                            else if (!sel.node.isRoot) {
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
                            if (!sel.node.isLeaf) {
                                if (!sel.isOpen) {
                                    sel.isOpen = true;
                                }
                                else {
                                    const nextHTMLElem = sel.children.children[0] as HTMLElement;
                                    if (nextHTMLElem) {
                                        const elem = this.htmlElements2Elements.get(nextHTMLElem);
                                        elem.select();
                                    }
                                }
                            }
                        }
                        else if (evt.key === "ArrowLeft") {
                            const sel = this.selectedElement;
                            if (sel.isOpen) {
                                sel.isOpen = false;
                            }
                            else if (!sel.node.isRoot && !sel.node.parent.isRoot) {
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
        );

        if (this.canChangeOrder) {

            let draggedElement: TreeViewNode<T, K> = null;
            let dropElement: TreeViewNode<T, K> = null;
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

                    if (elem && canReparent(draggedElement, elem, target)) {
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
                            hoverTimer = setTimeout(() => elem.isOpen = true, 1000) as any as number;
                        }

                        evt.preventDefault();
                    }
                }),

                onDrop((evt) => {
                    if (dropElement
                        && draggedElement
                        && canReparent(draggedElement, dropElement, lastTarget)) {
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

    get disabled(): boolean {
        return this.rootNode === null
            || !this.nodes2Elements.has(this.rootNode)
            || this.nodes2Elements.get(this.rootNode).disabled;
    }

    set disabled(v: boolean) {
        if (this.rootNode) {
            const rootElement = this.nodes2Elements.get(this.rootNode);
            if (rootElement) {
                rootElement.disabled = v;
            }
        }
    }

    get values(): T[] {
        return Array.from(this.tree.breadthFirst())
            .map(n => n.value);
    }

    set values(arr: readonly T[]) {
        this.tree = buildTree(
            arr,
            this.options.getKey,
            this.options.getParentKey,
            this.options.getOrder);
    }

    get tree() {
        return this.rootNode;
    }

    set tree(rootNode: TreeNode<T>) {
        if (rootNode !== this.tree) {
            this.locked = true;

            for (const element of this.elements) {
                element.removeBubbler(this);
            }

            elementClearChildren(this.children);
            arrayClear(this.elements);
            this.nodes2Elements.clear();
            this.htmlElements2Nodes.clear();
            this.htmlElements2Elements.clear();

            this.rootNode = rootNode;
            if (this.rootNode) {
                for (const node of this.rootNode.depthFirst()) {
                    this.createElement(node);
                }

                const rootElement = this.nodes2Elements.get(rootNode);
                elementApply(this.children, rootElement);
            }

            this.locked = false;

            for (const elem of this.elements) {
                if (!elem.node.isLeaf) {
                    this.reorderChildren(elem);
                }
            }

            this.nodes2Elements.get(this.rootNode).refresh();
        }
    }

    get selectedValue(): T {
        const node = this.selectedNode;
        if (node) {
            return node.value;
        }

        return null;
    }

    set selectedValue(v: T) {
        this.selectedNode = this.tree.find(v);
    }

    get selectedNode(): TreeNode<T> {
        const elem = this.selectedElement;
        if (elem) {
            return elem.node;
        }

        return null;
    }

    set selectedNode(v: TreeNode<T>) {
        let isFirst = true;
        while (isDefined(v) && !v.isRoot) {
            const elem = this.nodes2Elements.get(v);
            if (elem) {
                if (isFirst) {
                    elem.select();
                    isFirst = false;
                }
                elem.isOpen = true;
                v = v.parent;
            }
        }
    }

    private get selectedElement(): TreeViewNode<T, K> {
        for (const elem of this.elements) {
            if (elem.selected) {
                return elem;
            }
        }

        return null;
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

    private reorderChildren(parentElement: TreeViewNode<T, K>) {
        if (!this.locked && this.canChangeOrder) {
            const numChildren = parentElement.children.children.length;
            for (let i = 0; i < numChildren; ++i) {
                const htmlElem = parentElement.children.children[i] as HTMLElement;
                const node = this.htmlElements2Nodes.get(htmlElem);
                const elem = this.nodes2Elements.get(node);
                const index = this.options.orderBy(node.value);
                if (index !== i) {
                    this.dispatchEvent(new TreeViewNodeMovedEvent(node, i));
                }
                elem.refresh();
            }

            parentElement.refresh();
        }
    }

    private createElement(node: TreeNode<T>): TreeViewNode<T, K> {
        const element = new TreeViewNode(
            this,
            node,
            this.getLabel,
            this.options.canAdd,
            !!this.options.canAdd,
            (v) => this.createElement(v),
            this.canChangeOrder
                ? this.options.orderBy
                : null,
            this.options.getIcon,
            this.options.getDescription,
            this.options.getChildDescription);

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
                    const index = this.options.orderBy(node.value);
                    let nextNodeIndex = Number.MAX_SAFE_INTEGER;
                    let nextNode: TreeNode<T> = null;
                    for (const sibling of parentNode.children) {
                        if (sibling !== node) {
                            const sibIndex = this.options.orderBy(sibling.value);
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

    addValue(value: T) {
        const parentID = this.options.getParentKey(value);
        const parentNode = this.tree.search(n => this.options.getKey(n.value) === parentID);
        const parentElement = this.nodes2Elements.get(parentNode);
        parentElement.add(value);
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
        let nextSiblingElement: TreeViewNode<T, K> = null;

        if (delta !== 0) {
            nextSiblingElement = nextParentElement;
            nextParentElement = this.nodes2Elements.get(newParentNode.parent);

            if (delta === 1) {
                const index = elementGetIndexInParent(nextSiblingElement) + 1;
                const nextSiblingHTMLElement = nextParentElement.children.children[index] as HTMLElement;
                nextSiblingElement = this.htmlElements2Elements.get(nextSiblingHTMLElement);
            }

            newParentNode = nextParentElement.node;
        }

        elementInsertBefore(nextParentElement.children, curElement, nextSiblingElement);
        this.reorderChildren(curParentElement);

        if (nextParentElement !== curParentElement) {
            newParentNode.connectTo(node);
            this.reorderChildren(nextParentElement);
            this.dispatchEvent(new TreeViewNodeReparentedEvent(node, newParentNode));
        }

        nextParentElement.isOpen = true;
    }

    removeValue(value: T) {
        this.removeNode(this.tree.find(value));
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
            if (element.node !== this.rootNode && !element.node.isLeaf) {
                element.isOpen = false;
            }
        }
    }

    expandAll() {
        for (const element of this.elements) {
            if (element.node !== this.rootNode && !element.node.isLeaf) {
                element.isOpen = true;
            }
        }
    }
}


function canReparent<T, K>(child: TreeViewNode<T, K>, parent: TreeViewNode<T, K>, target: HTMLElement) {
    return isDefined(parent)
        && parent.canAddNode(target)
        && isDefined(child)
        && !child.node.contains(parent.node)
        && child.node.parent !== parent.node;
}