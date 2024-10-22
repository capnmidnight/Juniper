import { Div } from "@juniper-lib/dom";
export function editorNotImplemented(dataType, storageType) {
    return () => {
        const message = `Editing of ${storageType} ${dataType} has not been implemented.`;
        console.warn(message);
        return Div(message);
    };
}
export function editorNotSupported(dataType, storageType) {
    return () => {
        throw new Error(`Editing of ${storageType} ${dataType} will not been implemented.`);
    };
}
//# sourceMappingURL=editorNotImplemented.js.map