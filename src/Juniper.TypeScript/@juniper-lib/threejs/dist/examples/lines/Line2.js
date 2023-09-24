import { LineSegments2 } from './LineSegments2';
import { LineGeometry } from './LineGeometry';
import { LineMaterial } from './LineMaterial';
class Line2 extends LineSegments2 {
    constructor(geometry = new LineGeometry(), material = new LineMaterial({ color: Math.random() * 0xffffff })) {
        super(geometry, material);
        this.type = 'Line2';
    }
}
Line2.prototype.isLine2 = true;
export { Line2 };
//# sourceMappingURL=Line2.js.map