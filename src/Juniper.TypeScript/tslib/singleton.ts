import { isNullOrUndefined } from "./typeChecks";

export function singleton<T>(name: string, create?: () => T): T {
    const box = globalThis as any;
    let value = box[name] as T;
    if (isNullOrUndefined(value)) {
        if (isNullOrUndefined(create)) {
            throw new Error(`No value ${name} found`);
        }
        value = create();
        box[name] = value;
    }
    return value;
}