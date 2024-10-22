import { DataType, DataTypeMap, StorageType } from "./DataType";
import { INamed } from "./INamed";
export interface DataTreeModel {
    entityTypes: Record<number, FlatEntityTypeModel>;
    entities: Record<number, FlatEntityModel>;
    properties: Record<number, FlatPropertyModel>;
}
export interface FlatEntityTypeModel extends INamed {
    isPrimary: boolean;
    parentId?: number;
}
export declare function isFlatEntityTypeModel(obj: unknown): obj is FlatEntityTypeModel;
export interface FlatEntityModel extends INamed {
    typeId: number;
    properties: number[];
    parents: FlatRelationshipModel[];
    children: FlatRelationshipModel[];
}
export declare function isFlatEntityModel(obj: unknown): boolean;
export interface FlatPropertyModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends INamed {
    typeId: number;
    description: string;
    type: DataTypeT;
    storage: StorageTypeT;
    unitsCategory: string;
    units: string;
    value: DataTypeMap[DataTypeT][StorageTypeT];
}
export declare function isFlatPropertyModel(obj: unknown): obj is FlatPropertyModel;
export declare function isFlatPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is FlatPropertyModel<DataTypeT, StorageType>;
export declare function isFlatPropertyModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is FlatPropertyModel<DataType, StorageTypeT>;
export declare function isFlatPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is FlatPropertyModel<DataTypeT, StorageTypeT>;
export interface FlatRelationshipModel extends INamed {
}
export declare function isFlatRelationshipModel(obj: unknown): obj is FlatRelationshipModel;
//# sourceMappingURL=DataTreeModel.d.ts.map