export declare function makeLookup(descriptions: {
    [key: string]: string;
}): {
    all: number[];
    id: (description: string) => number;
    description: (id: string | number) => string | number;
};
//# sourceMappingURL=makeLookup.d.ts.map