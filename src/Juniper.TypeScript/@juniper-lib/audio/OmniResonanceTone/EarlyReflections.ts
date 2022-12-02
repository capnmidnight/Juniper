/**
 * @license
 * Copyright 2017 Google Inc. All Rights Reserved.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @file Ray-tracing-based early reflections model.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { isBadNumber, isGoodNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyVec3, vec3 } from "gl-matrix";
import { BiquadFilter, ChannelMerger, Delay, Gain } from "../nodes";
import { chain, connect, ErsatzAudioNode, removeVertex } from "../util";
import * as Utils from "./utils";

export interface EarlyReflectionsOptions {
    /**
     * Room dimensions (in meters). Defaults to
     * {@linkcode Utils.DEFAULT_ROOM_DIMENSIONS DEFAULT_ROOM_DIMENSIONS}.
     */
    dimensions: Utils.RoomDimensions;

    /**
     * Frequency-independent reflection coeffs per wall. Defaults to
     * {@linkcode Utils.DEFAULT_REFLECTION_COEFFICIENTS
     * DEFAULT_REFLECTION_COEFFICIENTS}.
     */
    coefficients: Utils.ReflectionCube;

    /**
     * (in meters / second). Defaults to {@linkcode Utils.DEFAULT_SPEED_OF_SOUND DEFAULT_SPEED_OF_SOUND}.
     **/
    speedOfSound: number;

    /**
     * (in meters). Defaults to {@linkcode Utils.DEFAULT_POSITION DEFAULT_POSITION}.
     */
    listenerPosition: ReadonlyVec3;
}

/**
 * Ray-tracing-based early reflections model.
 */
export class EarlyReflections implements ErsatzAudioNode {

    /**
     * The room's speed of sound (in meters/second).
     */
    private _speedOfSound: number;

    /**
     * Mono (1-channel) input {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
     */
    readonly input: GainNode;

    /**
     * First-order ambisonic (4-channel) output {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
     */
    readonly output: GainNode;
    readonly halfDimensions: Utils.RoomDimensions = new Map();

    private readonly _coefficients: Utils.ReflectionCube = new Map();
    private readonly _lowpass: BiquadFilterNode;
    private readonly _delays: Map<Utils.CubeSide, DelayNode>;
    private readonly _gains: Map<Utils.CubeSide, GainNode>;
    private readonly _inverters: Map<Utils.InverseCubeSide, GainNode>;
    private readonly _merger: ChannelMergerNode;
    private readonly _listenerPosition: vec3;
    private readonly _distances: Utils.ReflectionCube = new Map();

    /**
     * Ray-tracing-based early reflections model.
     * @param name a name for this node, to help differentiate it from other nodes in graph rendering.
     * @param context Associated {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param options
     */
    constructor(name: string, context: BaseAudioContext, options: Partial<EarlyReflectionsOptions>) {
        // Use defaults for undefined arguments.
        if (isNullOrUndefined(options)) {
            options = {};
        }
        if (isBadNumber(options.speedOfSound)) {
            options.speedOfSound = Utils.DEFAULT_SPEED_OF_SOUND;
        }
        if (isNullOrUndefined(options.listenerPosition)) {
            options.listenerPosition = Utils.DEFAULT_POSITION;
        }

        // Assign room's speed of sound.
        this.speedOfSound = options.speedOfSound;

        // Create nodes.
        this.input = Gain(`${name}-early-reflections-input-gain`, context);
        this.output = Gain(`${name}-early-reflections-output-gain`, context);
        this._lowpass = BiquadFilter(`${name}-early-reflections-lowpass-biquad`, context, {
            type: "lowpass",
            Q: 0,
            frequency: Utils.DEFAULT_REFLECTION_CUTOFF_FREQUENCY
        });

        this._delays = new Map([
            ["left", Delay(`${name}-early-reflections-delays-left`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["right", Delay(`${name}-early-reflections-delays-right`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["front", Delay(`${name}-early-reflections-delays-front`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["back", Delay(`${name}-early-reflections-delays-back`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["up", Delay(`${name}-early-reflections-delays-up`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["down", Delay(`${name}-early-reflections-delays-down`, context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })]
        ]);

        // gainPerWall = (ReflectionCoeff / Attenuation)
        this._gains = new Map([
            ["left", Gain(`${name}-early-reflections-gains-left`, context, { gain: 0 })],
            ["right", Gain(`${name}-early-reflections-gains-right`, context, { gain: 0 })],
            ["front", Gain(`${name}-early-reflections-gains-front`, context, { gain: 0 })],
            ["back", Gain(`${name}-early-reflections-gains-back`, context, { gain: 0 })],
            ["up", Gain(`${name}-early-reflections-gains-up`, context, { gain: 0 })],
            ["down", Gain(`${name}-early-reflections-gains-down`, context, { gain: 0 })]
        ]);

        // 3 of these are needed for right/back/down walls.
        this._inverters = new Map([
            ["right", Gain(`${name}-early-reflections-inverters-right`, context, { gain: -1 })],
            ["back", Gain(`${name}-early-reflections-inverters-right`, context, { gain: -1 })],
            ["down", Gain(`${name}-early-reflections-inverters-right`, context, { gain: -1 })]
        ]);

        // First-order encoding only.
        this._merger = ChannelMerger(`${name}-early-reflections-merger`, context, {
            channelCount: 4
        });

        // Connect nodes.
        connect(this.input, this._lowpass);
        connect(this._merger, this.output);

        // Connect gains to ambisonic channel output.
        // Left: [1 1 0 0]
        chain(this._lowpass, this._delays.get("left"), this._gains.get("left"));
        connect(this._gains.get("left"), [0, 0, this._merger]);
        connect(this._gains.get("left"), [0, 1, this._merger]);

        // Right: [1 -1 0 0]
        chain(this._lowpass, this._delays.get("right"), this._gains.get("right"), this._inverters.get("right"));
        connect(this._gains.get("right"), [0, 0, this._merger]);
        connect(this._inverters.get("right"), [0, 1, this._merger]);

        // Up: [1 0 1 0]
        chain(this._lowpass, this._delays.get("up"), this._gains.get("up"));
        connect(this._gains.get("up"), [0, 0, this._merger]);
        connect(this._gains.get("up"), [0, 2, this._merger]);

        // Down: [1 0 -1 0]
        chain(this._lowpass, this._delays.get("down"), this._gains.get("down"), this._inverters.get("down"));
        connect(this._gains.get("down"), [0, 0, this._merger]);
        connect(this._inverters.get("down"), [0, 2, this._merger]);

        // Front: [1 0 0 1]
        chain(this._lowpass, this._delays.get("front"), this._gains.get("front"));
        connect(this._gains.get("front"), [0, 0, this._merger]);
        connect(this._gains.get("front"), [0, 3, this._merger]);

        // Back: [1 0 0 -1]
        chain(this._lowpass, this._delays.get("back"), this._gains.get("back"), this._inverters.get("back"));
        connect(this._gains.get("back"), [0, 0, this._merger]);
        connect(this._inverters.get("back"), [0, 3, this._merger]);

        connect(this._merger, this.output);

        // Initialize.
        this._listenerPosition = vec3.clone(options.listenerPosition);
        this.setRoomProperties(options.dimensions, options.coefficients);

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this.input);
            removeVertex(this.output);
            removeVertex(this._lowpass);
            removeVertex(this._merger);
            for (const delay of this._delays.values()) {
                removeVertex(delay);
            }
            for (const gain of this._gains.values()) {
                removeVertex(gain);
            }
            for (const inverter of this._inverters.values()) {
                removeVertex(inverter);
            }
        }
    }


    /**
     * Set the listener's position (in meters),
     * where [0,0,0] is the center of the room.
     */
    setListenerPosition(v: ReadonlyVec3) {
        // Assign listener position.
        vec3.copy(this._listenerPosition, v);
        this.update();
    }

    get speedOfSound() {
        return this._speedOfSound;
    }

    set speedOfSound(v) {
        if (v !== this.speedOfSound
            && isGoodNumber(v)) {
            this._speedOfSound = v;
            this.update();
        }
    }


    /**
     * Set the room's properties which determines the characteristics of
     * reflections.
     * @param dimensions Room dimensions (in meters). Defaults to {@linkcode Utils.DEFAULT_ROOM_DIMENSIONS DEFAULT_ROOM_DIMENSIONS}.
     * @param coefficients Frequency-independent reflection coeffs per wall. Defaults to {@linkcode Utils.DEFAULT_REFLECTION_COEFFICIENTS DEFAULT_REFLECTION_COEFFICIENTS}.
     */
    setRoomProperties(dimensions: Utils.RoomDimensions, coefficients: Utils.ReflectionCube) {
        // Sanitize dimensions and store half-dimensions.
        if (isNullOrUndefined(dimensions)) {
            dimensions = new Map();
        }
        for (const [key, value] of Utils.DEFAULT_ROOM_DIMENSIONS) {
            if (isBadNumber(dimensions.get(key))) {
                dimensions.set(key, value);
            }

            this.halfDimensions.set(key, 0.5 * dimensions.get(key));
        }

        if (isNullOrUndefined(coefficients)) {
            coefficients = new Map();
        }

        for (const [key, value] of Utils.DEFAULT_REFLECTION_COEFFICIENTS) {
            if (isBadNumber(coefficients.get(key))) {
                coefficients.set(key, value);
            }

            this._coefficients.set(key, coefficients.get(key));
        }

        // Update listener position with new room properties.
        this.update();
    }

    private readonly parts: ReadonlyMap<Utils.CubeSide, [Utils.RoomDimensionsAxis, number, number]> = new Map([
        ["left", ["width", 0, 1]],
        ["right", ["width", 0, -1]],
        ["down", ["height", 1, 1]],
        ["up", ["height", 1, -1]],
        ["front", ["depth", 2, 1]],
        ["back", ["depth", 2, -1]]
    ]);

    private update() {
        // Determine distances to each wall.
        for (const side of Utils.CUBE_SIDES) {
            const [dimAxis, vecAxis, negate] = this.parts.get(side);
            const k = this.halfDimensions.get(dimAxis) + negate * this._listenerPosition[vecAxis];
            const distance = Utils.DEFAULT_REFLECTION_MULTIPLIER
                * Math.max(0, k)
                + Utils.DEFAULT_REFLECTION_MIN_DISTANCE;
            this._distances.set(side, distance);
            this._delays.get(side).delayTime.value = this._distances.get(side) / this.speedOfSound;

            // Compute and assign gain, uses logarithmic rolloff: "g = R / (d + 1)"
            this._gains.get(side).gain.value = this._coefficients.get(side) / this._distances.get(side);
        }
    }
}
