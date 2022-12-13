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
 * @file A collection of convolvers. Can be used for the optimized FOA binaural
 * rendering. (e.g. SH-MaxRe HRTFs)
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


/**
 * FOAConvolver. A collection of 2 stereo convolvers for 4-channel FOA stream.
 */
export class FOAConvolver extends BaseNodeCluster {

    private _active = false;
    private _isBufferLoaded = false;

    private readonly _splitterWYZX: JuniperChannelSplitterNode;
    private readonly _summingBus: JuniperGainNode;
    private readonly _convolverWY: JuniperConvolverNode;
    private readonly _convolverZX: JuniperConvolverNode;
    private readonly _mergerBinaural: JuniperChannelMergerNode;

    get input() { return this._splitterWYZX; }
    get output() { return this._summingBus; }

    /**
     * FOAConvolver. A collection of 2 stereo convolvers for 4-channel FOA stream.
     * @param context The associated AudioContext.
     * @param hrirBufferList - An ordered-list of stereo
     * AudioBuffers for convolution. (i.e. 2 stereo AudioBuffers for FOA)
     */
    constructor(context: JuniperAudioContext, hrirBufferList?: AudioBuffer[]) {

        const splitterWYZX = new JuniperChannelSplitterNode(context, { channelCount: 4 });
        const summingBus = new JuniperGainNode(context);

        const mergerWY = new JuniperChannelMergerNode(context, { channelCount: 2 });
        const mergerZX = new JuniperChannelMergerNode(context, { channelCount: 2 });

        // By default, WebAudio's convolver does the normalization based on IR's
        // energy. For the precise convolution, it must be disabled before the buffer
        // assignment.
        const convolverWY = new JuniperConvolverNode(context, { disableNormalization: true });
        const convolverZX = new JuniperConvolverNode(context, { disableNormalization: true });

        const splitterWY = new JuniperChannelSplitterNode(context, { channelCount: 2 });
        const splitterZX = new JuniperChannelSplitterNode(context, { channelCount: 2 });

        // For asymmetric degree.
        const inverter = new JuniperGainNode(context, { gain: -1 });
        const mergerBinaural = new JuniperChannelMergerNode(context, { channelCount: 2 });

        // Group W and Y, then Z and X.

        splitterWYZX.connect(mergerWY, 0, 0);
        splitterWYZX.connect(mergerWY, 1, 1);
        splitterWYZX.connect(mergerZX, 2, 0);
        splitterWYZX.connect(mergerZX, 3, 1);

        // Create a network of convolvers using splitter/merger.
        mergerWY
            .connect(convolverWY)
            .connect(splitterWY);
        mergerZX
            .connect(convolverZX)
            .connect(splitterZX);

        splitterWY.connect(mergerBinaural, 0, 0);
        splitterWY.connect(mergerBinaural, 0, 1);
        splitterWY.connect(mergerBinaural, 1, 0);
        splitterWY.connect(inverter, 1, 0);
        inverter.connect(mergerBinaural, 0, 1);
        splitterZX.connect(mergerBinaural, 0, 0);
        splitterZX.connect(mergerBinaural, 0, 1);
        splitterZX.connect(mergerBinaural, 1, 0);
        splitterZX.connect(mergerBinaural, 1, 1);

        super("foa-convolver", context, [splitterWYZX], [summingBus], [mergerWY,
            mergerZX,
            convolverWY,
            convolverZX,
            splitterWY,
            splitterZX,
            inverter,
            mergerBinaural]);

        if (hrirBufferList) {
            this.setHRIRBufferList(hrirBufferList);
        }

        this.enable();

        Object.seal(this);
    }


    /**
     * Assigns 2 HRIR AudioBuffers to 2 convolvers: Note that we use 2 stereo
     * convolutions for 4-channel direct convolution. Using mono convolver or
     * 4-channel convolver is not viable because mono convolution wastefully
     * produces the stereo outputs, and the 4-ch convolver does cross-channel
     * convolution. (See Web Audio API spec)
     * @param hrirBufferList - An array of stereo AudioBuffers for
     * convolvers.
     */
    setHRIRBufferList(hrirBufferList: AudioBuffer[]) {
        // After these assignments, the channel data in the buffer is immutable in
        // FireFox. (i.e. neutered) So we should avoid re-assigning buffers, otherwise
        // an exception will be thrown.
        if (this._isBufferLoaded) {
            return;
        }

        this._convolverWY.buffer = hrirBufferList[0];
        this._convolverZX.buffer = hrirBufferList[1];
        this._isBufferLoaded = true;
    }


    /**
     * Enable FOAConvolver instance. The audio graph will be activated and pulled by
     * the WebAudio engine. (i.e. consume CPU cycle)
     */
    enable() {
        this._mergerBinaural.connect(this._summingBus);
        this._active = true;
    }


    /**
     * Disable FOAConvolver instance. The inner graph will be disconnected from the
     * audio destination, thus no CPU cycle will be consumed.
     */
    disable() {
        this._mergerBinaural.disconnect(this._summingBus);
        this._active = false;
    }

    get isActive() {
        return this._active;
    }
}