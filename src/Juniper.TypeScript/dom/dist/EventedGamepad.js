import { TypedEvent, TypedEventBase } from "juniper-tslib";
class GamepadButtonEvent extends TypedEvent {
    button;
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
    axis;
    value;
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
export class EventedGamepad extends TypedEventBase {
    pad;
    lastAxisValues = new Array();
    _isStick;
    btnDownEvts = new Array();
    btnUpEvts = new Array();
    wasPressed = new Array();
    axisThresholdMax = 0.9;
    axisThresholdMin = 0.1;
    axisMaxEvts = new Array();
    wasAxisMaxed = new Array();
    sticks = new Array();
    constructor(pad) {
        super();
        this.pad = pad;
        this._isStick = (a) => a % 2 === 0 && a < pad.axes.length - 1;
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
    setPad(pad) {
        this.pad = pad;
        this.update();
    }
    get id() {
        return this.pad.id;
    }
    get index() {
        return this.pad.index;
    }
    get connected() {
        return this.pad.connected;
    }
    get mapping() {
        return this.pad.mapping;
    }
    get timestamp() {
        return this.pad.timestamp;
    }
    get hand() {
        return this.pad.hand;
    }
    get pose() {
        return this.pad.pose;
    }
    get buttons() {
        return this.pad.buttons;
    }
    get axes() {
        return this.pad.axes;
    }
    get hapticActuators() {
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
