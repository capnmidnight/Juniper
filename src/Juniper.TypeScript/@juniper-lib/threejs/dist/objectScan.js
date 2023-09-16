export function objectScan(obj, test) {
    const queue = [obj];
    while (queue.length > 0) {
        const here = queue.shift();
        if (test(here)) {
            return here;
        }
        if (here.children.length > 0) {
            queue.push(...here.children);
        }
    }
    return null;
}
//# sourceMappingURL=objectScan.js.map