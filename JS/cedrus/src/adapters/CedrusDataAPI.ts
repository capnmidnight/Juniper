import { IResponse, isDefined, isNumber, isString, makeLookup, singleton } from "@juniper-lib/util";
import { Task } from "@juniper-lib/events";
import { IDictionary, IFetcher, unwrapResponse } from "@juniper-lib/fetcher";
import { Application_JsonUTF8 } from "@juniper-lib/mediatypes";
import {
    ClassificationCaveatModel, ClassificationLevelModel, ClassificationModel,
    DataTreeModel, DataType, EntityModel,
    EntityTypeModel, FileModel,
    ISequenced, PropertyModel, PropertyTypeModel, PropertyTypeValidValueModel,
    RelationshipModel, RelationshipTypeModel, RoleModel, SetClassificationCaveatInput,
    SetClassificationInput, SetEntityInput, SetPropertyInput,
    SetPropertyTypeInput,
    SetRelationshipInput,
    TemplateModel, TemplatePropertyModel, UserModel, UserRoleModel,
    isEntityModel
} from "../models";
import { IDOrName, cloneIDorName, toIDOrName } from "../models/IDOrName";


function resolveId(objOrId: number | ISequenced | IDOrName): number {
    return isNumber(objOrId)
        ? objOrId
        : isDefined(objOrId)
            ? objOrId.id
            : null;
}

export class CedrusDataAPI {
    static get dataSourceTask() { return singleton("Juniper::Cedrus::CedrusDataAPI::DataSourceTask", () => new Task<CedrusDataAPI>()); }

    protected readonly fetcher: IFetcher;

    constructor(fetcher: IFetcher) {

        this.fetcher = fetcher;

        CedrusDataAPI.dataSourceTask.resolve(this);
    }

    ///////////
    // OTHER //
    ///////////

    #formatPath(controller: string, path: string) {
        return `api/${controller}/v1/${path}`;
    }

    protected get<T>(controller: string, path: string): Promise<T> {
        return this.fetcher
            .get(this.#formatPath(controller, path))
            .object<T>()
            .then(unwrapResponse)
    }

    protected head(controller: string, path: string): Promise<IResponse> {
        return this.fetcher
            .head(this.#formatPath(controller, path))
            .exec();
    }

    protected post<T>(controller: string, path: string, body: any): Promise<T> {
        return this.fetcher
            .post(this.#formatPath(controller, path))
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

    ///////////////////////////
    // CLASSIFICATION LEVELS //
    //////////////////////////

    getClassificationLevels(): Promise<ClassificationLevelModel[]> {
        return this.get<ClassificationLevelModel[]>("cedrus", "classifications/levels");
    }

    deleteClassificationLevel(classLevelId: number): Promise<void>;
    deleteClassificationLevel(classLevel: ClassificationLevelModel): Promise<void>;
    deleteClassificationLevel(classLevelOrId: number | ClassificationLevelModel): Promise<void> {
        const classLevelId = resolveId(classLevelOrId);
        return this.delete("cedrus", `classifications/levels/${classLevelId}`);
    }

    /////////////////////////////
    // CLASSIFICATIONS CAVEATS //
    /////////////////////////////

    getClassificationCaveats(): Promise<ClassificationCaveatModel[]> {
        return this.get<ClassificationCaveatModel[]>("cedrus", "classifications/caveats");
    }

    setClassificationCaveat(input: SetClassificationCaveatInput) {
        return this.post<ClassificationCaveatModel>("cedrus", "classifications/caveats", input);
    }

    deleteClassificationCaveat(classCaveatId: number): Promise<void>;
    deleteClassificationCaveat(classCaveat: ClassificationCaveatModel): Promise<void>;
    deleteClassificationCaveat(classCaveatOrId: number | ClassificationCaveatModel): Promise<void> {
        const classCaveatId = resolveId(classCaveatOrId);
        return this.delete("cedrus", `classifications/caveats/${classCaveatId}`);
    }

    /////////////////////
    // CLASSIFICATIONS //
    /////////////////////

    getClassifications(): Promise<ClassificationModel[]> {
        return this.get<ClassificationModel[]>("cedrus", "classifications");
    }

    setClassification(input: SetClassificationInput): Promise<ClassificationModel> {
        return this.post<ClassificationModel>("cedrus", "classifications", input);
    }

    deleteClassification(classId: number): Promise<void>;
    deleteClassification(classification: ClassificationModel): Promise<void>;
    deleteClassification(classificationOrId: number | ClassificationModel): Promise<void> {
        const classId = resolveId(classificationOrId);
        return this.delete("cedrus", `classifications/${classId}`);
    }

    //////////////
    // ENTITIES //
    //////////////

    getEntities(): Promise<EntityModel[]> {
        return this.get<EntityModel[]>("cedrus", "entities/full");
    }

    getEntitiesOnly(): Promise<EntityModel[]> {
        return this.get<EntityModel[]>("cedrus", "entities");
    }

    getEntity(entityId: number): Promise<EntityModel> {
        return this.get<EntityModel>("cedrus", `entities/${entityId}`);
    }

    findEntity(entityTypeId: number, entityName: string): Promise<EntityModel>;
    findEntity(entityTypeName: string, entityName: string): Promise<EntityModel>
    findEntity(entityTypeIdOrName: number | string, entityName: string): Promise<EntityModel> {
        const entityType = toIDOrName(entityTypeIdOrName);
        return this.post<EntityModel>("cedrus", "entities/search", {
            type: entityType,
            name: entityName
        });
    }

    setEntity(input: SetEntityInput): Promise<EntityModel> {
        return this.post<EntityModel>("cedrus", "entities", input);
    }

    deleteEntity(entityId: number): Promise<void>
    deleteEntity(entity: EntityModel): Promise<void>
    deleteEntity(entityOrId: number | EntityModel): Promise<void> {
        const entityId = resolveId(entityOrId);
        return this.delete("cedrus", `entities/${entityId}`);
    }

    //////////////////
    // ENTITY TYPES //
    //////////////////

    getEntityTypes(): Promise<EntityTypeModel[]> {
        return this.get<EntityTypeModel[]>("cedrus", "entities/types");
    }

    setEntityType(name: string): Promise<EntityTypeModel> {
        return this.post<EntityTypeModel>("cedrus", "entities/types", name);
    }

    deleteEntityType(entityTypeId: number): Promise<void>;
    deleteEntityType(entityType: EntityTypeModel): Promise<void>;
    deleteEntityType(entityTypeOrId: number | EntityTypeModel): Promise<void> {
        const entityTypeId = resolveId(entityTypeOrId);
        return this.delete("cedrus", `entities/types/${entityTypeId}`)
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

    deleteFile(fileId: number): Promise<void>;
    deleteFile(fileGuid: string): Promise<void>;
    deleteFile(file: FileModel): Promise<void>;
    deleteFile(fileOrId: number | string | FileModel): Promise<void> {
        const fileId = isString(fileOrId)
            ? fileOrId
            : resolveId(fileOrId);
        return this.delete("cedrus", `files/${fileId}`);
    }

    ////////////////
    // PROPERTIES //
    ////////////////

    getProperties(entityId?: number) {
        if (isDefined(entityId)) {
            return this.get<PropertyModel[]>("cedrus", `entities/${entityId}/properties`);
        }
        else {
            return this.get<PropertyModel[]>("cedrus", "properties");
        }
    }

    setProperty<DataTypeT extends DataType>(entityId: number, input: SetPropertyInput<DataTypeT>) {
        return this.post<PropertyModel<DataTypeT>>("cedrus", `entities/${entityId}/properties`, input);
    }

    endProperty(propId: number): Promise<void>;
    endProperty(prop: PropertyModel): Promise<void>;
    endProperty(propOrId: number | PropertyModel): Promise<void> {
        const propertyId = resolveId(propOrId);
        return this.delete("cedrus", `properties/${propertyId}`);
    }

    ////////////////////
    // PROPERTY TYPES //
    ////////////////////

    getPropertyTypes() {
        return this.get<PropertyTypeModel[]>("cedrus", "properties/types");
    }

    setPropertyType<DataTypeT extends DataType>(name: string, dataType: DataTypeT, category: string, description: string) {
        return this.post<PropertyTypeModel>("cedrus", "properties/types", {
            name,
            dataType,
            category,
            description
        } as SetPropertyTypeInput);
    }

    deletePropertyType(propTypeId: number): Promise<void>;
    deletePropertyType(propType: PropertyTypeModel): Promise<void>;
    deletePropertyType(propTypeOrId: number | PropertyTypeModel): Promise<void> {
        const propertyTypeId = resolveId(propTypeOrId);
        return this.delete("cedrus", `properties/types/${propertyTypeId}`);
    }

    ////////////////////////////////
    // PROPERTY TYPE VALID VALUES //
    ////////////////////////////////

    getPropertyTypeValidValues(propertyTypeId?: number) {
        if (isDefined(propertyTypeId)) {
            return this.get<PropertyTypeValidValueModel[]>("cedrus", `properties/types/${propertyTypeId}/values`);
        }
        else {
            return this.get<PropertyTypeValidValueModel[]>("cedrus", "properties/types/values")
        }
    }

    setPropertyTypeValidValue<DataTypeT extends DataType>(propertyTypeId: number, value: string): Promise<PropertyTypeValidValueModel<DataTypeT>>;
    setPropertyTypeValidValue<DataTypeT extends DataType>(propertyType: PropertyTypeModel<DataTypeT>, value: string): Promise<PropertyTypeValidValueModel<DataTypeT>>;
    setPropertyTypeValidValue<DataTypeT extends DataType>(propertyTypeOrId: number | PropertyTypeModel<DataTypeT>, value: string) {
        const propertyTypeId = resolveId(propertyTypeOrId);
        return this.post<PropertyTypeValidValueModel<DataTypeT>>("cedrus", `properties/types/${propertyTypeId}/values`, value);
    }

    deletePropertyTypeValidValue(propTypeValidValueId: number): Promise<void>;
    deletePropertyTypeValidValue(propTypeValidValue: PropertyTypeValidValueModel): Promise<void>;
    deletePropertyTypeValidValue(propTypeValidValueOrId: number | PropertyTypeValidValueModel): Promise<void> {
        const propertyTypeValidValueId = resolveId(propTypeValidValueOrId);
        return this.delete("cedrus", `properties/types/values/${propertyTypeValidValueId}`);
    }

    ///////////////////
    // RELATIONSHIPS //
    ///////////////////

    getRelationships() {
        return this.get<RelationshipModel[]>("cedrus", "relationships");
    }

    setRelationship(parentEntity: EntityModel, input: SetRelationshipInput): Promise<RelationshipModel>;
    setRelationship(parentEntity: EntityModel, entity: EntityModel): Promise<RelationshipModel>;
    setRelationship(parentEntity: EntityModel, entityId: number): Promise<RelationshipModel>;
    setRelationship(parentEntityId: number, input: SetRelationshipInput): Promise<RelationshipModel>;
    setRelationship(parentEntityId: number, entity: EntityModel): Promise<RelationshipModel>;
    setRelationship(parentEntityId: number, entityId: number): Promise<RelationshipModel>;
    setRelationship(parentEntityOrId: number | EntityModel, input: number | EntityModel | SetRelationshipInput): Promise<RelationshipModel> {
        const parentEntityId = resolveId(parentEntityOrId);

        if (isNumber(input)) {
            input = { childEntity: { id: input } };
        }
        else if (isEntityModel(input)) {
            input = { childEntity: { id: resolveId(input) } };
        }
        else {
            input = {
                type: { id: resolveId(input.type) },
                childEntity: { id: resolveId(input.childEntity) },
                classification: cloneIDorName(input.classification),
                start: input.start,
                end: input.end
            }
        }

        return this.post<RelationshipModel>("cedrus", `entities/${parentEntityId}/relationships`, input);
    }

    endRelationship(relId: number): Promise<void>;
    endRelationship(relationship: RelationshipModel): Promise<void>;
    endRelationship(relationshipOrId: number | RelationshipModel): Promise<void> {
        const relationshipId = resolveId(relationshipOrId);
        return this.delete("cedrus", `relationships/${relationshipId}`);
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

    deleteRelationshipType(relTypeId: number): Promise<void>;
    deleteRelationshipType(relType: RelationshipTypeModel): Promise<void>;
    deleteRelationshipType(relTypeOrId: number | RelationshipTypeModel): Promise<void> {
        const relTypeId = resolveId(relTypeOrId);
        return this.delete("cedrus", `relationships/types/${relTypeId}`);
    }

    ///////////
    // ROLES //
    ///////////

    getRoles() {
        return this.get<RoleModel[]>("cedrus", "roles");
    }

    getRoleUsers(roleId?: number) {
        if (isDefined(roleId)) {
            return this.get<UserRoleModel[]>("cedrus", `roles/${roleId}/users`);
        }
        else {
            return this.get<UserRoleModel[]>("cedrus", "roles/users");
        }
    }

    deleteRole(roleId: number): Promise<void>;
    deleteRole(role: RoleModel): Promise<void>;
    deleteRole(roleOrId: number | RoleModel) {
        const roleId = resolveId(roleOrId);
        return this.delete("cedrus", `roles/${roleId}`);
    }

    //////////
    // TAGS //
    //////////

    getTags() {
        return this.get<string[]>("cedrus", "tags");
    }

    tagEntity(entity: EntityModel, name: string, description?: string): Promise<EntityModel>;
    tagEntity(entityId: number, name: string, description?: string): Promise<EntityModel>;
    tagEntity(entityOrId: number | EntityModel, name: string, description?: string): Promise<EntityModel> {
        const entityId = resolveId(entityOrId);
        return this.post<EntityModel>("cedrus", `entities/${entityId}/tags`, {
            name,
            description
        });
    }

    ///////////////
    // TEMPLATES //
    ///////////////

    getTemplates(entityTypeId?: number) {
        if (isDefined(entityTypeId)) {
            return this.get<TemplateModel[]>("cedrus", `entities/types/${entityTypeId}/templates`);
        }
        else {
            return this.get<TemplateModel[]>("cedrus", "templates");
        }
    }

    deleteTemplate(template: TemplateModel): Promise<void>;
    deleteTemplate(templateId: number): Promise<void>;
    deleteTemplate(templateOrId: number | TemplateModel): Promise<void> {
        const templateId = resolveId(templateOrId);
        return this.delete("cedrus", `/templates/${templateId}`);
    }

    getTemplateProperties(entityTypeId?: number) {
        if (isDefined(entityTypeId)) {
            return this.get<TemplatePropertyModel[]>("cedrus", `entities/types/${entityTypeId}/templates/properties`);
        }
        else {
            return this.get<TemplatePropertyModel[]>("cedrus", "templates/properties");
        }
    }

    deleteTemplateProperty(template: TemplateModel, propertyType: PropertyTypeModel): Promise<void>;
    deleteTemplateProperty(template: TemplateModel, propertyTypeId: number): Promise<void>;
    deleteTemplateProperty(templateId: number, propertyType: PropertyTypeModel): Promise<void>;
    deleteTemplateProperty(templateId: number, propertyTypeId: number): Promise<void>;
    deleteTemplateProperty(templateOrId: number | TemplateModel, propertyTypeOrId: number | PropertyTypeModel): Promise<void> {
        const templateId = resolveId(templateOrId);
        const propertyTypeId = resolveId(propertyTypeOrId);
        return this.delete("cedrus", `templates/${templateId}/properties/${propertyTypeId}`)
    }

    //////////
    // TREE //
    //////////

    getTree(entityTypes?: string[], relTypePriority?: string[]) {
        if (entityTypes?.length > 0 && relTypePriority?.length > 0) {
            return this.get<DataTreeModel>("cedrus", `tree?entityTypes=${entityTypes.join(',')}&relTypes=${relTypePriority.join(',')}`);
        }
        else if (entityTypes?.length > 0) {
            return this.get<DataTreeModel>("cedrus", `tree?entityTypes=${entityTypes.join(',')}`);
        }
        else if (relTypePriority?.length > 0) {
            return this.get<DataTreeModel>("cedrus", `tree?relTypes=${relTypePriority.join(',')}`);
        }
        else {
            return this.get<DataTreeModel>("cedrus", "tree");
        }
    }

    ///////////
    // USERS //
    ///////////

    getUsers() {
        return this.get<UserModel[]>("cedrus", "users");
    }

    getUserRoles(userId?: number) {
        if (isDefined(userId)) {
            return this.get<UserRoleModel[]>("cedrus", `users/${userId}/roles`);
        }
        else {
            return this.get<UserRoleModel[]>("cedrus", "users/roles");
        }
    }

    deleteUser(userId: number): Promise<void>;
    deleteUser(user: UserModel): Promise<void>;
    deleteUser(userOrId: number | UserModel) {
        const userId = resolveId(userOrId);
        return this.delete("cedrus", `users/${userId}`);
    }

    removeUserFromRole(user: UserModel, role: RoleModel): Promise<void>;
    removeUserFromRole(userId: number, role: RoleModel): Promise<void>;
    removeUserFromRole(user: UserModel, roleId: number): Promise<void>;
    removeUserFromRole(userId: number, roleId: number): Promise<void>;
    removeUserFromRole(userOrId: UserModel | number, roleOrId: RoleModel | number): Promise<void> {
        const userId = resolveId(userOrId);
        const roleId = resolveId(roleOrId);
        return this.delete("cedrus", `users/${userId}/roles/${roleId}`);
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