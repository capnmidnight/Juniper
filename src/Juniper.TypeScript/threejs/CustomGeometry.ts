type v2 = [number, number];
type v3 = [number, number, number];
type v5 = [number, number, number, number, number];

export type TrianglePosUV = [v5, v5, v5];
export type QuadPosUV = [v5, v5, v5, v5];

interface ITrianglePosUVNormal {
    positions: [v3, v3, v3];
    uvs: [v2, v2, v2];
    normal: v3;
}

function normalizeTriangles(trias: TrianglePosUV[]): ITrianglePosUVNormal[] {
    return trias.map(normalizeTriangle);
}

function normalizeQuad(quad: QuadPosUV): [ITrianglePosUVNormal, ITrianglePosUVNormal] {
    return [
        normalizeTriangle([quad[1], quad[0], quad[2]]),
        normalizeTriangle([quad[2], quad[0], quad[3]])
    ];
}

function normalizeQuads(quads: QuadPosUV[]): ITrianglePosUVNormal[] {
    return quads.map(normalizeQuad).flat();
}

const A = new THREE.Vector3();
const B = new THREE.Vector3();
const C = new THREE.Vector3();

function normalizeTriangle(tria: TrianglePosUV): ITrianglePosUVNormal {
    const positions: [v3, v3, v3] = [
        [tria[0][0], tria[0][1], tria[0][2]],
        [tria[1][0], tria[1][1], tria[1][2]],
        [tria[2][0], tria[2][1], tria[2][2]]
    ];
    const uvs: [v2, v2, v2] = [
        [tria[0][3], tria[0][4]],
        [tria[1][3], tria[1][4]],
        [tria[2][3], tria[2][4]]
    ];

    C.fromArray(positions[0]);
    A.fromArray(positions[1]).sub(C);
    C.fromArray(positions[2]);
    B.fromArray(positions[1]).sub(C);
    A.cross(B);
    const normal = A.toArray();

    return {
        positions,
        uvs,
        normal
    };
}

function createGeometry(nFaces: ITrianglePosUVNormal[]) {
    const positions = nFaces.map(f => f.positions).flat(2);
    const uvs = nFaces.map(f => f.uvs).flat(2);
    const normals = nFaces.flatMap(f => f.normal);
    const geom = new THREE.BufferGeometry();

    geom.setAttribute("position", new THREE.BufferAttribute(new Float32Array(positions), 3, false));
    geom.setAttribute("uv", new THREE.BufferAttribute(new Float32Array(uvs), 2, false));
    geom.setAttribute("normal", new THREE.BufferAttribute(new Float32Array(normals), 3, true));

    return geom;
}

export function createTriGeometry(...trias: TrianglePosUV[]) {
    const nFaces = normalizeTriangles(trias);
    return createGeometry(nFaces);
}

export function createQuadGeometry(...quads: QuadPosUV[]) {
    const nFaces = normalizeQuads(quads);
    return createGeometry(nFaces);
}