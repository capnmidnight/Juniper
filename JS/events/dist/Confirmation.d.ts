import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";
type ConfirmationEventsMap = {
    "confirm": TypedEvent<"confirm">;
    "cancel": TypedEvent<"cancel">;
};
export declare class Confirmation extends TypedEventTarget<ConfirmationEventsMap> {
    readonly confirm: () => boolean;
    confirmed(): Promise<boolean>;
    readonly cancel: () => boolean;
    cancelled(): Promise<boolean>;
}
export {};
//# sourceMappingURL=Confirmation.d.ts.map