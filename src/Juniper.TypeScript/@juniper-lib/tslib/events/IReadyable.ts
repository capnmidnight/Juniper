
export interface IReadyable {
    readonly isReady: boolean;
    readonly ready: Promise<void>;
}
