export class Wireframe extends Mesh<import("three").BufferGeometry, import("three").Material | import("three").Material[]> {
    constructor(geometry?: LineSegmentsGeometry, material?: LineMaterial);
    type: string;
    computeLineDistances(): this;
    isWireframe: boolean;
}
import { Mesh } from 'three';
import { LineSegmentsGeometry } from './LineSegmentsGeometry';
import { LineMaterial } from './LineMaterial';
//# sourceMappingURL=Wireframe.d.ts.map