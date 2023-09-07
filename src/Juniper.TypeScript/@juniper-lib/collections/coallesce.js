export function coallesce(overwrite, to, from, ...fieldNames) {
    for (const fieldName of fieldNames) {
        if (!(fieldName in to) || overwrite) {
            to[fieldName] = from[fieldName];
        }
    }
}
//# sourceMappingURL=coallesce.js.map