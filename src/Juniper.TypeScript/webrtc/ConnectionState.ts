import { waitFor } from "@juniper/tslib";

export enum ConnectionState {
    Disconnected = "Disconnected",
    Connecting = "Connecting",
    Connected = "Connected",
    Disconnecting = "Disconnecting"
}

async function settleState(
    _name: string,
    getState: () => ConnectionState,
    act: () => Promise<void>,
    target: ConnectionState,
    movingToTarget: ConnectionState,
    leavingTarget: ConnectionState,
    antiTarget: ConnectionState) {
    if (getState() === movingToTarget) {
        await waitFor(() => getState() === target);
    }
    else {
        if (getState() === leavingTarget) {
            await waitFor(() => getState() === antiTarget);
        }

        if (getState() === antiTarget) {
            await act();
        }
    }
}

export function whenDisconnected(name: string, getState: () => ConnectionState, act: () => Promise<void>) {
    return settleState(name, getState, act,
        ConnectionState.Connected,
        ConnectionState.Connecting,
        ConnectionState.Disconnecting,
        ConnectionState.Disconnected);
}

export function settleConnected(name: string, getState: () => ConnectionState, act: () => Promise<void>) {
    return settleState(name, getState, act,
        ConnectionState.Disconnected,
        ConnectionState.Disconnecting,
        ConnectionState.Connecting,
        ConnectionState.Connected);
}