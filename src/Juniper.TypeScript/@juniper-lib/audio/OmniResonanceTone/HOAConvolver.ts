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

import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperChannelMergerNode } from "../context/JuniperChannelMergerNode";
import { JuniperChannelSplitterNode } from "../context/JuniperChannelSplitterNode";
import { JuniperConvolverNode } from "../context/JuniperConvolverNode";
import { JuniperGainNode } from "../context/JuniperGainNode";


/**
 * @file A collection of convolvers. Can be used for the optimized HOA binaural
 * rendering. (e.g. SH-MaxRe HRTFs)
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


export class HOAConvolver extends BaseNodeCluster {
    private readonly _outputGain: JuniperGainNode;
    private readonly _binauralMerger: JuniperChannelMergerNode;
    private readonly _convolvers: JuniperConvolverNode[];

    private _active = false;
    private _isBufferLoaded = false;

    /**
     * A convolver network for N-channel HOA stream.
     * @constructor
     * @param {AudioContext} context - Associated AudioContext.
     * @param {Number} ambisonicOrder - Ambisonic order. (2 or 3)
     * @param {AudioBuffer[]} [hrirBufferList] - An ordered-list of stereo
     * AudioBuffers for convolution. (SOA: 5 AudioBuffers, TOA: 8 AudioBuffers)
     */
    constructor(context: JuniperAudioContext, ambisonicOrder: number, hrirBufferList?: AudioBuffer[]) {

        // The number of channels K based on the ambisonic order N where K = (N+1)^2.
        const numberOfChannels = (ambisonicOrder + 1) * (ambisonicOrder + 1);

        const numberOfStereoChannels = Math.ceil(numberOfChannels / 2);

        /**
         * Build the internal audio graph.
         * For TOA convolution:
         *   input -> splitter(16) -[0,1]-> merger(2) -> convolver(2) -> splitter(2)
         *                         -[2,3]-> merger(2) -> convolver(2) -> splitter(2)
         *                         -[4,5]-> ... (6 more, 8 branches total)
         */

        const inputSplitter = new JuniperChannelSplitterNode(context, {
            channelCount: numberOfChannels
        });
        const stereoMergers = [];
        const convolvers = [];
        const stereoSplitters = [];
        const positiveIndexSphericalHarmonics = new JuniperGainNode(context);
        const negativeIndexSphericalHarmonics = new JuniperGainNode(context);
        const inverter = new JuniperGainNode(context, { gain: -1 });
        const binauralMerger = new JuniperChannelMergerNode(context, { channelCount: 2 });
        const outputGain = new JuniperGainNode(context);

        for (let i = 0; i < numberOfStereoChannels; ++i) {
            stereoMergers[i] = new JuniperChannelMergerNode(context, { channelCount: 2 });
            convolvers[i] = new JuniperConvolverNode(context, { disableNormalization: true });
            stereoSplitters[i] = new JuniperChannelSplitterNode(context, { channelCount: 2 });
        }

        for (let l = 0; l <= ambisonicOrder; ++l) {
            for (let m = -l; m <= l; m++) {
                // We compute the ACN index (k) of ambisonics channel using the degree (l)
                // and index (m): k = l^2 + l + m
                const acnIndex = l * l + l + m;
                const stereoIndex = Math.floor(acnIndex / 2);

                // Split channels from input into array of stereo convolvers.
                // Then create a network of mergers that produces the stereo output.
                inputSplitter
                    .connect(stereoMergers[stereoIndex], acnIndex, acnIndex % 2)
                    .connect(convolvers[stereoIndex])
                    .connect(stereoSplitters[stereoIndex]);

                // Positive index (m >= 0) spherical harmonics are symmetrical around the
                // front axis, while negative index (m < 0) spherical harmonics are
                // anti-symmetrical around the front axis. We will exploit this symmetry
                // to reduce the number of convolutions required when rendering to a
                // symmetrical binaural renderer.
                if (m >= 0) {
                    stereoSplitters[stereoIndex].connect(positiveIndexSphericalHarmonics, acnIndex % 2);
                } else {
                    stereoSplitters[stereoIndex].connect(negativeIndexSphericalHarmonics, acnIndex % 2);
                }
            }
        }

        positiveIndexSphericalHarmonics.connect(binauralMerger, 0, 0);
        positiveIndexSphericalHarmonics.connect(binauralMerger, 0, 1);
        negativeIndexSphericalHarmonics.connect(binauralMerger, 0, 0);
        negativeIndexSphericalHarmonics
            .connect(inverter)
            .connect(binauralMerger, 0, 1);

        super("hoa-convolver", context, [inputSplitter], [outputGain], [
            ...stereoMergers,
            ...convolvers,
            ...stereoSplitters,
            positiveIndexSphericalHarmonics,
            negativeIndexSphericalHarmonics,
            inverter,
            binauralMerger
        ]);

        this._convolvers = convolvers;
        this._binauralMerger = binauralMerger;
        this._outputGain = outputGain;

        if (hrirBufferList) {
            this.setHRIRBufferList(hrirBufferList);
        }

        this.enable();
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
        this._binauralMerger.connect(this._outputGain);
        this._active = true;
    }


    /**
     * Disable HOAConvolver instance. The inner graph will be disconnected from the
     * audio destination, thus no CPU cycle will be consumed.
     */
    disable() {
        this._binauralMerger.disconnect(this._outputGain);
        this._active = false;
    }

    get isActive() {
        return this._active;
    }
}