import { HalfPi } from "@juniper-lib/tslib/dist/math";
import { BufferAttribute, BufferGeometry, Vector3 } from "three";

type UV = [number, number];
type Pos = [number, number, number];
export type PosUV = [number, number, number, number, number];
type PosUVBounds = {
    pos: Pos;
    uv: UV;
    pUV: UV;
}

export type TrianglePosUV = [PosUV, PosUV, PosUV];
export type QuadPosUV = [PosUV, PosUV, PosUV, PosUV];
export type QuadPosUVBounds = {
    minUV: UV;
    deltaUV: UV;
    verts: [PosUVBounds, PosUVBounds, PosUVBounds, PosUVBounds];
}

interface ITrianglePosUVNormal {
    positions: [Pos, Pos, Pos];
    uvs: [UV, UV, UV];
    normal: Pos;
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

const A = new Vector3();
const B = new Vector3();
const C = new Vector3();

function normalizeTriangle(tria: TrianglePosUV): ITrianglePosUVNormal {
    const positions: [Pos, Pos, Pos] = [
        [tria[0][0], tria[0][1], tria[0][2]],
        [tria[1][0], tria[1][1], tria[1][2]],
        [tria[2][0], tria[2][1], tria[2][2]]
    ];
    const uvs: [UV, UV, UV] = [
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
    const positions = nFaces.map((f) => f.positions).flat(2);
    const uvs = nFaces.map((f) => f.uvs).flat(2);
    const normals = nFaces.flatMap((f) => f.normal);
    const geom = new BufferGeometry();

    geom.setAttribute("position", new BufferAttribute(new Float32Array(positions), 3, false));
    geom.setAttribute("uv", new BufferAttribute(new Float32Array(uvs), 2, false));
    geom.setAttribute("normal", new BufferAttribute(new Float32Array(normals), 3, true));

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

export function createEACGeometry(subDivs: number, ...quads: QuadPosUV[]) {
    let remappingQuads = mapEACSubdivision(quads);
    for (let i = 0; i < subDivs; ++i) {
        remappingQuads = subdivide(remappingQuads);
    }
    quads = unmapEACSubdivision(remappingQuads);
    const faces = normalizeQuads(quads);
    return createGeometry(faces);
}

function mapEACSubdivision(quads: QuadPosUV[]): QuadPosUVBounds[] {
    return quads.map((quad) => {
        let minU = Number.MAX_VALUE;
        let maxU = Number.MIN_VALUE;
        let minV = Number.MAX_VALUE;
        let maxV = Number.MIN_VALUE;
        for (const vert of quad) {
            const u = vert[3];
            const v = vert[4];
            minU = Math.min(minU, u);
            maxU = Math.max(maxU, u);
            minV = Math.min(minV, v);
            maxV = Math.max(maxV, v);
        }

        const minUV: UV = [minU, minV];
        const deltaUV: UV = [maxU - minU, maxV - minV];
        return {
            minUV,
            deltaUV,
            verts: [
                mapEACSubdivVert(minUV, deltaUV, quad[0]),
                mapEACSubdivVert(minUV, deltaUV, quad[1]),
                mapEACSubdivVert(minUV, deltaUV, quad[2]),
                mapEACSubdivVert(minUV, deltaUV, quad[3])
            ]
        };
    });
}

function mapEACSubdivVert(minUV: UV, deltaUV: UV, vert: PosUV): PosUVBounds {
    return {
        pos: [vert[0], vert[1], vert[2]],
        uv: [vert[3], vert[4]],
        pUV: [
            (vert[3] - minUV[0]) / deltaUV[0],
            (vert[4] - minUV[1]) / deltaUV[1]
        ]
    };
}

function unmapEACSubdivision(quadsx: QuadPosUVBounds[]): QuadPosUV[] {
    return quadsx.map((quadx) => [
        unmapEACSubdivVert(quadx, 0),
        unmapEACSubdivVert(quadx, 1),
        unmapEACSubdivVert(quadx, 2),
        unmapEACSubdivVert(quadx, 3)
    ]);
}

function unmapEACSubdivVert(quadx: QuadPosUVBounds, i: number): PosUV {
    const vert = quadx.verts[i];
    return [
        vert.pos[0],
        vert.pos[1],
        vert.pos[2],
        vert.uv[0],
        vert.uv[1],
    ];
}

function subdivide(quadsx: QuadPosUVBounds[]): QuadPosUVBounds[] {
    return quadsx.flatMap((quadx) => {
        const midU1 = midpoint(quadx, quadx.verts[0], quadx.verts[1]);
        const midU2 = midpoint(quadx, quadx.verts[2], quadx.verts[3]);
        const midV1 = midpoint(quadx, quadx.verts[0], quadx.verts[3]);
        const midV2 = midpoint(quadx, quadx.verts[1], quadx.verts[2]);
        const mid = midpoint(quadx, midU1, midU2);
        return [{
            minUV: quadx.minUV,
            deltaUV: quadx.deltaUV,
            verts: [quadx.verts[0], midU1, mid, midV1]
        }, {
            minUV: quadx.minUV,
            deltaUV: quadx.deltaUV,
            verts: [midU1, quadx.verts[1], midV2, mid]
        }, {
            minUV: quadx.minUV,
            deltaUV: quadx.deltaUV,
            verts: [mid, midV2, quadx.verts[2], midU2]
        }, {
            minUV: quadx.minUV,
            deltaUV: quadx.deltaUV,
            verts: [midV1, mid, midU2, quadx.verts[3]]
        }];
    });
}

function midpoint(quadx: QuadPosUVBounds, from: PosUVBounds, to: PosUVBounds): PosUVBounds {
    const dx = to.pos[0] - from.pos[0];
    const dy = to.pos[1] - from.pos[1];
    const dz = to.pos[2] - from.pos[2];
    const x = from.pos[0] + 0.5 * dx;
    const y = from.pos[1] + 0.5 * dy;
    const z = from.pos[2] + 0.5 * dz;

    const dpu = to.pUV[0] - from.pUV[0];
    const dpv = to.pUV[1] - from.pUV[1];
    const pu = from.pUV[0] + 0.5 * dpu;
    const pv = from.pUV[1] + 0.5 * dpv;
    const mu = mapEACUV(pu - 0.5) + 0.5;
    const mv = mapEACUV(pv - 0.5) + 0.5;
    const u = mu * quadx.deltaUV[0] + quadx.minUV[0];
    const v = mv * quadx.deltaUV[1] + quadx.minUV[1];
    
    return {
        pos: [x, y, z],
        pUV: [pu, pv],
        uv: [u, v]
    };
}

function mapEACUV(uv: number): number {
    return Math.atan(2 * uv) / HalfPi;
}
