import { TypedEventTarget } from '@juniper-lib/events';
import { TypedItemSelectedEvent } from '@juniper-lib/widgets';
import { DataTreeModel, FlatEntityModel } from '../models';
export { Layout as OrgChartLayout } from 'd3-org-chart';
type CedrusOrgChartEventMap = {
    itemselected: TypedItemSelectedEvent<FlatEntityModel>;
};
export declare class CedrusOrgChart extends TypedEventTarget<CedrusOrgChartEventMap> {
    #private;
    get layout(): import("d3-org-chart").Layout;
    set layout(v: import("d3-org-chart").Layout);
    constructor(container: HTMLElement);
    expandAll(): this;
    collapseAll(): this;
    fit(): this;
    filter(v: string): void;
    setTree(tree: DataTreeModel, groupByEntityType?: boolean): void;
}
//# sourceMappingURL=CedrusOrgChart.d.ts.map