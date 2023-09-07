import { isNullOrUndefined } from "./typeChecks";
export function singleton(name, create) {
    const box = globalThis;
    let value = box[name];
    if (isNullOrUndefined(value)) {
        if (isNullOrUndefined(create)) {
            throw new Error(`No value ${name} found`);
        }
        value = create();
        box[name] = value;
    }
    return value;
}
//# sourceMappingURL=singleton.js.map