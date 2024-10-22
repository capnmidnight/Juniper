import { Exception } from "@juniper-lib/tslib/dist/Exception";

export async function checkMessages(location: string, shader: GPUShaderModule) {
    const compInfo = await shader.getCompilationInfo();
    const infos = compInfo.messages.filter(m => m.type === "info");
    const warnings = compInfo.messages.filter(m => m.type === "warning");
    const errors = compInfo.messages.filter(m => m.type === "error");

    for (const info of infos) {
        console.info(info);
    }

    for (const warning of warnings) {
        console.warn(warning);
    }

    if (errors.length > 0) {
        throw new Exception(`Shader compilation error (${location})`, errors);
    }
}
