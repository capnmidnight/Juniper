import { first, IResponse, isDefined, isNumber, isString, makeLookup, singleton } from "@juniper-lib/util";
import { Task } from "@juniper-lib/events";
import { createFetcher, IDictionary, IFetcher, unwrapResponse } from "@juniper-lib/fetcher";
import { Application_JsonUTF8 } from "@juniper-lib/mediatypes";
import { IProgress } from "@juniper-lib/progress";
import { FileViewValue } from "@juniper-lib/widgets";
import { DataTreeModel } from "../models/DataTreeModel";
import { DataType, DataTypeMap, StorageType } from "../models/DataType";
import { ENTITY, EntityModel, ReviewParts, SetEntityInput } from "../models/EntityModel";
import { ENTITY_TYPE, EntityTypeModel, SetEntityTypeInput } from "../models/EntityTypeModel";
import { FileModel, isFileModel } from "../models/FileModel";
import { ISequenced } from "../models/ISequenced";
import { cloneAllIdOrNames, cloneIdOrName, TypedNameOrId } from "../models/NameOrId";
import { PropertyModel, SetPropertyInput } from "../models/PropertyModel";
import { PropertyTemplateModel, SetPropertyTemplateInput } from "../models/PropertyTemplateModel";
import { PROPERTY_TYPE, PropertyTypeModel, SetPropertyTypeInput } from "../models/PropertyTypeModel";
import { PropertyTypeValidValueModel } from "../models/PropertyTypeValidValueModel";
import { GetRelationshipInput, RelationshipModel, SetRelationshipInput } from "../models/RelationshipModel";
import { RelationshipTemplateModel, SetRelationshipTemplateInput } from "../models/RelationshipTemplateModel";
import { RELATIONSHIP_TYPE, RelationshipTypeModel } from "../models/RelationshipTypeModel";
import { RoleModel } from "../models/RoleModel";
import { SetUserInput, UserModel } from "../models/UserModel";
import { UserRoleModel } from "../models/UserRoleModel";


function resolveId(objOrId?: number | ISequenced | TypedNameOrId): number {
    return isNumber(objOrId)
        ? objOrId
        : isDefined(objOrId)
            ? objOrId.id
            : null;
}

export class CedrusDataAPI {
    static get dataSourceTask() {

        const task = singleton("Juniper::Cedrus::CedrusDataAPI::DataSourceTask", () => {
            const task = new Task<CedrusDataAPI>();
            task.then(() => {
                if (timer) {
                    clearTimeout(timer);
                    timer = null;
                }
            });
            let timer = setTimeout(() => {
                timer = null;
                task.resolve(new CedrusDataAPI());
            }, 1000);
            return task;
        });

        return task
    }

    protected readonly fetcher: IFetcher;

    constructor(fetcher?: IFetcher) {

        this.fetcher = fetcher ?? createFetcher();

        CedrusDataAPI.dataSourceTask.resolve(this);
    }

    ///////////
    // OTHER //
    ///////////

    #formatPath(controller: string, path: string) {
        return `/api/${controller}/v1/${path}`;
    }

    protected get<T>(controller: string, path: string, prog?: IProgress): Promise<T> {
        return this.fetcher
            .get(this.#formatPath(controller, path))
            .progress(prog)
            .object<T>()
            .then(unwrapResponse)
    }

    getObject<T>(path: string, prog?: IProgress): Promise<T> {
        return this.fetcher
            .get(path)
            .progress(prog)
            .object<T>(Application_JsonUTF8)
            .then(unwrapResponse);
    }

    getText(path: string, prog?: IProgress): Promise<string> {
        return this.fetcher
            .get(path)
            .progress(prog)
            .text()
            .then(unwrapResponse);
    }

    protected head(controller: string, path: string): Promise<IResponse> {
        return this.fetcher
            .head(this.#formatPath(controller, path))
            .exec();
    }

    protected post<T>(controller: string, path: string, body: any, prog?: IProgress): Promise<T> {
        return this.fetcher
            .post(this.#formatPath(controller, path))
            .progress(prog)
            .body(body, Application_JsonUTF8)
            .object<T>()
            .then(unwrapResponse)
    }

    protected delete(controller: string, path: string): Promise<void> {
        return this.fetcher
            .delete(this.#formatPath(controller, path))
            .exec()
            .then(unwrapResponse)
    }

    getEndpoints() {
        return this.get<object[]>("cedrus", "endpoints");
    }

    //////////////
    // ENTITIES //
    //////////////

    getEntities(...searchParams: TypedNameOrId<"entity" | "entityType">[]) {
        if (isDefined(searchParams)) {
            if (searchParams.length === 0) {
                searchParams = null;
            }
            else {
                searchParams = cloneAllIdOrNames(searchParams);
            }
        }
        return this.post<EntityModel[]>("cedrus", "entities/search", searchParams);
    }

    getEntity(entityId: number): Promise<EntityModel>;
    getEntity(entityName: string): Promise<EntityModel>;
    getEntity(entity: number | string): Promise<EntityModel> {
        return this.getEntities(ENTITY(entity)).then(first);
    }

    setEntity(name: string, type: ENTITY_TYPE): Promise<EntityModel> {
        const input: SetEntityInput = {
            name,
            type: cloneIdOrName(type)
        };

        return this.post<EntityModel>("cedrus", "entities", input);
    }

    deleteEntity(entity: EntityModel): Promise<void> {
        return this.delete("cedrus", `entities/${entity.id}`);
    }

    markEntityReviewed(entity: ENTITY): Promise<ReviewParts> {
        return this.post<ReviewParts>("cedrus", "entities/review", entity);
    }

    //////////////////
    // ENTITY TYPES //
    //////////////////

    getEntityTypes(...searchParams: ENTITY_TYPE[]): Promise<EntityTypeModel[]> {
        return this.post<EntityTypeModel[]>("cedrus", "entities/types/search", searchParams);
    }

    getEntityType(entityTypeId: number): Promise<EntityTypeModel>;
    getEntityType(entityTypeName: string): Promise<EntityTypeModel>;
    getEntityType(entityType: number | string) {
        return this.getEntityTypes(ENTITY_TYPE(entityType)).then(first);
    }

    setEntityType(input: SetEntityTypeInput): Promise<EntityTypeModel> {
        input.parentEntityType = cloneIdOrName(input.parentEntityType);
        return this.post<EntityTypeModel>("cedrus", "entities/types", input);
    }

    deleteEntityType(entityType: EntityTypeModel): Promise<void> {
        return this.delete("cedrus", `entities/types/${entityType.id}`)
    }


    ///////////
    // FILES //
    ///////////

    getFiles() {
        return this.get<FileModel[]>("cedrus", "files");
    }

    getFileInfo(path: string) {
        return this.fetcher.head(path).exec();
    }

    uploadFiles(files: File[] | FileList | IterableIterator<File>, prog?: IProgress): Promise<FileModel[]> {
        const form = new FormData();

        for (const file of files) {
            form.append("Files", file);
        }

        return this.post<FileModel[]>("cedrus", "files", form, prog);
    }

    async mergeFiles(files: FileViewValue[], prog?: IProgress): Promise<string[]> {
        const fileModels = new Map<FileViewValue, string>();
        const toSave = new Array<File>();
        for (const value of files) {
            if (isString(value)) {
                fileModels.set(value, value);
            }
            else if (value instanceof URL) {
                fileModels.set(value, value.href);
            }
            else if (isFileModel(value)) {
                fileModels.set(value, value.path);
            }
            else if (value instanceof File) {
                toSave.push(value);
            }
        }

        if (toSave.length > 0) {
            const savedFiles = await this.uploadFiles(toSave, prog);
            for (let i = 0; i < toSave.length; ++i) {
                fileModels.set(toSave[i], savedFiles[i].path);
            }
        }

        const models = files
            .filter(v => fileModels.has(v))
            .map(v => fileModels.get(v));

        return models;
    }

    deleteFile(file: FileModel): Promise<void> {
        return this.delete("cedrus", `files/${file.guid}`);
    }

    ////////////////
    // PROPERTIES //
    ////////////////

    getProperties(...searchParams: TypedNameOrId<"entity" | "entityType" | "propertyType">[]): Promise<PropertyModel[]> {
        if (isDefined(searchParams)) {
            if (searchParams.length === 0) {
                searchParams = null;
            }
            else {
                searchParams = cloneAllIdOrNames(searchParams);
            }
        }

        return this.post<PropertyModel[]>("cedrus", `properties/search`, searchParams);
    }

    setProperty<DataTypeT extends DataType, StorageTypeT extends StorageType>(entity: ENTITY, type: PROPERTY_TYPE, value: DataTypeMap[DataTypeT][StorageTypeT], unitOfMeasure: string, reference?: ENTITY): Promise<PropertyModel<DataTypeT, StorageTypeT>> {
        const input: SetPropertyInput<DataTypeT, StorageTypeT> = {
            type: cloneIdOrName(type),
            unitOfMeasure,
            value
        };

        if (isDefined(reference)) {
            input.reference = cloneIdOrName(reference);
        }

        return this.post<PropertyModel<DataTypeT, StorageTypeT>>("cedrus", `entities/${entity.id}/properties`, input);
    }

    deleteProperty(property: PropertyModel): Promise<void> {
        return this.delete("cedrus", `properties/${property.id}`);
    }

    ////////////////////
    // PROPERTY TYPES //
    ////////////////////

    getPropertyTypes() {
        return this.get<PropertyTypeModel[]>("cedrus", "properties/types");
    }

    setPropertyType<DataTypeT extends DataType, StorageTypeT extends StorageType>(name: string, type: DataTypeT, storage: StorageTypeT, category: string, description: string) {
        return this.post<PropertyTypeModel>("cedrus", "properties/types", {
            name,
            type,
            storage,
            category,
            description
        } as SetPropertyTypeInput);
    }

    deletePropertyType(propertyType: PropertyTypeModel): Promise<void> {
        return this.delete("cedrus", `properties/types/${propertyType.id}`);
    }

    ////////////////////////////////
    // PROPERTY TYPE VALID VALUES //
    ////////////////////////////////

    getPropertyTypeValidValues(propertyType?: PropertyTypeModel) {
        if (isDefined(propertyType)) {
            return this.get<PropertyTypeValidValueModel[]>("cedrus", `properties/types/${propertyType.id}/values`);
        }
        else {
            return this.get<PropertyTypeValidValueModel[]>("cedrus", "properties/types/values")
        }
    }

    setPropertyTypeValidValue<DataTypeT extends DataType, StorageTypeT extends StorageType>(propertyType: PropertyTypeModel<DataTypeT, StorageTypeT>, ...values: string[]): Promise<PropertyTypeValidValueModel<DataTypeT, StorageTypeT>[]> {
        return this.post<PropertyTypeValidValueModel<DataTypeT, StorageTypeT>[]>("cedrus", `properties/types/${propertyType.id}/values`, values);
    }

    deletePropertyTypeValidValue(propertyTypeValidValue: PropertyTypeValidValueModel): Promise<void> {
        return this.delete("cedrus", `properties/types/values/${propertyTypeValidValue.id}`);
    }

    ///////////////////
    // RELATIONSHIPS //
    ///////////////////

    getRelationships(searchParams?: GetRelationshipInput) {
        if (isDefined(searchParams)) {
            if (isDefined(searchParams.parent)) {
                searchParams.parent = cloneAllIdOrNames(searchParams.parent);
            }
            if (isDefined(searchParams.child)) {
                searchParams.child = cloneAllIdOrNames(searchParams.child);
            }
            if (isDefined(searchParams.both)) {
                searchParams.both = cloneAllIdOrNames(searchParams.both);
            }
        }

        return this.post<RelationshipModel[]>("cedrus", "relationships/search", searchParams);
    }

    setRelationship(parentEntity: EntityModel, childEntity: EntityModel, propertyEntity: EntityModel, type?: RelationshipTypeModel): Promise<RelationshipModel> {
        const input: SetRelationshipInput = {
            childEntity: cloneIdOrName(childEntity),
            propertyEntity: cloneIdOrName(propertyEntity)
        };

        if (isDefined(type)) {
            input.type = cloneIdOrName(type);
        }

        return this.post<RelationshipModel>("cedrus", `entities/${parentEntity.id}/relationships`, input);
    }

    deleteRelationship(relationship: RelationshipModel): Promise<void> {
        return this.delete("cedrus", `relationships/${relationship.id}`);
    }

    ////////////////////////
    // RELATIONSHIP TYPES //
    ////////////////////////

    getRelationshipTypes() {
        return this.get<RelationshipTypeModel[]>("cedrus", "relationships/types");
    }

    setRelationshipType(parentRole: string, childRole?: string) {
        return this.post<RelationshipTypeModel>("cedrus", "relationships/types", [
            parentRole,
            childRole
        ]);
    }

    deleteRelationshipType(relationshipType: RelationshipTypeModel): Promise<void> {
        return this.delete("cedrus", `relationships/types/${relationshipType.id}`);
    }

    ///////////
    // ROLES //
    ///////////

    getRoles() {
        return this.get<RoleModel[]>("cedrus", "roles");
    }

    getRoleUsers(role?: RoleModel) {
        if (isDefined(role)) {
            return this.get<UserRoleModel[]>("cedrus", `roles/${role.id}/users`);
        }
        else {
            return this.get<UserRoleModel[]>("cedrus", "roles/users");
        }
    }

    createRole(name: string) {
        return this.post<RoleModel>("cedrus", "roles", name);
    }

    deleteRole(role: RoleModel): Promise<void> {
        return this.delete("cedrus", `roles/${role.id}`);
    }

    //////////
    // TAGS //
    //////////

    getTags() {
        return this.get<string[]>("cedrus", "tags");
    }

    tagEntity(entity: EntityModel, tagName: string): Promise<EntityModel> {
        return this.post<EntityModel>("cedrus", `entities/${entity.id}/tags`, tagName);
    }

    ////////////////////////
    // PROPERTY TEMPLATES //
    ////////////////////////

    getPropertyTemplates(entityType?: EntityTypeModel) {
        if (isDefined(entityType)) {
            return this.get<PropertyTemplateModel[]>("cedrus", `entities/types/${entityType.id}/templates/properties`);
        }
        else {
            return this.get<PropertyTemplateModel[]>("cedrus", "templates/properties");
        }
    }

    setPropertyTemplate(name: string, entityType: EntityTypeModel, ...propertyTypes: PropertyTypeModel[]) {
        const entityTypeId = resolveId(entityType);
        const input: SetPropertyTemplateInput = {
            name,
            propertyTypes: cloneAllIdOrNames(propertyTypes)
        }
        return this.post<PropertyTemplateModel>("cedrus", `entities/types/${entityTypeId}/templates/properties`, input);
    }

    deletePropertyTemplate(template: PropertyTemplateModel): Promise<void> {
        return this.delete("cedrus", `templates/properties/${template.id}`);
    }

    ////////////////////////////
    // RELATIONSHIP TEMPLATES //
    ////////////////////////////

    getRelationshipTemplates(entityType?: EntityTypeModel) {
        if (isDefined(entityType)) {
            return this.get<RelationshipTemplateModel[]>("cedrus", `entities/types/${entityType.id}/templates/relationships`);
        }
        else {
            return this.get<RelationshipTemplateModel[]>("cedrus", "templates/relationships");
        }
    }

    setRelationshipTemplate(name: string, entityType: ENTITY_TYPE, relationshipType: RELATIONSHIP_TYPE, propertyEntityType: ENTITY_TYPE, ...entityTypes: ENTITY_TYPE[]) {
        const entityTypeId = resolveId(entityType);
        const input: SetRelationshipTemplateInput = {
            name,
            relationshipType: cloneIdOrName(relationshipType),
            propertyEntityType: cloneIdOrName(propertyEntityType),
            allowedEntityTypes: cloneAllIdOrNames(entityTypes)
        }
        return this.post<RelationshipTemplateModel>("cedrus", `entities/types/${entityTypeId}/templates/relationships`, input);
    }

    deleteRelationshipTemplate(template: RelationshipTemplateModel): Promise<void> {
        return this.delete("cedrus", `templates/relationships/${template.id}`);
    }

    //////////
    // TREE //
    //////////

    getTree(...searchParams: TypedNameOrId<"entityType" | "propertyType">[]): Promise<DataTreeModel> {
        if (isDefined(searchParams)) {
            searchParams = cloneAllIdOrNames(searchParams);
        }
        return this.post<DataTreeModel>("cedrus", "tree", searchParams);
    }

    ///////////
    // USERS //
    ///////////

    getUsers() {
        return this.get<UserModel[]>("cedrus", "users");
    }

    getUserRoles(user?: UserModel) {
        if (isDefined(user)) {
            return this.get<UserRoleModel[]>("cedrus", `users/${user.id}/roles`);
        }
        else {
            return this.get<UserRoleModel[]>("cedrus", "users/roles");
        }
    }

    setUser(userName: string, email: string) {
        const input: SetUserInput = {
            userName,
            email
        };

        return this.post<UserModel>("cedrus", "users", input);
    }

    deleteUser(user: UserModel): Promise<void> {
        return this.delete("cedrus", `users/${user.id}`);
    }

    removeUserFromRole(user: UserModel, role: RoleModel): Promise<void> {
        return this.delete("cedrus", `users/${user.id}/roles/${role.id}`);
    }

    addUserToRole(user: UserModel, role: RoleModel): Promise<UserRoleModel> {
        return this.post<UserRoleModel>("cedrus", `users/${user.id}/roles`, role.id);
    }

    grantUserAccess(userId: number) {
        return this.post<void>("cedrus", `users/grant`, userId)
    }

    ///////////
    // UNITS //
    ///////////

    #unitsData: Promise<Map<string, string[]>>;
    getUnitsData() {
        if (!this.#unitsData) {
            this.#unitsData = this.get<IDictionary<string, string[]>>("cedrus", "units")
                .then(kvs => makeLookup(kvs, kv => kv.key, kv => kv.value));
        }
        return this.#unitsData
    }

    #unitAbbreviations: Promise<Map<string, string>>;
    getUnitAbbreviations() {
        if (!this.#unitAbbreviations) {
            this.#unitAbbreviations = this.get<IDictionary<string, string>>("cedrus", "units/abbreviations")
                .then(kvs => makeLookup(kvs, kv => kv.key, kv => kv.value));;
        }
        return this.#unitAbbreviations;
    }

    convertUnits(value: number, from: string, to: string) {
        return this.post<number>("cedrus", `units/convert/${from}/${to}`, value);
    }
}