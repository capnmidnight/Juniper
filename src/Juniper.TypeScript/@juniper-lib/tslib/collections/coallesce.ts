export function coallesce<T>(to: T, from: T, ...fieldNames: (keyof T)[]): void {
    for (const fieldName of fieldNames) {
        if (!(fieldName in to)) {
            to[fieldName] = from[fieldName];
        }
    }
}
