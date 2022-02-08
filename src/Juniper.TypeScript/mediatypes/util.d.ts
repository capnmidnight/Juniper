export interface MediaType {
    typeName: string;
    subTypeName: string;
    value: string;
    fullValue: string;
    extensions: ReadonlyArray<string>;
    primaryExtension: string;
    withParameter(key: string, value: string): MediaType;
    toString(): string;
    addExtension(fileName: string): string;
    matches(value: MediaType | string): boolean;
    matchesFileName(fileName: string): boolean;
}
export declare function deprecate(type: MediaType, msg: string): MediaType;
export declare function mediaTypeParse(value: string): MediaType;
export declare function mediaTypesMatch(value: string | MediaType, pattern: string | MediaType): boolean;
export declare function mediaTypeGuessByFileName(fileName: string): MediaType[];
export declare function mediaTypeGuessByExtension(ext: string): MediaType[];
export declare function mediaTypeNormalizeFileName(fileName: string, fileType: string): string;
export declare function create(group: string, value: string, ...extensions: string[]): void;
export declare const specialize: (group: string) => (value: string, ...extensions: string[]) => void;
