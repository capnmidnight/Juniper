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
 * @file Distance-based attenuation filter.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { EPSILON_FLOAT } from "@juniper-lib/tslib/math";
import { isBadNumber, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import * as Utils from "./utils";

export interface AttenuationOptions {
    /**
     * Min. distance (in meters). Defaults to {@linkcode Utils.DEFAULT_MIN_DISTANCE DEFAULT_MIN_DISTANCE}.
     */
    minDistance: number;

    /**
     * Max. distance (in meters). Defaults to {@linkcode Utils.DEFAULT_MAX_DISTANCE DEFAULT_MAX_DISTANCE}.
     */
    maxDistance: number;

    /**
     * Rolloff model to use, chosen from options in {@linkcode Utils.ATTENUATION_ROLLOFFS ATTENUATION_ROLLOFFS}. 
     * Defaults to {@linkcode Utils.DEFAULT_ATTENUATION_ROLLOFF DEFAULT_ATTENUATION_ROLLOFF}
     */
    rolloff: Utils.AttenuationRolloff;
}

export class Attenuation extends JuniperGainNode {

    /**
     * Min. distance (in meters).
     */
    minDistance: number;

    /**
     * Max. distance (in meters).
     */
    maxDistance: number;

    private _rolloff: Utils.AttenuationRolloff;

    /**
     * Distance-based attenuation filter.
     * @param {AudioContext} context Associated {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param options
     */
    constructor(context: JuniperAudioContext, options?: Partial<AttenuationOptions>) {
        // Use defaults for undefined arguments.
        if (isNullOrUndefined(options)) {
            options = {};
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

        super(context);
        this.name = "attenuation";

        // Assign values.
        this.minDistance = options.minDistance;
        this.maxDistance = options.maxDistance;
        this.setRolloff(options.rolloff);

        // Initialize distance to max distance.
        this.setDistance(options.maxDistance);

        Object.seal(this);
    }


    /**
     * Set distance from the listener.
     * @param {Number} distance Distance (in meters).
     */
    setDistance(distance: number) {
        let gain = 1;
        if (this._rolloff == "logarithmic") {
            if (distance > this.maxDistance) {
                gain = 0;
            } else if (distance > this.minDistance) {
                const range = this.maxDistance - this.minDistance;
                if (range > EPSILON_FLOAT) {
                    // Compute the distance attenuation value by the logarithmic curve
                    // "1 / (d + 1)" with an offset of |minDistance|.
                    const relativeDistance = distance - this.minDistance;
                    const attenuation = 1 / (relativeDistance + 1);
                    const attenuationMax = 1 / (range + 1);
                    gain = (attenuation - attenuationMax) / (1 - attenuationMax);
                }
            }
        } else if (this._rolloff == "linear") {
            if (distance > this.maxDistance) {
                gain = 0;
            } else if (distance > this.minDistance) {
                const range = this.maxDistance - this.minDistance;
                if (range > EPSILON_FLOAT) {
                    gain = (this.maxDistance - distance) / range;
                }
            }
        }
        this.gain.value = gain;
    }


    /**
     * Set rolloff.
     * @param rolloff Rolloff model to use, chosen from options in {@linkcode Utils.ATTENUATION_ROLLOFFS ATTENUATION_ROLLOFFS}.
     */
    setRolloff(rolloff: Utils.AttenuationRolloff) {
        const isValidModel = Utils.ATTENUATION_ROLLOFFS.indexOf(rolloff) >= 0;
        if (rolloff == undefined || !isValidModel) {
            if (!isValidModel) {
                Utils.log("Invalid rolloff model (\"" + rolloff +
                    "\"). Using default: \"" + Utils.DEFAULT_ATTENUATION_ROLLOFF + "\".");
            }
            rolloff = Utils.DEFAULT_ATTENUATION_ROLLOFF;
        } else {
            rolloff = rolloff.toString().toLowerCase() as Utils.AttenuationRolloff;
        }
        this._rolloff = rolloff;
    }
}