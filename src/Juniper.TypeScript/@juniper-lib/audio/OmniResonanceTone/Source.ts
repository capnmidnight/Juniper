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
 * @file Source model to spatialize an audio buffer.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { EPSILON_FLOAT, rad2deg } from "@juniper-lib/tslib/math";
import { isBadNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { mat4, ReadonlyVec3, vec3 } from "gl-matrix";
import { Gain } from "../nodes";
import { chain, removeVertex } from "../util";
import { Attenuation } from "./Attenuation";
import { Directivity } from "./Directivity";
import { Encoder } from "./Encoder";
import type { Scene } from "./Scene";
import * as Utils from "./utils";



/**
 * Determine the distance a source is outside of a room. Attenuate gain going
 * to the reflections and reverb when the source is outside of the room.
 * @param {Number} distance Distance in meters.
 * @return {Number} Gain (linear) of source.
 * @private
 */
function _computeDistanceOutsideRoom(distance: number) {
    // We apply a linear ramp from 1 to 0 as the source is up to 1m outside.
    let gain = 1;
    if (distance > EPSILON_FLOAT) {
        gain = 1 - distance / Utils.SOURCE_MAX_OUTSIDE_ROOM_DISTANCE;

        // Clamp gain between 0 and 1.
        gain = Math.max(0, Math.min(1, gain));
    }
    return gain;
}

/**
 * Options for constructing a new Source.
 * @typedef {Object} Source~SourceOptions
 * @property {Float32Array} position
 * The source's initial position (in meters), where origin is the center of
 * the room. Defaults to {@linkcode Utils.DEFAULT_POSITION DEFAULT_POSITION}.
 * @property {Float32Array} forward
 * The source's initial forward vector. Defaults to
 * {@linkcode Utils.DEFAULT_FORWARD DEFAULT_FORWARD}.
 * @property {Float32Array} up
 * The source's initial up vector. Defaults to
 * {@linkcode Utils.DEFAULT_UP DEFAULT_UP}.
 * @property {Number} minDistance
 * Min. distance (in meters). Defaults to
 * {@linkcode Utils.DEFAULT_MIN_DISTANCE DEFAULT_MIN_DISTANCE}.
 * @property {Number} maxDistance
 * Max. distance (in meters). Defaults to
 * {@linkcode Utils.DEFAULT_MAX_DISTANCE DEFAULT_MAX_DISTANCE}.
 * @property {string} rolloff
 * Rolloff model to use, chosen from options in
 * {@linkcode Utils.ATTENUATION_ROLLOFFS ATTENUATION_ROLLOFFS}. Defaults to
 * {@linkcode Utils.DEFAULT_ATTENUATION_ROLLOFF DEFAULT_ATTENUATION_ROLLOFF}.
 * @property {Number} gain Input gain (linear). Defaults to
 * {@linkcode Utils.DEFAULT_SOURCE_GAIN DEFAULT_SOURCE_GAIN}.
 * @property {Number} alpha Directivity alpha. Defaults to
 * {@linkcode Utils.DEFAULT_DIRECTIVITY_ALPHA DEFAULT_DIRECTIVITY_ALPHA}.
 * @property {Number} sharpness Directivity sharpness. Defaults to
 * {@linkcode Utils.DEFAULT_DIRECTIVITY_SHARPNESS
 * DEFAULT_DIRECTIVITY_SHARPNESS}.
 * @property {Number} sourceWidth
 * Source width (in degrees). Where 0 degrees is a point source and 360 degrees
 * is an omnidirectional source. Defaults to
 * {@linkcode Utils.DEFAULT_SOURCE_WIDTH DEFAULT_SOURCE_WIDTH}.
 */

export interface SourceOptions {
    position: ReadonlyVec3;
    forward: ReadonlyVec3;
    up: ReadonlyVec3;
    minDistance: number;
    maxDistance: number;
    rolloff: Utils.AttenuationRolloff;
    gain: number;
    alpha: number;
    sharpness: number;
    sourceWidth: number;
}

export class Source implements IDisposable {
    private readonly _scene: Scene;
    private readonly _position: vec3;
    private readonly _forward: vec3;
    private readonly _up: vec3;
    private readonly _dx: vec3;
    private readonly _right: vec3;
    private readonly _encoder: Encoder
    private readonly _directivity: Directivity;
    private readonly _toEarly: GainNode;
    private readonly _toLate: GainNode;
    private readonly _attenuation: Attenuation;

    /**
     * Mono (1-channel) input {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
     */
    readonly output: GainNode;

    /**
     * @class Source
     * @description Source model to spatialize an audio buffer.
     * @param {Scene} scene Associated {@link Scene
     * ResonanceAudio} instance.
     * @param {Source~SourceOptions} options
     * Options for constructing a new Source.
     */
    constructor(name: string, private readonly scene: Scene, context: BaseAudioContext, options: Partial<SourceOptions>) {
        // Public variables.
        /**
         *
         */

        // Use defaults for undefined arguments.
        if (options == undefined) {
            options = {};
        }
        if (isNullOrUndefined(options.position)) {
            options.position = Utils.DEFAULT_POSITION;
        }
        if (isNullOrUndefined(options.forward)) {
            options.forward = Utils.DEFAULT_FORWARD;
        }
        if (isNullOrUndefined(options.up)) {
            options.up = Utils.DEFAULT_UP;
        }
        if (isBadNumber(options.minDistance)) {
            options.minDistance = Utils.DEFAULT_MIN_DISTANCE;
        }
        if (isBadNumber(options.maxDistance)) {
            options.maxDistance = Utils.DEFAULT_MAX_DISTANCE;
        }
        if (isNullOrUndefined(options.rolloff)) {
            options.rolloff = Utils.DEFAULT_ATTENUATION_ROLLOFF;
        }
        if (isBadNumber(options.gain)) {
            options.gain = Utils.DEFAULT_SOURCE_GAIN;
        }
        if (isBadNumber(options.alpha)) {
            options.alpha = Utils.DEFAULT_DIRECTIVITY_ALPHA;
        }
        if (isNullOrUndefined(options.sharpness) || Number.isNaN(options.sharpness)) {
            options.sharpness = Utils.DEFAULT_DIRECTIVITY_SHARPNESS;
        }
        if (isBadNumber(options.sourceWidth)) {
            options.sourceWidth = Utils.DEFAULT_SOURCE_WIDTH;
        }

        // Member variables.
        this._scene = scene;
        this._position = vec3.clone(options.position);
        this._forward = vec3.clone(options.forward);
        this._up = vec3.clone(options.up);
        this._dx = vec3.create();
        this._right = vec3.cross(vec3.create(), this._forward, this._up);

        // Create audio nodes.
        this.output = Gain(`${name}-res-source-input`, context, { gain: options.gain });
        this._directivity = new Directivity(`${name}-res-source`, context, {
            alpha: options.alpha,
            sharpness: options.sharpness,
        });
        this._toEarly = Gain(`${name}-res-source-to-early`, context);
        this._toLate = Gain(`${name}-res-source-to-late`, context);
        this._attenuation = new Attenuation(`${name}-res-source`, context, {
            minDistance: options.minDistance,
            maxDistance: options.maxDistance,
            rolloff: options.rolloff,
        });
        this._encoder = new Encoder(`${name}-res-source`, context, {
            ambisonicOrder: scene.listener.ambisonicOrder,
            sourceWidth: options.sourceWidth,
        });

        // Assign initial conditions.
        this.setPosition(options.position);

        // Connect nodes.
        chain(this.output, this._attenuation, this._toEarly, scene.room.early);
        chain(this.output, this._toLate, scene.room.late);
        chain(this._attenuation, this._directivity, this._encoder, scene.listener);

        Object.seal(this);
    }

    get ambisonicOrder() {
        return this._encoder.ambisonicOrder;
    }

    set ambisonicOrder(v) {
        this._encoder.ambisonicOrder = v;
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this.output);
            this._directivity.dispose();
            removeVertex(this._toEarly);
            removeVertex(this._toLate);
            this._attenuation.dispose();
            this._encoder.dispose();
            this.scene.removeSource(this);
        }
    }


    /**
     * Set source's rolloff.
     * @param {string} rolloff
     * Rolloff model to use, chosen from options in
     * {@linkcode Utils.ATTENUATION_ROLLOFFS ATTENUATION_ROLLOFFS}.
     */
    setRolloff(rolloff: Utils.AttenuationRolloff) {
        this._attenuation.setRolloff(rolloff);
    }


    /**
     * Set source's minimum distance (in meters).
     * @param {Number} minDistance
     */
    setMinDistance(minDistance: number) {
        this._attenuation.minDistance = minDistance;
    }


    /**
     * Set source's maximum distance (in meters).
     * @param {Number} maxDistance
     */
    setMaxDistance(maxDistance: number) {
        this._attenuation.maxDistance = maxDistance;
    }


    /**
     * Set source's gain (linear).
     * @param {Number} gain
     */
    setGain(gain: number) {
        this.output.gain.value = gain;
    }


    // TODO(bitllama): Make sure this works with Three.js as intended.
    /**
     * Set source's position and orientation using a
     * Three.js modelViewMatrix object.
     * @param {Float32Array} mat
     * The Matrix4 representing the object position and rotation in world space.
     */
    setFromMatrix(mat: mat4) {
        this._right[0] = mat[0];
        this._right[1] = mat[1];
        this._right[2] = mat[2];
        this._up[0] = mat[4];
        this._up[1] = mat[5];
        this._up[2] = mat[6];
        this._forward[0] = mat[8];
        this._forward[1] = mat[9];
        this._forward[2] = mat[10];
        this._position[0] = mat[12];
        this._position[1] = mat[13];
        this._position[2] = mat[14];

        // Normalize to remove scaling.
        vec3.normalize(this._right, this._right);
        vec3.normalize(this._up, this._up);
        vec3.normalize(this._forward, this._forward);

        this.update();
    }

    /**
     * Set source's position (in meters), where origin is the center of
     * the room.
     * @param {Number} x
     * @param {Number} y
     * @param {Number} z
     */
    setPosition(v: ReadonlyVec3) {
        // Assign new position.
        vec3.copy(this._position, v);
        this.update();
    }


    /**
     * Set the source's orientation using forward and up vectors.
     * @param {Number} forwardX
     * @param {Number} forwardY
     * @param {Number} forwardZ
     * @param {Number} upX
     * @param {Number} upY
     * @param {Number} upZ
     */
    setOrientation(fwd: ReadonlyVec3, up: ReadonlyVec3) {
        vec3.copy(this._forward, fwd);
        vec3.copy(this._up, up);
        vec3.cross(this._right, this._forward, this._up);
        this.update();
    }


    /**
     * Set the source width (in degrees). Where 0 degrees is a point source and 360
     * degrees is an omnidirectional source.
     * @param {Number} sourceWidth (in degrees).
     */
    setSourceWidth(sourceWidth: number) {
        this._encoder.setSourceWidth(sourceWidth);
        this.update();
    }


    /**
     * Set source's directivity pattern (defined by alpha), where 0 is an
     * omnidirectional pattern, 1 is a bidirectional pattern, 0.5 is a cardiod
     * pattern. The sharpness of the pattern is increased exponentially.
     * @param {Number} alpha
     * Determines directivity pattern (0 to 1).
     * @param {Number} sharpness
     * Determines the sharpness of the directivity pattern (1 to Inf).
     */
    setDirectivityPattern(alpha: number, sharpness: number) {
        this._directivity.setPattern(alpha, sharpness);
        this.update();
    }

    update() {
        // Handle far-field effect.
        let distance = this._scene.room.getDistanceOutsideRoom(this._position);
        let gain = _computeDistanceOutsideRoom(distance);
        this._toLate.gain.value = gain;
        this._toEarly.gain.value = gain;

        vec3.sub(this._dx, this._position, this._scene.listener.position);
        distance = vec3.len(this._dx);
        if (distance > EPSILON_FLOAT) {
            // Normalize direction vector.
            vec3.scale(this._dx, this._dx, 1 / distance);
        }

        // Compuete angle of direction vector.
        let azimuth = rad2deg(Math.atan2(-this._dx[0], this._dx[2]));
        let elevation = rad2deg(Math.atan2(
            this._dx[1],
            Math.sqrt(this._dx[0] * this._dx[0] +
                this._dx[2] * this._dx[2])));

        // Set distance/directivity/direction values.
        this._attenuation.setDistance(distance);
        this._directivity.computeAngle(this._forward, this._dx);
        this._encoder.setDirection(azimuth, elevation);
    }
}