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
 * @file A collection of convolvers. Can be used for the optimized HOA binaural
 * rendering. (e.g. SH-MaxRe HRTFs)
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import {
    ChannelMerger,
    ChannelSplitter, Convolver, Gain
} from "../nodes";
import {
    chain,
    connect,
    disconnect,
    ErsatzAudioNode,
    removeVertex
} from "../util";


export class HOAConvolver implements ErsatzAudioNode {

    private readonly _inputSplitter: ChannelSplitterNode;
    private readonly _outputGain: GainNode;
    private readonly _positiveIndexSphericalHarmonics: GainNode;
    private readonly _negativeIndexSphericalHarmonics: GainNode;
    private readonly _inverter: GainNode;
    private readonly _binauralMerger: ChannelMergerNode;
    private readonly _stereoMergers: ChannelMergerNode[];
    private readonly _convolvers: ConvolverNode[];
    private readonly _stereoSplitters: ChannelSplitterNode[];

    get input() { return this._inputSplitter; }
    get output() { return this._outputGain; }

    private _active = false;
    private _isBufferLoaded = false;

    private readonly _ambisonicOrder: number;
    private readonly _numberOfChannels: number;

    /**
     * A convolver network for N-channel HOA stream.
     * @constructor
     * @param {AudioContext} context - Associated AudioContext.
     * @param {Number} ambisonicOrder - Ambisonic order. (2 or 3)
     * @param {AudioBuffer[]} [hrirBufferList] - An ordered-list of stereo
     * AudioBuffers for convolution. (SOA: 5 AudioBuffers, TOA: 8 AudioBuffers)
     */
    constructor(name: string, context: BaseAudioContext, ambisonicOrder: number, hrirBufferList?: AudioBuffer[]) {

        // The number of channels K based on the ambisonic order N where K = (N+1)^2.
        this._ambisonicOrder = ambisonicOrder;
        this._numberOfChannels = (this._ambisonicOrder + 1) * (this._ambisonicOrder + 1);

        const numberOfStereoChannels = Math.ceil(this._numberOfChannels / 2);

        /**
         * Build the internal audio graph.
         * For TOA convolution:
         *   input -> splitter(16) -[0,1]-> merger(2) -> convolver(2) -> splitter(2)
         *                         -[2,3]-> merger(2) -> convolver(2) -> splitter(2)
         *                         -[4,5]-> ... (6 more, 8 branches total)
         */

        this._inputSplitter =
            context.createChannelSplitter(this._numberOfChannels);
        this._stereoMergers = [];
        this._convolvers = [];
        this._stereoSplitters = [];
        this._positiveIndexSphericalHarmonics = Gain(`${name}-hoa-convolver-positiveIndexSphericalHarmonics`, context);
        this._negativeIndexSphericalHarmonics = Gain(`${name}-hoa-convolver-negativeIndexSphericalHarmonics`, context);
        this._inverter = Gain(`${name}-hoa-convolver-inverter`, context, { gain: -1 });
        this._binauralMerger = ChannelMerger(`${name}-hoa-convolver-binauralMerger`, context, { channelCount: 2 });
        this._outputGain = Gain(`${name}-hoa-convolver-outputGain`, context);

        for (let i = 0; i < numberOfStereoChannels; ++i) {
            this._stereoMergers[i] = ChannelMerger(`${name}-hoa-convolver-stereoMergers[i]`, context, { channelCount: 2 });
            this._convolvers[i] = Convolver(`${name}-hoa-convolver-convolvers[i]`, context, { disableNormalization: true });
            this._stereoSplitters[i] = ChannelSplitter(`${name}-hoa-convolver-stereoSplitters[i]`, context, { channelCount: 2 });
        }

        for (let l = 0; l <= this._ambisonicOrder; ++l) {
            for (let m = -l; m <= l; m++) {
                // We compute the ACN index (k) of ambisonics channel using the degree (l)
                // and index (m): k = l^2 + l + m
                const acnIndex = l * l + l + m;
                const stereoIndex = Math.floor(acnIndex / 2);

                // Split channels from input into array of stereo convolvers.
                // Then create a network of mergers that produces the stereo output.
                connect(this._inputSplitter, [acnIndex, acnIndex % 2, this._stereoMergers[stereoIndex]]);
                chain(this._stereoMergers[stereoIndex], this._convolvers[stereoIndex], this._stereoSplitters[stereoIndex]);

                // Positive index (m >= 0) spherical harmonics are symmetrical around the
                // front axis, while negative index (m < 0) spherical harmonics are
                // anti-symmetrical around the front axis. We will exploit this symmetry
                // to reduce the number of convolutions required when rendering to a
                // symmetrical binaural renderer.
                if (m >= 0) {
                    connect(this._stereoSplitters[stereoIndex], [acnIndex % 2, this._positiveIndexSphericalHarmonics]);
                } else {
                    connect(this._stereoSplitters[stereoIndex], [acnIndex % 2, this._negativeIndexSphericalHarmonics]);
                }
            }
        }

        connect(this._positiveIndexSphericalHarmonics, [0, 0, this._binauralMerger]);
        connect(this._positiveIndexSphericalHarmonics, [0, 1, this._binauralMerger]);
        connect(this._negativeIndexSphericalHarmonics, [0, 0, this._binauralMerger]);
        connect(this._negativeIndexSphericalHarmonics, this._inverter);
        connect(this._inverter, [0, 1, this._binauralMerger]);


        if (hrirBufferList) {
            this.setHRIRBufferList(hrirBufferList);
        }

        this.enable();
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this._inputSplitter);
            removeVertex(this._outputGain);
            removeVertex(this._positiveIndexSphericalHarmonics);
            removeVertex(this._negativeIndexSphericalHarmonics);
            removeVertex(this._inverter);
            removeVertex(this._binauralMerger);
            this._stereoMergers.forEach(removeVertex);
            this._convolvers.forEach(removeVertex);
            this._stereoSplitters.forEach(removeVertex);
        }
    }


    /**
     * Assigns N HRIR AudioBuffers to N convolvers: Note that we use 2 stereo
     * convolutions for 4-channel direct convolution. Using mono convolver or
     * 4-channel convolver is not viable because mono convolution wastefully
     * produces the stereo outputs, and the 4-ch convolver does cross-channel
     * convolution. (See Web Audio API spec)
     * @param {AudioBuffer[]} hrirBufferList - An array of stereo AudioBuffers for
     * convolvers.
     */
    setHRIRBufferList(hrirBufferList: AudioBuffer[]) {
        // After these assignments, the channel data in the buffer is immutable in
        // FireFox. (i.e. neutered) So we should avoid re-assigning buffers, otherwise
        // an exception will be thrown.
        if (this._isBufferLoaded) {
            return;
        }

        for (let i = 0; i < hrirBufferList.length; ++i) {
            this._convolvers[i].buffer = hrirBufferList[i];
        }

        this._isBufferLoaded = true;
    }


    /**
     * Enable HOAConvolver instance. The audio graph will be activated and pulled by
     * the WebAudio engine. (i.e. consume CPU cycle)
     */
    enable() {
        connect(this._binauralMerger, this._outputGain);
        this._active = true;
    }


    /**
     * Disable HOAConvolver instance. The inner graph will be disconnected from the
     * audio destination, thus no CPU cycle will be consumed.
     */
    disable() {
        disconnect(this._binauralMerger, this._outputGain);
        this._active = false;
    }

    get isActive() {
        return this._active;
    }
}