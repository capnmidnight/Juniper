/**
 * @license
 * Copyright 2022 Sean T. McBeth. All Rights Reserved.
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
 * @file An interface describing the commonalities between FOARenderer
 * and HOARenderer.
 * 
 * @author Sean T. McBeth <sean.mcbeth+gh@gmail.com>
 */

import { ReadonlyMat3, ReadonlyMat4 } from "gl-matrix";
import { IAudioNode } from "../IAudioNode";

export interface IRenderer extends IAudioNode {
    readonly rotator: IAudioNode;
    initialize(): Promise<void>;
    setRotationMatrix3(rotationMatrix3: ReadonlyMat3): void;
    setRotationMatrix4(rotationMatrix4: ReadonlyMat4): void;
}
