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
    const faces = normalizeTriangles(trias);
    return createGeometry(faces);
}

export function createQuadGeometry(...quads: QuadPosUV[]) {
    const faces = normalizeQuads(quads);
    return createGeometry(faces);
}

export function mapEACUV(uv: number): number {
    return 2 * Math.atan(2 * uv) / Math.PI;
}

function midpoint(from: v5, to: v5): v5 {
    const delta = new Array<number>(5);
    for (let i = 0; i < delta.length; ++i) {
        delta[i] = to[i] - from[i];
    }

    return from.map((v, i) => v + 0.5 * delta[i]) as v5;
}

export function createEACGeometry(subDivs: number, ...quads: QuadPosUV[]) {
    for (let i = 0; i < subDivs; ++i) {
        quads = subdivide(quads);
    }
    const faces = normalizeQuads(quads);
    for (const face of faces) {
        for (const uv of face.uvs) {
            uv[0] = mapEACUV(uv[0]);
            uv[1] = mapEACUV(uv[1]);
        }
    }
    return createGeometry(faces);
}

function subdivide(quads: QuadPosUV[]) {
    const quads2 = new Array<QuadPosUV>(quads.length * 4);
    for (let i = 0; i < quads.length; ++i) {
        const quad = quads[i];
        const midU1 = midpoint(quad[0], quad[1]);
        const midU2 = midpoint(quad[2], quad[3]);
        const midV1 = midpoint(quad[0], quad[3]);
        const midV2 = midpoint(quad[1], quad[2]);
        const mid = midpoint(midU1, midU2);
        quads2[i * 4 + 0] = [quad[0], midU1, mid, midV1];
        quads2[i * 4 + 1] = [midU1, quad[1], midV2, mid];
        quads2[i * 4 + 2] = [mid, midV2, quad[2], midU2];
        quads2[i * 4 + 3] = [midV1, mid, midU2, quad[3]];
    }
    return quads2;
}
