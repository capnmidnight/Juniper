export declare const typePattern: RegExp;
export declare function mediaTypesToAcceptValue(types: MediaType[]): string;
export declare class MediaType {
    private readonly _type;
    private readonly _fullSubType;
    private readonly _tree;
    private readonly _subType;
    private readonly _suffix;
    private readonly _parameters;
    private readonly _value;
    private readonly _fullValue;
    private readonly _extensions;
    private readonly _primaryExtension;
    private depMessage;
    constructor(_type: string, _fullSubType: string, extensions?: ReadonlyArray<string>);
    static parse(value: string): MediaType;
    deprecate(message: string): this;
    private check;
    matches(value: MediaType | string): boolean;
    withParameter(key: string, value: string): MediaType;
    get typeName(): string;
    get tree(): string;
    get suffix(): string;
    get subTypeName(): string;
    get value(): string;
    __getValueUnsafe(): string;
    get fullValue(): string;
    get parameters(): ReadonlyMap<string, string>;
    get extensions(): ReadonlyArray<string>;
    __getExtensionsUnsafe(): readonly string[];
    get primaryExtension(): string;
    toString(): string;
    toFileSystemAPIAccepts(): Record<string, string | string[]>;
    addExtension(fileName: string): string;
    static removeExtension(fileName: string): string;
}
export declare function create(group: string, value: string, ...extensions: string[]): MediaType;
export declare function specialize(group: string): (value: string, ...extensions: string[]) => MediaType;
//# sourceMappingURL=util.d.ts.map