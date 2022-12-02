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
 * @file Streamlined AudioBuffer loader.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { unwrapResponse } from "@juniper-lib/fetcher/unwrapResponse";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { isAudioContext } from "../util";
import * as Utils from "./utils";

/**
 * @typedef {string} BufferDataType
 */

/**
 * Buffer data type for ENUM.
 * @enum {BufferDataType}
 */
export type BufferDataType =
    | "base64"
    | "url";

interface BufferListBase64Options {
    dataType: "base64";
    verbose?: boolean;
}

interface BufferListURLOptions {
    dataType: "url";
    fetcher: IFetcher;
    verbose?: boolean;
}

export type BufferListOptions =
    | BufferListBase64Options
    | BufferListURLOptions;

function isBufferListURLOptions(obj: BufferListOptions): obj is BufferListURLOptions {
    return isDefined(obj)
        && obj.dataType === "url";
}


/**
 * BufferList object mananges the async loading/decoding of multiple
 * AudioBuffers from multiple URLs.
 */
export class BufferList {

    private readonly _context: BaseAudioContext;

    private readonly _bufferData: string[];

    private readonly _dataType: BufferDataType = "base64";
    private readonly _verbose: boolean = true;
    private readonly _fetcher: IFetcher = null;

    /**
     * BufferList object mananges the async loading/decoding of multiple
     * AudioBuffers from multiple URLs.
     * @param context - Associated BaseAudioContext.
     * @param bufferData - An ordered list of URLs.
     * @param options - Options
     */
    constructor(context: BaseAudioContext, bufferData: string[], options?: BufferListOptions) {
        if (!isAudioContext(context)) {
            throw Utils.formatError("BufferList: Invalid BaseAudioContext.");
        }

        this._context = context;

        if (isDefined(options)) {
            if (isDefined(options.dataType)) {
                this._dataType = options.dataType;
            }
            if (isDefined(options.verbose)) {
                this._verbose = Boolean(options.verbose);
            }
            if (isBufferListURLOptions(options)) {
                this._fetcher = options.fetcher;
            }
        }

        if (this._dataType === "url"
            && isNullOrUndefined(this._fetcher)) {
            throw Utils.formatError("No data fetcher provided!");
        }

        this._bufferData = this._dataType === "base64"
            ? bufferData
            : bufferData.slice(0);

        Object.seal(this);
    }


    /**
     * Starts AudioBuffer loading tasks.
     * @return The promise resolves with an array of AudioBuffer.
     */
    async load() {
        const data = await Promise.all(this._bufferData.map((v, i) =>
            this._dataType === "base64"
                ? this._launchAsyncLoadTask(v, i)
                : this._launchAsyncLoadTaskXHR(v, i)));

        if (this._verbose) {
            const messageString = this._dataType === "base64"
                ? this._bufferData.length + " AudioBuffers from Base64-encoded HRIRs"
                : this._bufferData.length + " files via XHR";
            Utils.log(`BufferList: ${messageString} loaded successfully.`);
        }

        return data;
    }


    /**
     * Run async loading task for Base64-encoded string.
     * @private
     * @param taskId Task ID number from the ordered list |bufferData|.
     */
    private async _launchAsyncLoadTask(v: string, taskId: number) {
        try {
            const audioBuffer = await this._context.decodeAudioData(
                Utils.getArrayBufferFromBase64String(v));
            this._updateProgress(v, taskId);
            return audioBuffer;
        }
        catch (exp) {
            this._updateProgress(v, taskId);
            const message = `BufferList: decoding ArrayByffer("${taskId}" from Base64-encoded data failed. (${exp})`;
            Utils.log(message);
            throw Utils.formatError(message);
        }
    }


    /**
     * Run async loading task via XHR for audio file URLs.
     * @private
     * @param taskId Task ID number from the ordered list |bufferData|.
     */
    private async _launchAsyncLoadTaskXHR(v: string, taskId: number) {
        try {
            const audioBuffer = await this._fetcher
                .get(v)
                .audioBuffer(this._context)
                .then(unwrapResponse);
            this._updateProgress(v, taskId);
            return audioBuffer;
        }
        catch (exp) {
            this._updateProgress(v, taskId);
            const message = `BufferList: decoding "${v}" failed. (${exp})`;
            Utils.log(message);
            throw Utils.formatError(message);
        }
    }


    /**
     * Updates the overall progress on loading tasks.
     * @param taskId Task ID number.
     * @param audioBuffer Decoded AudioBuffer object.
     */
    private _updateProgress(v: string, taskId: number) {
        if (this._verbose) {
            const messageString = this._dataType === "base64"
                ? `ArrayBuffer(${taskId}) from Base64-encoded HRIR`
                : `"${v}"`;
            Utils.log(`BufferList: ${messageString} successfully loaded.`);
        }
    }
}