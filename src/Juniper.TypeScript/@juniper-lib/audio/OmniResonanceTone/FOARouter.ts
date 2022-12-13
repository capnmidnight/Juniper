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
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperChannelMergerNode } from "../context/JuniperChannelMergerNode";
import { JuniperChannelSplitterNode } from "../context/JuniperChannelSplitterNode";


type ChannelMapTypes =
    | "DEFAULT"
    | "SAFARI"
    | "FUMA";

export type ChannelMapValues = [number, number, number, number];

/**
 * Channel router for FOA stream.
 */
export class FOARouter extends BaseNodeCluster {
    private readonly _splitter: JuniperChannelSplitterNode;
    private readonly _merger: JuniperChannelMergerNode;
    private readonly _channelMap: ChannelMapValues;

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
     * @param context - Associated AudioContext.
     * @param channelMap - Routing destination array.
     */
    constructor(context: JuniperAudioContext, channelMap: ChannelMapValues) {

        const splitter = new JuniperChannelSplitterNode(context, { channelCount: 4 });
        const merger = new JuniperChannelMergerNode(context, { channelCount: 4 });
        super("foa-router", context, [splitter], [merger]);

        this._splitter = splitter;
        this._merger = merger;
        this.setChannelMap(channelMap || FOARouter.ChannelMap.get("DEFAULT"));

        Object.seal(this);
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
        this._splitter.disconnect();
        for (let i = 0; i < this._channelMap.length; ++i) {
            this._splitter
                .connect(this._merger, i, this._channelMap[i]);
        }
    }
}
