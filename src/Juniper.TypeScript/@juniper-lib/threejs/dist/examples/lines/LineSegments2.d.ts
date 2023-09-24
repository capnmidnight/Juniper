export class LineSegments2 extends Mesh<import("three").BufferGeometry, import("three").Material | import("three").Material[]> {
    constructor(geometry?: LineSegmentsGeometry, material?: LineMaterial);
    type: string;
    computeLineDistances(): this;
    raycast(raycaster: any, intersects: any): void;
    isLineSegments2: boolean;
}
import { Mesh } from 'three';
import { LineSegmentsGeometry } from './LineSegmentsGeometry';
import { LineMaterial } from './LineMaterial';
//# sourceMappingURL=LineSegments2.d.ts.map