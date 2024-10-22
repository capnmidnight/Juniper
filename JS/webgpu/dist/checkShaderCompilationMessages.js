import { Exception } from "@juniper-lib/util";
export async function checkShaderCompilationMessages(location, shader) {
    const compInfo = await shader.getCompilationInfo();
    const infos = compInfo.messages.filter(m => m.type === "info");
    const warnings = compInfo.messages.filter(m => m.type === "warning");
    const errors = compInfo.messages.filter(m => m.type === "error");
    for (const info of infos) {
        console.info(location, info);
    }
    for (const warning of warnings) {
        console.warn(location, warning);
    }
    if (errors.length > 0) {
        console.error(...errors);
        throw new Exception(`Shader Compilation Error (${location}): `, errors);
    }
}
//# sourceMappingURL=checkShaderCompilationMessages.js.map