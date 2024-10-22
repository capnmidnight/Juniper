import { arrayReplace, arrayScan, compareBy, groupBy, makeLookup, mapReplace, singleton } from "@juniper-lib/util";
import { TypedEventTarget } from "@juniper-lib/events";
import { UpdatedEvent } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { ENTITY_TYPE } from "../../models";
export const referencePropertyTypes = [
    "ReferenceFile",
    "ReferenceLink"
];
export function toReferenceModel(entity, properties) {
    const propsByType = makeLookup(properties, p => p.type.name);
    const type = arrayScan(referencePropertyTypes, p => propsByType.has(p));
    return {
        id: entity.id,
        name: propsByType.get("Name")?.value,
        authors: propsByType.get("Authors")?.value,
        date: propsByType.get("Publication Date")?.value,
        type,
        value: propsByType.get(type)?.value
    };
}
export class ReferenceDataManager extends TypedEventTarget {
    static get instance() { return singleton("ReferenceEditorElement::ReferenceDataManager", () => new ReferenceDataManager()); }
    #entities = [];
    get entities() { return this.#entities; }
    #references = [];
    get references() { return this.#references; }
    #referencesById = new Map();
    getReference(id) { return this.#referencesById.get(id); }
    #updated = new UpdatedEvent();
    constructor() {
        super();
        this.#getData();
    }
    async #getData() {
        const ds = await CedrusDataAPI.dataSourceTask;
        const properties = await ds.getProperties(ENTITY_TYPE("Reference"));
        const propsByEntities = groupBy(properties, p => p.entity);
        const entities = Array.from(propsByEntities.keys());
        const references = entities.map(e => toReferenceModel(e, propsByEntities.get(e)));
        const referencesById = makeLookup(references, e => e.id);
        arrayReplace(this.#entities, entities);
        arrayReplace(this.#references, references);
        mapReplace(this.#referencesById, referencesById);
        this.#onUpdate();
    }
    #onUpdate() {
        this.#references.sort(compareBy(e => e.name));
        this.dispatchEvent(this.#updated);
    }
    addReference(entity, properties) {
        const reference = toReferenceModel(entity, properties);
        this.#entities.push(entity);
        this.#references.push(reference);
        this.#referencesById.set(reference.id, reference);
        this.#onUpdate();
        return reference;
    }
}
//# sourceMappingURL=ReferenceDataManager.js.map