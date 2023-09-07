import { BaseGraphDialog } from "@juniper-lib/graphics2d/BaseGraphDialog";
import { Object3D } from "three";
import type { BaseEnvironment } from "./BaseEnvironment";
export declare class SceneGraphDialog extends BaseGraphDialog<Object3D> {
    private readonly env;
    constructor(env: BaseEnvironment<unknown>);
    onShown(): void;
}
//# sourceMappingURL=SceneGraphDialog.d.ts.map