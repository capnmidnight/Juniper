import { arrayReplace, arrayScan, distinct, groupBy } from '@juniper-lib/util';
import { Button, ClassList, CustomData, Div, HtmlRender, Optional, SingletonStyleBlob, StyleBlob, backgroundColor, border, borderRadius, boxShadow, cssVarDecl, cursor, deg, display, em, fontSize, gap, justifyContent, margin, padding, perc, px, rule, stroke, strokeWidth } from '@juniper-lib/dom';
import { openFileFolder } from '@juniper-lib/emoji';
import { TypedEventTarget } from '@juniper-lib/events';
import { TypedItemSelectedEvent } from '@juniper-lib/widgets';
import { HierarchyNode } from 'd3-hierarchy';
import { Connection, OrgChart } from 'd3-org-chart';
import { DataTreeModel, FlatEntityModel, FlatEntityTypeModel, FlatRelationshipModel, isFlatEntityModel, isFlatEntityTypeModel } from '../models';

export { Layout as OrgChartLayout } from 'd3-org-chart';

interface FlatNodeModel {
    parentId?: number;
    id: number;
    entity: FlatEntityModel;
    _highlighted?: boolean;
    _expanded?: boolean;
}

type CedrusOrgChartEventMap = {
    itemselected: TypedItemSelectedEvent<FlatEntityModel>;
};

export class CedrusOrgChart extends TypedEventTarget<CedrusOrgChartEventMap> {

    readonly #container: HTMLElement;
    readonly #chart: OrgChart<FlatNodeModel>;

    #cardStyles: HTMLLinkElement;

    get layout() { return this.#chart.layout(); }
    set layout(v) { this.#chart.layout(v).render().fit(); }

    #tree: DataTreeModel = null;
    #data: FlatNodeModel[];

    constructor(container: HTMLElement) {
        super();

        this.#container = container;

        SingletonStyleBlob("Juniper::Cedrus::CedrusOrgChart", () => [
            rule(":root",
                cssVarDecl("sat", perc(90)),
                cssVarDecl("lgh1", perc(95)),
                cssVarDecl("lgh2", perc(70)),
                cssVarDecl("lgh3", perc(40)),
                cssVarDecl("ba", "0.5"),
                cssVarDecl("bx", px(5)),
                cssVarDecl("by", px(5)),
                cssVarDecl("bb", px(10))
            ),

            rule("g.nodes-wrapper > g",
                cursor("default")
            ),

            rule(".link",
                stroke("black"),
                strokeWidth(px(5))
            ),

            rule(".cardName",
                fontSize(px(15)),
                display("flex"),
                gap(px(5)),
                margin(px(5)),
                justifyContent("space-between")
            ),

            rule(".cardBody",
                backgroundColor("white"),
                borderRadius(px(10)),
                border("1px solid grey"),
                boxShadow("hsla(0, 0%, 0%, var(--ba)) var(--bx) var(--by) var(--bb)"),
                padding(em(1))
            )
        ]);

        this.#cardStyles = StyleBlob();
        document.head.append(this.#cardStyles);

        this.#container.addEventListener("click", (evt) => {
            if (this.#tree
                && evt.target instanceof HTMLElement
                && "entityId" in evt.target.dataset) {
                const entityId = parseFloat(evt.target.dataset.entityId);
                const entity = this.#tree.entities[entityId];
                this.dispatchEvent(new TypedItemSelectedEvent(entity));
            }
        });

        HtmlRender(this.#container, "Loading...");

        const updateConnection = this.#updateConnection.bind(this);

        this.#chart = new OrgChart<FlatNodeModel>()
            .container(container as any)
            .nodeHeight(() => 85)
            .nodeWidth(() => 220)
            .childrenMargin(() => 50)
            .compactMarginBetween(() => 50)
            .compactMarginPair(() => 150)
            .neighbourMargin(() => 50)
            .siblingsMargin(() => 25)
            .nodeContent(node => this.#makeNode(node))
            .onNodeClick(node => this.#clickNode(node))
            .connectionsUpdate(function (node) {
                // D3 OrgChart incorrectly types parameters of this callback
                const svg = this as unknown as SVGPathElement;
                const connection = node as unknown as Connection;
                updateConnection(svg, connection);
            }) as OrgChart<FlatNodeModel>;


    }

    #updateConnection(svg: SVGPathElement, connection: Connection) {
        svg.setAttribute("stroke", connection.label === "Clone" ? "#ccc" : "#E27396");
        svg.setAttribute('stroke-linecap', 'round');
        svg.setAttribute("stroke-width", '5');
        svg.setAttribute('pointer-events', 'none');
        svg.setAttribute("marker-start", `url(#${connection.from + "_" + connection.to})`);
        svg.setAttribute("marker-end", `url(#arrow-${connection.from + "_" + connection.to})`);
    }

    #makeNode(node: HierarchyNode<FlatNodeModel>) {
        const edge = node.data;
        const entity = edge.entity;
        const props = entity.properties.map(id => this.#tree.properties[id]);
        const title = arrayScan(props, p => p.name == "Title");
        const titleText = title?.value?.toString() ?? "";
        const isEntity = entity.id < this.#fakeEntityIdCounter;
        const entityType = this.#tree.entityTypes[entity.typeId]
        const entityTypeName = entityType?.name ?? "Unknown";

        return Div(
            ClassList(
                "cardBody",
                entityTypeName.replaceAll(" ", "-").replaceAll(/-{2,}/g, "-")
            ),
            CustomData("type-name", entityTypeName),
            Div(
                ClassList("cardType"),
                entityTypeName ?? "Unknown"
            ),
            Div(
                ClassList("cardName"),
                entity.name,
                ...Optional(isEntity,
                    Button(
                        CustomData("entity-id", entity.id),
                        openFileFolder.value
                    )
                )
            ),
            ...Optional(titleText,
                Div(
                    ClassList("cardTitle"),
                    titleText
                )
            )
        ).outerHTML;
    }

    #clickNode(node: HierarchyNode<FlatNodeModel>) {
        const clones = this.#data
            .filter(item => item.entity === node.data.entity);

        const conns: Connection[] = clones
            .filter(item => item.id !== node.data.id)
            .map(clone => {
                return {
                    from: node.data.id,
                    to: clone.id,
                    label: ""
                }
            });

        const expandIds = new Set(
            clones.map(clone => clone.id)
        );

        for (const item of this.#data) {
            item._expanded = expandIds.has(item.id);
        }

        return this.#chart.connections(conns).render();
    }

    expandAll() {
        this.#chart.expandAll();
        return this;
    }

    collapseAll() {
        this.#chart.collapseAll();
        return this;
    }

    fit() {
        this.#chart.fit();
        return this;
    }

    filter(v: string) {
        v = v.toLowerCase();
        for (const item of this.#data) {
            const select = v != '' && item.entity.name.toLowerCase().includes(v);
            item._highlighted = select;
            item._expanded = select;
        }

        this.#chart.render().fit();
    }

    #getEntityTypes() {
        return Object.values(this.#tree.entityTypes).filter(isFlatEntityTypeModel);
    }

    #fakeEntityTypeIdCounter = Number.MAX_SAFE_INTEGER;
    #createFakeEntityType(typeName: string) {
        const fakeEntityType: FlatEntityTypeModel = {
            id: this.#fakeEntityTypeIdCounter--,
            name: typeName,
            isPrimary: true
        };

        this.#tree.entityTypes[fakeEntityType.id] = fakeEntityType;
        return fakeEntityType;
    }

    #getEntities() {
        return Object.values(this.#tree.entities).filter(isFlatEntityModel);
    }

    #fakeEntityIdCounter = Number.MAX_SAFE_INTEGER;
    #createFakeEntity(type: FlatEntityTypeModel, name: string, children: FlatRelationshipModel[]) {
        const fakeEntity: FlatEntityModel = {
            id: this.#fakeEntityIdCounter--,
            typeId: type.id,
            name,
            properties: [],
            parents: [],
            children
        };

        this.#tree.entities[fakeEntity.id] = fakeEntity;

        if (children) {
            for (const relationship of children) {
                const child = this.#tree.entities[relationship.id];
                child.parents.push({
                    id: fakeEntity.id,
                    name: "Default"
                });
            }
        }

        return fakeEntity;
    }

    setTree(tree: DataTreeModel, groupByEntityType = false) {
        this.#tree = tree;
        this.#data = [];

        if (groupByEntityType) {
            const entities = this.#getEntities();
            const entitiesByEntityTypeId = groupBy(entities, e => e.typeId);
            for (const type of this.#getEntityTypes().filter(et => et.isPrimary)) {
                this.#createFakeEntity(type, type.name, entitiesByEntityTypeId.get(type.id));
            }
        }

        const roots = this.#getEntities().filter(e => e.parents.length === 0);

        if (roots.length > 1) {
            const unirootType = this.#createFakeEntityType("Root");

            const uniroot = this.#createFakeEntity(unirootType, "Organizational Chart", roots.map(root => {
                return {
                    id: root.id,
                    name: "Default"
                }
            }));

            arrayReplace(roots, [uniroot]);
        }

        const queue: [FlatEntityModel, number?][] = roots.map(root => [root, null]);

        while (queue.length > 0) {
            const [entity, parentId] = queue.shift();
            const id = this.#data.length + 1;
            this.#data.push({
                parentId,
                id,
                entity
            });

            if (entity.children.length > 0) {
                queue.push(...entity.children
                    .filter(child => this.#tree.entities[child.id])
                    .map(child => [
                        this.#tree.entities[child.id],
                        id
                    ] as [FlatEntityModel, number?])
                );
            }
        }

        this.#container.innerHTML = "";

        const rules = distinct(this.#getEntityTypes().map(type => type.name))
            .map((type, i, arr) => {
                const angle = deg(360 * i / arr.length);
                return rule(`.cardBody[data-type-name='${type}']`,
                    backgroundColor(`hsl(${angle}, var(--sat), var(--lgh1))`),
                    boxShadow(`hsla(${angle}, var(--sat), var(--lgh2), var(--ba)) var(--bx) var(--by) var(--bb)`),
                    border(`1px solid hsl(${angle}, var(--sat), var(--lgh3))`)
                );
            });

        const styleSheet = StyleBlob(...rules);
        this.#cardStyles.replaceWith(styleSheet);
        this.#cardStyles = styleSheet;
        this.#chart.data(this.#data).render().fit();
    }
}
