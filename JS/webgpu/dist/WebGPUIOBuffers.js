import { createBindGroupEntry } from "./createBindGroups";
/**
 * Creates a pipeline of buffers that can be used to write a block
 * of data to the GPU, modify it on the GPU, then read it back again
 * and replace the existing data on the CPU.
 */
export class WebGPUIOBuffers {
    #device;
    #labelPrefix;
    #writeData;
    #writeBuffer;
    #copyBuffer;
    #readBuffer;
    /**
     * Creates a pipeline for writing, modifying, and reading back
     * data from the GPU.
     *
     * @param device the GPU device on which the modification will take place.
     * @param writeBuffer the data to read/write/copy
     * @param labelPrefix a label prefix to apply to all of the generated WGPU types, to help differentiate them from others in the debugging view.
     */
    constructor(device, writeBuffer, labelPrefix) {
        this.#device = device;
        this.#labelPrefix = labelPrefix;
        this.#writeData = writeBuffer;
        this.#writeBuffer = device.createBuffer({
            label: this.#labelPrefix + "In",
            size: writeBuffer.byteLength,
            usage: GPUBufferUsage.STORAGE | GPUBufferUsage.COPY_DST
        });
        this.#copyBuffer = device.createBuffer({
            label: this.#labelPrefix + "Copy",
            size: writeBuffer.byteLength,
            usage: GPUBufferUsage.MAP_READ | GPUBufferUsage.COPY_DST
        });
        this.#readBuffer = device.createBuffer({
            label: this.#labelPrefix + "Out",
            size: writeBuffer.byteLength,
            usage: GPUBufferUsage.STORAGE | GPUBufferUsage.COPY_SRC
        });
    }
    #createBindGroupEntry(binding, buffer) {
        return createBindGroupEntry(binding, buffer.label + "Binding", buffer);
    }
    /**
     * Creates a single bind-group dedicated to this data pipeline.
     * Your shader should expect to not use this numbered group for
     * any other purpose.
     * @param group the group index
     * @param pipeline the command pipeline for which to create the bindgroup
     * @returns
     */
    createBindGroup(group, pipeline) {
        return this.#device.createBindGroup({
            label: this.#labelPrefix + "Group",
            layout: pipeline.getBindGroupLayout(group),
            entries: [
                this.#createBindGroupEntry(0, this.#writeBuffer),
                this.#createBindGroupEntry(1, this.#readBuffer)
            ]
        });
    }
    /**
     * Copy the data from CPU -> GPU
     */
    write() {
        this.#device.queue.writeBuffer(this.#writeBuffer, 0, this.#writeData);
    }
    /**
     * Copy the data from GPU -> CPU (temp)
     * @param commandEncoder
     */
    copy(commandEncoder) {
        commandEncoder.copyBufferToBuffer(this.#readBuffer, 0, this.#copyBuffer, 0, this.#readBuffer.size);
    }
    /**
     * Replace the existing CPU data with the recently read GPU data from the CPU (temp) buffer.
     */
    async read() {
        await this.#copyBuffer.mapAsync(GPUMapMode.READ);
        const gpuToCpuArrayBuffer = this.#copyBuffer.getMappedRange();
        const dump = new Uint8Array(gpuToCpuArrayBuffer);
        this.#writeData.set(dump);
        this.#copyBuffer.unmap();
    }
}
//# sourceMappingURL=WebGPUIOBuffers.js.map