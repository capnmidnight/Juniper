import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
export declare class GamepadButtonEvent<T extends "gamepadbuttondown" | "gamepadbuttonup" = "gamepadbuttondown" | "gamepadbuttonup"> extends TypedEvent<T> {
    button: number;
    constructor(type: T, button: number);
}
export declare class GamepadButtonUpEvent extends GamepadButtonEvent<"gamepadbuttonup"> {
    constructor(button: number);
}
export declare class GamepadButtonDownEvent extends GamepadButtonEvent<"gamepadbuttondown"> {
    constructor(button: number);
}
declare class GamepadAxisEvent<T extends string> extends TypedEvent<T> {
    axis: number;
    value: number;
    constructor(type: T, axis: number, value: number);
}
export declare class GamepadAxisMaxedEvent extends GamepadAxisEvent<"gamepadaxismaxed"> {
    constructor(axis: number, value: number);
}
type EventedGamepadEvents = {
    gamepadbuttonup: GamepadButtonUpEvent;
    gamepadbuttondown: GamepadButtonDownEvent;
    gamepadaxismaxed: GamepadAxisMaxedEvent;
};
export declare class EventedGamepad extends TypedEventTarget<EventedGamepadEvents> implements Gamepad {
    private readonly lastAxisValues;
    private readonly btnDownEvts;
    private readonly btnUpEvts;
    private readonly wasPressed;
    private readonly axisMaxEvts;
    private readonly wasAxisMaxed;
    private readonly sticks;
    private axisThresholdMax;
    private axisThresholdMin;
    private _pad;
    constructor();
    get displayId(): any;
    get pad(): Gamepad;
    set pad(pad: Gamepad);
    get id(): string;
    get index(): number;
    get connected(): boolean;
    get mapping(): GamepadMappingType;
    get timestamp(): number;
    get hand(): GamepadHand;
    get pose(): GamepadPose;
    get buttons(): readonly GamepadButton[];
    get axes(): readonly number[];
    get hapticActuators(): readonly GamepadHapticActuator[];
    get vibrationActuator(): GamepadHapticActuator;
}
export {};
//# sourceMappingURL=EventedGamepad.d.ts.map