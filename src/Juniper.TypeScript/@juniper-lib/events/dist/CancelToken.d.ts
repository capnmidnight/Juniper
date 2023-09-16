import { Exception } from "@juniper-lib/tslib/dist/Exception";
export declare class CancelSignalException extends Exception {
    constructor();
}
export declare class CancelToken {
    private _cancelled;
    get cancelled(): boolean;
    check(): void;
    cancel(): void;
}
//# sourceMappingURL=CancelToken.d.ts.map