export enum VertexComponent {
    Position,
    UV,
    Color,
    Alpha,
    Metallic,
    Roughness,
    Normal,
    Occlusion,
    Emissive
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
