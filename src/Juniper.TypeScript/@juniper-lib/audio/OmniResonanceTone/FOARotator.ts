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
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperChannelMergerNode } from "../context/JuniperChannelMergerNode";
import { JuniperChannelSplitterNode } from "../context/JuniperChannelSplitterNode";
import { JuniperGainNode } from "../context/JuniperGainNode";


/**
 * First-order-ambisonic decoder based on gain node network.
 */
export class FOARotator extends BaseNodeCluster {

    private readonly _splitter: JuniperChannelSplitterNode;
    private readonly _m0: JuniperGainNode;
    private readonly _m1: JuniperGainNode;
    private readonly _m2: JuniperGainNode;
    private readonly _m3: JuniperGainNode;
    private readonly _m4: JuniperGainNode;
    private readonly _m5: JuniperGainNode;
    private readonly _m6: JuniperGainNode;
    private readonly _m7: JuniperGainNode;
    private readonly _m8: JuniperGainNode;
    private readonly _merger: JuniperChannelMergerNode;

    get input() { return this._splitter; }
    get output() { return this._merger; }

    /**
     * First-order-ambisonic decoder based on gain node network.
     * @param context - Associated AudioContext.
     */
    constructor(context: JuniperAudioContext) {
        const splitter = new JuniperChannelSplitterNode(context, { channelCount: 4 });
        const inY = new JuniperGainNode(context, { gain: -1 });
        const inZ = new JuniperGainNode(context);
        const inX = new JuniperGainNode(context, { gain: -1 });
        const m0 = new JuniperGainNode(context);
        const m1 = new JuniperGainNode(context);
        const m2 = new JuniperGainNode(context);
        const m3 = new JuniperGainNode(context);
        const m4 = new JuniperGainNode(context);
        const m5 = new JuniperGainNode(context);
        const m6 = new JuniperGainNode(context);
        const m7 = new JuniperGainNode(context);
        const m8 = new JuniperGainNode(context);
        const outY = new JuniperGainNode(context, { gain: -1 });
        const outX = new JuniperGainNode(context, { gain: -1 });
        const outZ = new JuniperGainNode(context);
        const merger = new JuniperChannelMergerNode(context, { channelCount: 4 });

        // ACN channel ordering: [1, 2, 3] => [-Y, Z, -X]
        // Y (from channel 1)
        splitter.connect(inY, 1);
        // Z (from channel 2)
        splitter.connect(inZ, 2);
        // X (from channel 3)
        splitter.connect(inX, 3);

        // Apply the rotation in the world space.
        // |Y|   | m0  m3  m6 |   | Y * m0 + Z * m3 + X * m6 |   | Yr |
        // |Z| * | m1  m4  m7 | = | Y * m1 + Z * m4 + X * m7 | = | Zr |
        // |X|   | m2  m5  m8 |   | Y * m2 + Z * m5 + X * m8 |   | Xr |
        inY.connect(m0);
        inY.connect(m1);
        inY.connect(m2);
        inZ.connect(m3);
        inZ.connect(m4);
        inZ.connect(m5);
        inX.connect(m6);
        inX.connect(m7);
        inX.connect(m8);
        m0.connect(outY);
        m1.connect(outZ);
        m2.connect(outX);
        m3.connect(outY);
        m4.connect(outZ);
        m5.connect(outX);
        m6.connect(outY);
        m7.connect(outZ);
        m8.connect(outX);

        // Transform 3: world space to audio space.
        // W -> W (to channel 0)
        splitter.connect(merger, 0, 0);
        // Y (to channel 1)
        outY.connect(merger, 0, 1);
        // Z (to channel 2)
        outZ.connect(merger, 0, 2);
        // X (to channel 3)
        outX.connect(merger, 0, 3);

        super("foa-rotator", context, [splitter], [merger], [
            splitter,
            inY,
            inZ,
            inX,
            m0,
            m1,
            m2,
            m3,
            m4,
            m5,
            m6,
            m7,
            m8,
            outY,
            outX,
            outZ,
            merger
        ]);

        this._splitter = splitter;
        this._m0 = m0;
        this._m1 = m1;
        this._m2 = m2;
        this._m3 = m3;
        this._m4 = m4;
        this._m5 = m5;
        this._m6 = m6;
        this._m7 = m7;
        this._m8 = m8;
        this._merger = merger;

        this.setRotationMatrix3(mat3.identity(mat3.create()));

        Object.seal(this);
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