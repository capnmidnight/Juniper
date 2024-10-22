import { isDefined, isNullOrUndefined } from "./typeChecks";
export function singleton(name, create) {
    const box = globalThis;
    let value = box[name];
    if (isNullOrUndefined(value)
        && isDefined(create)) {
        value = create();
        box[name] = value;
    }
    return value;
}
//# sourceMappingURL=singleton.js.map