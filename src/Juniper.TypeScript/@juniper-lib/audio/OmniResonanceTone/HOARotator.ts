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
 * @file Sound field rotator for higher-order-ambisonics decoding.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { mat3, mat4, ReadonlyMat3, ReadonlyMat4 } from "gl-matrix";
import { ChannelMerger, ChannelSplitter, Gain } from "../nodes";
import { connect, ErsatzAudioNode, removeVertex } from "../util";

/**
 * Kronecker Delta function.
 */
function getKroneckerDelta(i: number, j: number): number {
    return i === j ? 1 : 0;
}


/**
 * A helper function to allow us to access a matrix array in the same
 * manner, assuming it is a (2l+1)x(2l+1) matrix. [2] uses an odd convention of
 * referring to the rows and columns using centered indices, so the middle row
 * and column are (0, 0) and the upper left would have negative coordinates.
 * @param matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param l
 * @param i
 * @param j
 * @param gainValue
 */
function setCenteredElement(matrix: GainNode[][], l: number, i: number, j: number, gainValue: number) {
    const index = (j + l) * (2 * l + 1) + (i + l);
    // Row-wise indexing.
    matrix[l - 1][index].gain.value = gainValue;
}


/**
 * This is a helper function to allow us to access a matrix array in the same
 * manner, assuming it is a (2l+1) x (2l+1) matrix.
 * @param matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param l
 * @param i
 * @param j
 */
function getCenteredElement(matrix: GainNode[][], l: number, i: number, j: number): number {
    // Row-wise indexing.
    const index = (j + l) * (2 * l + 1) + (i + l);
    return matrix[l - 1][index].gain.value;
}


/**
 * Helper function defined in [2] that is used by the functions U, V, W.
 * This should not be called on its own, as U, V, and W (and their coefficients)
 * select the appropriate matrix elements to access arguments |a| and |b|.
 * @param matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param i
 * @param a
 * @param b
 * @param l
 */
function getP(matrix: GainNode[][], i: number, a: number, b: number, l: number) {
    if (b === l) {
        return getCenteredElement(matrix, 1, i, 1) *
            getCenteredElement(matrix, l - 1, a, l - 1) -
            getCenteredElement(matrix, 1, i, -1) *
            getCenteredElement(matrix, l - 1, a, -l + 1);
    } else if (b === -l) {
        return getCenteredElement(matrix, 1, i, 1) *
            getCenteredElement(matrix, l - 1, a, -l + 1) +
            getCenteredElement(matrix, 1, i, -1) *
            getCenteredElement(matrix, l - 1, a, l - 1);
    } else {
        return getCenteredElement(matrix, 1, i, 0) *
            getCenteredElement(matrix, l - 1, a, b);
    }
}


/**
 * The functions U, V, and W should only be called if the correspondingly
 * named coefficient u, v, w from the function ComputeUVWCoeff() is non-zero.
 * When the coefficient is 0, these would attempt to access matrix elements that
 * are out of bounds. The vector of rotations, |r|, must have the |l - 1|
 * previously completed band rotations. These functions are valid for |l >= 2|.
 * @param matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param m
 * @param n
 * @param l
 */
function getU(matrix: GainNode[][], m: number, n: number, l: number): number {
    // Although [1, 2] split U into three cases for m == 0, m < 0, m > 0
    // the actual values are the same for all three cases.
    return getP(matrix, 0, m, n, l);
}


/**
 * The functions U, V, and W should only be called if the correspondingly
 * named coefficient u, v, w from the function ComputeUVWCoeff() is non-zero.
 * When the coefficient is 0, these would attempt to access matrix elements that
 * are out of bounds. The vector of rotations, |r|, must have the |l - 1|
 * previously completed band rotations. These functions are valid for |l >= 2|.
 * @param matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param m
 * @param n
 * @param l
 */
function getV(matrix: GainNode[][], m: number, n: number, l: number): number {
    if (m === 0) {
        return getP(matrix, 1, 1, n, l) + getP(matrix, -1, -1, n, l);
    } else if (m > 0) {
        const d = getKroneckerDelta(m, 1);
        return getP(matrix, 1, m - 1, n, l) * Math.sqrt(1 + d) -
            getP(matrix, -1, -m + 1, n, l) * (1 - d);
    } else {
        // Note there is apparent errata in [1,2,2b] dealing with this particular
        // case. [2b] writes it should be P*(1-d)+P*(1-d)^0.5
        // [1] writes it as P*(1+d)+P*(1-d)^0.5, but going through the math by hand,
        // you must have it as P*(1-d)+P*(1+d)^0.5 to form a 2^.5 term, which
        // parallels the case where m > 0.
        const d = getKroneckerDelta(m, -1);
        return getP(matrix, 1, m + 1, n, l) * (1 - d) +
            getP(matrix, -1, -m - 1, n, l) * Math.sqrt(1 + d);
    }
}


/**
 * The functions U, V, and W should only be called if the correspondingly
 * named coefficient u, v, w from the function ComputeUVWCoeff() is non-zero.
 * When the coefficient is 0, these would attempt to access matrix elements that
 * are out of bounds. The vector of rotations, |r|, must have the |l - 1|
 * previously completed band rotations. These functions are valid for |l >= 2|.
 * @param matrix N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 * @param m
 * @param n
 * @param l
 */
function getW(matrix: GainNode[][], m: number, n: number, l: number) {
    // Whenever this happens, w is also 0 so W can be anything.
    if (m === 0) {
        return 0;
    }

    return m > 0 ? getP(matrix, 1, m + 1, n, l) + getP(matrix, -1, -m - 1, n, l) :
        getP(matrix, 1, m - 1, n, l) - getP(matrix, -1, -m + 1, n, l);
}


/**
 * Calculates the coefficients applied to the U, V, and W functions. Because
 * their equations share many common terms they are computed simultaneously.
 * @param m
 * @param n
 * @param l
 * @return 3 coefficients for U, V and W functions.
 */
function computeUVWCoeff(m: number, n: number, l: number): [number, number, number] {
    const d = getKroneckerDelta(m, 0);
    const reciprocalDenominator =
        Math.abs(n) === l ? 1 / (2 * l * (2 * l - 1)) : 1 / ((l + n) * (l - n));

    return [
        Math.sqrt((l + m) * (l - m) * reciprocalDenominator),
        0.5 * (1 - 2 * d) * Math.sqrt((1 + d) *
            (l + Math.abs(m) - 1) *
            (l + Math.abs(m)) *
            reciprocalDenominator),
        -0.5 * (1 - d) * Math.sqrt((l - Math.abs(m) - 1) * (l - Math.abs(m))) *
        reciprocalDenominator,
    ];
}


/**
 * Calculates the (2l+1) x (2l+1) rotation matrix for the band l.
 * This uses the matrices computed for band 1 and band l-1 to compute the
 * matrix for band l. |rotations| must contain the previously computed l-1
 * rotation matrices.
 * This implementation comes from p. 5 (6346), Table 1 and 2 in [2] taking
 * into account the corrections from [2b].
 * @param matrix - N matrices of gainNodes, each with where
 * n=1,2,...,N.
 * @param l
 */
function computeBandRotation(matrix: GainNode[][], l: number) {
    // The lth band rotation matrix has rows and columns equal to the number of
    // coefficients within that band (-l <= m <= l implies 2l + 1 coefficients).
    for (let m = -l; m <= l; m++) {
        for (let n = -l; n <= l; n++) {
            const uvwCoefficients = computeUVWCoeff(m, n, l);

            // The functions U, V, W are only safe to call if the coefficients
            // u, v, w are not zero.
            if (Math.abs(uvwCoefficients[0]) > 0) {
                uvwCoefficients[0] *= getU(matrix, m, n, l);
            }
            if (Math.abs(uvwCoefficients[1]) > 0) {
                uvwCoefficients[1] *= getV(matrix, m, n, l);
            }
            if (Math.abs(uvwCoefficients[2]) > 0) {
                uvwCoefficients[2] *= getW(matrix, m, n, l);
            }

            setCenteredElement(
                matrix, l, m, n,
                uvwCoefficients[0] + uvwCoefficients[1] + uvwCoefficients[2]);
        }
    }
}


/**
 * Compute the HOA rotation matrix after setting the transform matrix.
 * @param {Array} matrix - N matrices of gainNodes, each with (2n+1) x (2n+1)
 * elements, where n=1,2,...,N.
 */
function computeHOAMatrices(matrix: GainNode[][]) {
    // We start by computing the 2nd-order matrix from the 1st-order matrix.
    for (let i = 2; i <= matrix.length; i++) {
        computeBandRotation(matrix, i);
    }
}


/**
 * Higher-order-ambisonic decoder based on gain node network. We expect
 * the order of the channels to conform to ACN ordering. Below are the helper
 * methods to compute SH rotation using recursion. The code uses maths described
 * in the following papers:
 *  [1] R. Green, "Spherical Harmonic Lighting: The Gritty Details", GDC 2003,
 *      http://www.research.scea.com/gdc2003/spherical-harmonic-lighting.pdf
 *  [2] J. Ivanic and K. Ruedenberg, "Rotation Matrices for Real
 *      Spherical Harmonics. Direct Determination by Recursion", J. Phys.
 *      Chem., vol. 100, no. 15, pp. 6342-6347, 1996.
 *      http://pubs.acs.org/doi/pdf/10.1021/jp953350u
 *  [2b] Corrections to initial publication:
 *       http://pubs.acs.org/doi/pdf/10.1021/jp9833350
 * @constructor
 * @param {AudioContext} context - Associated AudioContext.
 * @param {Number} ambisonicOrder - Ambisonic order.
 */
export class HOARotator implements ErsatzAudioNode {

    private readonly _splitter: ChannelSplitterNode;
    private readonly _merger: ChannelMergerNode;
    private readonly _gainNodeMatrix: GainNode[][];

    get input() { return this._splitter; }
    get output() { return this._merger; }

    readonly ambisonicOrder: number;

    constructor(name: string, context: BaseAudioContext, ambisonicOrder: number) {
        this.ambisonicOrder = ambisonicOrder;

        // We need to determine the number of channels K based on the ambisonic order
        // N where K = (N + 1)^2.
        const channelCount = (ambisonicOrder + 1) * (ambisonicOrder + 1);

        this._splitter = ChannelSplitter(`${name}-hoa-rotator-splitter`, context, { channelCount });
        this._merger = ChannelMerger(`${name}-hoa-rotator-splitter`, context, { channelCount });

        // Create a set of per-order rotation matrices using gain nodes.
        this._gainNodeMatrix = [];
        let orderOffset;
        let rows;
        let inputIndex;
        let outputIndex;
        let matrixIndex;
        for (let i = 1; i <= ambisonicOrder; i++) {
            // Each ambisonic order requires a separate (2l + 1) x (2l + 1) rotation
            // matrix. We compute the offset value as the first channel index of the
            // current order where
            //   k_last = l^2 + l + m,
            // and m = -l
            //   k_last = l^2
            orderOffset = i * i;

            // Uses row-major indexing.
            rows = (2 * i + 1);

            this._gainNodeMatrix[i - 1] = [];
            for (let j = 0; j < rows; j++) {
                inputIndex = orderOffset + j;
                for (let k = 0; k < rows; k++) {
                    outputIndex = orderOffset + k;
                    matrixIndex = j * rows + k;
                    this._gainNodeMatrix[i - 1][matrixIndex] = Gain(`${name}-hoa-rotator-gain-node-matrix-${i - 1}-${matrixIndex}`, context);
                    connect(this._splitter, [inputIndex, this._gainNodeMatrix[i - 1][matrixIndex]]);
                    connect(this._gainNodeMatrix[i - 1][matrixIndex], [0, outputIndex, this._merger]);
                }
            }
        }

        // W-channel is not involved in rotation, skip straight to ouput.
        connect(this._splitter, [0, 0, this._merger]);

        // Default Identity matrix.
        this.setRotationMatrix3(mat3.identity(mat3.create()));

        Object.seal(this);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            removeVertex(this._splitter);
            removeVertex(this._merger);
        }
    }


    /**
     * Updates the rotation matrix with 3x3 matrix.
     * @param {Number[]} mat - A 3x3 rotation matrix. (column-major)
     */
    setRotationMatrix3(mat: ReadonlyMat3) {
        this._gainNodeMatrix[0][0].gain.value = -mat[0];
        this._gainNodeMatrix[0][1].gain.value = mat[1];
        this._gainNodeMatrix[0][2].gain.value = -mat[2];
        this._gainNodeMatrix[0][3].gain.value = -mat[3];
        this._gainNodeMatrix[0][4].gain.value = mat[4];
        this._gainNodeMatrix[0][5].gain.value = -mat[5];
        this._gainNodeMatrix[0][6].gain.value = -mat[6];
        this._gainNodeMatrix[0][7].gain.value = mat[7];
        this._gainNodeMatrix[0][8].gain.value = -mat[8];
        computeHOAMatrices(this._gainNodeMatrix);
    }


    /**
     * Returns the current 3x3 rotation matrix.
     * @return {Number[]} - A 3x3 rotation matrix. (column-major)
     */
    getRotationMatrix3(dest: mat3) {
        dest[0] = -this._gainNodeMatrix[0][0].gain.value;
        dest[1] = this._gainNodeMatrix[0][1].gain.value;
        dest[2] = -this._gainNodeMatrix[0][2].gain.value;

        dest[3] = -this._gainNodeMatrix[0][4].gain.value;
        dest[4] = this._gainNodeMatrix[0][5].gain.value;
        dest[5] = -this._gainNodeMatrix[0][6].gain.value;

        dest[6] = -this._gainNodeMatrix[0][8].gain.value;
        dest[7] = this._gainNodeMatrix[0][9].gain.value;
        dest[8] = -this._gainNodeMatrix[0][10].gain.value;
    }


    /**
     * Updates the rotation matrix with 4x4 matrix.
     * @param {Number[]} mat - A 4x4 rotation matrix. (column-major)
     */
    setRotationMatrix4(mat: ReadonlyMat4) {
        this._gainNodeMatrix[0][0].gain.value = -mat[0];
        this._gainNodeMatrix[0][1].gain.value = mat[1];
        this._gainNodeMatrix[0][2].gain.value = -mat[2];

        this._gainNodeMatrix[0][3].gain.value = -mat[4];
        this._gainNodeMatrix[0][4].gain.value = mat[5];
        this._gainNodeMatrix[0][5].gain.value = -mat[6];

        this._gainNodeMatrix[0][6].gain.value = -mat[8];
        this._gainNodeMatrix[0][7].gain.value = mat[9];
        this._gainNodeMatrix[0][8].gain.value = -mat[10];
        computeHOAMatrices(this._gainNodeMatrix);
    };


    /**
     * Returns the current 4x4 rotation matrix.
     * @return {Number[]} - A 4x4 rotation matrix. (column-major)
     */
    getRotationMatrix4(dest: mat4) {
        dest[0] = -this._gainNodeMatrix[0][0].gain.value;
        dest[1] = this._gainNodeMatrix[0][1].gain.value;
        dest[2] = -this._gainNodeMatrix[0][2].gain.value;
        dest[3] = 0;
        dest[4] = -this._gainNodeMatrix[0][3].gain.value;
        dest[5] = this._gainNodeMatrix[0][4].gain.value;
        dest[6] = -this._gainNodeMatrix[0][5].gain.value;
        dest[7] = 0;
        dest[8] = -this._gainNodeMatrix[0][6].gain.value;
        dest[9] = this._gainNodeMatrix[0][7].gain.value;
        dest[10] = -this._gainNodeMatrix[0][8].gain.value;
        dest[11] = 0;
        dest[12] = 0;
        dest[13] = 0;
        dest[14] = 0;
        dest[15] = 0;
    }
}
