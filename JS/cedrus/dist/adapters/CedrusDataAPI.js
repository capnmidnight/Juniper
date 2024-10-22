import { first, isDefined, isNumber, isString, makeLookup, singleton } from "@juniper-lib/util";
import { Task } from "@juniper-lib/events";
import { createFetcher, unwrapResponse } from "@juniper-lib/fetcher";
import { Application_JsonUTF8 } from "@juniper-lib/mediatypes";
import { ENTITY } from "../models/EntityModel";
import { ENTITY_TYPE } from "../models/EntityTypeModel";
import { isFileModel } from "../models/FileModel";
import { cloneAllIdOrNames, cloneIdOrName } from "../models/NameOrId";
function resolveId(objOrId) {
    return isNumber(objOrId)
        ? objOrId
        : isDefined(objOrId)
            ? objOrId.id
            : null;
}
export class CedrusDataAPI {
    static get dataSourceTask() {
        const task = singleton("Juniper::Cedrus::CedrusDataAPI::DataSourceTask", () => {
            const task = new Task();
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
        return task;
    }
    constructor(fetcher) {
        this.fetcher = fetcher ?? createFetcher();
        CedrusDataAPI.dataSourceTask.resolve(this);
    }
    ///////////
    // OTHER //
    ///////////
    #formatPath(controller, path) {
        return `/api/${controller}/v1/${path}`;
    }
    get(controller, path, prog) {
        return this.fetcher
            .get(this.#formatPath(controller, path))
            .progress(prog)
            .object()
            .then(unwrapResponse);
    }
    getObject(path, prog) {
        return this.fetcher
            .get(path)
            .progress(prog)
            .object(Application_JsonUTF8)
            .then(unwrapResponse);
    }
    getText(path, prog) {
        return this.fetcher
            .get(path)
            .progress(prog)
            .text()
            .then(unwrapResponse);
    }
    head(controller, path) {
        return this.fetcher
            .head(this.#formatPath(controller, path))
            .exec();
    }
    post(controller, path, body, prog) {
        return this.fetcher
            .post(this.#formatPath(controller, path))
            .progress(prog)
            .body(body, Application_JsonUTF8)
            .object()
            .then(unwrapResponse);
    }
    delete(controller, path) {
        return this.fetcher
            .delete(this.#formatPath(controller, path))
            .exec()
            .then(unwrapResponse);
    }
    getEndpoints() {
        return this.get("cedrus", "endpoints");
    }
    //////////////
    // ENTITIES //
    //////////////
    getEntities(...searchParams) {
        if (isDefined(searchParams)) {
            if (searchParams.length === 0) {
                searchParams = null;
            }
            else {
                searchParams = cloneAllIdOrNames(searchParams);
            }
        }
        return this.post("cedrus", "entities/search", searchParams);
    }
    getEntity(entity) {
        return this.getEntities(ENTITY(entity)).then(first);
    }
    setEntity(name, type) {
        const input = {
            name,
            type: cloneIdOrName(type)
        };
        return this.post("cedrus", "entities", input);
    }
    deleteEntity(entity) {
        return this.delete("cedrus", `entities/${entity.id}`);
    }
    markEntityReviewed(entity) {
        return this.post("cedrus", "entities/review", entity);
    }
    //////////////////
    // ENTITY TYPES //
    //////////////////
    getEntityTypes(...searchParams) {
        return this.post("cedrus", "entities/types/search", searchParams);
    }
    getEntityType(entityType) {
        return this.getEntityTypes(ENTITY_TYPE(entityType)).then(first);
    }
    setEntityType(input) {
        input.parentEntityType = cloneIdOrName(input.parentEntityType);
        return this.post("cedrus", "entities/types", input);
    }
    deleteEntityType(entityType) {
        return this.delete("cedrus", `entities/types/${entityType.id}`);
    }
    ///////////
    // FILES //
    ///////////
    getFiles() {
        return this.get("cedrus", "files");
    }
    getFileInfo(path) {
        return this.fetcher.head(path).exec();
    }
    uploadFiles(files, prog) {
        const form = new FormData();
        for (const file of files) {
            form.append("Files", file);
        }
        return this.post("cedrus", "files", form, prog);
    }
    async mergeFiles(files, prog) {
        const fileModels = new Map();
        const toSave = new Array();
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
    deleteFile(file) {
        return this.delete("cedrus", `files/${file.guid}`);
    }
    ////////////////
    // PROPERTIES //
    ////////////////
    getProperties(...searchParams) {
        if (isDefined(searchParams)) {
            if (searchParams.length === 0) {
                searchParams = null;
            }
            else {
                searchParams = cloneAllIdOrNames(searchParams);
            }
        }
        return this.post("cedrus", `properties/search`, searchParams);
    }
    setProperty(entity, type, value, unitOfMeasure, reference) {
        const input = {
            type: cloneIdOrName(type),
            unitOfMeasure,
            value
        };
        if (isDefined(reference)) {
            input.reference = cloneIdOrName(reference);
        }
        return this.post("cedrus", `entities/${entity.id}/properties`, input);
    }
    deleteProperty(property) {
        return this.delete("cedrus", `properties/${property.id}`);
    }
    ////////////////////
    // PROPERTY TYPES //
    ////////////////////
    getPropertyTypes() {
        return this.get("cedrus", "properties/types");
    }
    setPropertyType(name, type, storage, category, description) {
        return this.post("cedrus", "properties/types", {
            name,
            type,
            storage,
            category,
            description
        });
    }
    deletePropertyType(propertyType) {
        return this.delete("cedrus", `properties/types/${propertyType.id}`);
    }
    ////////////////////////////////
    // PROPERTY TYPE VALID VALUES //
    ////////////////////////////////
    getPropertyTypeValidValues(propertyType) {
        if (isDefined(propertyType)) {
            return this.get("cedrus", `properties/types/${propertyType.id}/values`);
        }
        else {
            return this.get("cedrus", "properties/types/values");
        }
    }
    setPropertyTypeValidValue(propertyType, ...values) {
        return this.post("cedrus", `properties/types/${propertyType.id}/values`, values);
    }
    deletePropertyTypeValidValue(propertyTypeValidValue) {
        return this.delete("cedrus", `properties/types/values/${propertyTypeValidValue.id}`);
    }
    ///////////////////
    // RELATIONSHIPS //
    ///////////////////
    getRelationships(searchParams) {
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
        return this.post("cedrus", "relationships/search", searchParams);
    }
    setRelationship(parentEntity, childEntity, propertyEntity, type) {
        const input = {
            childEntity: cloneIdOrName(childEntity),
            propertyEntity: cloneIdOrName(propertyEntity)
        };
        if (isDefined(type)) {
            input.type = cloneIdOrName(type);
        }
        return this.post("cedrus", `entities/${parentEntity.id}/relationships`, input);
    }
    deleteRelationship(relationship) {
        return this.delete("cedrus", `relationships/${relationship.id}`);
    }
    ////////////////////////
    // RELATIONSHIP TYPES //
    ////////////////////////
    getRelationshipTypes() {
        return this.get("cedrus", "relationships/types");
    }
    setRelationshipType(parentRole, childRole) {
        return this.post("cedrus", "relationships/types", [
            parentRole,
            childRole
        ]);
    }
    deleteRelationshipType(relationshipType) {
        return this.delete("cedrus", `relationships/types/${relationshipType.id}`);
    }
    ///////////
    // ROLES //
    ///////////
    getRoles() {
        return this.get("cedrus", "roles");
    }
    getRoleUsers(role) {
        if (isDefined(role)) {
            return this.get("cedrus", `roles/${role.id}/users`);
        }
        else {
            return this.get("cedrus", "roles/users");
        }
    }
    createRole(name) {
        return this.post("cedrus", "roles", name);
    }
    deleteRole(role) {
        return this.delete("cedrus", `roles/${role.id}`);
    }
    //////////
    // TAGS //
    //////////
    getTags() {
        return this.get("cedrus", "tags");
    }
    tagEntity(entity, tagName) {
        return this.post("cedrus", `entities/${entity.id}/tags`, tagName);
    }
    ////////////////////////
    // PROPERTY TEMPLATES //
    ////////////////////////
    getPropertyTemplates(entityType) {
        if (isDefined(entityType)) {
            return this.get("cedrus", `entities/types/${entityType.id}/templates/properties`);
        }
        else {
            return this.get("cedrus", "templates/properties");
        }
    }
    setPropertyTemplate(name, entityType, ...propertyTypes) {
        const entityTypeId = resolveId(entityType);
        const input = {
            name,
            propertyTypes: cloneAllIdOrNames(propertyTypes)
        };
        return this.post("cedrus", `entities/types/${entityTypeId}/templates/properties`, input);
    }
    deletePropertyTemplate(template) {
        return this.delete("cedrus", `templates/properties/${template.id}`);
    }
    ////////////////////////////
    // RELATIONSHIP TEMPLATES //
    ////////////////////////////
    getRelationshipTemplates(entityType) {
        if (isDefined(entityType)) {
            return this.get("cedrus", `entities/types/${entityType.id}/templates/relationships`);
        }
        else {
            return this.get("cedrus", "templates/relationships");
        }
    }
    setRelationshipTemplate(name, entityType, relationshipType, propertyEntityType, ...entityTypes) {
        const entityTypeId = resolveId(entityType);
        const input = {
            name,
            relationshipType: cloneIdOrName(relationshipType),
            propertyEntityType: cloneIdOrName(propertyEntityType),
            allowedEntityTypes: cloneAllIdOrNames(entityTypes)
        };
        return this.post("cedrus", `entities/types/${entityTypeId}/templates/relationships`, input);
    }
    deleteRelationshipTemplate(template) {
        return this.delete("cedrus", `templates/relationships/${template.id}`);
    }
    //////////
    // TREE //
    //////////
    getTree(...searchParams) {
        if (isDefined(searchParams)) {
            searchParams = cloneAllIdOrNames(searchParams);
        }
        return this.post("cedrus", "tree", searchParams);
    }
    ///////////
    // USERS //
    ///////////
    getUsers() {
        return this.get("cedrus", "users");
    }
    getUserRoles(user) {
        if (isDefined(user)) {
            return this.get("cedrus", `users/${user.id}/roles`);
        }
        else {
            return this.get("cedrus", "users/roles");
        }
    }
    setUser(userName, email) {
        const input = {
            userName,
            email
        };
        return this.post("cedrus", "users", input);
    }
    deleteUser(user) {
        return this.delete("cedrus", `users/${user.id}`);
    }
    removeUserFromRole(user, role) {
        return this.delete("cedrus", `users/${user.id}/roles/${role.id}`);
    }
    addUserToRole(user, role) {
        return this.post("cedrus", `users/${user.id}/roles`, role.id);
    }
    grantUserAccess(userId) {
        return this.post("cedrus", `users/grant`, userId);
    }
    ///////////
    // UNITS //
    ///////////
    #unitsData;
    getUnitsData() {
        if (!this.#unitsData) {
            this.#unitsData = this.get("cedrus", "units")
                .then(kvs => makeLookup(kvs, kv => kv.key, kv => kv.value));
        }
        return this.#unitsData;
    }
    #unitAbbreviations;
    getUnitAbbreviations() {
        if (!this.#unitAbbreviations) {
            this.#unitAbbreviations = this.get("cedrus", "units/abbreviations")
                .then(kvs => makeLookup(kvs, kv => kv.key, kv => kv.value));
            ;
        }
        return this.#unitAbbreviations;
    }
    convertUnits(value, from, to) {
        return this.post("cedrus", `units/convert/${from}/${to}`, value);
    }
}
//# sourceMappingURL=CedrusDataAPI.js.map