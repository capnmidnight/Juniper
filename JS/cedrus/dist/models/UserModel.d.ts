import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export interface UserModel extends INamed, ITypeStamped<"user"> {
    email: string;
    roles: string[];
}
export declare function isUserModel(obj: unknown): obj is UserModel;
export interface SetUserInput {
    userName: string;
    email: string;
}
export type USER = TypedNameOrId<"user">;
export declare function USER(idOrName: number | string): USER;
//# sourceMappingURL=UserModel.d.ts.map