import { TypedEvent, TypedEventBase } from "@juniper/tslib";

class GamepadButtonEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public button: number) {
        super(type);
    }
}

export class GamepadButtonUpEvent extends GamepadButtonEvent<"gamepadbuttonup"> {
    constructor(button: number) {
        super("gamepadbuttonup", button);
    }
}

export class GamepadButtonDownEvent extends GamepadButtonEvent<"gamepadbuttondown"> {
    constructor(button: number) {
        super("gamepadbuttondown", button);
    }
}

class GamepadAxisEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public axis: number, public value: number) {
        super(type);
    }
}

export class GamepadAxisMaxedEvent extends GamepadAxisEvent<"gamepadaxismaxed"> {
    constructor(axis: number, value: number) {
        super("gamepadaxismaxed", axis, value);
    }
}

type Stick = { x: number, y: number; };

interface EventedGamepadEvents {
    gamepadbuttonup: GamepadButtonUpEvent;
    gamepadbuttondown: GamepadButtonDownEvent;
    gamepadaxismaxed: GamepadAxisMaxedEvent;
}

export class EventedGamepad
    extends TypedEventBase<EventedGamepadEvents>
    implements Gamepad {
    private readonly lastAxisValues = new Array<number>();
    private readonly _isStick: (a: any) => boolean;

    private readonly btnDownEvts = new Array<GamepadButtonDownEvent>();
    private readonly btnUpEvts = new Array<GamepadButtonUpEvent>();
    private readonly wasPressed = new Array<boolean>();
    private axisThresholdMax = 0.9;
    private axisThresholdMin = 0.1;
    private readonly axisMaxEvts = new Array<GamepadAxisMaxedEvent>();
    private readonly wasAxisMaxed = new Array<boolean>();
    private readonly sticks = new Array<Stick>();

    constructor(private pad: Gamepad) {
        super();
        this._isStick = (a: number) => a % 2 === 0 && a < pad.axes.length - 1;

        for (let b = 0; b < pad.buttons.length; ++b) {
            this.btnDownEvts[b] = new GamepadButtonDownEvent(b);
            this.btnUpEvts[b] = new GamepadButtonUpEvent(b);
            this.wasPressed[b] = false;
        }

        for (let a = 0; a < pad.axes.length; ++a) {
            this.axisMaxEvts[a] = new GamepadAxisMaxedEvent(a, 0);
            this.wasAxisMaxed[a] = false;
            if (this._isStick(a)) {
                this.sticks[a / 2] = { x: 0, y: 0 };
            }

            this.lastAxisValues[a] = pad.axes[a];
        }

        Object.seal(this);
    }

    setPad(pad: Gamepad) {
        this.pad = pad;
        this.update();
    }

    get id(): string {
        return this.pad.id;
    }

    get index(): number {
        return this.pad.index;
    }

    get connected(): boolean {
        return this.pad.connected;
    }

    get mapping() {
        return this.pad.mapping;
    }

    get timestamp() {
        return this.pad.timestamp;
    }

    get hand(): GamepadHand {
        return this.pad.hand;
    }

    get pose(): GamepadPose {
        return this.pad.pose;
    }

    get buttons(): readonly GamepadButton[] {
        return this.pad.buttons;
    }

    get axes(): readonly number[] {
        return this.pad.axes;
    }

    get hapticActuators(): readonly GamepadHapticActuator[] {
        return this.pad.hapticActuators;
    }

    update() {
        for (let b = 0; b < this.pad.buttons.length; ++b) {
            const wasPressed = this.wasPressed[b];
            const pressed = this.pad.buttons[b].pressed;
            if (pressed !== wasPressed) {
                this.wasPressed[b] = pressed;
                this.dispatchEvent((pressed
                    ? this.btnDownEvts
                    : this.btnUpEvts)[b]);
            }
        }

        for (let a = 0; a < this.pad.axes.length; ++a) {
            const wasMaxed = this.wasAxisMaxed[a];
            const val = this.pad.axes[a];
            const dir = Math.sign(val);
            const mag = Math.abs(val);
            const maxed = mag >= this.axisThresholdMax;
            const mined = mag <= this.axisThresholdMin;
            const correctedVal = dir * (maxed ? 1 : (mined ? 0 : mag));
            if (maxed && !wasMaxed) {
                this.axisMaxEvts[a].value = correctedVal;
                this.dispatchEvent(this.axisMaxEvts[a]);
            }

            this.wasAxisMaxed[a] = maxed;
            this.lastAxisValues[a] = correctedVal;
        }

        for (let a = 0; a < this.axes.length - 1; a += 2) {
            const stick = this.sticks[a / 2];
            stick.x = this.axes[a];
            stick.y = this.axes[a + 1];
        }
    }
}
