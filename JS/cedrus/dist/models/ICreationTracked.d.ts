import { UserModel } from "./UserModel";
export interface ICreationTracked {
    createdOn: Date;
    user: UserModel;
}
export declare function isICreationTracked(obj: unknown): obj is ICreationTracked;
//# sourceMappingURL=ICreationTracked.d.ts.map