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
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";
import { JuniperChannelMergerNode } from "../context/JuniperChannelMergerNode";
import { JuniperDelayNode } from "../context/JuniperDelayNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
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
export class EarlyReflections extends BaseNodeCluster {

    /**
     * The room's speed of sound (in meters/second).
     */
    private _speedOfSound: number;

    readonly halfDimensions: Utils.RoomDimensions = new Map();

    private readonly _coefficients: Utils.ReflectionCube = new Map();
    private readonly _delays: Map<Utils.CubeSide, JuniperDelayNode>;
    private readonly _gains: Map<Utils.CubeSide, JuniperGainNode>;
    private readonly _listenerPosition: vec3;
    private readonly _distances: Utils.ReflectionCube = new Map();

    /**
     * Ray-tracing-based early reflections model.
     * @param context Associated {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param options
     */
    constructor(context: JuniperAudioContext, options: Partial<EarlyReflectionsOptions>) {
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


        // Create nodes.
        const lowpass = new JuniperBiquadFilterNode(context, {
            type: "lowpass",
            Q: 0,
            frequency: Utils.DEFAULT_REFLECTION_CUTOFF_FREQUENCY
        });
        const delays = new Map<Utils.CubeSide, JuniperDelayNode>([
            ["left", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["right", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["front", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["back", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["up", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })],
            ["down", new JuniperDelayNode(context, { delayTime: 0, maxDelayTime: Utils.DEFAULT_REFLECTION_MAX_DURATION })]
        ]);

        // gainPerWall = (ReflectionCoeff / Attenuation)
        const gains = new Map<Utils.CubeSide, JuniperGainNode>([
            ["left", new JuniperGainNode(context, { gain: 0 })],
            ["right", new JuniperGainNode(context, { gain: 0 })],
            ["front", new JuniperGainNode(context, { gain: 0 })],
            ["back", new JuniperGainNode(context, { gain: 0 })],
            ["up", new JuniperGainNode(context, { gain: 0 })],
            ["down", new JuniperGainNode(context, { gain: 0 })]
        ]);


        // 3 of these are needed for right/back/down walls.
        const inverters = new Map<Utils.InverseCubeSide, JuniperGainNode>([
            ["right", new JuniperGainNode(context, { gain: -1 })],
            ["back", new JuniperGainNode(context, { gain: -1 })],
            ["down", new JuniperGainNode(context, { gain: -1 })]
        ]);

        // First-order encoding only.
        const merger = new JuniperChannelMergerNode(context, {
            channelCount: 4
        });

        // Connect nodes.

        // Connect gains to ambisonic channel output.
        // Left: [1 1 0 0]
        lowpass
            .connect(delays.get("left"))
            .connect(gains.get("left"))
            .connect(merger, 0, 0);

        gains.get("left").connect(merger, 0, 1);

        // Right: [1 -1 0 0]
        lowpass
            .connect(delays.get("right"))
            .connect(gains.get("right"))
            .connect(merger, 0, 0);

        gains.get("right")
            .connect(inverters.get("right"))
            .connect(merger, 0, 1);

        // Up: [1 0 1 0]
        lowpass
            .connect(delays.get("up"))
            .connect(gains.get("up"))
            .connect(merger, 0, 0);

        gains.get("up")
            .connect(merger, 0, 2);

        // Down: [1 0 -1 0]
        lowpass
            .connect(delays.get("down"))
            .connect(gains.get("down"))
            .connect(merger, 0, 0);

        gains.get("down")
            .connect(inverters.get("down"))
            .connect(merger, 0, 2);

        // Front: [1 0 0 1]
        lowpass
            .connect(delays.get("front"))
            .connect(gains.get("front"))
            .connect(merger, 0, 0);

        gains.get("front")
            .connect(merger, 0, 3);

        // Back: [1 0 0 -1]
        lowpass
            .connect(delays.get("back"))
            .connect(gains.get("back"))
            .connect(merger, 0, 0);
        gains.get("back")
            .connect(inverters.get("back"))
            .connect(merger, 0, 3);

        super("early-reflection", context, [lowpass], [merger], [
            ...Array.from(delays.values()),
            ...Array.from(gains.values()),
            ...Array.from(inverters.values())
        ]);

        // Assign room's speed of sound.
        this.speedOfSound = options.speedOfSound;
        this._delays = delays;
        this._gains = gains;

        // Initialize.
        this._listenerPosition = vec3.clone(options.listenerPosition);
        this.setRoomProperties(options.dimensions, options.coefficients);

        Object.seal(this);
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
