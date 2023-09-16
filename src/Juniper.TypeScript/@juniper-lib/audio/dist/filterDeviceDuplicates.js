export function filterDeviceDuplicates(devices) {
    const filtered = [];
    for (let i = 0; i < devices.length; ++i) {
        const a = devices[i];
        let found = false;
        for (let j = 0; j < filtered.length && !found; ++j) {
            const b = filtered[j];
            found = a.kind === b.kind && b.label.indexOf(a.label) > 0;
        }
        if (!found) {
            filtered.push(a);
        }
    }
    return filtered;
}
//# sourceMappingURL=filterDeviceDuplicates.js.map