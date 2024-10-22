import { XOR, isDefined, isNullOrUndefined, isNumber, isString } from "@juniper-lib/util";
import { ITypeStamped } from "./ITypeStamped";

export type NameOrId = XOR<{
    id: number;
}, {
    name: string;
}> | {
    id: number;
    name: string
};

export type TypedNameOrId<TypeStamp extends string = string> = ITypeStamped<TypeStamp> & NameOrId;
export function TypedNameOrId<TypeStamp extends string>(idOrName: number | string, typeStamp: TypeStamp): TypedNameOrId<TypeStamp> {
    if (isNumber(idOrName)) {
        return { typeStamp, id: idOrName };
    }
    else {
        return { typeStamp, name: idOrName };
    }
}

function cloneIdOrNameInternal<TypeStamp extends string = string>(input: string | TypedNameOrId<TypeStamp>, typeStamp?: TypeStamp): TypedNameOrId<TypeStamp> {
    if (isNullOrUndefined(input)) {
        return null;
    }
    else if (isString(input)) {
        return { typeStamp, name: input };
    }
    else if (isDefined(input.id)) {
        return { typeStamp: input.typeStamp, id: input.id };
    }
    else if (isDefined(input.name)) {
        return { typeStamp: input.typeStamp, name: input.name };
    }
    else {
        throw new Error("Could not clone ID-Or-Name object.")
    }
}

export function cloneIdOrName<TypeStamp extends string = string>(input: TypedNameOrId<TypeStamp>): TypedNameOrId<TypeStamp>;
export function cloneIdOrName<TypeStamp extends string = string>(input: string | TypedNameOrId<TypeStamp>, typeStamp: TypeStamp): TypedNameOrId<TypeStamp>;
export function cloneIdOrName<TypeStamp extends string = string>(input: string | TypedNameOrId<TypeStamp>, typeStamp?: TypeStamp): TypedNameOrId<TypeStamp> {
    return cloneIdOrNameInternal(input, typeStamp);
}

export function cloneAllIdOrNames<TypeStamp extends string = string>(input: TypedNameOrId<TypeStamp>[]): TypedNameOrId<TypeStamp>[];
export function cloneAllIdOrNames<TypeStamp extends string = string>(input: (string | TypedNameOrId<TypeStamp>)[], typeStamp: TypeStamp): TypedNameOrId<TypeStamp>[];
export function cloneAllIdOrNames<TypeStamp extends string = string>(input: (string | TypedNameOrId<TypeStamp>)[], typeStamp?: TypeStamp): TypedNameOrId<TypeStamp>[] {
    return input.map(item => cloneIdOrNameInternal(item, typeStamp));
}