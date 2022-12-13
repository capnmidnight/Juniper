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
 * @file Complete room model with early and late reflections.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { EPSILON_FLOAT, TWENTY_FOUR_LOG10 } from "@juniper-lib/tslib/math";
import { isDefined, isGoodNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyVec3 } from "gl-matrix";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperChannelMergerNode } from "../context/JuniperChannelMergerNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { EarlyReflections } from "./EarlyReflections";
import { LateReflections } from "./LateReflections";
import * as Utils from "./utils";

/**
 * Sanitize dimensions.
 */
function _sanitizeDimensions(dimensions: Utils.RoomDimensions): Utils.RoomDimensions {
    let output: Utils.RoomDimensions;
    if (isNullOrUndefined(dimensions)) {
        output = new Map(Utils.DEFAULT_ROOM_DIMENSIONS);
    }
    else {
        for (const axis of Utils.ROOM_DIMENSION_AXES) {
            output.set(axis, dimensions.has(axis)
                ? dimensions.get(axis)
                : Utils.DEFAULT_ROOM_DIMENSIONS.get(axis));
        }
    }
    return output;
}

function _sanitizeMaterials(materials: Utils.RoomAudioMaterialNames): Utils.RoomAudioMaterialNames {
    let output: Utils.RoomAudioMaterialNames;
    if (isNullOrUndefined(materials)) {
        output = new Map(Utils.DEFAULT_ROOM_MATERIALS);
    }
    else {
        for (const side of Utils.CUBE_SIDES) {
            output.set(side, materials.has(side)
                ? materials.get(side)
                : Utils.DEFAULT_ROOM_MATERIALS.get(side));
        }
    }
    return output;
}


/**
 * Generate absorption coefficients from material names.
 * @param {Object} materialsSanitized
 * @return {Object}
 */
function _getCoefficientsFromMaterials(materialsSanitized: Utils.RoomAudioMaterialNames): Utils.RoomAudioMaterialCoefficients {
    const output: Utils.RoomAudioMaterialCoefficients = new Map();
    for (const side of Utils.CUBE_SIDES) {
        // If element is not present, use default coefficients.
        const materialName = materialsSanitized.get(side);
        output.set(side, Utils.ROOM_MATERIAL_COEFFICIENTS.get(materialName));
    }
    return output;
}

/**
 * Compute frequency-dependent reverb durations.
 * @param {Utils~RoomDimensions} dimensions
 * @param {Object} coefficients
 * @param {Number} speedOfSound
 * @return {Array}
 */
function _getDurationsFromProperties(
    dimensions: Utils.RoomDimensions,
    coefficients: Utils.RoomAudioMaterialCoefficients,
    speedOfSound: number): Utils.AudioMaterialCoefficients {
    let durations: Utils.AudioMaterialCoefficients = [0, 0, 0, 0, 0, 0, 0, 0, 0];

    // Acoustic constant.
    let k = TWENTY_FOUR_LOG10 / speedOfSound;

    // Compute volume, skip if room is not present.
    const width = dimensions.get("width");
    const height = dimensions.get("height");
    const depth = dimensions.get("depth");

    const volume = width * height * depth;
    if (volume < Utils.ROOM_MIN_VOLUME) {
        return durations;
    }

    // Room surface area.
    const leftRightArea = width * height;
    const floorCeilingArea = width * depth;
    const frontBackArea = depth * height;
    const totalArea = 2 * (leftRightArea + floorCeilingArea + frontBackArea);

    const left = coefficients.get("left");
    const right = coefficients.get("right");
    const down = coefficients.get("down");
    const up = coefficients.get("up");
    const front = coefficients.get("front");
    const back = coefficients.get("back");

    for (let i = 0; i < Utils.NUMBER_REVERB_FREQUENCY_BANDS; i++) {
        // Effective absorptive area.
        const absorbtionArea =
            (left[i] + right[i]) * leftRightArea +
            (down[i] + up[i]) * floorCeilingArea +
            (front[i] + back[i]) * frontBackArea;
        const meanAbsorbtionArea = absorbtionArea / totalArea;

        // Compute reverberation using Eyring equation [1].
        // [1] Beranek, Leo L. "Analysis of Sabine and Eyring equations and their
        //     application to concert hall audience and chair absorption." The
        //     Journal of the Acoustical Society of America, Vol. 120, No. 3.
        //     (2006), pp. 1399-1399.
        durations[i] = Utils.ROOM_EYRING_CORRECTION_COEFFICIENT * k * volume /
            (-totalArea * Math.log(1 - meanAbsorbtionArea) + 4 *
                Utils.ROOM_AIR_ABSORPTION_COEFFICIENTS[i] * volume);
    }
    return durations;
}

/**
 * Compute reflection coefficients from absorption coefficients.
 * @param {Object} absorptionCoefficients
 * @return {Object}
 */
function _computeReflectionCoefficients(absorptionCoefficients: Utils.RoomAudioMaterialCoefficients): Utils.ReflectionCube {
    const reflectionCoefficients: Utils.ReflectionCube = new Map();
    for (const side of Utils.CUBE_SIDES) {
        // Compute average absorption coefficient (per wall).
        const coeffs = absorptionCoefficients.get(side);

        let value = 0;
        for (let j = 0; j < Utils.NUMBER_REFLECTION_AVERAGING_BANDS; j++) {
            const bandIndex = j + Utils.ROOM_STARTING_AVERAGING_BAND;
            value += coeffs[bandIndex];
        }

        value /= Utils.NUMBER_REFLECTION_AVERAGING_BANDS;

        // Convert absorption coefficient to reflection coefficient.
        value = Math.sqrt(1 - value);
        reflectionCoefficients.set(side, value)
    }
    return reflectionCoefficients as Utils.ReflectionCube;
}

export interface RoomOptions {
    listenerPosition: ReadonlyVec3;
    dimensions: Utils.RoomDimensions;
    materials: Utils.RoomAudioMaterialNames;
    speedOfSound: number;
}

/**
 * @class Room
 * @description Model that manages early and late reflections using acoustic
 * properties and listener position relative to a rectangular room.
 * @param {AudioContext} context
 * Associated {@link
https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
 * @param {Object} options
 * @param {Float32Array} options.listenerPosition
 * The listener's initial position (in meters), where origin is the center of
 * the room. Defaults to {@linkcode Utils.DEFAULT_POSITION DEFAULT_POSITION}.
 * @param {Utils~RoomDimensions} options.dimensions Room dimensions (in meters). Defaults to
 * {@linkcode Utils.DEFAULT_ROOM_DIMENSIONS DEFAULT_ROOM_DIMENSIONS}.
 * @param {Utils~RoomMaterials} options.materials Named acoustic materials per wall.
 * Defaults to {@linkcode Utils.DEFAULT_ROOM_MATERIALS DEFAULT_ROOM_MATERIALS}.
 * @param {Number} options.speedOfSound
 * (in meters/second). Defaults to
 * {@linkcode Utils.DEFAULT_SPEED_OF_SOUND DEFAULT_SPEED_OF_SOUND}.
 */
export class Room extends BaseNodeCluster {

    private _early: EarlyReflections;
    private _late: LateReflections;
    private merger: JuniperChannelMergerNode;

    readonly output: JuniperGainNode;
    get early() { return this._early; }
    get late() { return this._late; }

    constructor(context: JuniperAudioContext, options: Partial<RoomOptions>) {
        // Public variables.
        /**
         * EarlyReflections {@link EarlyReflections EarlyReflections} submodule.
         * @member {AudioNode} early
         * @memberof Room
         * @instance
         */
        /**
         * LateReflections {@link LateReflections LateReflections} submodule.
         * @member {AudioNode} late
         * @memberof Room
         * @instance
         */
        /**
         * Ambisonic (multichannel) output {@link
         * https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
         * @member {AudioNode} output
         * @memberof Room
         * @instance
         */

        // Use defaults for undefined arguments.
        if (options == undefined) {
            options = {};
        }
        if (isNullOrUndefined(options.listenerPosition)) {
            options.listenerPosition = Utils.DEFAULT_POSITION;
        }
        if (isNullOrUndefined(options.speedOfSound)) {
            options.speedOfSound = Utils.DEFAULT_SPEED_OF_SOUND;
        }

        const output = new JuniperGainNode(context);
        const merger = new JuniperChannelMergerNode(context);

        merger.connect(output);

        super("ort-room", context, [], [output], [merger])

        this.speedOfSound = options.speedOfSound;

        this.output = output;
        this.merger = merger;

        // Sanitize room-properties-related arguments.
        this._setProperties(options.dimensions, options.materials, options.listenerPosition);

        Object.seal(this);
    }

    /**
     * Set the room's dimensions and wall materials.
     * @param {Utils~RoomDimensions} dimensions Room dimensions (in meters). Defaults to
     * {@linkcode Utils.DEFAULT_ROOM_DIMENSIONS DEFAULT_ROOM_DIMENSIONS}.
     * @param {Utils~RoomMaterials} materials Named acoustic materials per wall. Defaults to
     * {@linkcode Utils.DEFAULT_ROOM_MATERIALS DEFAULT_ROOM_MATERIALS}.
     */
    setProperties(dimensions: Utils.RoomDimensions, materials: Utils.RoomAudioMaterialNames) {
        this._setProperties(dimensions, materials, null);
    }

    private _setProperties(dimensions: Utils.RoomDimensions, materials: Utils.RoomAudioMaterialNames, listenerPosition: ReadonlyVec3) {
        // Compute late response.
        dimensions = _sanitizeDimensions(dimensions);
        materials = _sanitizeMaterials(materials);
        const materialCoeffs = _getCoefficientsFromMaterials(materials);
        const durations = _getDurationsFromProperties(dimensions, materialCoeffs, this.speedOfSound);
        const coefficients = _computeReflectionCoefficients(materialCoeffs);

        if (isNullOrUndefined(this.early)) {
            this._early = new EarlyReflections(this.context, {
                dimensions,
                coefficients,
                speedOfSound: this.speedOfSound,
                listenerPosition,
            });
            this.add(this._early);
            this._early.connect(this.output);
        }
        else {
            this.early.setRoomProperties(dimensions, coefficients);
        }

        if (isNullOrUndefined(this.late)) {
            this._late = new LateReflections(this.context, {
                durations,
            });
            this.add(this._late);
            this._late.connect(this.merger, 0, 0);
        }
        else {
            this.late.setDurations(durations);
        }
    }

    private _speedOfSound: number;
    get speedOfSound() {
        return this._speedOfSound;
    }

    set speedOfSound(v) {
        if (v !== this.speedOfSound
            && isGoodNumber(v)) {
            this._speedOfSound = v;
            if (isDefined(this.early)) {
                this.early.speedOfSound = this.speedOfSound;
            }
        }
    }

    /**
     * Set the listener's position (in meters), where origin is the center of
     * the room.
     * @param {Number} x
     * @param {Number} y
     * @param {Number} z
     */
    setListenerPosition(v: ReadonlyVec3) {
        this.early.setListenerPosition(v);

        // Disable room effects if the listener is outside the room boundaries.
        let distance = this.getDistanceOutsideRoom(v);
        let gain = 1;
        if (distance > EPSILON_FLOAT) {
            gain = 1 - distance / Utils.LISTENER_MAX_OUTSIDE_ROOM_DISTANCE;

            // Clamp gain between 0 and 1.
            gain = Math.max(0, Math.min(1, gain));
        }
        this.output.gain.value = gain;
    }


    /**
     * Compute distance outside room of provided position (in meters).
     * @param {Number} x
     * @param {Number} y
     * @param {Number} z
     * @return {Number}
     * Distance outside room (in meters). Returns 0 if inside room.
     */
    getDistanceOutsideRoom(v: ReadonlyVec3) {
        const [x, y, z] = v;
        const width = this.early.halfDimensions.get("width");
        const height = this.early.halfDimensions.get("height");
        const depth = this.early.halfDimensions.get("depth");
        const dx = Math.max(0, -width - x, x - width);
        const dy = Math.max(0, -height - y, y - height);
        const dz = Math.max(0, -depth - z, z - depth);
        return Math.sqrt(dx * dx + dy * dy + dz * dz);
    }
}