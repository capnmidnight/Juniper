import { XOR, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/util";

export type IDOrName = XOR<{
    id: number;
}, {
    name: string;
}> | {
    id: number;
    name: string
};

export function toIDOrName(idOrName: number | string): IDOrName {
    if (isNumber(idOrName)) {
        return { id: idOrName };
    }
    else {
        return { name: idOrName };
    }
}

export function cloneIDorName(input: IDOrName): IDOrName {
    if (isNullOrUndefined(input)) {
        return null;
    }
    else if (isDefined(input.id)) {
        return { id: input.id };
    }
    else if (isDefined(input.name)) {
        return { name: input.name };
    }
    else {
        throw new Error("Could not clone ID-Or-Name object.")
    }
}