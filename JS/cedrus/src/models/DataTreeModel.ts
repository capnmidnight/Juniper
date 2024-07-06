import { FlatEntityModel } from "./FlatEntityModel";
import { FlatPropertyModel } from "./FlatPropertyModel";

export interface DataTreeModel {
    entities: Record<number, FlatEntityModel>;
    properties: Record<number, FlatPropertyModel>;
    roots: number[];
}
