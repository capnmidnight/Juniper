export declare enum ConnectionState {
    Disconnected = "Disconnected",
    Connecting = "Connecting",
    Connected = "Connected",
    Disconnecting = "Disconnecting"
}
export declare function whenDisconnected(name: string, getState: () => ConnectionState, act: () => Promise<void>): Promise<void>;
export declare function settleConnected(name: string, getState: () => ConnectionState, act: () => Promise<void>): Promise<void>;
//# sourceMappingURL=ConnectionState.d.ts.map