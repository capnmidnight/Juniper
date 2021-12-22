import { LineSegmentsGeometry } from './LineSegmentsGeometry';

class WireframeGeometry2 extends LineSegmentsGeometry {

	constructor( geometry ) {

		super();

		this.type = 'WireframeGeometry2';

		this.fromWireframeGeometry( new THREE.WireframeGeometry( geometry ) );

		// set colors, maybe

	}

}

WireframeGeometry2.prototype.isWireframeGeometry2 = true;

export { WireframeGeometry2 };
