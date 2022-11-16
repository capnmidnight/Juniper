export function coallesce<T>(overwrite: boolean, to: T, from: T, ...fieldNames: (keyof T)[]): void {
    for (const fieldName of fieldNames) {
        if (!(fieldName in to) || overwrite) {
            to[fieldName] = from[fieldName];
        }
    }
}
