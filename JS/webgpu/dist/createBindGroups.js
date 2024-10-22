export function createBindGroups(device, pipeline, ...entrieses) {
    return entrieses.map((entries, group) => createBindGroup(device, pipeline, group, ...entries));
}
export function createBindGroup(device, pipeline, group, ...entries) {
    return device.createBindGroup({
        label: `Group ${group}`,
        layout: pipeline.getBindGroupLayout(group),
        entries: entries.map((entry, binding) => createBindGroupEntry(binding, entry.label, entry.buffer))
    });
}
export function createBindGroupEntry(binding, label, buffer) {
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
//# sourceMappingURL=createBindGroups.js.map