import { DataType, StorageType } from "../../models";
import { IPropertyEditorFactory } from "./IPropertyEditorFactory";
export declare function editorNotImplemented<DataTypeT extends DataType, StorageTypeT extends StorageType>(dataType: DataTypeT, storageType: StorageTypeT): IPropertyEditorFactory<DataTypeT, StorageTypeT>;
export declare function editorNotSupported<DataTypeT extends DataType, StorageTypeT extends StorageType>(dataType: DataTypeT, storageType: StorageTypeT): () => void;
//# sourceMappingURL=editorNotImplemented.d.ts.map