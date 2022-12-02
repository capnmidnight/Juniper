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
 * @file A collection of convolvers. Can be used for the optimized FOA binaural
 * rendering. (e.g. SH-MaxRe HRTFs)
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import {
    ChannelMerger,
    ChannelSplitter,
    Convolver,
    Gain
} from "../nodes";
import {
    chain,
    connect,
    disconnect,
    ErsatzAudioNode,
    removeVertex
} from "../util";


/**
 * FOAConvolver. A collection of 2 stereo convolvers for 4-channel FOA stream.
 */
export class FOAConvolver implements ErsatzAudioNode {

    private _active = false;
    private _isBufferLoaded = false;

    private readonly _splitterWYZX: ChannelSplitterNode;
    private readonly _summingBus: GainNode;
    private readonly _mergerWY: ChannelMergerNode;
    private readonly _mergerZX: ChannelMergerNode;
    private readonly _convolverWY: ConvolverNode;
    private readonly _convolverZX: ConvolverNode;
    private readonly _splitterWY: ChannelSplitterNode;
    private readonly _splitterZX: ChannelSplitterNode;
    private readonly _inverter: GainNode;
    private readonly _mergerBinaural: ChannelMergerNode;

    get input() { return this._splitterWYZX; }
    get output() { return this._summingBus; }

    /**
     * FOAConvolver. A collection of 2 stereo convolvers for 4-channel FOA stream.
     * @param context The associated AudioContext.
     * @param hrirBufferList - An ordered-list of stereo
     * AudioBuffers for convolution. (i.e. 2 stereo AudioBuffers for FOA)
     */
    constructor(name: string, context: BaseAudioContext, hrirBufferList?: AudioBuffer[]) {

        this._splitterWYZX = ChannelSplitter(`${name}-foa-convolver-splitterWYZX`, context, { channelCount: 4 } );
        this._mergerWY = ChannelMerger(`${name}-foa-convolver-mergerWY`, context, { channelCount: 2 });
        this._mergerZX = ChannelMerger(`${name}-foa-convolver-mergerZX`, context, { channelCount: 2 });

        // By default, WebAudio's convolver does the normalization based on IR's
        // energy. For the precise convolution, it must be disabled before the buffer
        // assignment.
        this._convolverWY = Convolver(`${name}-foa-convolver-convolverWY`, context, { disableNormalization: true });
        this._convolverZX = Convolver(`${name}-foa-convolver-convolverZX`, context, { disableNormalization: true });

        this._splitterWY = ChannelSplitter(`${name}-foa-convolver-splitterWY`, context, { channelCount: 2 });
        this._splitterZX = ChannelSplitter(`${name}-foa-convolver-splitterZX`, context, { channelCount: 2 });

        // For asymmetric degree.
        this._inverter = Gain(`${name}-foa-convolver-inverter`, context, { gain: -1 });
        this._mergerBinaural = ChannelMerger(`${name}-foa-convolver-mergerBinaural`, context, { channelCount: 2 });
        this._summingBus = Gain(`${name}-foa-convolver-summingBus`, context);

        // Group W and Y, then Z and X.

        connect(this._splitterWYZX, [0, 0, this._mergerWY]);
        connect(this._splitterWYZX, [1, 1, this._mergerWY]);
        connect(this._splitterWYZX, [2, 0, this._mergerZX]);
        connect(this._splitterWYZX, [3, 1, this._mergerZX]);

        // Create a network of convolvers using splitter/merger.
        chain(this._mergerWY, this._convolverWY, this._splitterWY);
        chain(this._mergerZX, this._convolverZX, this._splitterZX);

        connect(this._splitterWY, [0, 0, this._mergerBinaural]);
        connect(this._splitterWY, [0, 1, this._mergerBinaural]);
        connect(this._splitterWY, [1, 0, this._mergerBinaural]);
        connect(this._splitterWY, [1, 0, this._inverter]);
        connect(this._inverter, [0, 1, this._mergerBinaural]);
        connect(this._splitterZX, [0, 0, this._mergerBinaural]);
        connect(this._splitterZX, [0, 1, this._mergerBinaural]);
        connect(this._splitterZX, [1, 0, this._mergerBinaural]);
        connect(this._splitterZX, [1, 1, this._mergerBinaural]);

        if (hrirBufferList) {
            this.setHRIRBufferList(hrirBufferList);
        }

        this.enable();

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this._splitterWYZX);
            removeVertex(this._splitterWY);
            removeVertex(this._splitterZX);
            removeVertex(this._mergerBinaural);
            removeVertex(this._mergerWY);
            removeVertex(this._mergerZX);
            removeVertex(this._convolverWY);
            removeVertex(this._convolverZX);
            removeVertex(this._inverter);
            removeVertex(this._summingBus);
        }
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
        connect(this._mergerBinaural, this._summingBus);
        this._active = true;
    }


    /**
     * Disable FOAConvolver instance. The inner graph will be disconnected from the
     * audio destination, thus no CPU cycle will be consumed.
     */
    disable() {
        disconnect(this._mergerBinaural, this._summingBus);
        this._active = false;
    }

    get isActive() {
        return this._active;
    }
}