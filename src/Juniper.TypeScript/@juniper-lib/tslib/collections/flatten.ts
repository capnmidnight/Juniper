export function flatten<T extends { children: T[] }>(values: ArrayLike<T>): T[] {
    const q = Array.from(values);
    const all: T[] = [];
    while (q.length > 0) {
        const here = q.shift();
        all.push(here);
        if (here.children.length > 0) {
            q.push(...here.children);
        }
    }

    return all.reverse();
}