type GamepadHand = "left" | "right";

interface GamepadPose {
    readonly hasOrientation: boolean;
    readonly hasPosition: boolean;
    readonly position: Float32Array;
    readonly linearVelocity: Float32Array;
    readonly linearAcceleration: Float32Array;
    readonly orientation: Float32Array;
    readonly angularVelocity: Float32Array;
    readonly angularAcceleration: Float32Array;
}

interface Gamepad {
    readonly mapping: GamepadMappingType;
    readonly hand: GamepadHand;
    readonly pose: GamepadPose;
}
