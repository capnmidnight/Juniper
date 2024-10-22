import { DataType, DataTypeMap, StorageType } from "./DataType";
import { ENTITY, EntityModel } from "./EntityModel";
import { ICreationTracked } from "./ICreationTracked";
import { ISequenced } from "./ISequenced";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE, PropertyTypeModel } from "./PropertyTypeModel";
import { UserModel } from "./UserModel";
export interface PropertyModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends ISequenced, ITypeStamped<"property">, ICreationTracked {
    type: PropertyTypeModel<DataTypeT, StorageTypeT>;
    value: DataTypeMap[DataTypeT][StorageTypeT];
    units: string;
    reference?: EntityModel;
    entity?: EntityModel;
    updatedBy: UserModel;
    updatedOn: Date;
}
export declare function isPropertyModel(obj: unknown): obj is PropertyModel<DataType, StorageType>;
export declare function isPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyModel<DataTypeT, StorageType>;
export declare function isPropertyModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyModel<DataType, StorageTypeT>;
export declare function isPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyModel<DataTypeT, StorageTypeT>;
export type SetPropertyInput<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> = {
    type: PROPERTY_TYPE;
    value: DataTypeMap[DataTypeT][StorageTypeT];
    unitOfMeasure?: string;
    reference?: ENTITY;
};
export type PROPERTY = TypedNameOrId<"property">;
export declare function PROPERTY(idOrName: number | string): PROPERTY;
//# sourceMappingURL=PropertyModel.d.ts.map