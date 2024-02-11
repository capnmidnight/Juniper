export interface IDexDBIndexDef<T = any> {
    name: string;
    keyPath: (keyof T & string) | (keyof T & string)[];
    options?: IDBIndexParameters;
}
export interface IDexDBOptionsDef<T = any> {
    autoIncrement?: boolean;
    keyPath?: (keyof T & string) | (keyof T & string)[];
}
export interface StoreDef {
    name: string;
    options?: IDexDBOptionsDef;
    indexes?: IDexDBIndexDef[];
}
//# sourceMappingURL=interfaces.d.ts.map