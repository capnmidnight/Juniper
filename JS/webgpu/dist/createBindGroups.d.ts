export declare function createBindGroups(device: GPUDevice, pipeline: GPUPipelineBase, ...entrieses: {
    label: string;
    buffer: GPUBuffer;
}[][]): GPUBindGroup[];
export declare function createBindGroup(device: GPUDevice, pipeline: GPUPipelineBase, group: number, ...entries: {
    label: string;
    buffer: GPUBuffer;
}[]): GPUBindGroup;
export declare function createBindGroupEntry(binding: number, label: string, buffer: GPUBuffer): GPUBindGroupEntry;
//# sourceMappingURL=createBindGroups.d.ts.map