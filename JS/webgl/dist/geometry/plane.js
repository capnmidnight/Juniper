import { VertexComponent } from "./GeometryDescription";
const layout = [
    [VertexComponent.Position, { size: 3, normalized: false }],
    [VertexComponent.UV, { size: 2, normalized: true }]
];
const verts = new Float32Array([
    //           vertices    UVs
    /* 0 TLF */ -1, 1, -1, 0, 0,
    /* 1 TRF */ 1, 1, -1, 1, 0,
    /* 2 BLF */ -1, -1, -1, 0, 1,
    /* 3 BRF */ 1, -1, -1, 1, 1
]);
export const invPlane = {
    type: "TRIANGLES",
    layout,
    verts,
    indices: new Uint8Array([
        /* F */ 0, 2, 1,
        /* F */ 1, 2, 3
    ])
};
export const plane = {
    type: "TRIANGLES",
    layout,
    verts,
    indices: new Uint8Array([
        /* F */ 0, 1, 2,
        /* F */ 2, 1, 3
    ])
};
//# sourceMappingURL=plane.js.map