import { Div } from "@juniper-lib/dom";
import { DataType, StorageType } from "../../models";
import { IPropertyEditorFactory } from "./IPropertyEditorFactory";

export function editorNotImplemented<DataTypeT extends DataType, StorageTypeT extends StorageType>(dataType: DataTypeT, storageType: StorageTypeT): IPropertyEditorFactory<DataTypeT, StorageTypeT> {
    return () => {
        const message = `Editing of ${storageType} ${dataType} has not been implemented.`;
        console.warn(message);
        return Div(message) as any;
    }
}

export function editorNotSupported<DataTypeT extends DataType, StorageTypeT extends StorageType>(dataType: DataTypeT, storageType: StorageTypeT): () => void {
    return () => {
        throw new Error(`Editing of ${storageType} ${dataType} will not been implemented.`);
    }
}
