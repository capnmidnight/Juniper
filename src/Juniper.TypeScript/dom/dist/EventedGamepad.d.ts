import { TypedEvent, TypedEventBase } from "juniper-tslib";
declare class GamepadButtonEvent<T extends string> extends TypedEvent<T> {
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
interface EventedGamepadEvents {
    gamepadbuttonup: GamepadButtonUpEvent;
    gamepadbuttondown: GamepadButtonDownEvent;
    gamepadaxismaxed: GamepadAxisMaxedEvent;
}
export declare class EventedGamepad extends TypedEventBase<EventedGamepadEvents> implements Gamepad {
    private pad;
    private readonly lastAxisValues;
    private readonly _isStick;
    private readonly btnDownEvts;
    private readonly btnUpEvts;
    private readonly wasPressed;
    private axisThresholdMax;
    private axisThresholdMin;
    private readonly axisMaxEvts;
    private readonly wasAxisMaxed;
    private readonly sticks;
    constructor(pad: Gamepad);
    setPad(pad: Gamepad): void;
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
    update(): void;
}
export {};
