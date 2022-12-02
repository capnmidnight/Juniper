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
 * @file Static data manager for HRIR list and URLs. For Omnitone's HRIR list
 * structure, see src/resources/README.md for the detail.
 * @author Andrew Allen <bitllama@google.com>
 * 
 * Conversion to TypeScript, modernization, and miscellaneous defect fixes by:
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */


const HRIRList = [
    // Zero-order ambisonic. Not supported. (0 files, 1 channel)
    null,
    // First-order ambisonic. (2 files, 4 channels)
    ['omnitone-foa-1.wav', 'omnitone-foa-2.wav'],
    // Second-order ambisonic. (5 files, 9 channels)
    [
        'omnitone-soa-1.wav', 'omnitone-soa-2.wav', 'omnitone-soa-3.wav',
        'omnitone-soa-4.wav', 'omnitone-soa-5.wav',
    ],
    // Third-order ambisonic. (8 files, 16 channels)
    [
        'omnitone-toa-1.wav', 'omnitone-toa-2.wav', 'omnitone-toa-3.wav',
        'omnitone-toa-4.wav', 'omnitone-toa-5.wav', 'omnitone-toa-6.wav',
        'omnitone-toa-7.wav', 'omnitone-toa-8.wav',
    ],
];


// Base URL.
const SourceURL = {
    GITHUB:
        'https://cdn.rawgit.com/GoogleChrome/omnitone/master/build/resources/',
};


export interface PathListDescription {
    source: string;
    ambisonicOrder: number;
}

/**
 * [getPathSet description]
 * @param {object} [setting] - Setting object.
 * @param {string} [setting.source="github"] - The base location for the HRIR
 * set.
 * @param {number} [setting.ambisonicOrder=1] - Requested ambisonic order.
 * @return {string[]} pathList - HRIR path set (2~8 URLs)
 */
export function getPathList(setting: PathListDescription) {
    let filenames: string[];
    let staticPath: string;
    let pathList: string[];

    const setting_ = setting || { ambisonicOrder: 1, source: 'github' };

    switch (setting_.ambisonicOrder) {
        case 1:
        case 2:
        case 3:
            filenames = HRIRList[setting_.ambisonicOrder];
            break;
        default:
            // Invalid order gets the null path list.
            filenames = HRIRList[0];
            break;
    }

    switch (setting_.source) {
        case 'github':
            staticPath = SourceURL.GITHUB;
            break;
        default:
            // By default, use GitHub's CDN.
            staticPath = setting_.source;
            break;
    }

    if (filenames) {
        pathList = [];
        filenames.forEach(function (filename) {
            pathList.push(staticPath + filename);
        });
    }

    return pathList;
};
