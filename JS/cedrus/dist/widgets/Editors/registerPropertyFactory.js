import { registerFactory } from "@juniper-lib/dom";
export function registerPropertyFactory(storageType, dataType, PropertyEditorElement) {
    const tagName = `${storageType}-${dataType}-editor`.toLowerCase();
    return registerFactory(tagName, PropertyEditorElement);
}
//# sourceMappingURL=registerPropertyFactory.js.map