export declare const typePattern: RegExp;
export declare function mediaTypesToAcceptValue(types: MediaType[]): string;
type FileExtension = `.${string}`;
type MIMEType = `${string}/${string}`;
export declare class MediaType {
    #private;
    constructor(type: string, fullSubType: string, extensions?: ReadonlyArray<string>);
    static parse(value: string): MediaType;
    deprecate(message: string): this;
    matches(value: MediaType | string): boolean;
    withParameter(key: string, value: string): MediaType;
    get typeName(): string;
    get tree(): string;
    get suffix(): string;
    get subTypeName(): string;
    get value(): MIMEType;
    __getValueUnsafe(): `${string}/${string}`;
    get fullValue(): string;
    get parameters(): ReadonlyMap<string, string>;
    get extensions(): ReadonlyArray<FileExtension>;
    __getExtensionsUnsafe(): readonly `.${string}`[];
    get primaryExtension(): string;
    toString(): string;
    toFileSystemAPIAccepts(): Record<MIMEType, FileExtension | FileExtension[]>;
    addExtension(fileName: string): string;
    static removeExtension(fileName: string): string;
}
export declare function create(group: string, value: string, ...extensions: string[]): MediaType;
export declare function specialize(group: string): (value: string, ...extensions: string[]) => MediaType;
export {};
//# sourceMappingURL=util.d.ts.map