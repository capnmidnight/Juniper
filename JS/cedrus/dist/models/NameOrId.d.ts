import { XOR } from "@juniper-lib/util";
import { ITypeStamped } from "./ITypeStamped";
export type NameOrId = XOR<{
    id: number;
}, {
    name: string;
}> | {
    id: number;
    name: string;
};
export type TypedNameOrId<TypeStamp extends string = string> = ITypeStamped<TypeStamp> & NameOrId;
export declare function TypedNameOrId<TypeStamp extends string>(idOrName: number | string, typeStamp: TypeStamp): TypedNameOrId<TypeStamp>;
export declare function cloneIdOrName<TypeStamp extends string = string>(input: TypedNameOrId<TypeStamp>): TypedNameOrId<TypeStamp>;
export declare function cloneIdOrName<TypeStamp extends string = string>(input: string | TypedNameOrId<TypeStamp>, typeStamp: TypeStamp): TypedNameOrId<TypeStamp>;
export declare function cloneAllIdOrNames<TypeStamp extends string = string>(input: TypedNameOrId<TypeStamp>[]): TypedNameOrId<TypeStamp>[];
export declare function cloneAllIdOrNames<TypeStamp extends string = string>(input: (string | TypedNameOrId<TypeStamp>)[], typeStamp: TypeStamp): TypedNameOrId<TypeStamp>[];
//# sourceMappingURL=NameOrId.d.ts.map