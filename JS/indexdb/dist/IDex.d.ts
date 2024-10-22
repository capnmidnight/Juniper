import { IDexDatabase } from "./IDexDatabase";
import type { IDexDBIndexDef, StoreDef } from "./IDexStore";
/**
 * The entry point for getting access to IndexedDB with Promises.
 */
export declare class IDex {
    #private;
    get name(): string;
    get isOpen(): boolean;
    constructor(name: string);
    delete(): Promise<boolean>;
    open(): Promise<IDexDatabase>;
    assert(...storeDefs: StoreDef[]): Promise<{
        storesByName: Map<string, StoreDef>;
        storeNamesToAdd: Set<string>;
        storeNamesToRemove: Set<string>;
        storeNamesToChange: Set<string>;
        indexesByNameByStoreName: Map<string, Map<string, IDexDBIndexDef<any>>>;
        indexNamesToAddByStoreName: Map<string, Set<string>>;
        indexNamesToRemoveByStoreName: Map<string, Set<string>>;
    }>;
    deleteStores(...storeNames: string[]): Promise<{
        storesByName: Map<string, StoreDef>;
        storeNamesToAdd: Set<string>;
        storeNamesToRemove: Set<string>;
        storeNamesToChange: Set<string>;
        indexesByNameByStoreName: Map<string, Map<string, IDexDBIndexDef<any>>>;
        indexNamesToAddByStoreName: Map<string, Set<string>>;
        indexNamesToRemoveByStoreName: Map<string, Set<string>>;
    }>;
    addStores(...storeDefs: StoreDef[]): Promise<{
        storesByName: Map<string, StoreDef>;
        storeNamesToAdd: Set<string>;
        storeNamesToRemove: Set<string>;
        storeNamesToChange: Set<string>;
        indexesByNameByStoreName: Map<string, Map<string, IDexDBIndexDef<any>>>;
        indexNamesToAddByStoreName: Map<string, Set<string>>;
        indexNamesToRemoveByStoreName: Map<string, Set<string>>;
    }>;
    changeStores(...storeDefs: StoreDef[]): Promise<{
        storesByName: Map<string, StoreDef>;
        storeNamesToAdd: Set<string>;
        storeNamesToRemove: Set<string>;
        storeNamesToChange: Set<string>;
        indexesByNameByStoreName: Map<string, Map<string, IDexDBIndexDef<any>>>;
        indexNamesToAddByStoreName: Map<string, Set<string>>;
        indexNamesToRemoveByStoreName: Map<string, Set<string>>;
    }>;
}
//# sourceMappingURL=IDex.d.ts.map