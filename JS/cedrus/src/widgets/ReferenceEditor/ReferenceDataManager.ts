import { arrayReplace, arrayScan, compareBy, groupBy, makeLookup, mapReplace, singleton } from "@juniper-lib/util";
import { TypedEventTarget } from "@juniper-lib/events";
import { UpdatedEvent } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { ENTITY_TYPE, EntityModel, PropertyModel } from "../../models";

export type ReferencePropertyTypeTypes =
    | "ReferenceFile"
    | "ReferenceLink";

export const referencePropertyTypes: ReferencePropertyTypeTypes[] = [
    "ReferenceFile",
    "ReferenceLink"
];

export interface ReferenceModel {
    id: number;
    name: string;
    authors: string;
    date: Date;
    type: ReferencePropertyTypeTypes;
    value: string;
}

export function toReferenceModel(entity: EntityModel, properties: PropertyModel[]): ReferenceModel {
    const propsByType = makeLookup(properties, p => p.type.name);
    const type = arrayScan(referencePropertyTypes, p => propsByType.has(p));
    return {
        id: entity.id,
        name: propsByType.get("Name")?.value as string,
        authors: propsByType.get("Authors")?.value as string,
        date: propsByType.get("Publication Date")?.value as Date,
        type,
        value: propsByType.get(type)?.value as string
    };
}

type ReferenceDataManagerEventsMap = {
    "updated": UpdatedEvent<ReferenceDataManager>;
}

export class ReferenceDataManager extends TypedEventTarget<ReferenceDataManagerEventsMap> {

    static get instance() { return singleton("ReferenceEditorElement::ReferenceDataManager", () => new ReferenceDataManager()); }

    readonly #entities: EntityModel[] = [];
    get entities() { return this.#entities; }

    readonly #references: ReferenceModel[] = [];
    get references() { return this.#references; }

    readonly #referencesById = new Map<number, ReferenceModel>();
    getReference(id: number) { return this.#referencesById.get(id); }

    readonly #updated = new UpdatedEvent<ReferenceDataManager>();

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

    addReference(entity: EntityModel, properties: PropertyModel[]) {
        const reference = toReferenceModel(entity, properties);
        this.#entities.push(entity);
        this.#references.push(reference);
        this.#referencesById.set(reference.id, reference);
        this.#onUpdate();
        return reference;
    }
}
