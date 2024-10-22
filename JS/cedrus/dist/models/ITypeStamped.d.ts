export interface ITypeStamped<TypeStamp extends string> {
    typeStamp: TypeStamp;
}
export declare function isTypeStamped<T extends string>(key: T, obj: unknown): obj is ITypeStamped<T>;
//# sourceMappingURL=ITypeStamped.d.ts.map