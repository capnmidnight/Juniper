export function flatten(values) {
    const q = Array.from(values);
    const all = [];
    while (q.length > 0) {
        const here = q.shift();
        all.push(here);
        if (here.children.length > 0) {
            q.push(...here.children);
        }
    }
    return all.reverse();
}
//# sourceMappingURL=flatten.js.map