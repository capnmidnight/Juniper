type Base = 2 | 10;

const labels = new Map<Base, Map<number, string>>([
    [2, new Map<number, string>([
        [0, "B"],
        [1, "KiB"],
        [2, "MiB"],
        [3, "GiB"],
        [4, "TiB"]
    ])],
    [10, new Map<number, string>([
        [0, "B"],
        [1, "KB"],
        [2, "MB"],
        [3, "GB"],
        [4, "TB"]
    ])]
]);

export function formatBytes(value: number, base: 2 | 10 = 10) {
    const isNegative = value < 0;
    value = Math.abs(value);

    const systemBase = base === 2 ? 1024 : 1000;
    let size = Math.min(4, Math.floor(Math.log(value) / Math.log(systemBase)));
    let divisor = Math.pow(systemBase, size);
    if (2 * value >= systemBase * divisor && size < 4) {
        size++;
        divisor *= systemBase;
    }
    const levels = labels.get(base);
    const label = levels.get(size);
    const converted = value / divisor;
    const str = `${isNegative ? "-" : ""}${converted.toFixed(2)} ${label}`;

    return str;
}
