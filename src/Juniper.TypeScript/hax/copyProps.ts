export function copyProps(from: any, to: any) {
    for (const key in from) {
        let value = from[key];
        if (value instanceof Function) {
            value = value.bind(from);
        }
        to[key] = value;
    }
}