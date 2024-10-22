import { registerFactory, TypedCustomElementConstructor } from "@juniper-lib/dom";
import { DataType, StorageType } from "../../models";
import { IPropertyEditorElement } from "./IPropertyEditorFactory";


export function registerPropertyFactory<DataTypeT extends DataType, StorageTypeT extends StorageType, ElementT extends IPropertyEditorElement<DataTypeT, StorageTypeT>>(
    storageType: StorageTypeT,
    dataType: DataTypeT,
    PropertyEditorElement: TypedCustomElementConstructor<ElementT>
) {
    const tagName = `${storageType}-${dataType}-editor`.toLowerCase();
    return registerFactory<ElementT>(tagName, PropertyEditorElement);
}