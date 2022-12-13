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
 * @file Late reverberation filter for Ambisonic content.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

// Internal dependencies.
import { LOG1000, LOG2_DIV2, Tau } from "@juniper-lib/tslib/math";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperConvolverNode } from "../context/JuniperConvolverNode";
import { JuniperDelayNode } from "../context/JuniperDelayNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
import * as Utils from "./utils";

export interface LateReflectionsOptions {
    /**
     * Multiband RT60 durations(in seconds) for each frequency band, listed as
     * {@linkcode Utils.DEFAULT_REVERB_FREQUENCY_BANDS DEFAULT_REVERB_FREQUENCY_BANDS}. Defaults to
     * {@linkcode Utils.DEFAULT_REVERB_DURATIONS DEFAULT_REVERB_DURATIONS}.
     */
    durations: Utils.AudioMaterialCoefficients;

    /**
     * Pre-delay (in milliseconds). Defaults to {@linkcode Utils.DEFAULT_REVERB_PREDELAY DEFAULT_REVERB_PREDELAY}.
     */
    predelay: number;

    /**
     * Output gain(linear).Defaults to {@linkcode Utils.DEFAULT_REVERB_GAIN DEFAULT_REVERB_GAIN}.
     */
    gain: number;

    /**
     * Bandwidth(in octaves) for each frequency
     * band.Defaults to {@linkcode Utils.DEFAULT_REVERB_BANDWIDTH DEFAULT_REVERB_BANDWIDTH}.
     */
    bandwidth: number;
    /**
     * @param { Number } options.tailonset Length(in milliseconds) of impulse
    * response to apply a half - Hann window.Defaults to
        * {@linkcode Utils.DEFAULT_REVERB_TAIL_ONSET DEFAULT_REVERB_TAIL_ONSET}.
       */
    tailonset: number;
}

export class LateReflections extends BaseNodeCluster {
    private readonly _convolver: JuniperConvolverNode;
    private readonly _bandwidthCoeff: number;
    private readonly _tailonsetSamples: number;

    /**
     * @class LateReflections
     * @description Late-reflections reverberation filter for Ambisonic content.
     * @param {AudioContext} context
     * Associated {@link
    https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param {Object} options
     */
    constructor(context: JuniperAudioContext, options: Partial<LateReflectionsOptions>) {
        // Use defaults for undefined arguments.
        if (options == undefined) {
            options = {};
        }
        if (options.durations == undefined) {
            options.durations = Utils.DEFAULT_REVERB_DURATIONS;
        }
        if (options.predelay == undefined) {
            options.predelay = Utils.DEFAULT_REVERB_PREDELAY;
        }
        if (options.gain == undefined) {
            options.gain = Utils.DEFAULT_REVERB_GAIN;
        }
        if (options.bandwidth == undefined) {
            options.bandwidth = Utils.DEFAULT_REVERB_BANDWIDTH;
        }
        if (options.tailonset == undefined) {
            options.tailonset = Utils.DEFAULT_REVERB_TAIL_ONSET;
        }

        // Assign pre-computed variables.
        const delayTime = options.predelay / 1000;
        const bandwidthCoeff = options.bandwidth * LOG2_DIV2;
        const tailonsetSamples = options.tailonset / 1000;

        // Create nodes.
        const predelay = new JuniperDelayNode(context, {
            delayTime
        });
        const convolver = new JuniperConvolverNode(context, {
            disableNormalization: true
        });

        const output = new JuniperGainNode(context, {
            // Set reverb attenuation.
            gain: options.gain
        });

        predelay
            .connect(convolver)
            .connect(output);

        super("late-reflection", context, [predelay], [output], [convolver]);

        this._bandwidthCoeff = bandwidthCoeff;
        this._tailonsetSamples = tailonsetSamples;
        this._convolver = convolver;

        // Compute IR using RT60 values.
        this.setDurations(options.durations);

        Object.seal(this);
    }

    /**
     * Re-compute a new impulse response by providing Multiband RT60 durations.
     * @param {Array} durations
     * Multiband RT60 durations (in seconds) for each frequency band, listed as
     * {@linkcode Utils.DEFAULT_REVERB_FREQUENCY_BANDS
     * DEFAULT_REVERB_FREQUENCY_BANDS}.
     */
    setDurations(durations: Utils.AudioMaterialCoefficients) {
        if (durations.length !== Utils.NUMBER_REVERB_FREQUENCY_BANDS) {
            Utils.log('Warning: invalid number of RT60 values provided to reverb.');
            return;
        }

        // Compute impulse response.
        let durationsSamples =
            new Float32Array(Utils.NUMBER_REVERB_FREQUENCY_BANDS);
        let sampleRate = this.context.sampleRate;

        for (let i = 0; i < durations.length; i++) {
            // Clamp within suitable range.
            durations[i] =
                Math.max(0, Math.min(Utils.DEFAULT_REVERB_MAX_DURATION, durations[i]));

            // Convert seconds to samples.
            durationsSamples[i] = Math.round(durations[i] * sampleRate *
                Utils.DEFAULT_REVERB_DURATION_MULTIPLIER);
        };

        // Determine max RT60 length in samples.
        let durationsSamplesMax = 0;
        for (let i = 0; i < durationsSamples.length; i++) {
            if (durationsSamples[i] > durationsSamplesMax) {
                durationsSamplesMax = durationsSamples[i];
            }
        }

        // Skip this step if there is no reverberation to compute.
        if (durationsSamplesMax < 1) {
            durationsSamplesMax = 1;
        }

        // Create impulse response buffer.
        let buffer = this.context.createBuffer(1, durationsSamplesMax, sampleRate);
        let bufferData = buffer.getChannelData(0);

        // Create noise signal (computed once, referenced in each band's routine).
        let noiseSignal = new Float32Array(durationsSamplesMax);
        for (let i = 0; i < durationsSamplesMax; i++) {
            noiseSignal[i] = Math.random() * 2 - 1;
        }

        // Compute the decay rate per-band and filter the decaying noise signal.
        for (let i = 0; i < Utils.NUMBER_REVERB_FREQUENCY_BANDS; i++) {
            // Compute decay rate.
            let decayRate = -LOG1000 / durationsSamples[i];

            // Construct a standard one-zero, two-pole bandpass filter:
            // H(z) = (b0 * z^0 + b1 * z^-1 + b2 * z^-2) / (1 + a1 * z^-1 + a2 * z^-2)
            let omega = Tau * Utils.DEFAULT_REVERB_FREQUENCY_BANDS[i] / sampleRate;
            let sinOmega = Math.sin(omega);
            let alpha = sinOmega * Math.sinh(this._bandwidthCoeff * omega / sinOmega);
            let a0CoeffReciprocal = 1 / (1 + alpha);
            let b0Coeff = alpha * a0CoeffReciprocal;
            let a1Coeff = -2 * Math.cos(omega) * a0CoeffReciprocal;
            let a2Coeff = (1 - alpha) * a0CoeffReciprocal;

            // We optimize since b2 = -b0, b1 = 0.
            // Update equation for two-pole bandpass filter:
            //   u[n] = x[n] - a1 * x[n-1] - a2 * x[n-2]
            //   y[n] = b0 * (u[n] - u[n-2])
            let um1 = 0;
            let um2 = 0;
            for (let j = 0; j < durationsSamples[i]; j++) {
                // Exponentially-decaying white noise.
                let x = noiseSignal[j] * Math.exp(decayRate * j);

                // Filter signal with bandpass filter and add to output.
                let u = x - a1Coeff * um1 - a2Coeff * um2;
                bufferData[j] += b0Coeff * (u - um2);

                // Update coefficients.
                um2 = um1;
                um1 = u;
            }
        }

        // Create and apply half of a Hann window to the beginning of the
        // impulse response.
        let halfHannLength = Math.round(this._tailonsetSamples);
        for (let i = 0; i < Math.min(bufferData.length, halfHannLength); i++) {
            let halfHann =
                0.5 * (1 - Math.cos(Tau * i / (2 * halfHannLength - 1)));
            bufferData[i] *= halfHann;
        }
        this._convolver.buffer = buffer;
    }
}
