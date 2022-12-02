/**
 * @license
 * Copyright 2016 Google Inc. All Rights Reserved.
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
 * @file Sound field rotator for first-order-ambisonics decoding.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { mat3, mat4, ReadonlyMat3, ReadonlyMat4 } from "gl-matrix";
import {
    ChannelMerger,
    ChannelSplitter,
    Gain
} from "../nodes";
import {
    connect,
    ErsatzAudioNode,
    removeVertex
} from "../util";


/**
 * First-order-ambisonic decoder based on gain node network.
 */
export class FOARotator implements ErsatzAudioNode {

    private readonly _splitter: ChannelSplitterNode;
    private readonly _inY: GainNode;
    private readonly _inZ: GainNode;
    private readonly _inX: GainNode;
    private readonly _m0: GainNode;
    private readonly _m1: GainNode;
    private readonly _m2: GainNode;
    private readonly _m3: GainNode;
    private readonly _m4: GainNode;
    private readonly _m5: GainNode;
    private readonly _m6: GainNode;
    private readonly _m7: GainNode;
    private readonly _m8: GainNode;
    private readonly _outY: GainNode;
    private readonly _outZ: GainNode;
    private readonly _outX: GainNode;
    private readonly _merger: ChannelMergerNode;

    get input() { return this._splitter; }
    get output() { return this._merger; }

    /**
     * First-order-ambisonic decoder based on gain node network.
     * @param context - Associated AudioContext.
     */
    constructor(name: string, context: BaseAudioContext) {

        this._splitter = ChannelSplitter(`${name}-foa-rotator-splitter`, context, { channelCount: 4 });
        this._inY = Gain(`${name}-foa-rotator-inY`, context, { gain: -1 });
        this._inZ = Gain(`${name}-foa-rotator-inZ`, context);
        this._inX = Gain(`${name}-foa-rotator-inX`, context, { gain: -1 });
        this._m0 = Gain(`${name}-foa-rotator-m0`, context);
        this._m1 = Gain(`${name}-foa-rotator-m1`, context);
        this._m2 = Gain(`${name}-foa-rotator-m2`, context);
        this._m3 = Gain(`${name}-foa-rotator-m3`, context);
        this._m4 = Gain(`${name}-foa-rotator-m4`, context);
        this._m5 = Gain(`${name}-foa-rotator-m5`, context);
        this._m6 = Gain(`${name}-foa-rotator-m6`, context);
        this._m7 = Gain(`${name}-foa-rotator-m7`, context);
        this._m8 = Gain(`${name}-foa-rotator-m8`, context);
        this._outY = Gain(`${name}-foa-rotator-outY`, context, { gain: -1 });
        this._outX = Gain(`${name}-foa-rotator-outX`, context, { gain: -1 });
        this._outZ = Gain(`${name}-foa-rotator-outZ`, context);
        this._merger = ChannelMerger(`${name}-foa-rotator-merger`, context, { channelCount: 4 });

        // ACN channel ordering: [1, 2, 3] => [-Y, Z, -X]
        // Y (from channel 1)
        connect(this._splitter, [1, this._inY]);
        // Z (from channel 2)
        connect(this._splitter, [2, this._inZ]);
        // X (from channel 3)
        connect(this._splitter, [3, this._inX]);

        // Apply the rotation in the world space.
        // |Y|   | m0  m3  m6 |   | Y * m0 + Z * m3 + X * m6 |   | Yr |
        // |Z| * | m1  m4  m7 | = | Y * m1 + Z * m4 + X * m7 | = | Zr |
        // |X|   | m2  m5  m8 |   | Y * m2 + Z * m5 + X * m8 |   | Xr |
        connect(this._inY, this._m0);
        connect(this._inY, this._m1);
        connect(this._inY, this._m2);
        connect(this._inZ, this._m3);
        connect(this._inZ, this._m4);
        connect(this._inZ, this._m5);
        connect(this._inX, this._m6);
        connect(this._inX, this._m7);
        connect(this._inX, this._m8);
        connect(this._m0, this._outY);
        connect(this._m1, this._outZ);
        connect(this._m2, this._outX);
        connect(this._m3, this._outY);
        connect(this._m4, this._outZ);
        connect(this._m5, this._outX);
        connect(this._m6, this._outY);
        connect(this._m7, this._outZ);
        connect(this._m8, this._outX);

        // Transform 3: world space to audio space.
        // W -> W (to channel 0)
        connect(this._splitter, [0, 0, this._merger]);
        // Y (to channel 1)
        connect(this._outY, [0, 1, this._merger]);
        // Z (to channel 2)
        connect(this._outZ, [0, 2, this._merger]);
        // X (to channel 3)
        connect(this._outX, [0, 3, this._merger]);

        this.setRotationMatrix3(mat3.identity(mat3.create()));

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this._splitter);
            removeVertex(this._inY);
            removeVertex(this._inZ);
            removeVertex(this._inX);
            removeVertex(this._m0);
            removeVertex(this._m1);
            removeVertex(this._m2);
            removeVertex(this._m3);
            removeVertex(this._m4);
            removeVertex(this._m5);
            removeVertex(this._m6);
            removeVertex(this._m7);
            removeVertex(this._m8);
            removeVertex(this._outY);
            removeVertex(this._outZ);
            removeVertex(this._outX);
            removeVertex(this._merger);
        }
    }


    /**
     * Updates the rotation matrix with 3x3 matrix.
     * @param {Number[]} mat - A 3x3 rotation matrix. (column-major)
     */
    setRotationMatrix3(mat: ReadonlyMat3) {
        this._m0.gain.value = mat[0];
        this._m1.gain.value = mat[1];
        this._m2.gain.value = mat[2];
        this._m3.gain.value = mat[3];
        this._m4.gain.value = mat[4];
        this._m5.gain.value = mat[5];
        this._m6.gain.value = mat[6];
        this._m7.gain.value = mat[7];
        this._m8.gain.value = mat[8];
    }


    /**
     * Returns the current 3x3 rotation matrix.
     * @return {Number[]} - A 3x3 rotation matrix. (column-major)
     */
    getRotationMatrix3(dest: mat3) {
        dest[0] = this._m0.gain.value;
        dest[1] = this._m1.gain.value;
        dest[2] = this._m2.gain.value;
        dest[3] = this._m3.gain.value;
        dest[4] = this._m4.gain.value;
        dest[5] = this._m5.gain.value;
        dest[6] = this._m6.gain.value;
        dest[7] = this._m7.gain.value;
        dest[8] = this._m8.gain.value;
    }


    /**
     * Updates the rotation matrix with 4x4 matrix.
     * @param {Number[]} mat - A 4x4 rotation matrix. (column-major)
     */
    setRotationMatrix4(mat: ReadonlyMat4) {
        this._m0.gain.value = mat[0];
        this._m1.gain.value = mat[1];
        this._m2.gain.value = mat[2];

        this._m3.gain.value = mat[4];
        this._m4.gain.value = mat[5];
        this._m5.gain.value = mat[6];

        this._m6.gain.value = mat[8];
        this._m7.gain.value = mat[9];
        this._m8.gain.value = mat[10];
    }


    /**
     * Returns the current 4x4 rotation matrix.
     * @return {Number[]} - A 4x4 rotation matrix. (column-major)
     */
    getRotationMatrix4(dest: mat4) {
        dest[0] = this._m0.gain.value;
        dest[1] = this._m1.gain.value;
        dest[2] = this._m2.gain.value;
        dest[3] = 0;
        dest[4] = this._m3.gain.value;
        dest[5] = this._m4.gain.value;
        dest[6] = this._m5.gain.value;
        dest[7] = 0;
        dest[8] = this._m6.gain.value;
        dest[9] = this._m7.gain.value;
        dest[10] = this._m8.gain.value;
        dest[11] = 0;
        dest[12] = 0;
        dest[13] = 0;
        dest[14] = 0;
        dest[15] = 0;
    }
}