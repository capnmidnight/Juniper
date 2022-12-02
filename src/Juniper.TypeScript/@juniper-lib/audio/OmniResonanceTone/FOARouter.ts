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
 * @file An audio channel router to resolve different channel layouts between
 * browsers.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


import { arrayReplace } from "@juniper-lib/tslib/collections/arrays";
import {
    ChannelMerger,
    ChannelSplitter
} from "../nodes";
import {
    connect,
    disconnect,
    ErsatzAudioNode,
    removeVertex
} from "../util";


type ChannelMapTypes =
    | "DEFAULT"
    | "SAFARI"
    | "FUMA";

export type ChannelMapValues = [number, number, number, number];

/**
 * Channel router for FOA stream.
 */
export class FOARouter implements ErsatzAudioNode {
    private readonly _splitter: ChannelSplitterNode;
    private readonly _merger: ChannelMergerNode;
    private readonly _channelMap: ChannelMapValues;

    get input() { return this._splitter; }
    get output() { return this._merger; }

    /**
     * Static channel map ENUM.
     * @static
     * @type {ChannelMap}
     */
    public static readonly ChannelMap: Map<ChannelMapTypes, ChannelMapValues> = new Map([
        /** @type {Number[]} - ACN channel map for Chrome and FireFox. (FFMPEG) */
        ["DEFAULT", [0, 1, 2, 3]],
        /** @type {Number[]} - Safari's 4-channel map for AAC codec. */
        ["SAFARI", [2, 0, 1, 3]],
        /** @type {Number[]} - ACN > FuMa conversion map. */
        ["FUMA", [0, 3, 1, 2]],
    ]);

    /**
     * Channel router for FOA stream.
     * @param name
     * @param context - Associated AudioContext.
     * @param channelMap - Routing destination array.
     */
    constructor(name: string, context: BaseAudioContext, channelMap: ChannelMapValues) {

        this._splitter = ChannelSplitter(`${name}-foa-router-splitter`, context, { channelCount: 4 });
        this._merger = ChannelMerger(`${name}-foa-router-merger`, context, { channelCount: 4 });

        this.setChannelMap(channelMap || FOARouter.ChannelMap.get("DEFAULT"));

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
     * Sets channel map.
     * @param channelMap - A new channel map for FOA stream.
     */
    setChannelMap(channelMap: ChannelMapValues) {
        if (!Array.isArray(channelMap)) {
            return;
        }
        arrayReplace(this._channelMap, ...channelMap);
        disconnect(this._splitter);
        connect(this._splitter, [0, this._channelMap[0], this._merger]);
        connect(this._splitter, [1, this._channelMap[1], this._merger]);
        connect(this._splitter, [2, this._channelMap[2], this._merger]);
        connect(this._splitter, [3, this._channelMap[3], this._merger]);
    }
}
