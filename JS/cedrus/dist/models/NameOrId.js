import { isDefined, isNullOrUndefined, isNumber, isString } from "@juniper-lib/util";
export function TypedNameOrId(idOrName, typeStamp) {
    if (isNumber(idOrName)) {
        return { typeStamp, id: idOrName };
    }
    else {
        return { typeStamp, name: idOrName };
    }
}
function cloneIdOrNameInternal(input, typeStamp) {
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
        throw new Error("Could not clone ID-Or-Name object.");
    }
}
export function cloneIdOrName(input, typeStamp) {
    return cloneIdOrNameInternal(input, typeStamp);
}
export function cloneAllIdOrNames(input, typeStamp) {
    return input.map(item => cloneIdOrNameInternal(item, typeStamp));
}
//# sourceMappingURL=NameOrId.js.map