export function mapUnderwrite<KeyT, ValueT>(dest: Map<KeyT, ValueT>, key: KeyT, value: ValueT): Map<KeyT, ValueT> {
    if (!dest.has(key)) {
        dest.set(key, value);
    }

    return dest;
}

