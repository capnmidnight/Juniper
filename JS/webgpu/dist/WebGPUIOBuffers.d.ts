/**
 * Creates a pipeline of buffers that can be used to write a block
 * of data to the GPU, modify it on the GPU, then read it back again
 * and replace the existing data on the CPU.
 */
export declare class WebGPUIOBuffers {
    #private;
    /**
     * Creates a pipeline for writing, modifying, and reading back
     * data from the GPU.
     *
     * @param device the GPU device on which the modification will take place.
     * @param writeBuffer the data to read/write/copy
     * @param labelPrefix a label prefix to apply to all of the generated WGPU types, to help differentiate them from others in the debugging view.
     */
    constructor(device: GPUDevice, writeBuffer: Uint8Array, labelPrefix: string);
    /**
     * Creates a single bind-group dedicated to this data pipeline.
     * Your shader should expect to not use this numbered group for
     * any other purpose.
     * @param group the group index
     * @param pipeline the command pipeline for which to create the bindgroup
     * @returns
     */
    createBindGroup(group: number, pipeline: GPUPipelineBase): GPUBindGroup;
    /**
     * Copy the data from CPU -> GPU
     */
    write(): void;
    /**
     * Copy the data from GPU -> CPU (temp)
     * @param commandEncoder
     */
    copy(commandEncoder: GPUCommandEncoder): void;
    /**
     * Replace the existing CPU data with the recently read GPU data from the CPU (temp) buffer.
     */
    read(): Promise<void>;
}
//# sourceMappingURL=WebGPUIOBuffers.d.ts.map