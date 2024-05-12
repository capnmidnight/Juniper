export function createBindGroups(device: GPUDevice, pipeline: GPUPipelineBase, ...entrieses: { label: string; buffer: GPUBuffer; }[][]): GPUBindGroup[] {
    return entrieses.map((entries, group) => createBindGroup(device, pipeline, group, ...entries));
}

export function createBindGroup(device: GPUDevice, pipeline: GPUPipelineBase, group: number, ...entries: { label: string; buffer: GPUBuffer; }[]): GPUBindGroup {
    return device.createBindGroup({
        label: `Group ${group}`,
        layout: pipeline.getBindGroupLayout(group),
        entries: entries.map((entry, binding) =>
            createBindGroupEntry(
                binding,
                entry.label,
                entry.buffer
            )
        )
    });
}

export function createBindGroupEntry(binding: number, label: string, buffer: GPUBuffer): GPUBindGroupEntry {
    return {
        binding,
        resource: {
            label,
            buffer,
            offset: 0,
            size: buffer.size
        }
    };
}
