export class XREstimatedLight extends Group {
    constructor(renderer: any, environmentEstimation?: boolean);
    lightProbe: LightProbe;
    directionalLight: DirectionalLight;
    environment: any;
    dispose: () => void;
}
import { Group } from "three";
import { LightProbe } from "three";
import { DirectionalLight } from "three";
//# sourceMappingURL=XREstimatedLight.d.ts.map