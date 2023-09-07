export declare enum VertexComponent {
    Position = 0,
    UV = 1,
    Color = 2,
    Alpha = 3,
    Metallic = 4,
    Roughness = 5,
    Normal = 6,
    Occlusion = 7,
    Emissive = 8
}
export interface VertexComponentDesc {
    size: number;
    normalized: boolean;
}
export type VertexComponentLayout = [VertexComponent, VertexComponentDesc];
export interface GeometryDescription {
    type: string;
    layout: readonly VertexComponentLayout[];
    verts: Float32Array;
    indices: Uint8Array | Uint16Array;
}
//# sourceMappingURL=GeometryDescription.d.ts.map