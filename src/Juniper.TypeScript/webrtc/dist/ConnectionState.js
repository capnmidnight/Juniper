import { waitFor } from "juniper-tslib";
export var ConnectionState;
(function (ConnectionState) {
    ConnectionState["Disconnected"] = "Disconnected";
    ConnectionState["Connecting"] = "Connecting";
    ConnectionState["Connected"] = "Connected";
    ConnectionState["Disconnecting"] = "Disconnecting";
})(ConnectionState || (ConnectionState = {}));
async function settleState(_name, getState, act, target, movingToTarget, leavingTarget, antiTarget) {
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
export function whenDisconnected(name, getState, act) {
    return settleState(name, getState, act, ConnectionState.Connected, ConnectionState.Connecting, ConnectionState.Disconnecting, ConnectionState.Disconnected);
}
export function settleConnected(name, getState, act) {
    return settleState(name, getState, act, ConnectionState.Disconnected, ConnectionState.Disconnecting, ConnectionState.Connecting, ConnectionState.Connected);
}
