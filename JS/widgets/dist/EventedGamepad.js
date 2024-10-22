import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
export class GamepadButtonEvent extends TypedEvent {
    constructor(type, button) {
        super(type);
        this.button = button;
    }
}
export class GamepadButtonUpEvent extends GamepadButtonEvent {
    constructor(button) {
        super("gamepadbuttonup", button);
    }
}
export class GamepadButtonDownEvent extends GamepadButtonEvent {
    constructor(button) {
        super("gamepadbuttondown", button);
    }
}
class GamepadAxisEvent extends TypedEvent {
    constructor(type, axis, value) {
        super(type);
        this.axis = axis;
        this.value = value;
    }
}
export class GamepadAxisMaxedEvent extends GamepadAxisEvent {
    constructor(axis, value) {
        super("gamepadaxismaxed", axis, value);
    }
}
export class EventedGamepad extends TypedEventTarget {
    constructor() {
        super();
        this.lastAxisValues = new Array();
        this.btnDownEvts = new Array();
        this.btnUpEvts = new Array();
        this.wasPressed = new Array();
        this.axisMaxEvts = new Array();
        this.wasAxisMaxed = new Array();
        this.sticks = new Array();
        this.axisThresholdMax = 0.9;
        this.axisThresholdMin = 0.1;
        this._pad = null;
        Object.seal(this);
    }
    get displayId() {
        if ("displayId" in this.pad) {
            return this.pad.displayId;
        }
        return undefined;
    }
    get pad() {
        return this._pad;
    }
    set pad(pad) {
        this._pad = pad;
        if (this.pad) {
            if (this.btnUpEvts.length === 0) {
                for (let b = 0; b < pad.buttons.length; ++b) {
                    this.btnDownEvts[b] = new GamepadButtonDownEvent(b);
                    this.btnUpEvts[b] = new GamepadButtonUpEvent(b);
                    this.wasPressed[b] = false;
                }
                for (let a = 0; a < pad.axes.length; ++a) {
                    this.axisMaxEvts[a] = new GamepadAxisMaxedEvent(a, 0);
                    this.wasAxisMaxed[a] = false;
                    if (a % 2 === 0 && a < pad.axes.length - 1) {
                        this.sticks[a / 2] = { x: 0, y: 0 };
                    }
                    this.lastAxisValues[a] = pad.axes[a];
                }
            }
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
    get id() {
        if (!this.pad) {
            return null;
        }
        return this.pad.id;
    }
    get index() {
        if (!this.pad) {
            return null;
        }
        return this.pad.index;
    }
    get connected() {
        return this.pad && this.pad.connected;
    }
    get mapping() {
        if (!this.pad) {
            return null;
        }
        return this.pad.mapping;
    }
    get timestamp() {
        if (!this.pad) {
            return null;
        }
        return this.pad.timestamp;
    }
    get hand() {
        if (!this.pad) {
            return null;
        }
        return this.pad.hand;
    }
    get pose() {
        if (!this.pad) {
            return null;
        }
        return this.pad.pose;
    }
    get buttons() {
        if (!this.pad) {
            return null;
        }
        return this.pad.buttons;
    }
    get axes() {
        if (!this.pad) {
            return null;
        }
        return this.pad.axes;
    }
    get hapticActuators() {
        if (!this.pad) {
            return null;
        }
        return this.pad.hapticActuators;
    }
    get vibrationActuator() {
        if (!this.pad) {
            return null;
        }
        return this.pad.vibrationActuator;
    }
}
//# sourceMappingURL=EventedGamepad.js.map