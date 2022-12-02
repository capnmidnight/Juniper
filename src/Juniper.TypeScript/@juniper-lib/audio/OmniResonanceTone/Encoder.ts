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
 * @file Spatially encodes input using weighted spherical harmonics.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { arrayClear } from "@juniper-lib/tslib/collections/arrays";
import { isBadNumber, isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ChannelMerger, Gain } from "../nodes";
import { connect, disconnect, ErsatzAudioNode, removeVertex } from "../util";
import * as Tables from "./Tables";
import * as Utils from "./utils";

export interface EncoderOptions {

    /**
     * Desired ambisonic order. Defaults to
     * {@linkcode Utils.DEFAULT_AMBISONIC_ORDER DEFAULT_AMBISONIC_ORDER}.
     */
    ambisonicOrder: number;

    /**
     * Azimuth (in degrees). Defaults to
     * {@linkcode Utils.DEFAULT_AZIMUTH DEFAULT_AZIMUTH}.
     */
    azimuth: number;

    /**
     * Elevation (in degrees). Defaults to
     * {@linkcode Utils.DEFAULT_ELEVATION DEFAULT_ELEVATION}.
     */
    elevation: number;

    /**
     * Source width (in degrees). Where 0 degrees is a point source and 360 degrees
     * is an omnidirectional source. Defaults to
     * {@linkcode Utils.DEFAULT_SOURCE_WIDTH DEFAULT_SOURCE_WIDTH}.
     */
    sourceWidth: number;
}

/**
 * Spatially encodes input using weighted spherical harmonics.
 */
export class Encoder implements ErsatzAudioNode {

    /**
     * Validate the provided ambisonic order.
     * @param ambisonicOrder Desired ambisonic order.
     * @return Validated/adjusted ambisonic order.
     * @private
     */
    static validateAmbisonicOrder(ambisonicOrder: number) {
        if (isBadNumber(ambisonicOrder)) {
            Utils.log('Error: Invalid ambisonic order',
                ambisonicOrder, '\nUsing ambisonicOrder=1 instead.');
            ambisonicOrder = 1;
        } else if (ambisonicOrder < 1) {
            Utils.log('Error: Unable to render ambisonic order',
                ambisonicOrder, '(Min order is 1)',
                '\nUsing min order instead.');
            ambisonicOrder = 1;
        } else if (ambisonicOrder > Tables.SPHERICAL_HARMONICS_MAX_ORDER) {
            Utils.log('Error: Unable to render ambisonic order',
                ambisonicOrder, '(Max order is',
                Tables.SPHERICAL_HARMONICS_MAX_ORDER, ')\nUsing max order instead.');
            ambisonicOrder = Tables.SPHERICAL_HARMONICS_MAX_ORDER;
        }
        return ambisonicOrder;
    }


    // Public variables.
    /**
     * Mono (1-channel) input {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
     */
    readonly input: GainNode;

    /**
     * Ambisonic (multichannel) output {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioNode AudioNode}.
     */
    readonly output: GainNode;

    private _ambisonicOrder: number;
    private _azimuth: number;
    private _elevation: number;
    private _spreadIndex: number;

    private readonly _channelGain = new Array<GainNode>();
    private _merger: ChannelMergerNode = null;

    /**
     * Spatially encodes input using weighted spherical harmonics.
     * @param name a name for this node, to help differentiate it from other nodes in graph rendering.
     * @param context Associated {@link https://developer.mozilla.org/en-US/docs/Web/API/AudioContext AudioContext}.
     * @param options
     */
    constructor(private readonly name: string, context: BaseAudioContext, options?: Partial<EncoderOptions>) {

        // Use defaults for undefined arguments.
        if (isNullOrUndefined(options)) {
            options = {};
        }
        if (isBadNumber(options.ambisonicOrder)) {
            options.ambisonicOrder = Utils.DEFAULT_AMBISONIC_ORDER;
        }
        if (isBadNumber(options.azimuth)) {
            options.azimuth = Utils.DEFAULT_AZIMUTH;
        }
        if (isBadNumber(options.elevation)) {
            options.elevation = Utils.DEFAULT_ELEVATION;
        }
        if (isBadNumber(options.sourceWidth)) {
            options.sourceWidth = Utils.DEFAULT_SOURCE_WIDTH;
        }

        // Create I/O nodes.
        this.input = Gain(`${name}-encoder-input-gain`, context);
        this.output = Gain(`${name}-encoder-output-gain`, context);
        this._azimuth = options.azimuth;
        this._elevation = options.elevation;
        this.setSourceWidth(options.sourceWidth);

        // Set initial order, angle and source width.
        this.ambisonicOrder = options.ambisonicOrder;

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this.input);
            removeVertex(this.output);
            this.removeMiddle();
        }
    }

    private removeMiddle() {
        if (this._channelGain.length > 0) {
            this._channelGain.forEach(removeVertex);
            arrayClear(this._channelGain);
        }
        if (isDefined(this._merger)) {
            removeVertex(this._merger);
        }
    }

    private get _context() {
        return this.input.context;
    }

    get ambisonicOrder() {
        return this._ambisonicOrder;
    }

    /**
     * Set the desired ambisonic order.
     * @param ambisonicOrder Desired ambisonic order.
     */
    set ambisonicOrder(ambisonicOrder: number) {
        this._ambisonicOrder = Encoder.validateAmbisonicOrder(ambisonicOrder);

        disconnect(this.input);
        this.removeMiddle();

        // Create audio graph.
        const channelCount = (this._ambisonicOrder + 1) * (this._ambisonicOrder + 1);
        this._merger = ChannelMerger(`${this.name}-encoder-channel-merger`, this._context, {
            channelCount
        });

        for (let i = 0; i < channelCount; i++) {
            this._channelGain[i] = Gain(`${this.name}-encoder-channel-${i}-gain`, this._context);
            connect(this.input, this._channelGain[i]);
            connect(this._channelGain[i], [0, i, this._merger]);
        }
        connect(this._merger, this.output);
    }


    /**
     * Set the direction of the encoded source signal.
     * @param azimuth Azimuth (in degrees). Defaults to {@linkcode Utils.DEFAULT_AZIMUTH DEFAULT_AZIMUTH}.
     * @param elevation Elevation (in degrees). Defaults to {@linkcode Utils.DEFAULT_ELEVATION DEFAULT_ELEVATION}.
     */
    setDirection(azimuth: number, elevation: number) {
        // Format input direction to nearest indices.
        if (isBadNumber(azimuth)) {
            azimuth = Utils.DEFAULT_AZIMUTH;
        }
        if (isBadNumber(elevation)) {
            elevation = Utils.DEFAULT_ELEVATION;
        }

        // Store the formatted input (for updating source width).
        this._azimuth = azimuth;
        this._elevation = elevation;

        // Format direction for index lookups.
        azimuth = Math.round(azimuth % 360);
        if (azimuth < 0) {
            azimuth += 360;
        }
        elevation = Math.round(Math.min(90, Math.max(-90, elevation))) + 90;

        // Assign gains to each output.
        this.update();
    }

    /**
     * Set the source width (in degrees). Where 0 degrees is a point source and 360
     * degrees is an omnidirectional source.
     * @param sourceWidth (in degrees).
     */
    setSourceWidth(sourceWidth: number) {
        // The MAX_RE_WEIGHTS is a 360 x (Tables.SPHERICAL_HARMONICS_MAX_ORDER+1)
        // size table.
        this._spreadIndex = Math.min(359, Math.max(0, Math.round(sourceWidth)));
        this.update();
    }


    private update() {
        this._channelGain[0].gain.value = Tables.MAX_RE_WEIGHTS[this._spreadIndex][0];
        for (let i = 1; i <= this._ambisonicOrder; i++) {
            let degreeWeight = Tables.MAX_RE_WEIGHTS[this._spreadIndex][i];
            for (let j = -i; j <= i; j++) {
                let acnChannel = (i * i) + i + j;
                let elevationIndex = i * (i + 1) / 2 + Math.abs(j) - 1;
                let val = Tables.SPHERICAL_HARMONICS[1][this._elevation][elevationIndex];
                if (j != 0) {
                    let azimuthIndex = Tables.SPHERICAL_HARMONICS_MAX_ORDER + j - 1;
                    if (j < 0) {
                        azimuthIndex = Tables.SPHERICAL_HARMONICS_MAX_ORDER + j;
                    }
                    val *= Tables.SPHERICAL_HARMONICS[0][this._azimuth][azimuthIndex];
                }
                this._channelGain[acnChannel].gain.value = val * degreeWeight;
            }
        }
    }
}