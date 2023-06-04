import { isDefined } from "@juniper-lib/tslib/typeChecks";

export function mapJoin<KeyT, ValueT>(dest: Map<KeyT, ValueT>, ...sources: Map<KeyT, ValueT>[]): Map<KeyT, ValueT> {
    for (const source of sources) {
        if (isDefined(source)) {
            for (const [key, value] of source) {
                dest.set(key, value);
            }
        }
    }

    return dest;
}
