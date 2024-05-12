import { EntityModel, isEntityModel } from "./EntityModel";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { IDOrName } from "./IDOrName";
import { ISequenced, isISequenced } from "./ISequenced";
import { ITimeSeries, isITimeSeries } from "./ITimeSeries";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { RelationshipTypeModel } from "./RelationshipTypeModel";

export interface RelationshipModel extends ISequenced, ITypeStamped<"relationship">, ICreationTracked, ITimeSeries<RelationshipTypeModel> {
    parent: EntityModel;
    child: EntityModel;
}

export function isRelationshipModel(obj: unknown): obj is RelationshipModel {
    return isTypeStamped("relationship", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && isITimeSeries(obj)
        && "parent" in obj
        && isEntityModel(obj.parent)
        && "child" in obj
        && isEntityModel(obj.child);
}

export type SetRelationshipInput = {
    type?: IDOrName;
    childEntity: IDOrName;
    start?: Date;
    end?: Date;
    classification?: IDOrName;
}
