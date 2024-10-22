import { TypedCustomElementConstructor } from "@juniper-lib/dom";
import { DataType, StorageType } from "../../models";
import { IPropertyEditorElement } from "./IPropertyEditorFactory";
export declare function registerPropertyFactory<DataTypeT extends DataType, StorageTypeT extends StorageType, ElementT extends IPropertyEditorElement<DataTypeT, StorageTypeT>>(storageType: StorageTypeT, dataType: DataTypeT, PropertyEditorElement: TypedCustomElementConstructor<ElementT>): import("@juniper-lib/dom").ElementFactory<ElementT>;
//# sourceMappingURL=registerPropertyFactory.d.ts.map