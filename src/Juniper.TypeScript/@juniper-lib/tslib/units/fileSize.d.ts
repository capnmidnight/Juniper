type Base2Units = "KiB" | "MiB" | "GiB" | "TiB";
type Base10Units = "KB" | "MB" | "GB" | "TB";
type Units = "B" | Base2Units | Base10Units;
export declare function formatBytes(value: number, base?: 2 | 10): string;
export declare function toBytes(value: number, units: Units): number;
export {};
//# sourceMappingURL=fileSize.d.ts.map