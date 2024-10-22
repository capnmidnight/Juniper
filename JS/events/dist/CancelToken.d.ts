import { Exception } from "@juniper-lib/util";
export declare class CancelSignalException extends Exception {
    constructor();
}
export declare class CancelToken {
    #private;
    get cancelled(): boolean;
    check(): void;
    cancel(): void;
}
//# sourceMappingURL=CancelToken.d.ts.map