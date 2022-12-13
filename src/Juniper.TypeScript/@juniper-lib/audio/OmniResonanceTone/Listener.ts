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
 * @file Listener model to spatialize sources in an environment.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { isBadNumber, isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { mat3, ReadonlyMat4, ReadonlyVec3, vec3 } from "gl-matrix";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { Encoder } from "./Encoder";
import { FOARenderer } from "./FOARenderer";
import { HOARenderer } from "./HOARenderer";
import { IRenderer } from "./IRenderer";
import * as Utils from "./utils";

export interface ListenerOptions {
    ambisonicOrder?: number;
    position: ReadonlyVec3;
    forward: ReadonlyVec3;
    up: ReadonlyVec3;
}


export class Listener extends BaseNodeCluster {

    private readonly input: JuniperGainNode;
    readonly output: JuniperGainNode;
    readonly ambisonicOutput: JuniperGainNode;
    private renderer: IRenderer;

    /**
     * Position (in meters).
     */
    readonly position = vec3.create();

    readonly _ambisonicOrder: number;

    /**
     * @class Listener
     * @description Listener model to spatialize sources in an environment.
     * @param {AudioContext} context
     * Associated {@link
    https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param {Object} options
     * @param {Number} options.ambisonicOrder
     * Desired ambisonic order. Defaults to
     * {@linkcode Utils.DEFAULT_AMBISONIC_ORDER DEFAULT_AMBISONIC_ORDER}.
     * @param {Float32Array} options.position
     * Initial position (in meters), where origin is the center of
     * the room. Defaults to
     * {@linkcode Utils.DEFAULT_POSITION DEFAULT_POSITION}.
     * @param {Float32Array} options.forward
     * The listener's initial forward vector. Defaults to
     * {@linkcode Utils.DEFAULT_FORWARD DEFAULT_FORWARD}.
     * @param {Float32Array} options.up
     * The listener's initial up vector. Defaults to
     * {@linkcode Utils.DEFAULT_UP DEFAULT_UP}.
     */
    constructor(context: JuniperAudioContext, options: Partial<ListenerOptions>) {
        // Public variables.
        // Use defaults for undefined arguments.
        if (isNullOrUndefined(options)) {
            options = {};
        }
        if (isBadNumber(options.ambisonicOrder)) {
            options.ambisonicOrder = Utils.DEFAULT_AMBISONIC_ORDER;
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


        // These nodes are created in order to safely asynchronously load Omnitone
        // while the rest of the scene is being created.
        const input = new JuniperGainNode(context);
        const output = new JuniperGainNode(context);
        const ambisonicOutput = new JuniperGainNode(context);
        super("ort-listener", context, [input], [output, ambisonicOutput]);

        this.input = input;
        this.output = output;
        this.ambisonicOutput = ambisonicOutput;

        // Select the appropriate HRIR filters using 2-channel chunks since
        // multichannel audio is not yet supported by a majority of browsers.
        this.ambisonicOrder = Encoder.validateAmbisonicOrder(options.ambisonicOrder);

        // Set orientation and update rotation matrix accordingly.
        this.setOrientation(options.forward, options.up);

        Object.seal(this);
    }

    get ambisonicOrder() {
        return this._ambisonicOrder;
    }

    set ambisonicOrder(v) {
        v = Encoder.validateAmbisonicOrder(v);
        if (v !== this._ambisonicOrder) {
            // Create audio nodes.
            if (isDefined(this.renderer)) {
                this.input.disconnect();
                this.renderer.rotator.disconnect();
                this.renderer.disconnect();
                this.remove(this.renderer);
                this.renderer.dispose();
                this.renderer = null;
            }

            if (this.ambisonicOrder === 1) {
                this.renderer = new FOARenderer(this.context);
            } else if (this.ambisonicOrder > 1) {
                this.renderer = new HOARenderer(this.context, {
                    ambisonicOrder: this.ambisonicOrder,
                });
            }

            this.add(this.renderer);

            // Initialize Omnitone (async) and connect to audio graph when complete.
            this.renderer.initialize().then(async () => {
                // Connect pre-rotated soundfield to renderer.
                this.input
                    .connect(this.renderer)
                // Connect binaurally-rendered soundfield to binaural output.
                    .connect(this.output);

                // Connect rotated soundfield to ambisonic output.
                this.renderer.rotator
                    .connect(this.ambisonicOutput);
            });
        }
    }

    private readonly _right = vec3.create();
    private readonly _tempMatrix3 = mat3.create();
    /**
     * Set the source's orientation using forward and up vectors.
     */
    setOrientation(fwd: ReadonlyVec3, up: ReadonlyVec3) {
        vec3.cross(this._right, fwd, up);
        mat3.set(this._tempMatrix3,
            this._right[0], this._right[1], this._right[2],
            up[0], up[1], up[2],
            fwd[0], fwd[1], fwd[2]);
        this.renderer.setRotationMatrix3(this._tempMatrix3);
    }

    setPosition(v: ReadonlyVec3) {
        vec3.copy(this.position, v);
    }


    /**
     * Set the listener's position and orientation using a Three.js Matrix4 object.
     * @param {Object} matrix4
     * The Three.js Matrix4 object representing the listener's world transform.
     */
    setFromMatrix(mat: ReadonlyMat4) {
        // Update ambisonic rotation matrix internally.
        this.renderer.setRotationMatrix4(mat);

        // Extract position from matrix.
        this.position[0] = mat[12];
        this.position[1] = mat[13];
        this.position[2] = mat[14];
    }
}