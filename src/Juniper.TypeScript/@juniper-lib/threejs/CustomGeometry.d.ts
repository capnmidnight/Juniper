import { BufferGeometry } from "three";
type UV = [number, number];
type Pos = [number, number, number];
export type PosUV = [number, number, number, number, number];
type PosUVBounds = {
    pos: Pos;
    uv: UV;
    pUV: UV;
};
export type TrianglePosUV = [PosUV, PosUV, PosUV];
export type QuadPosUV = [PosUV, PosUV, PosUV, PosUV];
export type QuadPosUVBounds = {
    minUV: UV;
    deltaUV: UV;
    verts: [PosUVBounds, PosUVBounds, PosUVBounds, PosUVBounds];
};
export declare function createTriGeometry(...trias: TrianglePosUV[]): BufferGeometry;
export declare function createQuadGeometry(...quads: QuadPosUV[]): BufferGeometry;
export declare function createEACGeometry(subDivs: number, ...quads: QuadPosUV[]): BufferGeometry;
export {};
//# sourceMappingURL=CustomGeometry.d.ts.map