export class AnaglyphEffect {
    constructor(renderer: any, width?: number, height?: number);
    colorMatrixLeft: Matrix3;
    colorMatrixRight: Matrix3;
    setSize: (width: any, height: any) => void;
    render: (scene: any, camera: any) => void;
    dispose: () => void;
}
import { Matrix3 } from 'three';
//# sourceMappingURL=AnaglyphEffect.d.ts.map