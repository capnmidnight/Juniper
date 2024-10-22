type Base2Units = "B" | "KiB" | "MiB" | "GiB" | "TiB" | "PiB";
type Base10Units = "B" | "KB" | "MB" | "GB" | "TB" | "PB";
type Units = Base2Units | Base10Units;
export declare function formatBytes(value: number, base?: 2 | 10): string;
export declare function fromBytes(value: number, units: Units): number;
export declare function toBytes(value: number, units: Units): number;
export {};
//# sourceMappingURL=fileSize.d.ts.map