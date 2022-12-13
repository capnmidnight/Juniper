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
 * @file Directivity/occlusion filter.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { EPSILON_FLOAT } from "@juniper-lib/tslib/math";
import { isBadNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ReadonlyVec3, vec3 } from "gl-matrix";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";
import * as Utils from "./utils";

export interface DirectivityOptions {
    /**
     * Determines directivity pattern (0 to 1). See
     * {@link Directivity#setPattern setPattern} for more details. Defaults to
     * {@linkcode Utils.DEFAULT_DIRECTIVITY_ALPHA DEFAULT_DIRECTIVITY_ALPHA}.
     */
    alpha: number;

    /**
     * Determines the sharpness of the directivity pattern (1 to Inf). See
     * {@link Directivity#setPattern setPattern} for more details. Defaults to
     * {@linkcode Utils.DEFAULT_DIRECTIVITY_SHARPNESS DEFAULT_DIRECTIVITY_SHARPNESS}.
     */
    sharpness: number;
}

/**
 * Directivity/occlusion filter.
 */
export class Directivity extends JuniperBiquadFilterNode {
    private _alpha: number;
    private _sharpness: number;
    private _cosTheta: number;

    /**
     * Directivity/occlusion filter.
     * @param context Associated {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param options
     */
    constructor(context: JuniperAudioContext, options?: Partial<DirectivityOptions>) {
        // Use defaults for undefined arguments.
        if (isNullOrUndefined(options)) {
            options = {};
        }
        if (isBadNumber(options.alpha)) {
            options.alpha = Utils.DEFAULT_DIRECTIVITY_ALPHA;
        }
        if (isNullOrUndefined(options.sharpness) || Number.isNaN(options.sharpness)) {
            options.sharpness = Utils.DEFAULT_DIRECTIVITY_SHARPNESS;
        }
        super(context, {
            type: "lowpass",
            Q: 0,
            frequency: 0.5 * context.sampleRate
        });

        this._cosTheta = 0;
        this.setPattern(options.alpha, options.sharpness);

        Object.seal(this);
    }

    private _forwardNorm = vec3.create();
    private _directionNorm = vec3.create();

    /**
     * Compute the filter using the source's forward orientation and the listener's
     * position.
     * @param {Float32Array} forward The source's forward vector.
     * @param {Float32Array} direction The direction from the source to the
     * listener.
     */
    computeAngle(forward: ReadonlyVec3, direction: ReadonlyVec3) {
        vec3.normalize(this._forwardNorm, forward);
        vec3.normalize(this._directionNorm, direction);
        let coeff = 1;
        if (this._alpha > EPSILON_FLOAT) {
            this._cosTheta = vec3.dot(this._forwardNorm, this._directionNorm);
            coeff = (1 - this._alpha) + this._alpha * this._cosTheta;
            coeff = Math.pow(Math.abs(coeff), this._sharpness);
        }
        this.frequency.value = this.context.sampleRate * 0.5 * coeff;
    }


    /**
     * Set source's directivity pattern (defined by alpha), where 0 is an
     * omnidirectional pattern, 1 is a bidirectional pattern, 0.5 is a cardiod
     * pattern. The sharpness of the pattern is increased exponenentially.
     * @param {Number} alpha
     * Determines directivity pattern (0 to 1).
     * @param {Number} sharpness
     * Determines the sharpness of the directivity pattern (1 to Inf).
     * DEFAULT_DIRECTIVITY_SHARPNESS}.
     */
    setPattern(alpha: number, sharpness: number) {
        // Clamp and set values.
        this._alpha = Math.min(1, Math.max(0, alpha));
        this._sharpness = Math.max(1, sharpness);

        // Update angle calculation using new values.
        this.computeAngle([this._cosTheta * this._cosTheta, 0, 0], [1, 0, 0]);
    }
}